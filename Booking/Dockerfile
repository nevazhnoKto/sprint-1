FROM ://microsoft.com AS base
WORKDIR /app
EXPOSE 8080

FROM ://microsoft.com AS build
WORKDIR /src

# Копируем проекты для кэширования слоев
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
RUN dotnet publish "Booking.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Booking.Presentation.dll"]