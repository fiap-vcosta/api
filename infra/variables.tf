variable "cluster_name" {
  type        = string
  description = "Nome do cluster kind."
  default     = "techchallenge"
}

variable "kubeconfig_path" {
  type        = string
  description = "Caminho onde o kubeconfig do kind será gravado."
  default     = "./kubeconfig"
}

variable "postgres_db" {
  type        = string
  description = "Nome do banco PostgreSQL."
  default     = "techchallenge"
}

variable "postgres_user" {
  type        = string
  description = "Usuário PostgreSQL."
  default     = "postgres"
}

variable "postgres_password" {
  type        = string
  description = "Senha PostgreSQL."
  sensitive   = true
}

variable "api_host_port" {
  type        = number
  description = "Porta no host mapeada para o NodePort da API no kind."
  default     = 8080
}

variable "api_node_port" {
  type        = number
  description = "NodePort da API (deve bater com k8s/service.yaml e o port mapping do kind)."
  default     = 30080
}
