# 1. Use the .NET 10 SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy the solution file
COPY ["NetStore.sln", "./"]

# Copy all project files into their respective folders
COPY ["API/API.csproj", "API/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]

# Restore dependencies for the entire solution
RUN dotnet restore "NetStore.sln"

# Copy everything else and build
COPY . .
# Move into the API folder and build/publish it
WORKDIR "/app/API"
RUN dotnet publish "API.csproj" -c Release -o /app/out

# 2. Build the final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app

COPY --from=build /app/out .

# Tell Azure the app runs on port 8080 (Modern .NET default)
EXPOSE 8080
ENTRYPOINT ["dotnet", "API.dll"]