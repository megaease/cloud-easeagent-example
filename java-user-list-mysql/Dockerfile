FROM openjdk:8u171-jdk-slim

RUN mkdir /jdbc-demo

COPY ./target/jdbc-demo-1.0.0.jar /jdbc-demo
COPY ./target/classes/application.yml /jdbc-demo
COPY ./target/classes/application-dev.yml /jdbc-demo
COPY ./resources/scripts/bootstrap.sh  /
COPY ./target/easeagent-dep.jar /easeagent-volume/easeagent.jar

VOLUME /tmp

WORKDIR /jdbc-demo

EXPOSE 38100

CMD [ "/bootstrap.sh" ]
