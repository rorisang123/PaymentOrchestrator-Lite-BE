FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["PaymentOrchestrator-Lite-BE.csproj", "."]
RUN dotnet restore "PaymentOrchestrator-Lite-BE.csproj"

COPY . .
WORKDIR "/src"
RUN dotnet build "PaymentOrchestrator-Lite-BE.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PaymentOrchestrator-Lite-BE.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "PaymentOrchestrator-Lite-BE.dll"]