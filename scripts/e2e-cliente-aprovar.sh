#!/usr/bin/env bash
# Caminho feliz: staff garante cliente+veículo (CPF válido) → cria OS →
# finaliza diagnóstico → auth (CPF) → aprovar (JWT + token opaco).
#
# Uso (local Compose + auth :8081):
#   ./scripts/e2e-cliente-aprovar.sh
#
# Uso (entrada oficial / API Gateway):
#   BASE_URL=https://vcosta-fiap.online \
#   AUTH_URL=https://vcosta-fiap.online/auth \
#   ./scripts/e2e-cliente-aprovar.sh
#
# Documento default: 92561324354 (CPF válido — seeds da API usam CPFs inválidos
# no algoritmo e o auth rejeita). Token opaco (RF21.2): Postgres Compose ou TOKEN_APROVACAO.
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
BASE_URL="${BASE_URL:-http://localhost:8080}"
AUTH_URL="${AUTH_URL:-http://localhost:8081}"
DOCUMENTO="${DOCUMENTO:-92561324354}"
LOGIN="${LOGIN:-admin}"
SENHA="${SENHA:-admin}"
HTTP_CODE=""

if [[ -f "${ROOT}/.env" ]]; then
  set -a
  # shellcheck disable=SC1091
  source "${ROOT}/.env"
  set +a
fi

POSTGRES_USER="${POSTGRES_USER:-postgres}"
POSTGRES_DB="${POSTGRES_DB:-techchallenge}"

json_get() {
  local json="$1" key="$2"
  python3 -c 'import json,sys; d=json.load(sys.stdin); v=d.get(sys.argv[1],""); print("" if v is None else v)' "$key" <<<"$json"
}

find_cliente_id() {
  local json="$1" documento="$2"
  python3 -c '
import json,sys
doc=sys.argv[1]
items=json.load(sys.stdin)
if isinstance(items, dict):
    items=items.get("items") or items.get("data") or []
for c in items:
    d=str(c.get("documento") or c.get("Documento") or "")
    if "".join(ch for ch in d if ch.isdigit())==doc:
        print(c.get("id") or c.get("Id") or "")
        break
' "$documento" <<<"$json"
}

unique_placa() {
  python3 -c '
import time
n=int(time.time()*1000)
letters="ABCDEFGHIJKLMNOPQRSTUVWXYZ"
print(
  f"{letters[n%26]}{letters[(n//26)%26]}{letters[(n//676)%26]}"
  f"{(n//10)%10}{letters[(n//100)%26]}{(n//1000)%10}{(n//10000)%10}"
)
'
}

curl_json() {
  local tmp
  tmp="$(mktemp)"
  HTTP_CODE="$(curl -sS -o "${tmp}" -w '%{http_code}' "$@")"
  cat "${tmp}"
  rm -f "${tmp}"
}

require_code() {
  local want="$1" step="$2" body="${3:-}"
  if [[ "${HTTP_CODE}" != "${want}" ]]; then
    echo "Falha em ${step}: HTTP ${HTTP_CODE} (esperado ${want})" >&2
    if [[ -n "${body}" ]]; then
      echo "Body: ${body}" >&2
    fi
    exit 1
  fi
}

require_code_one_of() {
  local step="$1" body="$2"
  shift 2
  local code
  for code in "$@"; do
    if [[ "${HTTP_CODE}" == "${code}" ]]; then
      return 0
    fi
  done
  echo "Falha em ${step}: HTTP ${HTTP_CODE} (esperado um de: $*)" >&2
  echo "Body: ${body}" >&2
  exit 1
}

fetch_token_aprovacao() {
  local ordem_id="$1"
  if [[ -n "${TOKEN_APROVACAO:-}" ]]; then
    echo "${TOKEN_APROVACAO}"
    return 0
  fi

  if command -v docker >/dev/null 2>&1 \
    && docker compose -f "${ROOT}/docker-compose.yml" ps db --status running 2>/dev/null | grep -q db; then
    docker compose -f "${ROOT}/docker-compose.yml" exec -T db \
      psql -U "${POSTGRES_USER}" -d "${POSTGRES_DB}" -tAc \
      "SELECT \"TokenAprovacao\" FROM \"OrdensServico\" WHERE \"Id\" = ${ordem_id};" \
      | tr -d '[:space:]'
    return 0
  fi

  echo "Token opaco indisponível: defina TOKEN_APROVACAO ou suba o Compose (db)." >&2
  echo "GCP: Cloud SQL Studio → SELECT \"TokenAprovacao\" FROM \"OrdensServico\" WHERE \"Id\" = ${ordem_id};" >&2
  exit 1
}

echo "==> Login staff em ${BASE_URL}/api/auth/login"
body="$(curl_json -X POST "${BASE_URL}/api/auth/login" \
  -H 'Content-Type: application/json' \
  -d "{\"login\":\"${LOGIN}\",\"senha\":\"${SENHA}\"}")"
require_code "200" "login" "${body}"
STAFF_TOKEN="$(json_get "${body}" token)"
if [[ -z "${STAFF_TOKEN}" ]]; then
  echo "Login sem token" >&2
  exit 1
fi

echo "==> Garantir cliente CPF ${DOCUMENTO}"
body="$(curl_json -H "Authorization: Bearer ${STAFF_TOKEN}" "${BASE_URL}/api/clientes")"
require_code "200" "listar clientes" "${body}"
CLIENTE_ID="$(find_cliente_id "${body}" "${DOCUMENTO}")"
if [[ -z "${CLIENTE_ID}" ]]; then
  body="$(curl_json -X POST "${BASE_URL}/api/clientes" \
    -H "Authorization: Bearer ${STAFF_TOKEN}" \
    -H 'Content-Type: application/json' \
    -d "{\"Nome\":\"E2E Cliente\",\"TipoDocumento\":0,\"Documento\":\"${DOCUMENTO}\"}")"
  require_code "201" "criar cliente" "${body}"
  CLIENTE_ID="$(json_get "${body}" id)"
fi
echo "    clienteId=${CLIENTE_ID}"

PLACA="$(unique_placa)"
echo "==> Criar veículo placa ${PLACA}"
body="$(curl_json -X POST "${BASE_URL}/api/veiculos" \
  -H "Authorization: Bearer ${STAFF_TOKEN}" \
  -H 'Content-Type: application/json' \
  -d "{\"Placa\":\"${PLACA}\",\"IdCliente\":${CLIENTE_ID},\"Modelo\":\"Gol\",\"Marca\":\"Volkswagen\"}")"
require_code "201" "criar veículo" "${body}"
VEICULO_ID="$(json_get "${body}" id)"
echo "    veiculoId=${VEICULO_ID}"

echo "==> Criar OS"
body="$(curl_json -X POST "${BASE_URL}/api/ordens-servico" \
  -H "Authorization: Bearer ${STAFF_TOKEN}" \
  -H 'Content-Type: application/json' \
  -d "{\"IdVeiculo\":${VEICULO_ID},\"Servicos\":[{\"IdServico\":1,\"ValorCobrado\":150,\"ItensNecessarios\":[{\"IdItemEstoque\":1,\"Quantidade\":1}]}]}")"
require_code "201" "criar OS" "${body}"
ORDEM_ID="$(json_get "${body}" id)"
if [[ -z "${ORDEM_ID}" ]]; then
  echo "Criar OS sem id" >&2
  exit 1
fi
echo "    ordemServicoId=${ORDEM_ID}"

echo "==> Finalizar diagnóstico"
body="$(curl_json -X POST "${BASE_URL}/api/ordens-servico/${ORDEM_ID}/finalizar-diagnostico" \
  -H "Authorization: Bearer ${STAFF_TOKEN}")"
require_code_one_of "finalizar diagnóstico" "${body}" 200 201 204

echo "==> Assert AguardandoAprovacao"
body="$(curl_json -H "Authorization: Bearer ${STAFF_TOKEN}" \
  "${BASE_URL}/api/ordens-servico/${ORDEM_ID}")"
require_code "200" "GET OS" "${body}"
STATUS="$(json_get "${body}" status)"
if [[ "${STATUS}" != "AguardandoAprovacao" ]]; then
  echo "Status inesperado: ${STATUS}" >&2
  exit 1
fi

echo "==> Token opaco (banco / TOKEN_APROVACAO)"
TOKEN_OPACO="$(fetch_token_aprovacao "${ORDEM_ID}")"
if [[ -z "${TOKEN_OPACO}" ]]; then
  echo "TokenAprovacao vazio para OS ${ORDEM_ID}" >&2
  exit 1
fi
echo "    tokenAprovacao=${TOKEN_OPACO:0:8}…"

echo "==> Auth cliente em ${AUTH_URL} (documento=${DOCUMENTO})"
body="$(curl_json -X POST "${AUTH_URL}" \
  -H 'Content-Type: application/json' \
  -d "{\"documento\":\"${DOCUMENTO}\"}")"
require_code "200" "auth" "${body}"
CLIENTE_TOKEN="$(json_get "${body}" token)"
if [[ -z "${CLIENTE_TOKEN}" ]]; then
  echo "Auth sem token" >&2
  exit 1
fi

echo "==> Aprovar público (JWT cliente + token opaco)"
body="$(curl_json -X POST \
  "${BASE_URL}/api/public/ordens-servico/aprovar?token=${TOKEN_OPACO}" \
  -H "Authorization: Bearer ${CLIENTE_TOKEN}")"
require_code_one_of "aprovar público" "${body}" 200 201 204

echo "==> Assert LiberadaParaExecucao"
body="$(curl_json -H "Authorization: Bearer ${STAFF_TOKEN}" \
  "${BASE_URL}/api/ordens-servico/${ORDEM_ID}")"
require_code "200" "GET OS pós-aprovação" "${body}"
STATUS="$(json_get "${body}" status)"
if [[ "${STATUS}" != "LiberadaParaExecucao" ]]; then
  echo "Status inesperado após aprovar: ${STATUS}" >&2
  exit 1
fi

echo "OK — OS ${ORDEM_ID} aprovada pelo cliente via ${AUTH_URL} + token opaco."
