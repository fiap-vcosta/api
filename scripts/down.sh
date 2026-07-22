#!/usr/bin/env bash
# Derruba a infra kind e remove artefatos locais gerados.
# Uso: ./scripts/down.sh
# Não remove ferramentas (docker, kubectl, kind, terraform, helm).
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$ROOT"
export PATH="${HOME}/.local/bin:${PATH}"

CLUSTER_NAME="techchallenge"

echo "==> Verificando ferramentas"
for cmd in docker kubectl kind terraform; do
  if ! command -v "$cmd" >/dev/null 2>&1; then
    echo "Erro: '${cmd}' não encontrado no PATH."
    exit 1
  fi
done

echo "==> Terraform destroy"
cd "${ROOT}/infra"
export KUBECONFIG="${ROOT}/infra/kubeconfig"
if [[ -f terraform.tfstate ]] || [[ -f .terraform/terraform.tfstate ]]; then
  terraform destroy -input=false -auto-approve || true
fi

if kind get clusters 2>/dev/null | grep -qx "${CLUSTER_NAME}"; then
  echo "==> kind delete cluster (fallback)"
  kind delete cluster --name "${CLUSTER_NAME}"
fi

echo "==> Removendo artefatos locais"
rm -f \
  "${ROOT}/infra/terraform.tfstate" \
  "${ROOT}/infra/terraform.tfstate.backup" \
  "${ROOT}/infra/kubeconfig" \
  "${ROOT}/infra/terraform.tfvars" \
  "${ROOT}/k8s/secret.yaml"
rm -rf "${ROOT}/infra/.terraform"

echo "Down concluído. Ferramentas instaladas foram mantidas."
