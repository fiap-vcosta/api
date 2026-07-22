resource "kind_cluster" "this" {
  name            = var.cluster_name
  wait_for_ready  = true
  kubeconfig_path = abspath(var.kubeconfig_path)

  kind_config {
    kind        = "Cluster"
    api_version = "kind.x-k8s.io/v1alpha4"

    node {
      role = "control-plane"

      extra_port_mappings {
        container_port = var.api_node_port
        host_port      = var.api_host_port
        protocol       = "TCP"
      }
    }
  }
}
