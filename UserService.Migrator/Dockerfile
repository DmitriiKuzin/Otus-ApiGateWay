FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
COPY ["UserService/UserService.csproj", "UserService/"]
RUN dotnet restore "UserService/UserService.csproj"  -r linux-musl-x64
COPY . .
WORKDIR "/src/UserService.Migrator"
RUN dotnet build "UserService.Migrator.csproj" -c Release -r linux-musl-x64 -o /app/build

FROM build AS publish
RUN dotnet publish "UserService.Migrator.csproj" -c Release --sc -o /app/publish -r linux-musl-x64

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["./UserService.Migrator"]
