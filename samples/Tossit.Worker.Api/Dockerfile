FROM microsoft/dotnet:1.1-sdk

COPY . /workerApp

WORKDIR /workerApp

RUN ["bash", "build.sh"]

EXPOSE 5001/tcp

WORKDIR /workerApp/samples/Tossit.Worker.Api

CMD ["dotnet", "run", "--server.urls", "http://*:5001"]
