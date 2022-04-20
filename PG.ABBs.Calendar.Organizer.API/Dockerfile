#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-buster-slim AS build
WORKDIR /src
COPY ["PG.ABBs.Calendar.Organizer.API/PG.ABBs.Calendar.Organizer.API.csproj", "PG.ABBs.Calendar.Organizer.API/"]
COPY ["PG.ABBs.Calendar.Organizer.LogService/PG.ABBs.Calendar.Organizer.LogService.csproj", "PG.ABBs.Calendar.Organizer.LogService/"]
COPY ["PG.ABBs.Calendar.Organizer.LogAnalytics/PG.ABBs.Calendar.Organizer.LogAnalytics.csproj", "PG.ABBs.Calendar.Organizer.LogAnalytics/"]
COPY ["PG.ABBs.Calendar.Organizer.DependencyResolution/PG.ABBs.Calendar.Organizer.DependencyResolution.csproj", "PG.ABBs.Calendar.Organizer.DependencyResolution/"]
COPY ["PG.ABBs.Calendar.Organizer.Service/PG.ABBs.Calendar.Organizer.Service.csproj", "PG.ABBs.Calendar.Organizer.Service/"]
COPY ["PG.ABBs.Calendar.Organizer.Content/PG.ABBs.Calendar.Organizer.Content.csproj", "PG.ABBs.Calendar.Organizer.Content/"]
COPY ["PG.ABBs.Calendar.Organizer.AzureStorage/PG.ABBs.Calendar.Organizer.AzureStorage.csproj", "PG.ABBs.Calendar.Organizer.AzureStorage/"]
COPY ["PG.ABBs.Calendar.Organizer.Data/PG.ABBs.Calendar.Organizer.Data.csproj", "PG.ABBs.Calendar.Organizer.Data/"]
RUN dotnet restore "PG.ABBs.Calendar.Organizer.API/PG.ABBs.Calendar.Organizer.API.csproj"
COPY . .
WORKDIR "/src/PG.ABBs.Calendar.Organizer.API"
RUN dotnet build "PG.ABBs.Calendar.Organizer.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PG.ABBs.Calendar.Organizer.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PG.ABBs.Calendar.Organizer.API.dll"]