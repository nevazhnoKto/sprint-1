FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

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
WORKDIR "/src/Event/Event.Presentation"
RUN dotnet publish "Event.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# ИСПРАВЛЕНО: используем Presentation.dll вместо Event.Presentation.dll
ENTRYPOINT ["dotnet", "Presentation.dll"]