FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY IgniteTourApp/IgniteTourApp.csproj IgniteTourApp/
COPY IgniteTourApp.Repository/IgniteTourApp.Repository.csproj IgniteTourApp/

RUN dotnet restore IgniteTourApp/IgniteTourApp.csproj
COPY . .
WORKDIR /src/IgniteTourApp
RUN dotnet build IgniteTourApp.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish IgniteTourApp.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IgniteTourApp.dll"]
