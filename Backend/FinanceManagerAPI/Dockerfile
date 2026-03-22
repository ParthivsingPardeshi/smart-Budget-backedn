FROM mcr.microsoft.com/dotnet/sdk:8.0

WORKDIR /app

COPY . .

WORKDIR /app/Backend/FinanceManagerAPI

RUN dotnet publish -c Release -o out

CMD ["dotnet", "out/FinanceManagerAPI.dll", "--urls", "http://0.0.0.0:8080"]
