#!/bin/bash
pushd `dirname $0` > /dev/null
SCRIPT_PATH=`pwd -P`
cd $SCRIPT_PATH/
EASEAGENT_CONFIG=$SCRIPT_PATH/agent.yml ./bootstrap.sh

