resource "helm_release" "metrics_server" {
  name       = "metrics-server"
  repository = "https://kubernetes-sigs.github.io/metrics-server/"
  chart      = "metrics-server"
  version    = "3.12.2"
  namespace  = "kube-system"

  wait          = true
  wait_for_jobs = true
  timeout       = 180

  values = [
    yamlencode({
      args = ["--kubelet-insecure-tls"]
    })
  ]

  depends_on = [kind_cluster.this]
}
