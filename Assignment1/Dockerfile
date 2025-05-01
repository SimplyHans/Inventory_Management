# Use the official ASP.NET 8.0 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use the .NET 8.0 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Assignment1/Assignment1.csproj", "Assignment1/"]
RUN dotnet restore "Assignment1/Assignment1.csproj"
COPY . .
WORKDIR "/src/Assignment1"
RUN dotnet build "Assignment1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Assignment1.csproj" -c Release -o /app/publish

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Assignment1.dll"]
