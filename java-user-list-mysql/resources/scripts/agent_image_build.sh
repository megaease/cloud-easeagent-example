#!/bin/bash
pushd `dirname $0` > /dev/null
SCRIPT_PATH=`pwd -P`
cd $SCRIPT_PATH/

tag=$1
if [ "$tag" == "" ];then
    echo "please add tag commend like: ./agent_image_build.sh agent_1.0.0"
    exit
fi

docker build -f ./DockerfileRebuild . -t megaease/java-apm-user-list-mysql-example:$tag