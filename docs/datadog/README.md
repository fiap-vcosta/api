# Datadog — demo da API

## Links (site `us5`)

| Recurso | URL |
|---------|-----|
| Dashboard | https://us5.datadoghq.com/dashboard/zdc-k5i-6uc/tech-challenge--api-demo |
| APM — serviço `api` | https://us5.datadoghq.com/apm/entity/service%3Aapi |
| Logs — transições OS | https://us5.datadoghq.com/logs?query=service%3Aapi%20env%3Aprod%20%22movida%20para%20o%20status%22 |
| Logs ↔ traces | abra um log com `trace_id` → **View Trace** |
| Health (smoke) | https://api.vcosta-fiap.online/health |

Monitores da demo (criar a partir dos JSON em [`monitor-*.json`](.) se ainda não existirem): erros HTTP, falhas OS, health/uptime.

## Importar / atualizar o dashboard

1. Abra o site Datadog da org (`DD_SITE` = `us5.datadoghq.com`).
2. **Dashboards** → abra o dashboard existente **ou** **New Dashboard** → **Import dashboard JSON**.
3. Cole o conteúdo de [`dashboard-api-demo.json`](dashboard-api-demo.json) (ou faça upload / replace).
4. Widgets APM: `trace.aspnet_core.request.*`. Widgets de OS: logs `"movida para o status…"`. Volume diário: query value 24h em OS **Recebida**.

## Cloud SQL (métricas GCP)

Os widgets de Cloud SQL **não** vêm do sidecar da API: exigem a integração **Google Cloud Platform** no Datadog.

1. Datadog → **Integrations** → **Google Cloud Platform** → conectar o projeto `vcosta-fiap-tech-challenge` (conta de serviço + roles de leitura de métricas, conforme o wizard).
2. Aguarde alguns minutos até aparecerem métricas `gcp.cloudsql.database.*`.
3. Reimporte o dashboard (ou confira os widgets Cloud SQL já existentes).

Filtro padrão da instância da demo: `database_id:vcosta-fiap-tech-challenge:tech-challenge-pg`.

Limite a coleta a tags/métricas necessárias — VMs GCE (nós GKE) **contam como host** no plano estudante.

## Monitores

Na UI: **Monitors** → **New Monitor**, ou importe o JSON:

| Arquivo | Objetivo |
|---------|----------|
| [`monitor-erros-http.json`](monitor-erros-http.json) | Erros APM HTTP `service:api` |
| [`monitor-falhas-os.json`](monitor-falhas-os.json) | Logs `status:error` ligados a OS |
| [`monitor-health-uptime.json`](monitor-health-uptime.json) | Ausência de hits em `/health` |

Ajuste o `@email` de notificação na UI para a conta estudante.

Alternativa: **Synthetics** → HTTP check em `https://api.vcosta-fiap.online/health` a cada 1–5 min (pause após `tf-destroy`).

## Correlação logs ↔ traces

Logs JSON da API levam `RequestId` e, com `DD_LOGS_INJECTION=true`, `dd.trace_id` / `dd.span_id`. No painel de Logs, abra um log e use **View Trace**.
