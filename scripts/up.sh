#!/usr/bin/env bash
# Sobe kind + Postgres + metrics-server + API (fluxo local equivalente ao CD).
# Uso: ./scripts/up.sh
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"
export PATH="${HOME}/.local/bin:${PATH}"

CLUSTER_NAME="techchallenge"
IMAGE="techchallenge-api:local"
KUBECONFIG_PATH="${ROOT}/infra/kubeconfig"

require_cmd() {
  if ! command -v "$1" >/dev/null 2>&1; then
    echo "Erro: '${1}' não encontrado no PATH. Instale antes de continuar (ver docs/04)."
    exit 1
  fi
  echo "OK  ${1}: $(command -v "$1")"
}

echo "==> Verificando ferramentas"
require_cmd docker
require_cmd kubectl
require_cmd kind
require_cmd terraform
require_cmd helm

if ! docker info >/dev/null 2>&1; then
  echo "Erro: Docker não está acessível (daemon parado ou sem permissão)."
  exit 1
fi

if ss -tln 2>/dev/null | grep -q ':8080 ' || netstat -tln 2>/dev/null | grep -q ':8080 '; then
  if ! kind get clusters 2>/dev/null | grep -qx "${CLUSTER_NAME}"; then
    echo "Aviso: porta 8080 em uso e cluster kind '${CLUSTER_NAME}' não existe."
    echo "       Pare o que estiver usando 8080 (ex.: docker compose --profile app stop api) e rode de novo."
    exit 1
  fi
fi

echo "==> Terraform (kind + Postgres + metrics-server)"
cd "${ROOT}/infra"
if [[ ! -f terraform.tfvars ]]; then
  cp terraform.tfvars.example terraform.tfvars
  echo "Criado infra/terraform.tfvars a partir do example."
fi
terraform init -input=false
terraform apply -input=false -auto-approve
export KUBECONFIG="${KUBECONFIG_PATH}"

echo "==> Build e load da imagem"
cd "${ROOT}"
docker build -t "${IMAGE}" .
kind load docker-image "${IMAGE}" --name "${CLUSTER_NAME}"

echo "==> Manifests da API"
if [[ ! -f k8s/secret.yaml ]]; then
  cp k8s/secret.yaml.example k8s/secret.yaml
  echo "Criado k8s/secret.yaml a partir do example."
fi
kubectl get namespace techchallenge
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secret.yaml
kubectl apply -f k8s/deployment.yaml
kubectl apply -f k8s/service.yaml
kubectl apply -f k8s/hpa.yaml
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
echo "Pronto."
echo "  API:     http://localhost:8080"
echo "  Swagger: http://localhost:8080/swagger/index.html"
echo "  KUBECONFIG=${KUBECONFIG_PATH}"
echo "  Stress:  ./scripts/stress-hpa.sh"
echo "  Down:    ./scripts/down.sh"
