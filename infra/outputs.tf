output "cluster_name" {
  description = "Nome do cluster kind."
  value       = kind_cluster.this.name
}

output "kubeconfig_path" {
  description = "Caminho do kubeconfig. Use: export KUBECONFIG=$(pwd)/kubeconfig"
  value       = kind_cluster.this.kubeconfig_path
}

output "api_url" {
  description = "URL da API no host (após deploy dos manifests em /k8s)."
  value       = "http://localhost:${var.api_host_port}"
}

output "postgres_service" {
  description = "DNS interno do Postgres no cluster (connection string da API)."
  value       = "postgres.${kubernetes_namespace.techchallenge.metadata[0].name}.svc.cluster.local"
}
