FROM mcr.microsoft.com/dotnet/runtime-deps:7.0-alpine AS base
WORKDIR /app
EXPOSE 8000

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Otus.2/Otus.2.csproj", "Otus.2/"]
RUN dotnet restore "Otus.2/Otus.2.csproj"  -r linux-musl-x64
COPY . .
WORKDIR "/src/Otus.2"
RUN dotnet build "Otus.2.csproj" -c Release -r linux-musl-x64 -o /app/build

FROM build AS publish
RUN dotnet publish "Otus.2.csproj" --sc -p:PublishTrimmed=true -c Release -o /app/publish -r linux-musl-x64

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ["./Otus.2"]
