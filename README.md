# Playwright PDF Generator - .NET Aspire

A lightweight, production-ready .NET Aspire service for generating PDFs using Playwright and Handlebars templates. This project demonstrates how to build a scalable PDF generation API that's fully free, customizable, and container-friendly.

## About the Project

This project is built as a .NET Aspire example application that showcases:

- **PDF Generation**: Uses Playwright's browser automation to render HTML templates into high-quality PDFs
- **Template Engine**: Integrates Handlebars for flexible, dynamic template rendering
- **Containerized Deployment**: Includes optimized Docker configurations for both standard and chiseled images
- **API Service**: RESTful API endpoints for PDF generation
- **Scalable Architecture**: Built with .NET Aspire for cloud-native, distributed applications

### Project Structure

- **PlaywrightPdfGenerator.ApiService**: The main API service that handles PDF generation requests
- **PlaywrightPdfGenerator.Logic**: Core business logic and PDF generation services
- **PlaywrightPdfGenerator.AppHost**: .NET Aspire orchestration host
- **PlaywrightPdfGenerator.ServiceDefaults**: Shared service configurations
- **docker/core**: Standard Docker images (unoptimized)
- **docker/chiseled**: Optimized chiseled Docker images with reduced size

## Setup

### Prerequisites

- .NET 8.0 SDK or later
- Docker and Docker Compose
- (Optional) Visual Studio 2022 or VS Code with C# extension

### Building Docker Images

You can build either the standard or chiseled Docker images:

#### Chiseled Image (Recommended - Smaller Size)
```bash
docker build -f ./docker/chiseled/Dockerfile -t playwright-pdf-api-chiseled ./PlaywrightPdfGenerator
```

#### Core Image (Standard)
```bash
docker build -f ./docker/core/Dockerfile -t playwright-pdf-api-core ./PlaywrightPdfGenerator
```

### Running with Docker Compose

1. Navigate to the docker directory:
   ```bash
   cd docker/chiseled
   # or
   cd docker/core
   ```

2. Start the services:
   ```bash
   docker compose up
   ```

3. The API will be available at the configured port (check docker-compose.yaml for details)

## Important Notes

> **⚠️ Font Support**: Default fonts might not work as this is build to use custom fonts. If you encounter font rendering issues, you may need to modify the Dockerfile to preserve certain font directories instead of removing them or add your own custom fonts (.tff). The chiseled images remove many system files to reduce size, which can affect font availability.

> **💡 Troubleshooting**: The optimized chiseled images remove many unnecessary system files to minimize the image size. While this works perfectly for basic PDF generation with text, styling, and images, if you encounter issues specific to your use case, you may need to adjust the Dockerfile to remove fewer system files. Simply comment out or modify the cleanup steps in the Dockerfile as needed for your requirements.

## Features

- ✅ High-quality PDF generation using Chromium (via Playwright)
- ✅ Handlebars template support for dynamic content
- ✅ Custom font support
- ✅ Responsive and styled PDFs
- ✅ Image embedding
- ✅ Optimized Docker images
- ✅ RESTful API
- ✅ Production-ready error handling

## License

This project is provided as-is for demonstration and educational purposes.
