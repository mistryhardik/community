FROM mcr.microsoft.com/dotnet/core/aspnet:2.1-stretch-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.1-stretch AS build
WORKDIR /src
COPY FictionalWebApp/FictionalWebApp.csproj FictionalWebApp/
COPY Fictional.Repository/Fictional.Repository.csproj FictionalWebApp/
COPY Fictional.CommandResults/Fictional.CommandResults.csproj FictionalWebApp/
COPY Fictional.ViewModels/Fictional.ViewModels.csproj FictionalWebApp/
COPY Fictional.Commands/Fictional.Commands.csproj FictionalWebApp/
COPY Fictional.Models/Fictional.Models.csproj FictionalWebApp/

RUN dotnet restore "FictionalWebApp/FictionalWebApp.csproj"
COPY . .
WORKDIR "/src/FictionalWebApp"
RUN dotnet build "FictionalWebApp.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "FictionalWebApp.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "FictionalWebApp.dll"]
