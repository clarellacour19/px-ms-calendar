FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY ["DefaultApp/DefaultApp.csproj", "DefaultApp/"]
RUN dotnet restore "DefaultApp/DefaultApp.csproj"
COPY . .
WORKDIR "/src/DefaultApp"
RUN dotnet build "DefaultApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DefaultApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DefaultApp.dll"]