# Use the official Microsoft SQL Server image
FROM mcr.microsoft.com/mssql/server:2022-latest

# Set environment variables for SQL Server
#ENV ACCEPT_EULA=Y
#ENV MSSQL_SA_PASSWORD=YourStrongPassword123
#ENV MSSQL_PID=Express

# Expose SQL Server port
EXPOSE 1433

# Install sqlpackage for deploying dacpac files
RUN apt-get update && apt-get install -y wget unzip && \
    wget -O sqlpackage.zip https://aka.ms/sqlpackage-linux && \
    unzip sqlpackage.zip -d /opt/sqlpackage && \
    chmod +x /opt/sqlpackage/sqlpackage && \
    rm sqlpackage.zip

# Create a directory for the dacpac file
RUN mkdir -p /usr/src/app

# Copy the dacpac file into the container
COPY ./InventoryDb/bin/$MODE/InventoryDB.dacpac /usr/src/app/InventoryDB.dacpac

# Run the dacpac deployment on container startup
CMD /opt/mssql/bin/sqlservr & sleep 30 && \
    /opt/sqlpackage/sqlpackage /Action:Publish /SourceFile:/usr/src/app/InventoryDB.dacpac /TargetServerName:localhost /TargetDatabaseName:InventoryDB /TargetUser:SA /TargetPassword:YourStrongPassword123 && \
    wait