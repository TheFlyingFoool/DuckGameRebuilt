#!/bin/bash

log_file=/home/$USER/Desktop/log.txt
rm -v $log_file

function log(){
echo "$@" | tee -a "${log_file}"
}
mono ./DuckGame.exe -nodinput -nothreading | tee -a "${log_file}"
