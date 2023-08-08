FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine AS build
WORKDIR /src
COPY ["Otus.2/Otus.2.csproj", "Otus.2/"]
RUN dotnet restore "Otus.2/Otus.2.csproj"  -r linux-musl-x64
COPY . .
WORKDIR "/src/Otus.2.Migrator"
RUN dotnet build "Otus.2.Migrator.csproj" -c Release -r linux-musl-x64 -o /app/build

FROM build AS publish
RUN dotnet publish "Otus.2.Migrator.csproj" -c Release --sc -o /app/publish -r linux-musl-x64

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["./Otus.2.Migrator"]
