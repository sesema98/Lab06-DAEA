#!/usr/bin/env bash
set -euo pipefail
if ! command -v dotnet >/dev/null 2>&1; then
  echo "Falta .NET 8 SDK: https://dotnet.microsoft.com/download/dotnet/8.0"
  exit 1
fi
echo "SDK detectado: $(dotnet --version)"
dotnet restore Semana04-XS/Semana04-XS.csproj
dotnet build Semana04-XS/Semana04-XS.csproj --configuration Release
echo "Compilación correcta. La interfaz WPF se ejecuta y prueba en Windows."
