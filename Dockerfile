FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.6-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["PG.ABBs.Webservices.DiaperSizerService/PG.ABBs.Webservices.DiaperSizerService/PG.ABBs.Webservices.DiaperSizerService.csproj", "./"]
RUN dotnet restore
COPY . ./
WORKDIR "/src/"
# Install OpenJDK-8
RUN apt-get update && \
    apt-get install -y software-properties-common && \
    add-apt-repository ppa:linuxuprising/java && \
    apt-get install -y openjdk-11-jdk && \
    apt-get install -y ant && \
    apt-get clean;
# Setup JAVA_HOME -- useful for docker commandline
ENV JAVA_HOME /usr/lib/jvm/java-11-openjdk-amd64/

# Begin sonar
RUN dotnet tool install --global dotnet-sonarscanner --version 5.3.1
ENV PATH="${PATH}:/root/.dotnet/tools"
RUN dotnet sonarscanner begin /k:"PX-Pampers-Microservices" /d:sonar.login="87fc8564b03ef79f0a14ef21c2511ba99bc3106c" /d:sonar.host.url="https://sonarqubeenterprise.pgcloud.com/sonarqube" /d:sonar.branch.name="DS"
RUN dotnet build -c Release -o /app
RUN dotnet sonarscanner end /d:sonar.login="87fc8564b03ef79f0a14ef21c2511ba99bc3106c"
# End sonar

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "PG.ABBs.Webservices.DiaperSizerService.dll"]