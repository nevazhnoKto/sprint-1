FROM ://microsoft.com AS base
WORKDIR /app
EXPOSE 8080

FROM ://microsoft.com AS build
WORKDIR /src

# Копируем проекты для кэширования слоев
COPY ["SharedContract/SharedContract/SharedContract.csproj", "SharedContract/SharedContract/"]
COPY ["Event/Event.Presentation/Event.Presentation.csproj", "Event/Event.Presentation/"]
COPY ["Event/Event.Application/Event.Application.csproj", "Event/Event.Application/"]
COPY ["Event/Event.Domain/Event.Domain.csproj", "Event/Event.Domain/"]
COPY ["Event/Event.Infrastructure/Event.Infrastructure.csproj", "Event/Event.Infrastructure/"]

RUN dotnet restore "Event/Event.Presentation/Event.Presentation.csproj"

COPY . .
WORKDIR "/src/Event/Event.Presentation"
RUN dotnet build "Event.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Event.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Event.Presentation.dll"]