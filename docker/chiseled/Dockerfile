# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0-noble-chiseled-extra AS base
USER app

WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0-noble AS build
ARG BUILD_CONFIGURATION=Release
ARG buildno=local
WORKDIR /src

COPY ["PlaywrightPdfGenerator.ApiService/PlaywrightPdfGenerator.ApiService.csproj", "PlaywrightPdfGenerator.ApiService/"]
COPY ["PlaywrightPdfGenerator.Logic/PlaywrightPdfGenerator.Logic.csproj", "PlaywrightPdfGenerator.Logic/"]
COPY ["PlaywrightPdfGenerator.ServiceDefaults/PlaywrightPdfGenerator.ServiceDefaults.csproj", "PlaywrightPdfGenerator.ServiceDefaults/"]

RUN dotnet restore "PlaywrightPdfGenerator.ApiService/PlaywrightPdfGenerator.ApiService.csproj"

COPY . .

WORKDIR /src/PlaywrightPdfGenerator.ApiService

RUN dotnet build "PlaywrightPdfGenerator.ApiService.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PlaywrightPdfGenerator.ApiService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage installs Playwright and its dependencies (incl. custom fonts)
FROM build AS playwright-install
WORKDIR /app
COPY --from=publish /app/publish .

# Install PowerShell
RUN apt-get update && \
    apt-get install -y apt-transport-https software-properties-common wget && \
    wget -q "https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb" && \
    dpkg -i packages-microsoft-prod.deb && \
    rm packages-microsoft-prod.deb && \
    apt-get update && \
    apt-get install -y powershell && \
    rm -rf /var/lib/apt/lists/*

# Install Playwright browsers and dependencies
RUN pwsh playwright.ps1 install --with-deps --only-shell chromium

# Uninstall PowerShell and related packages after Playwright installation
RUN apt-get remove -y powershell apt-transport-https software-properties-common wget && \
    apt-get autoremove -y && \
    rm -rf /var/lib/apt/lists/*

# Create custom fonts directory and copy font files
RUN mkdir -p /usr/share/fonts/custom
COPY PlaywrightPdfGenerator.Logic/Fonts/*.ttf /usr/share/fonts/custom/

# Copy font configuration
COPY PlaywrightPdfGenerator.Logic/Fonts/*.conf /etc/fonts/conf.d/

RUN cd /usr/share/fonts && \
# Remove entire truetype directory (contains large fonts)
rm -rf /usr/share/fonts/truetype && \
# Remove entire opentype directory (contains large fonts)
rm -rf /usr/share/fonts/opentype && \
# Remove other unnecessary font types
rm -rf /usr/share/fonts/X11

# Rebuild font cache after font cleanup and custom font installation
RUN fc-cache -f -v

# Set execute permissions on Playwright binaries before copying to final stage
RUN chmod -R u+x /root/.cache/ms-playwright
# Set execute permissions on Playwright Node.js runtime in the published app
RUN chmod -R u+x /app/.playwright

# Remove Perl and other build tools
RUN apt-get remove -y perl perl-modules-* && \
    apt-get autoremove -y && \
    rm -rf /usr/share/perl*

# Remove HUGE unnecessary libraries from /usr/lib/x86_64-linux-gnu
RUN cd /usr/lib/x86_64-linux-gnu && \
    # Graphics/3D (Chromium uses software rendering for PDF)
    rm -f libLLVM*.so* && \
    rm -f libclang*.so* && \
    rm -f libicudata.so.* && \
    rm -f libcrypto.so.* && \
    rm -f libicui18n.so.* && \
    rm -f libgallium*.so*

# Remove unnecessary documentation
RUN rm -rf /usr/share/doc/* && \
    rm -rf /usr/share/man/* && \
    rm -rf /usr/share/info/*

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final

WORKDIR /app
COPY --from=publish /app/publish .

# Copy Playwright Node.js runtime with execute permissions from playwright-install stage
COPY --chown=app:app --from=playwright-install /app/.playwright /app/.playwright

# Copy Playwright browsers and dependencies to app's home directory with correct ownership
COPY --chown=app:app --from=playwright-install /root/.cache/ms-playwright /home/app/.cache/ms-playwright

# Copy required system libraries from playwright-install stage
COPY --chown=app:app --from=playwright-install /usr/lib/x86_64-linux-gnu /usr/lib/x86_64-linux-gnu

# Copy fonts and font configuration (includes custom fonts)
COPY --chown=app:app --from=playwright-install /usr/share/fonts /usr/share/fonts
COPY --chown=app:app --from=playwright-install /usr/share/fontconfig /usr/share/fontconfig
COPY --chown=app:app --from=playwright-install /etc/fonts /etc/fonts
COPY --chown=app:app --from=playwright-install /var/cache/fontconfig /var/cache/fontconfig

# Copy additional directories that Playwright's --with-deps installs
COPY --chown=app:app --from=playwright-install /usr/share/X11 /usr/share/X11
COPY --chown=app:app --from=playwright-install /usr/share/libdrm /usr/share/libdrm
COPY --chown=app:app --from=playwright-install /usr/share/glib-2.0 /usr/share/glib-2.0

USER $APP_UID
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENTRYPOINT ["dotnet", "PlaywrightPdfGenerator.ApiService.dll"]

### FINAL ###
