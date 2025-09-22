#!/bin/bash

# ASP.NET Core API Deployment Script for Ubuntu Server
# This script helps with manual deployment and server maintenance

set -e

APP_NAME="aspdotnetapi"
APP_DIR="/var/www/html/aspdotnetapi"
SERVICE_NAME="aspdotnetapi"
BACKUP_DIR="/var/www/html/aspdotnetapi/backup"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

log_info() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

log_warn() {
    echo -e "${YELLOW}[WARN]${NC} $1"
}

log_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Function to check if running as root/sudo
check_sudo() {
    if [[ $EUID -ne 0 ]]; then
        log_error "This script must be run as root or with sudo"
        exit 1
    fi
}

# Function to install .NET runtime if not present
install_dotnet() {
    if ! command -v dotnet &> /dev/null; then
        log_info "Installing .NET 8.0 runtime..."
        
        # Add Microsoft package repository
        wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
        dpkg -i packages-microsoft-prod.deb
        rm packages-microsoft-prod.deb
        
        # Update package list and install .NET runtime
        apt-get update
        apt-get install -y aspnetcore-runtime-8.0
        
        log_info ".NET runtime installed successfully"
    else
        log_info ".NET runtime is already installed"
    fi
}

# Function to setup Apache proxy
setup_apache_proxy() {
    log_info "Setting up Apache virtual host..."
    
    # Enable required Apache modules
    a2enmod proxy
    a2enmod proxy_http
    a2enmod rewrite
    
    # Create virtual host configuration
    cat > /etc/apache2/sites-available/aspdotnetapi.conf << 'EOF'
<VirtualHost *:80>
    ServerName your-domain.com
    DocumentRoot /var/www/html/aspdotnetapi
    
    # Proxy all requests to the .NET application
    ProxyPreserveHost On
    ProxyPass / http://localhost:5001/
    ProxyPassReverse / http://localhost:5001/
    
    # Optional: Add headers for better proxy handling
    ProxyPassReverse / http://localhost:5001/
    ProxyRequests Off
    
    # Logging
    ErrorLog ${APACHE_LOG_DIR}/aspdotnetapi_error.log
    CustomLog ${APACHE_LOG_DIR}/aspdotnetapi_access.log combined
    
    # Optional: Security headers
    Header always set X-Content-Type-Options nosniff
    Header always set X-Frame-Options DENY
    Header always set X-XSS-Protection "1; mode=block"
</VirtualHost>
EOF
    
    # Enable the site
    a2ensite aspdotnetapi.conf
    
    # Disable default site if needed
    a2dissite 000-default.conf || true
    
    # Restart Apache
    systemctl restart apache2
    
    log_info "Apache virtual host configured successfully"
}

# Function to create systemd service
create_systemd_service() {
    log_info "Creating systemd service..."
    
    cat > /etc/systemd/system/${SERVICE_NAME}.service << EOF
[Unit]
Description=ASP.NET Core API Application
After=network.target

[Service]
Type=notify
ExecStart=/usr/bin/dotnet ${APP_DIR}/TestAPI.dll
Restart=always
RestartSec=5
KillSignal=SIGINT
SyslogIdentifier=${SERVICE_NAME}
User=www-data
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ASPNETCORE_URLS=http://localhost:5001
WorkingDirectory=${APP_DIR}

[Install]
WantedBy=multi-user.target
EOF
    
    systemctl daemon-reload
    systemctl enable ${SERVICE_NAME}
    
    log_info "Systemd service created successfully"
}

# Function to deploy application
deploy_app() {
    local source_path=$1
    
    if [[ -z "$source_path" ]]; then
        log_error "Source path is required for deployment"
        exit 1
    fi
    
    if [[ ! -f "$source_path" ]]; then
        log_error "Source file not found: $source_path"
        exit 1
    fi
    
    log_info "Deploying application from $source_path"
    
    # Stop service if running
    systemctl stop ${SERVICE_NAME} || true
    
    # Create backup
    if [[ -d "$APP_DIR" ]] && [[ -f "$APP_DIR/TestAPI.dll" ]]; then
        log_info "Creating backup..."
        rm -rf ${BACKUP_DIR} || true
        mkdir -p ${BACKUP_DIR}
        cp -r ${APP_DIR}/* ${BACKUP_DIR}/ || true
    fi
    
    # Create app directory
    mkdir -p ${APP_DIR}
    
    # Clear current deployment
    rm -rf ${APP_DIR}/*
    
    # Extract new deployment
    if [[ "$source_path" == *.tar.gz ]]; then
        tar -xzf "$source_path" -C ${APP_DIR}/
    elif [[ "$source_path" == *.zip ]]; then
        unzip -q "$source_path" -d ${APP_DIR}/
    else
        log_error "Unsupported file format. Use .tar.gz or .zip"
        exit 1
    fi
    
    # Set permissions
    chown -R www-data:www-data ${APP_DIR}
    chmod -R 755 ${APP_DIR}
    chmod +x ${APP_DIR}/TestAPI || true
    
    # Start service
    systemctl start ${SERVICE_NAME}
    
    log_info "Application deployed successfully"
}

# Function to show service status
show_status() {
    log_info "Service Status:"
    systemctl status ${SERVICE_NAME} --no-pager || true
    
    log_info "Application Logs (last 20 lines):"
    journalctl -u ${SERVICE_NAME} -n 20 --no-pager || true
    
    log_info "Port 5001 Status:"
    netstat -tlnp | grep :5001 || log_warn "Port 5001 not listening"
}

# Function to show logs
show_logs() {
    local lines=${1:-50}
    log_info "Showing last $lines lines of application logs:"
    journalctl -u ${SERVICE_NAME} -n $lines -f
}

# Function to restart service
restart_service() {
    log_info "Restarting ${SERVICE_NAME} service..."
    systemctl restart ${SERVICE_NAME}
    sleep 2
    systemctl status ${SERVICE_NAME} --no-pager
}

# Main script logic
case "$1" in
    "install")
        check_sudo
        install_dotnet
        create_systemd_service
        setup_apache_proxy
        log_info "Installation completed. You can now deploy your application."
        ;;
    "deploy")
        check_sudo
        deploy_app "$2"
        ;;
    "status")
        show_status
        ;;
    "logs")
        show_logs "$2"
        ;;
    "restart")
        check_sudo
        restart_service
        ;;
    "apache")
        check_sudo
        setup_apache_proxy
        ;;
    *)
        echo "Usage: $0 {install|deploy|status|logs|restart|apache}"
        echo ""
        echo "Commands:"
        echo "  install          - Install .NET runtime, create service, and setup Apache"
        echo "  deploy <file>    - Deploy application from tar.gz or zip file"
        echo "  status           - Show service status and logs"
        echo "  logs [lines]     - Show application logs (default: 50 lines)"
        echo "  restart          - Restart the application service"
        echo "  apache           - Setup/update Apache virtual host configuration"
        echo ""
        echo "Examples:"
        echo "  sudo $0 install"
        echo "  sudo $0 deploy /tmp/deployment.tar.gz"
        echo "  $0 status"
        echo "  $0 logs 100"
        exit 1
        ;;
esac