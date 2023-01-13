FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app
COPY ./publish ./publish
COPY ./bootstrap.sh .
EXPOSE 7116
CMD [ "/app/bootstrap.sh" ]