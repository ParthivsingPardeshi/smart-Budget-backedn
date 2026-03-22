FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

COPY . .

# 🔍 Find and publish using explicit path
RUN dotnet publish Backend/FinanceManagerAPI/FinanceManagerAPI.csproj -c Release -o out

CMD ["dotnet", "out/FinanceManagerAPI.dll", "--urls", "http://0.0.0.0:8080"]