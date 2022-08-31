FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src

COPY ["PingPongFeed.Service/PingPongFeed.Service.csproj", "PingPongFeed.Service/"]
RUN dotnet restore "PingPongFeed.Service/PingPongFeed.Service.csproj"

COPY . .
WORKDIR "/src/PingPongFeed.Service"
RUN dotnet build "PingPongFeed.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PingPongFeed.Service.csproj" -c Release -o /app/publish -p:PublishTrimmed=true -r linux-musl-x64

ENV TZ=Asia/Bangkok

FROM base AS final
WORKDIR /app/PingPongFeed.Service
COPY --from=publish /app/publish .
ADD PingPongFeed.Service/appsettings.json appsettings.json
EXPOSE 5000

ENV ASPNETCORE_URLS=http://+:5000
ENV TZ='Asia/Bangkok'
RUN apk add --no-cache tzdata

ENTRYPOINT ["dotnet", "PingPongFeed.Service.dll"]