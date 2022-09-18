#!/bin/bash

log_file=/home/$USER/Desktop/log.txt

function log(){
echo "$@" | tee -a "${log_file}"
}
mono ./DuckGame.exe -nodinput -nothreading | tee -a "${log_file}"
