#!/usr/bin/env bash
cd $(dirname "$0")
mono ./DuckGame.exe $@ | tee outputlog.txt
