FROM ://microsoft.com AS base
WORKDIR /app
EXPOSE 8080

FROM ://microsoft.com AS build
WORKDIR /src

# Копируем проекты для кэширования слоев
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
RUN dotnet publish "User.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "User.Presentation.dll"]