#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["TennisCourtBookings.WebApi/TennisCourtBookings.WebApi.csproj", "TennisCourtBookings.WebApi/"]
COPY ["TennisCourtBookings.Application/TennisCourtBookings.Application.csproj", "TennisCourtBookings.Application/"]
COPY ["TennisCourtBookings.Domain/TennisCourtBookings.Domain.csproj", "TennisCourtBookings.Domain/"]
COPY ["TennisCourtBookings.Persistence/TennisCourtBookings.Persistence.csproj", "TennisCourtBookings.Persistence/"]
RUN dotnet restore "./TennisCourtBookings.WebApi/./TennisCourtBookings.WebApi.csproj"
COPY . .
WORKDIR "/src/TennisCourtBookings.WebApi"
RUN dotnet build "./TennisCourtBookings.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./TennisCourtBookings.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TennisCourtBookings.WebApi.dll"]