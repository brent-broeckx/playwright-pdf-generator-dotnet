# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER root
WORKDIR /app

EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all project files
COPY ["PlaywrightPdfGenerator.ApiService/PlaywrightPdfGenerator.ApiService.csproj", "PlaywrightPdfGenerator.ApiService/"]
COPY ["PlaywrightPdfGenerator.Logic/PlaywrightPdfGenerator.Logic.csproj", "PlaywrightPdfGenerator.Logic/"]
COPY ["PlaywrightPdfGenerator.ServiceDefaults/PlaywrightPdfGenerator.ServiceDefaults.csproj", "PlaywrightPdfGenerator.ServiceDefaults/"]

# Restore dependencies
RUN dotnet restore "PlaywrightPdfGenerator.ApiService/PlaywrightPdfGenerator.ApiService.csproj"

# Copy all source files
COPY . .

# Build the ApiService project
WORKDIR "/src/PlaywrightPdfGenerator.ApiService"
RUN dotnet build "PlaywrightPdfGenerator.ApiService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Copy custom fonts
RUN mkdir -p /custom-fonts
COPY ["PlaywrightPdfGenerator.Logic/Fonts/*.ttf", "PlaywrightPdfGenerator.Logic/Fonts/*.woff", "PlaywrightPdfGenerator.Logic/Fonts/*.woff2", "/custom-fonts/"]


# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR "/src/PlaywrightPdfGenerator.ApiService"
RUN dotnet publish "PlaywrightPdfGenerator.ApiService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

COPY --from=publish /app/publish .

# Install PowerShell
RUN apt-get update && \
    apt-get install -y wget apt-transport-https software-properties-common && \
    wget -q "https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb" && \
    dpkg -i packages-microsoft-prod.deb && \
    rm packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y powershell && \
    rm -rf /var/lib/apt/lists/*

# Install Playwright with Chromium and dependencies
RUN pwsh playwright.ps1 install --with-deps --only-shell chromium

# Copy and install custom fonts
RUN mkdir -p /usr/share/fonts/custom
COPY --from=build /custom-fonts/* /usr/share/fonts/custom/
RUN fc-cache -f -v

ENTRYPOINT ["dotnet", "PlaywrightPdfGenerator.ApiService.dll"]
