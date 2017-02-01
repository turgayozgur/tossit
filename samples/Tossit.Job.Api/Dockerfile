FROM microsoft/dotnet:latest

COPY . /jobApp

WORKDIR /jobApp

RUN ["bash", "build.sh", "netstandard16"]

EXPOSE 5000/tcp

WORKDIR /jobApp/samples/Tossit.Job.Api

CMD ["dotnet", "run", "--server.urls", "http://*:5000"]
