FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src
COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0

# Install diagnostics tools
RUN apt-get update && apt-get install -y \
    procps \
    curl \
    && rm -rf /var/lib/apt/lists/*


WORKDIR /app
COPY --from=build /app/publish .

# Wait for target to be ready
RUN echo '#!/bin/sh\n\
echo "Waiting for target service..."\n\
until curl -f ${TARGET_URL}/health > /dev/null 2>&1; do\n\
  echo "Target not ready, waiting..."\n\
  sleep 2\n\
done\n\
echo "Target is ready, starting NBomber..."\n\
exec dotnet NBomberTest.dll' > /app/entrypoint.sh && chmod +x /app/entrypoint.sh

ENTRYPOINT ["/app/entrypoint.sh"]
