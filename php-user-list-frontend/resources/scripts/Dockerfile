FROM php:latest

# RUN apk add --no-cache bash
# RUN apk add --no-cache ca-certificates
# RUN apk add  --no-cache curl
RUN mkdir /app
WORKDIR /app
ADD ./frontend.php /app/
ADD ./vendor  /app/vendor
ADD ./agent_frontend.yml  /app/
ADD ./bootstrap.sh  /app/
EXPOSE 8081
CMD [ "/app/bootstrap.sh" ]
