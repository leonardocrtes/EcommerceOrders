# 1. Estágio de Compilação (SDK do .NET 8)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia os arquivos de projeto (.csproj) e restaura as dependências
COPY EcommerceOrders.sln ./
COPY src/EcommerceOrders.Domain/*.csproj ./src/EcommerceOrders.Domain/
COPY src/EcommerceOrders.Application/*.csproj ./src/EcommerceOrders.Application/
COPY src/EcommerceOrders.Infrastructure/*.csproj ./src/EcommerceOrders.Infrastructure/
COPY src/EcommerceOrders.Api/*.csproj ./src/EcommerceOrders.Api/
COPY tests/EcommerceOrders.Tests/*.csproj ./tests/EcommerceOrders.Tests/

RUN dotnet restore ./src/EcommerceOrders.Api/EcommerceOrders.Api.csproj

# Copia todo o código-fonte restante e compila em modo Release
COPY . .
WORKDIR /app/src/EcommerceOrders.Api
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# 2. Estágio de Execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Define a porta padrão do ASP.NET Core no .NET 8
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "EcommerceOrders.Api.dll"]