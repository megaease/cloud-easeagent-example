FROM php:latest

RUN mkdir /app
WORKDIR /app
ADD ./frontend.php /app/
COPY ./vendor  /app/
COPY ./agent_frontend.yml  /app/
COPY ./bootstrap.sh  /app/
EXPOSE 8081
CMD [ "/app/bootstrap.sh" ]
