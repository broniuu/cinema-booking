FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["CinemaBooking.Web.csproj", "."]
RUN dotnet restore "CinemaBooking.Web.csproj"
COPY . .
RUN dotnet build "./CinemaBooking.Web.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./CinemaBooking.Web.csproj" -c $BUILD_CONFIGURATION -o /app/publish

FROM base AS final
WORKDIR /app
COPY ["instruction.md", "."]
COPY ["instruction.pl.md", "."]
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CinemaBooking.Web.dll"]

