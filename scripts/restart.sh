#!/usr/bin/env bash
# Atualiza só a API no kind (após mudança de código).
# Pré-requisito: cluster já no ar (./scripts/up.sh).
# Uso: ./scripts/restart.sh
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"
export PATH="${HOME}/.local/bin:${PATH}"

CLUSTER_NAME="techchallenge"
IMAGE="techchallenge-api:local"
export KUBECONFIG="${ROOT}/infra/kubeconfig"

require_cmd() {
  if ! command -v "$1" >/dev/null 2>&1; then
    echo "Erro: '${1}' não encontrado no PATH."
    exit 1
  fi
}

echo "==> Verificando ferramentas"
require_cmd docker
require_cmd kind
require_cmd kubectl

if [[ ! -f "${KUBECONFIG}" ]]; then
  echo "Erro: ${KUBECONFIG} não existe. Rode ./scripts/up.sh antes."
  exit 1
fi

if ! kind get clusters 2>/dev/null | grep -qx "${CLUSTER_NAME}"; then
  echo "Erro: cluster kind '${CLUSTER_NAME}' não encontrado. Rode ./scripts/up.sh antes."
  exit 1
fi

echo "==> Build e load da imagem"
docker build -t "${IMAGE}" .
kind load docker-image "${IMAGE}" --name "${CLUSTER_NAME}"

echo "==> Restart do Deployment"
kubectl -n techchallenge rollout restart deployment/api
kubectl -n techchallenge rollout status deployment/api --timeout=180s

echo "==> Health check"
ok=0
for _ in $(seq 1 30); do
  if curl -fsS "http://localhost:8080/health"; then
    echo
    ok=1
    break
  fi
  sleep 2
done
if [[ "${ok}" -ne 1 ]]; then
  echo "Health check falhou."
  kubectl -n techchallenge get pods
  exit 1
fi

echo
echo "API atualizada: http://localhost:8080/swagger/index.html"
