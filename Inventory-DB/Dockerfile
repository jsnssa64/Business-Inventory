ARG BIN_MODE=Debug    # Default to Development if not specified Release or Debug

# Use the official Microsoft SQL Server image
FROM mcr.microsoft.com/mssql/server:2022-latest

# Set environment variables for SQL Server
ENV ASPNETCORE_ENVIRONMENT=${ASPNETCORE_ENVIRONMENT:-Development}

# Expose SQL Server port
EXPOSE 1433

USER root

# Install sqlpackage for deploying dacpac files
RUN apt-get update && apt-get install -y wget unzip && \
    wget -O sqlpackage.zip https://aka.ms/sqlpackage-linux && \
    unzip sqlpackage.zip -d /opt/sqlpackage && \
    chmod +x /opt/sqlpackage/sqlpackage && \
    rm sqlpackage.zip


# Create a directory for the dacpac file
RUN mkdir -p /usr/src/app

ARG BIN_MODE=Debug

# Copy the dacpac file into the container
COPY ./bin/${BIN_MODE}/InventoryDb.dacpac /usr/src/app/InventoryDb.dacpac
COPY ./bin/${BIN_MODE}/master.dacpac /usr/src/app/master.dacpac
COPY ./server/CreateServerLogin.sql /tmp/CreateServerLogin.sql

# Entrypoint script
COPY ./server/entrypoint.sh /entrypoint.sh
RUN chmod +x /entrypoint.sh

# Run everything from the entrypoint
CMD ["/entrypoint.sh"]