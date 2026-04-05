# Use the official .NET SDK image to build and publish the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the csproj and restore dependencies (cached layer)
COPY ["finance_backend.csproj", "./"]
RUN dotnet restore "finance_backend.csproj"

# Copy the remaining source code and publish the app
COPY . .
RUN dotnet publish "finance_backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the lightweight ASP.NET runtime image for the final production server
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Listen on port 8080 (standard for Railway, Google Cloud Run, and Render)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "finance_backend.dll"]
