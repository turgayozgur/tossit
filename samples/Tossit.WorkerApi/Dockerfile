FROM microsoft/dotnet:1.1-sdk-projectjson

COPY . /workerApp

WORKDIR /workerApp

RUN ["bash", "build.sh", "netstandard16"]

EXPOSE 5001/tcp

WORKDIR /workerApp/samples/Tossit.WorkerApi

CMD ["dotnet", "run", "--server.urls", "http://*:5001"]
