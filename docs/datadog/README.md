# Datadog — demo da API

## Importar o dashboard

1. Abra o site Datadog da org (`DD_SITE`).
2. **Dashboards** → **New Dashboard** → **Import dashboard JSON**.
3. Cole o conteúdo de [`dashboard-api-demo.json`](dashboard-api-demo.json) (ou faça upload).
4. Salve. Na janela da demo, filtre `env:demo` / `service:api` se os widgets vierem vazios nos primeiros minutos.

## Monitor sugerido (falhas no fluxo de OS)

Na UI: **Monitors** → **New Monitor** → **APM**:

- Métrica / query: taxa de erro do serviço `api` em `env:demo` (ex. `trace.aspnet.request.errors` / hits), ou monitor de **Error Tracking** nos spans de rotas `ordens-servico`.
- Janela: 5–10 minutos.
- Alerta: e-mail da conta estudante.
- Nome: `demo — erros HTTP na API (OS)`.

Alternativa barata: **Synthetics** → HTTP check em `https://api.vcosta-fiap.online/health` a cada 1–5 min (só enquanto a demo estiver no ar; pause após `tf-destroy`).

## Correlação logs ↔ traces

Logs JSON da API levam `RequestId` e, com `DD_LOGS_INJECTION=true`, `dd.trace_id` / `dd.span_id`. No painel de Logs, abra um log e use **View Trace**.
