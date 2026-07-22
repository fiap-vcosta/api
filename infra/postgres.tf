resource "kubernetes_namespace" "techchallenge" {
  metadata {
    name = "techchallenge"
  }

  depends_on = [kind_cluster.this]
}

resource "kubernetes_secret" "postgres" {
  metadata {
    name      = "postgres"
    namespace = kubernetes_namespace.techchallenge.metadata[0].name
  }

  data = {
    POSTGRES_DB       = var.postgres_db
    POSTGRES_USER     = var.postgres_user
    POSTGRES_PASSWORD = var.postgres_password
  }
}

resource "kubernetes_persistent_volume_claim" "postgres" {
  metadata {
    name      = "postgres"
    namespace = kubernetes_namespace.techchallenge.metadata[0].name
  }

  wait_until_bound = false

  spec {
    access_modes = ["ReadWriteOnce"]

    resources {
      requests = {
        storage = "1Gi"
      }
    }
  }
}

resource "kubernetes_deployment" "postgres" {
  metadata {
    name      = "postgres"
    namespace = kubernetes_namespace.techchallenge.metadata[0].name
    labels = {
      app = "postgres"
    }
  }

  spec {
    replicas = 1

    selector {
      match_labels = {
        app = "postgres"
      }
    }

    template {
      metadata {
        labels = {
          app = "postgres"
        }
      }

      spec {
        container {
          name  = "postgres"
          image = "postgres:16"

          port {
            container_port = 5432
            name           = "postgres"
          }

          env_from {
            secret_ref {
              name = kubernetes_secret.postgres.metadata[0].name
            }
          }

          volume_mount {
            name       = "data"
            mount_path = "/var/lib/postgresql/data"
            sub_path   = "pgdata"
          }

          readiness_probe {
            exec {
              command = ["pg_isready", "-U", var.postgres_user]
            }
            initial_delay_seconds = 5
            period_seconds        = 5
          }

          resources {
            requests = {
              cpu    = "100m"
              memory = "256Mi"
            }
            limits = {
              cpu    = "500m"
              memory = "512Mi"
            }
          }
        }

        volume {
          name = "data"
          persistent_volume_claim {
            claim_name = kubernetes_persistent_volume_claim.postgres.metadata[0].name
          }
        }
      }
    }
  }
}

resource "kubernetes_service" "postgres" {
  metadata {
    name      = "postgres"
    namespace = kubernetes_namespace.techchallenge.metadata[0].name
  }

  spec {
    selector = {
      app = "postgres"
    }

    port {
      port        = 5432
      target_port = 5432
      name        = "postgres"
    }
  }
}
