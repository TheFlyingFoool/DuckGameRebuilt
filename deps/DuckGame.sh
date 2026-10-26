#!/usr/bin/env bash
cd "$(dirname "$0")" || exit 1
mono ./DuckGame.exe "$@" | tee outputlog.txt
