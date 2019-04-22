FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as sdk

WORKDIR /app

COPY src ./

RUN dotnet build -c Release \
    && dotnet publish -c Release -o /published --no-restore EAVStore.Api/EAVStore.Api.csproj

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app

COPY --from=sdk /published ./

ENTRYPOINT [ "dotnet", "EAVStore.Api.dll" ]