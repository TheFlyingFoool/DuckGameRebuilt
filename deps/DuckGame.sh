#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")" || exit 1
export DYLD_LIBRARY_PATH="$PWD:${DYLD_LIBRARY_PATH:-}"
mono ./DuckGame.exe "$@" 2>&1 | tee outputlog.txt
