#!/bin/bash

server="localhost"
database="PPM"
username="SA"
password=${MSSQL_PWD}

backup_file="/var/opt/mssql/data/backup.bak"

backup_query="BACKUP DATABASE [$database] TO DISK='$backup_file' WITH INIT"

opt/mssql-tools/bin/sqlcmd -S $server -d $database -U $username -P $password -Q "$backup_query"