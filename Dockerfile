# Sử dụng hình ảnh .NET 8 ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Sử dụng hình ảnh .NET 8 SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY ShopApp/ShopApp.csproj /app/ShopApp/

RUN dotnet restore /app/ShopApp/ShopApp.csproj
COPY . .

WORKDIR /app/ShopApp
RUN dotnet build ShopApp.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish ShopApp.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "ShopApp.dll"]
