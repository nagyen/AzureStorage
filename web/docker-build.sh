#!/bin/bash
#publish web
sh ./publish-web.sh
#build dockerfile
docker build -t nyendluri/azure-table-storage:latest .