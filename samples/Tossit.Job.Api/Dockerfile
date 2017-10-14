FROM microsoft/dotnet:1.1-sdk

COPY . /jobApp

WORKDIR /jobApp

RUN ["bash", "build.sh"]

EXPOSE 5000/tcp

WORKDIR /jobApp/samples/Tossit.Job.Api

CMD ["dotnet", "run", "--server.urls", "http://*:5000"]
