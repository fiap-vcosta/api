#!/usr/bin/env bash
# Emite um JWT de cliente (claim cpf) com o material JwtClient do .env — substituto local da Function auth até a §8.
# Uso:
#   ./scripts/mint-client-jwt.sh <cpf>
#   ./scripts/mint-client-jwt.sh 11144477735
# Imprime só o token (stdout) para colar em {{tokenCliente}} no Requestly.
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
CPF="${1:-}"

if [[ -z "${CPF}" ]]; then
  echo "Uso: $0 <cpf>" >&2
  exit 1
fi

if [[ -f "${ROOT}/.env" ]]; then
  set -a
  # shellcheck disable=SC1091
  source "${ROOT}/.env"
  set +a
fi

KEY="${JWT_CLIENT_KEY:-local-jwt-client-key-change-me-32chars-min}"
ISSUER="${JWT_CLIENT_ISSUER:-tech-challenge-client}"
AUDIENCE="${JWT_CLIENT_AUDIENCE:-tech-challenge-client}"

exec python3 - "${CPF}" "${KEY}" "${ISSUER}" "${AUDIENCE}" <<'PY'
import base64, hashlib, hmac, json, sys, time

def b64url(data: bytes) -> str:
    return base64.urlsafe_b64encode(data).rstrip(b"=").decode("ascii")

cpf, key, issuer, audience = sys.argv[1], sys.argv[2], sys.argv[3], sys.argv[4]
digits = "".join(c for c in cpf if c.isdigit())
now = int(time.time())
header = b64url(json.dumps({"alg": "HS256", "typ": "JWT"}, separators=(",", ":")).encode())
payload = b64url(json.dumps({
    "cpf": digits,
    "iss": issuer,
    "aud": audience,
    "iat": now,
    "exp": now + 30 * 60
}, separators=(",", ":")).encode())
signing_input = f"{header}.{payload}".encode()
sig = b64url(hmac.new(key.encode(), signing_input, hashlib.sha256).digest())
print(f"{header}.{payload}.{sig}")
PY
