FROM openjdk:8u171-jdk-slim

RUN mkdir /app

COPY ./java-user-list-mysql-1.0.0.jar /app
COPY ./bootstrap.sh  /app
WORKDIR /app

EXPOSE 18888

CMD [ "/app/bootstrap.sh" ]
