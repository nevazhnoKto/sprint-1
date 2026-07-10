FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SharedContract/SharedContract/SharedContract.csproj", "SharedContract/SharedContract/"]
COPY ["Booking/Booking.Presentation/Booking.Presentation.csproj", "Booking/Booking.Presentation/"]
COPY ["Booking/Booking.Application/Booking.Application.csproj", "Booking/Booking.Application/"]
COPY ["Booking/Booking.Domain/Booking.Domain.csproj", "Booking/Booking.Domain/"]
COPY ["Booking/Booking.Infrastructure/Booking.Infrastructure.csproj", "Booking/Booking.Infrastructure/"]

RUN dotnet restore "Booking/Booking.Presentation/Booking.Presentation.csproj"

COPY . .

WORKDIR "/src/Booking/Booking.Presentation"
RUN dotnet build "Booking.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/Booking/Booking.Presentation"
RUN dotnet publish "Booking.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# ИСПРАВЛЕНО: используем Presentation.dll вместо Booking.Presentation.dll
ENTRYPOINT ["dotnet", "Presentation.dll"]