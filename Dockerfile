# Sử dụng hình ảnh .NET 8 ASP.NET Core runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

# Sử dụng hình ảnh .NET 8 SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Sao chép solution file và project file để thực hiện restore
COPY *.sln .
COPY ShopApp/*.csproj ./ShopApp/
RUN dotnet restore ./ShopApp/ShopApp.csproj

# Sao chép toàn bộ mã nguồn và build ứng dụng
COPY ShopApp/. ./ShopApp/
WORKDIR /src/ShopApp
RUN dotnet build -c Release -o /app/build

# Publish ứng dụng
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish --no-restore

# Tạo giai đoạn final từ runtime base
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShopApp.dll"]


# docker build -t shopapp-asp:1.0.0 -f ./Dockerfile .
# docker tag shopapp-asp:1.0.0 tuanflute/asp-web:1.0.0
# docker push tuanflute/asp-web:1.0.0