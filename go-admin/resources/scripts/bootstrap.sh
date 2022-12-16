#!/bin/bash
pushd `dirname $0` > /dev/null
SCRIPT_PATH=`pwd -P`
cd $SCRIPT_PATH/
# EASEAGENT_CONFIG=$SCRIPT_PATH/agent.yml
# REDIS_HOST_AND_PORT=localhost:6379
# REDIS_PASSWORD=123456
./go-admin
