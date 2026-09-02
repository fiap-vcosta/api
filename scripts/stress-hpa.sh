#!/usr/bin/env bash
# Gera carga HTTP na listagem de OS (JWT Admin) para exercitar o HPA.
# Pré-requisito: algumas OS no banco melhoram o custo por request.
# Uso: ./scripts/stress-hpa.sh [duração_segundos] [concorrência]
set -euo pipefail

BASE_URL="${BASE_URL:-http://localhost:8080}"
DURATION="${1:-120}"
CONCURRENCY="${2:-25}"
LOGIN="${LOGIN:-admin}"
PASSWORD="${PASSWORD:-admin}"

echo "Login em ${BASE_URL}/api/auth/login ..."
TOKEN="$(curl -fsS -X POST "${BASE_URL}/api/auth/login" \
  -H 'Content-Type: application/json' \
  -d "{\"login\":\"${LOGIN}\",\"password\":\"${PASSWORD}\"}" \
  | sed -n 's/.*"token"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p')"

if [[ -z "${TOKEN}" ]]; then
  echo "Erro: não foi possível obter o token JWT (login admin/admin)."
  exit 1
fi

URL="${BASE_URL}/api/ordens-servico"
echo "Stress: url=${URL} duration=${DURATION}s concurrency=${CONCURRENCY}"
echo "Em outro terminal: watch -n 2 kubectl get hpa,pods -n techchallenge"

if command -v hey >/dev/null 2>&1; then
  hey -z "${DURATION}s" -c "${CONCURRENCY}" -H "Authorization: Bearer ${TOKEN}" "${URL}"
  exit 0
fi

echo "hey não encontrado — usando loop curl."
end=$((SECONDS + DURATION))
while (( SECONDS < end )); do
  for ((i = 0; i < CONCURRENCY; i++)); do
    curl -s -o /dev/null -H "Authorization: Bearer ${TOKEN}" "${URL}" &
  done
  wait
done
echo "Stress finalizado."
