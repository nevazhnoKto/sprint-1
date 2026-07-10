FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["SharedContract/SharedContract/SharedContract.csproj", "SharedContract/SharedContract/"]
COPY ["User/User.Presentation/User.Presentation.csproj", "User/User.Presentation/"]
COPY ["User/User.Application/User.Application.csproj", "User/User.Application/"]
COPY ["User/User.Domain/User.Domain.csproj", "User/User.Domain/"]
COPY ["User/User.Infrastructure/User.Infrastructure.csproj", "User/User.Infrastructure/"]

RUN dotnet restore "User/User.Presentation/User.Presentation.csproj"

COPY . .

WORKDIR "/src/User/User.Presentation"
RUN dotnet build "User.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
WORKDIR "/src/User/User.Presentation"
RUN dotnet publish "User.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
# ИСПРАВЛЕНО: используем Presentation.dll вместо User.Presentation.dll
ENTRYPOINT ["dotnet", "Presentation.dll"]