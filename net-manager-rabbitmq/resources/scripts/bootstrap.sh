#!/bin/bash
pushd `dirname $0` > /dev/null
SCRIPT_PATH=`pwd -P`
cd $SCRIPT_PATH/
# EASEAGENT_CONFIG=$SCRIPT_PATH/agent.yml
# RABBITMQ_HOST=127.0.0.1
# RABBITMQ_PORT=5672
# USER_ENDPOINT=http://127.0.0.1:18888
ASPNETCORE_URLS="http://0.0.0.0:7116" dotnet publish/net-manager-rabbitmq.dll
