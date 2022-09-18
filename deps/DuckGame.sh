#!/bin/bash

log_file=/home/$USER/Desktop/log.txt

function log(){
echo "$@" | tee -a "${log_file}"
}
mono /home/dan/Desktop/bin5/DuckGame.exe -nodinput -nothreading | tee -a "${log_file}"
