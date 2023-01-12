FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app
COPY ./publish .
EXPOSE 7116
CMD [ "/app/bootstrap.sh" ]