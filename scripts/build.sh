#!/usr/bin/env bash

pushd `dirname $0` > /dev/null
SCRIPT_PATH=`pwd -P`
cd $SCRIPT_PATH/
BIN_PATH=$SCRIPT_PATH/../bin

mkdir -p $BIN_PATH/java
cd ../java-user-list
mvn clean install -Dmaven.test.skip && cp target/java-user-list-1.0.0.jar $BIN_PATH/java/
cp resources/scripts/* $BIN_PATH/java/
cd $SCRIPT_PATH

mkdir -p $BIN_PATH/go
cd ../go-admin
go build && mv go-admin $BIN_PATH/go/ 
cp resources/scripts/* $BIN_PATH/go/
cd $SCRIPT_PATH

mkdir -p $BIN_PATH/php
cd ../php-frontend
cp frontend.php $BIN_PATH/php/ && cp -r vendor $BIN_PATH/php/
cp resources/scripts/* $BIN_PATH/php/
cd $SCRIPT_PATH

cp -r mysql $BIN_PATH/
cp -r redis $BIN_PATH/