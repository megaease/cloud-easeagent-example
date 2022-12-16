#!/bin/bash
pushd `dirname $0` > /dev/null
SCRIPT_PATH=`pwd -P`
cd $SCRIPT_PATH/
# EASEAGENT_CONFIG=$SCRIPT_PATH/agent.yml
# USER_LIST_URL=http://127.0.0.1:18888/user/list
# CHECK_ROOT_URL=http://127.0.0.1:8090/is_root
php -S '0.0.0.0:8081' frontend.php
