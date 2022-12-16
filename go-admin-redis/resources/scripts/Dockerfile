FROM alpine:3.14.0
RUN apk add --no-cache bash
RUN apk add --no-cache ca-certificates

RUN mkdir /app
WORKDIR /app
ADD ./linux_amd64/go-admin-redis /app/
COPY ./bootstrap.sh  /app/
EXPOSE 8090
CMD [ "/app/bootstrap.sh" ]
