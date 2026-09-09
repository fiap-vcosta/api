# Kubernetes — API (fluxo kind, legado)

> **Legado:** este doc descreve o deploy no kind. A pasta [`/k8s`](../k8s) hoje contém os manifests do **GKE Autopilot** (namespace `tech-challenge`, sidecar do Cloud SQL Auth Proxy, Service `LoadBalancer`), aplicados pelo workflow `deploy` — ver [README](../README.md). Infra antiga: [04_infraestrutura-kind-terraform.md](04_infraestrutura-kind-terraform.md).  
Índice: [docs/README.md](README.md) · [README.md](../README.md).

## Deploy

Os atalhos `scripts/up.sh`, `restart.sh` e `down.sh` foram removidos junto com o Terraform do kind. O que segue é o passo a passo manual do fluxo antigo:

```bash
export KUBECONFIG="$(pwd)/infra/kubeconfig"
kubectl get namespace techchallenge

cp k8s/secret.yaml.example k8s/secret.yaml
# alinhe senha/JWT com o Postgres do terraform.tfvars

docker build -t techchallenge-api:local .
kind load docker-image techchallenge-api:local --name techchallenge

kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/secret.yaml
kubectl apply -f k8s/deployment.yaml
kubectl apply -f k8s/service.yaml
kubectl apply -f k8s/hpa.yaml

kubectl -n techchallenge rollout status deployment/api
curl -s http://localhost:8080/health
```

- API: http://localhost:8080  
- Swagger: http://localhost:8080/swagger/index.html  

O workflow CD kind foi removido; use os scripts/`kubectl` acima no lab local.

## HPA / stress

Crie algumas OS (Swagger) e rode:

```bash
watch -n 2 kubectl get hpa,pods -n techchallenge
./scripts/stress-hpa.sh
```

O script faz login (`admin`/`admin`) e gera carga em `GET /api/ordens-servico`.
