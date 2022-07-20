FROM mcr.microsoft.com/dotnet/sdk:6.0

WORKDIR /opt/cryptolabs

COPY src/ .
RUN dotnet publish --configuration Release

CMD ["dotnet", "CryptoLabsService/bin/Release/net6.0/CryptoLabsService.dll"]
