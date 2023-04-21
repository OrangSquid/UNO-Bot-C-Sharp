#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:7.0-alpine AS base
WORKDIR /app

ENV UNO_TOKEN "YOUR TOKEN HERE"

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
COPY ["UNODiscordBot/UNODiscordBot.csproj", "UNODiscordBot/"]
COPY ["UNOLib/UNOLib.csproj", "UNOLib/"]
RUN dotnet restore "UNODiscordBot/UNODiscordBot.csproj"
COPY . .
WORKDIR "/src/UNODiscordBot"
RUN dotnet build "UNODiscordBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UNODiscordBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UNODiscordBot.dll"]