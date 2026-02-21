# 1. Build the Angular Frontend
FROM node:20-alpine AS build-frontend
WORKDIR /app/frontend

# Copy the angular project (change 'client/' if your frontend folder has a different name)
COPY ["ClientApp/", "./"]
RUN npm install

# Build the app. Angular 17+ creates a 'browser' folder inside the dist output
RUN npm run build

# 2. Build the .NET Backend
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-backend
WORKDIR /app

# Copy the solution and project files
COPY ["NetStore.sln", "./"]
COPY ["API/API.csproj", "API/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "NetStore.sln"

# Copy the rest of the backend code
COPY . .
WORKDIR "/app/API"
RUN dotnet publish "API.csproj" -c Release -o /app/out

# 3. Final Runtime Image (Combine Both)
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-backend /app/out .

# Magic Step: Copy the Angular build from Stage 1 into the .NET wwwroot folder
# *NOTE: adjust 'client' below if your angular app folder is named differently*
COPY --from=build-frontend /app/API/wwwroot/browser ./wwwroot

EXPOSE 8080
ENTRYPOINT ["dotnet", "API.dll"]