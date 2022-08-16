FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /opt/cryptolabs

COPY ./src/ .
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /opt/cryptolabs
COPY --from=build-env /opt/cryptolabs .
ENTRYPOINT ["dotnet", "CryptoLabsService/bin/Release/net6.0/CryptoLabsService.dll"]


#CMD ["dotnet", "CryptoLabsService/bin/Release/net6.0/CryptoLabsService.dll"]