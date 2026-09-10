# ADR 001: Observabilidade com Datadog (APM)

**Data:** 10 de Setembro de 2026  
**Status:** Aceito  
**Autores:** Victor Costa

## 1. Contexto e Problema

A demo precisa evidenciar latência, saúde da API e falhas no fluxo de OS (vídeo + PDF). O cluster e o banco já sobem na GCP; falta um backend de observabilidade alinhado ao benefício estudante e ao esforço de instrumentação .NET.

O problema a ser resolvido é: **Qual ferramenta usamos para APM/logs na janela de demo, e onde a instrumentação mora?**

## 2. Decisão

- **Ferramenta:** **Datadog** (conta estudante), com **APM primeiro**; métricas/logs/dashboards conforme necessário na execução.
- **Instrumentação:** no workload da **API** (.NET), não como substituto de infra Terraform. Agent/sidecar ou tracer conforme o caminho mais simples no Autopilot na hora de implementar.
- **`/health` permanece público** para probes e checagem operacional.
- Implementação detalhada (agent, dashboards, alertas de falha de OS) na sequência de observabilidade — esta ADR fecha a **escolha de produto**.

## 3. Justificativa

* Benefício estudante reduz barreira de custo para a demo.
* APM cobre o que o vídeo pede (traces, latência) sem montar stack Prometheus/Grafana do zero.
* Instrumentar a app (não só o cluster) mostra o fluxo de negócio (OS), não só CPU de nó.

## 4. Alternativas Consideradas

* **Google Cloud Monitoring / Cloud Trace só:** nativo e barato; menos alinhado ao material “Datadog” e ao storytelling do enunciado se o professor esperar Datadog.
* **Grafana Cloud / Prometheus no cluster:** poderoso; mais peças e tempo de setup para a janela.
* **Só logs stdout sem APM:** insuficiente para traces e latência de request.
* **OpenTelemetry + backend genérico:** bom padrão; ainda precisaria de um destino — Datadog pode consumir OTLP na execução se couber.

## 5. Consequências

### Positivas
* Escolha única para README, vídeo e dashboards da demo.
* Escopo claro: app emite telemetria; infra só hospeda.

### Negativas / Riscos (Mitigações)
* **Segredo/API key Datadog** no deploy (não no Git).
  * *Mitigação:* Secret Manager / GitHub Secrets; rotacionar se vazar.
* **Overhead do tracer** sob stress HPA.
  * *Mitigação:* sampling padrão; desligar ou reduzir se distorcer a demo de escala.
* **Detalhe de agent no Autopilot** na execução.
  * *Mitigação:* preferir o caminho documentado mais curto (tracer in-process vs DaemonSet) quando for implementar.
