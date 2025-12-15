# =============================
# Etapa base com SDK (Dev)
# =============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dev
WORKDIR /src

# Argumentos para NuGet privado
ARG NUGET_SOURCE_URL
ARG NUGET_USERNAME
ARG NUGET_PASSWORD
ARG AZURE_DEVOPS_PAT

# Configurar NuGet source privado se fornecido
RUN if [ ! -z "$NUGET_SOURCE_URL" ] && [ ! -z "$NUGET_USERNAME" ] && [ ! -z "$NUGET_PASSWORD" ]; then \
    dotnet nuget add source "$NUGET_SOURCE_URL" --name "PrivateNuGet" --username "$NUGET_USERNAME" --password "$NUGET_PASSWORD" --store-password-in-clear-text; \
    fi

# Configurar Azure DevOps feed
RUN if [ ! -z "$AZURE_DEVOPS_PAT" ]; then \
    dotnet nuget add source "https://pkgs.dev.azure.com/async6/_packaging/async6/nuget/v3/index.json" --name "async6" --username "az" --password "$AZURE_DEVOPS_PAT" --store-password-in-clear-text; \
    fi

# Copia arquivos de projeto primeiro
COPY *.sln ./
COPY CommunicationService.API/*.csproj CommunicationService.API/
COPY CommunicationService.Domain/*.csproj CommunicationService.Domain/
COPY CommunicationService.CrossCutting/*.csproj CommunicationService.CrossCutting/
COPY CommunicationService.Application/*.csproj CommunicationService.Application/
COPY CommunicationService.Infrastructure/*.csproj CommunicationService.Infrastructure/
COPY CommunicationService.Persistence/*.csproj CommunicationService.Persistence/
COPY CommunicationService.Contracts/*.csproj CommunicationService.Contracts/
COPY CommunicationService.ExternalServices/*.csproj CommunicationService.ExternalServices/
COPY CommunicationService.Jobs/*.csproj CommunicationService.Jobs/
COPY CommunicationService.Tests/*.csproj CommunicationService.Tests/

# Restore packages
RUN dotnet restore CommunicationService.API/CommunicationService.API.csproj

# Copia todo o código
COPY . .

# Expõe porta padrão da API
EXPOSE 5000

# Comando para dev (hot reload)
ENTRYPOINT ["dotnet", "watch", "--project", "CommunicationService.API/CommunicationService.API.csproj", "run", "--urls", "http://0.0.0.0:5000"]

# =============================
# Etapa de Build (Prod)
# =============================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG DOTNET_CONFIGURATION=Release
WORKDIR /src

# Argumentos para autenticação do Azure DevOps
ARG AZURE_DEVOPS_PAT

# Create nuget.config with Azure DevOps package source and credentials
RUN echo '<?xml version="1.0" encoding="utf-8"?>' > nuget.config && \
    echo '<configuration>' >> nuget.config && \
    echo '  <packageSources>' >> nuget.config && \
    echo '    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />' >> nuget.config && \
    echo '    <add key="async6" value="https://pkgs.dev.azure.com/async6/_packaging/async6/nuget/v3/index.json" />' >> nuget.config && \
    echo '  </packageSources>' >> nuget.config && \
    if [ ! -z "$AZURE_DEVOPS_PAT" ]; then \
    echo '  <packageSourceCredentials>' >> nuget.config && \
    echo '    <async6>' >> nuget.config && \
    echo '      <add key="Username" value="az" />' >> nuget.config && \
    echo '      <add key="ClearTextPassword" value="'$AZURE_DEVOPS_PAT'" />' >> nuget.config && \
    echo '    </async6>' >> nuget.config && \
    echo '  </packageSourceCredentials>' >> nuget.config; \
    fi && \
    echo '</configuration>' >> nuget.config

# Copiar apenas os .csproj para cache eficiente
COPY CommunicationService.API/CommunicationService.API.csproj CommunicationService.API/
COPY CommunicationService.Domain/CommunicationService.Domain.csproj CommunicationService.Domain/
COPY CommunicationService.CrossCutting/CommunicationService.CrossCutting.csproj CommunicationService.CrossCutting/
COPY CommunicationService.Application/CommunicationService.Application.csproj CommunicationService.Application/
COPY CommunicationService.Infrastructure/CommunicationService.Infrastructure.csproj CommunicationService.Infrastructure/
COPY CommunicationService.Persistence/CommunicationService.Persistence.csproj CommunicationService.Persistence/
COPY CommunicationService.Contracts/CommunicationService.Contracts.csproj CommunicationService.Contracts/
COPY CommunicationService.ExternalServices/CommunicationService.ExternalServices.csproj CommunicationService.ExternalServices/
COPY CommunicationService.Jobs/CommunicationService.Jobs.csproj CommunicationService.Jobs/
COPY CommunicationService.Tests/CommunicationService.Tests.csproj CommunicationService.Tests/

RUN dotnet restore CommunicationService.API/CommunicationService.API.csproj

# Copiar todo o restante do código
COPY . .

# Instalar Entity Framework CLI para migrations
RUN dotnet tool install --global dotnet-ef --version 8.0.11

# Publish Release
RUN dotnet publish CommunicationService.API/CommunicationService.API.csproj -c Release -o /app/publish

# =============================
# Runtime Stage (Prod)
# =============================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Instalar ferramentas necessárias
RUN apt-get update && apt-get install -y --no-install-recommends \
    curl netcat-openbsd \
    && rm -rf /var/lib/apt/lists/*

# Copiar aplicação publicada
COPY --from=build /app/publish .

# Copiar Entity Framework CLI do estágio de build
COPY --from=build /root/.dotnet/tools /root/.dotnet/tools
ENV PATH="$PATH:/root/.dotnet/tools"

# Configurar variáveis de ambiente para logging
ENV ASPNETCORE_ENVIRONMENT=Production
ENV Logging__LogLevel__Default=Information
ENV Logging__LogLevel__Microsoft=Information
ENV Logging__LogLevel__Microsoft.AspNetCore=Information
ENV Logging__Console__FormatterName=json

# Copiar e configurar script de entrada
COPY entrypoint.sh /app/entrypoint.sh
RUN chmod +x /app/entrypoint.sh && \
    sed -i 's/\r$//' /app/entrypoint.sh

EXPOSE 5000
ENTRYPOINT ["/app/entrypoint.sh"]
