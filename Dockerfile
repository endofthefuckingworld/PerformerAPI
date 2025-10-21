# === build stage ===
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 建議先只複製 csproj 再還原（避免快取汙染）
COPY PerformerApi.csproj ./
RUN dotnet restore

# 再複製剩餘原始碼並發佈
COPY . .
RUN dotnet publish -c Release -o /app

# === runtime stage ===
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app ./

# Cloud Run 會注入環境變數 PORT；要監聽該埠
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}
# Cloud Run 習慣用 8080，但以 PORT 為準
EXPOSE 8080

ENTRYPOINT ["dotnet", "PerformerApi.dll"]