FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY webapp.csproj webapp/
RUN dotnet restore webapp/webapp.csproj
WORKDIR /src/webapp
COPY . .
RUN dotnet build webapp.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish webapp.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "webapp.dll"]
EXPOSE 80