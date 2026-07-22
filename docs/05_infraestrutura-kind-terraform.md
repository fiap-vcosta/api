# Infraestrutura local (kind + Terraform)

Sobe cluster kind, Postgres e metrics-server. Código em [`/infra`](../infra). Deploy da API: [06_kubernetes-api.md](06_kubernetes-api.md).

## Pré-requisitos

No `PATH`: `docker`, `kubectl`, `kind`, `terraform`, `helm`.

Instalação: [Docker](https://docs.docker.com/engine/install/), [kubectl](https://kubernetes.io/docs/tasks/tools/), [kind](https://kind.sigs.k8s.io/docs/user/quick-start/#installation), [Terraform](https://developer.hashicorp.com/terraform/install), [Helm](https://helm.sh/docs/intro/install/).

Opcional para stress do HPA: [`scripts/stress-hpa.sh`](../scripts/stress-hpa.sh) (usa `hey` se existir; senão `curl`).

## Subir

```bash
# Na raiz do repositório
./scripts/up.sh
```

Equivalente manual: pare o que usa a porta **8080**, depois `terraform apply` em `/infra` e o deploy em [06](06_kubernetes-api.md).

## Derrubar

```bash
./scripts/down.sh
```

## CD (self-hosted runner)

Workflow: [`.github/workflows/cd.yml`](../.github/workflows/cd.yml) (`runs-on: self-hosted` — mesma máquina do kind).

1. GitHub → **Settings → Actions → Runners → New self-hosted runner** (Linux x64); seguir o script; `./svc.sh install && ./svc.sh start`.
2. Runner **Idle**; `docker`, `kubectl`, `kind`, `terraform` e `helm` no `PATH` do serviço; usuário no grupo `docker`.
3. **Settings → Secrets and variables → Actions** (alinhados a [`.env.example`](../.env.example)):

| Secret |
|--------|
| `POSTGRES_DB` |
| `POSTGRES_USER` |
| `POSTGRES_PASSWORD` |
| `JWT_KEY` |
| `JWT_ISSUER` |
| `JWT_AUDIENCE` |

4. Disparar o workflow **CD** (`workflow_dispatch` ou push em `main`).

Apply manual sem Actions: use `infra/terraform.tfvars` (gitignored; modelo em `terraform.tfvars.example`).
