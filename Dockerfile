ARG DOTNET_VERSION=10.0
ARG TARGET_FRAMEWORK=net10.0

FROM mcr.microsoft.com/dotnet/sdk:${DOTNET_VERSION} AS build-env
WORKDIR /opt/cryptolabs

COPY ./src/ .
# Restore as distinct layers
RUN dotnet restore

# Build and publish a release
RUN dotnet publish -c Release

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:${DOTNET_VERSION}
ARG TARGET_FRAMEWORK
WORKDIR /opt/cryptolabs
COPY --from=build-env /opt/cryptolabs .

ENTRYPOINT ["sh", "-c", "dotnet CryptoLabsService/bin/Release/${TARGET_FRAMEWORK}/CryptoLabsService.dll"]

#CMD ["dotnet", "CryptoLabsService/bin/Release/${TARGET_FRAMEWORK}/CryptoLabsService.dll"]