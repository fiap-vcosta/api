# Infraestrutura local (kind + Terraform)

> **Fase 03:** kind + self-hosted runner **não** são o caminho de entrega. Entrega na GCP: repos `infra-db` / `infra-k8s`. Este doc permanece como referência de lab local.

Sobe cluster kind, Postgres e metrics-server. Código em [`/infra`](../infra). Deploy da API: [05_kubernetes-api.md](05_kubernetes-api.md).  
Índice: [docs/README.md](README.md) · [README.md](../README.md).

## Pré-requisitos

No `PATH`: `docker`, `kubectl`, `kind`, `terraform`, `helm`.

Instalação: [Docker](https://docs.docker.com/engine/install/), [kubectl](https://kubernetes.io/docs/tasks/tools/), [kind](https://kind.sigs.k8s.io/docs/user/quick-start/#installation), [Terraform](https://developer.hashicorp.com/terraform/install), [Helm](https://helm.sh/docs/intro/install/).

Opcional para stress do HPA: [`scripts/stress-hpa.sh`](../scripts/stress-hpa.sh) (usa `hey` se existir; senão `curl`).

## Subir

```bash
# Na raiz do repositório
./scripts/up.sh
```

Equivalente manual: pare o que usa a porta **8080**, depois `terraform apply` em `/infra` e o deploy em [05](05_kubernetes-api.md).

## Derrubar

```bash
./scripts/down.sh
```

## CD (self-hosted runner) — removido

O workflow `cd.yml` (kind + self-hosted) foi **removido** na Fase 03. Para lab local, use `./scripts/up.sh` nesta máquina. Deploy na nuvem: §5 (`infra-k8s` + workflow manual na `api`).

Apply local sem Actions: use `infra/terraform.tfvars` (gitignored; modelo em `terraform.tfvars.example`). Secrets JWT/Postgres no lab local ficam no tfvars / `k8s/secret.yaml`, não no CD.
