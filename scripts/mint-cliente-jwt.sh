#!/usr/bin/env bash
# Emite um JWT de cliente (claim documento) com o material JwtCliente do .env — substituto local da Function auth.
# Uso:
#   ./scripts/mint-cliente-jwt.sh <documento>
#   ./scripts/mint-cliente-jwt.sh 11144477735
# Imprime só o token (stdout) para colar em {{tokenCliente}} no Requestly.
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
DOCUMENTO="${1:-}"

if [[ -z "${DOCUMENTO}" ]]; then
  echo "Uso: $0 <documento>" >&2
  exit 1
fi

if [[ -f "${ROOT}/.env" ]]; then
  set -a
  # shellcheck disable=SC1091
  source "${ROOT}/.env"
  set +a
fi

KEY="${JWT_CLIENTE_KEY:-local-jwt-cliente-key-change-me-32chars-min}"
ISSUER="${JWT_CLIENTE_ISSUER:-tech-challenge-cliente}"
AUDIENCE="${JWT_CLIENTE_AUDIENCE:-tech-challenge-cliente}"

exec python3 - "${DOCUMENTO}" "${KEY}" "${ISSUER}" "${AUDIENCE}" <<'PY'
import base64, hashlib, hmac, json, sys, time

def b64url(data: bytes) -> str:
    return base64.urlsafe_b64encode(data).rstrip(b"=").decode("ascii")

documento, key, issuer, audience = sys.argv[1], sys.argv[2], sys.argv[3], sys.argv[4]
digits = "".join(c for c in documento if c.isdigit())
now = int(time.time())
header = b64url(json.dumps({"alg": "HS256", "typ": "JWT"}, separators=(",", ":")).encode())
payload = b64url(json.dumps({
    "documento": digits,
    "iss": issuer,
    "aud": audience,
    "iat": now,
    "exp": now + 30 * 60
}, separators=(",", ":")).encode())
signing_input = f"{header}.{payload}".encode()
sig = b64url(hmac.new(key.encode(), signing_input, hashlib.sha256).digest())
print(f"{header}.{payload}.{sig}")
PY
