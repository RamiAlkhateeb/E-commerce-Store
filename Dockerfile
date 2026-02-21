# 1. Build the Angular Frontend
FROM node:20-alpine AS build-frontend
WORKDIR /app/frontend

# Copy the angular project (change 'client/' if your frontend folder has a different name)
COPY ["ClientApp/", "./"]
RUN npm install

# Build the app. Angular 17+ creates a 'browser' folder inside the dist output
RUN npm run build

# 1. Use the .NET 10 SDK to build the app
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-backend
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

# Magic Step: Copy the Angular build from Stage 1 into the .NET wwwroot folder
# *NOTE: adjust 'client' below if your angular app folder is named differently*
COPY --from=build-frontend /app/frontend/dist/client/browser ./wwwroot

# Tell Azure the app runs on port 8080 (Modern .NET default)
EXPOSE 8080
ENTRYPOINT ["dotnet", "API.dll"]