FROM microsoft/dotnet:latest

COPY . /app

WORKDIR /app

EXPOSE 5000/tcp

ENTRYPOINT ["dotnet", "ServiceA.dll", "--server.urls", "http://*:5000"]
