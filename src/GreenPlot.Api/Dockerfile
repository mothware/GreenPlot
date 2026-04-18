FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY ["src/GreenPlot.Api/GreenPlot.Api.csproj", "src/GreenPlot.Api/"]
COPY ["src/GreenPlot.Application/GreenPlot.Application.csproj", "src/GreenPlot.Application/"]
COPY ["src/GreenPlot.Domain/GreenPlot.Domain.csproj", "src/GreenPlot.Domain/"]
COPY ["src/GreenPlot.Infrastructure/GreenPlot.Infrastructure.csproj", "src/GreenPlot.Infrastructure/"]

RUN dotnet restore "src/GreenPlot.Api/GreenPlot.Api.csproj"

COPY . .
RUN dotnet build "src/GreenPlot.Api/GreenPlot.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/GreenPlot.Api/GreenPlot.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "GreenPlot.Api.dll"]
