FROM mcr.microsoft.com/dotnet/core/sdk:2.1-bionic

WORKDIR /opt/cryptolabs

COPY src/ .
RUN dotnet publish --configuration Release

CMD ["dotnet", "CryptoLabsService/bin/Release/netcoreapp2.0/CryptoLabsService.dll"]
