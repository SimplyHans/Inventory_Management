# Use the official .NET runtime as the base image
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

# Use the SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["Assignment1/Assignment1.csproj", "Assignment1/"]
RUN dotnet restore "Assignment1/Assignment1.csproj"

# Copy the entire project and build it
COPY . .
WORKDIR "/src/Assignment1"
RUN dotnet build "Assignment1.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "Assignment1.csproj" -c Release -o /app/publish

# Final stage: run the app
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Assignment1.dll"]
