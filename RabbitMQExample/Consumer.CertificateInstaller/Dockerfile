FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Consumer.CertificateInstaller/Consumer.CertificateInstaller.csproj", "Consumer.CertificateInstaller/"]
RUN dotnet restore "Consumer.CertificateInstaller/Consumer.CertificateInstaller.csproj"
COPY . .
WORKDIR "/src/Consumer.CertificateInstaller"
RUN dotnet build "Consumer.CertificateInstaller.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Consumer.CertificateInstaller.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Consumer.CertificateInstaller.dll"]
