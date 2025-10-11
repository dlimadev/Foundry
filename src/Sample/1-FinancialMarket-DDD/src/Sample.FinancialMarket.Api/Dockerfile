# Estágio 1: Build (usando o SDK completo)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# O 'context' do build é a raiz da solução (definido no docker-compose.yml).
# Portanto, todos os caminhos de cópia começam a partir de lá.
# Copia todos os arquivos .csproj e restaura as dependências primeiro para otimizar o cache.
COPY ["src/samples/1-FinancialMarket-DDD/Sample.FinancialMarket.Api/Sample.FinancialMarket.Api.csproj", "src/samples/1-FinancialMarket-DDD/Sample.FinancialMarket.Api/"]
COPY ["src/samples/1-FinancialMarket-DDD/Sample.FinancialMarket.Application/Sample.FinancialMarket.Application.csproj", "src/samples/1-FinancialMarket-DDD/Sample.FinancialMarket.Application/"]
COPY ["src/samples/1-FinancialMarket-DDD/Sample.FinancialMarket.Domain/Sample.FinancialMarket.Domain.csproj", "src/samples/1-FinancialMarket-DDD/Sample.FinancialMarket.Domain/"]
COPY ["src/samples/1-FinancialMarket-DDD/Sample.FinancialMarket.Infrastructure/Sample.FinancialMarket.Infrastructure.csproj", "src/samples/1-FinancialMarket-DDD/Sample.FinancialMarket.Infrastructure/"]
COPY ["src/framework/Foundry.Domain/Foundry.Domain.csproj", "src/framework/Foundry.Domain/"]
COPY ["src/framework/Foundry.Application.Abstractions/Foundry.Application.Abstractions.csproj", "src/framework/Foundry.Application.Abstractions/"]
COPY ["src/framework/Foundry.Infrastructure/Foundry.Infrastructure.csproj", "src/framework/Foundry.Infrastructure/"]
COPY ["src/framework/Foundry.Api.BuildingBlocks/Foundry.Api.BuildingBlocks.csproj", "src/framework/Foundry.Api.BuildingBlocks/"]
RUN dotnet restore "src/samples/1-FinancialMarket-DDD/Sample.FinancialMarket.Api/Sample.FinancialMarket.Api.csproj"

# Copia todo o resto do código-fonte
COPY . .

# Publica a aplicação, gerando os arquivos otimizados para execução
WORKDIR "/src/src/samples/1-FinancialMarket-DDD/Sample.FinancialMarket.Api"
RUN dotnet publish "Sample.FinancialMarket.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio 2: Final (usando a imagem de runtime, muito mais leve)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Sample.FinancialMarket.Api.dll"]