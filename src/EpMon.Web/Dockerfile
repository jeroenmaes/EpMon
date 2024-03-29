#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
COPY ["src/EpMon.Web/EpMon.Web.csproj", "src/EpMon.Web/"]
COPY ["src/EpMon/EpMon.csproj", "src/EpMon/"]
COPY ["src/EpMon.Data/EpMon.Data.csproj", "src/EpMon.Data/"]
RUN dotnet restore "src/EpMon.Web/EpMon.Web.csproj"
COPY . .
WORKDIR "/src/src/EpMon.Web"
RUN dotnet build "EpMon.Web.csproj" -c Release -o /app/build --no-restore

FROM build AS publish
RUN dotnet publish "EpMon.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EpMon.Web.dll"]