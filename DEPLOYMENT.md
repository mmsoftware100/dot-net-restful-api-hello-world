# ASP.NET Core API Deployment Guide

This guide explains how to set up CI/CD deployment for the ASP.NET Core API to an Ubuntu server using GitHub Actions.

## 🚀 Quick Start

1. **Configure GitHub Secrets** (see below)
2. **Prepare Ubuntu Server** (see server setup section)
3. **Push to main/master branch** - deployment will trigger automatically

## 📋 Prerequisites

- Ubuntu server with SSH access
- Domain name (optional, can use IP address)
- GitHub repository with Actions enabled

## 🔐 Required GitHub Secrets

Go to your GitHub repository → Settings → Secrets and variables → Actions, and add these secrets:

| Secret Name | Description | Example |
|-------------|-------------|---------|
| `SERVER_HOST` | Your Ubuntu server IP address or domain | `192.168.1.100` or `your-domain.com` |
| `SERVER_USERNAME` | SSH username for your server | `ubuntu` or `root` |
| `SERVER_PASSWORD` | SSH password for your server | `your-secure-password` |

### Setting up GitHub Secrets:

1. Navigate to your repository on GitHub
2. Click **Settings** → **Secrets and variables** → **Actions**
3. Click **New repository secret**
4. Add each secret with the exact names listed above

## 🖥️ Ubuntu Server Setup

### 1. Initial Server Preparation

```bash
# Update system packages
sudo apt update && sudo apt upgrade -y

# Install required packages
sudo apt install -y apache2 wget curl

# Enable and start Apache
sudo systemctl enable apache2
sudo systemctl start apache2

# Enable required Apache modules
sudo a2enmod proxy
sudo a2enmod proxy_http
sudo a2enmod rewrite
sudo a2enmod headers
```

### 2. Automated Setup (Recommended)

Use the provided deployment script for easy setup:

```bash
# Copy the deployment script to your server
scp scripts/deploy-server.sh user@your-server:/tmp/

# SSH to your server
ssh user@your-server

# Make script executable and run installation
chmod +x /tmp/deploy-server.sh
sudo /tmp/deploy-server.sh install
```

### 3. Manual Setup (Alternative)

If you prefer manual setup, follow these steps:

#### Install .NET 8.0 Runtime

```bash
# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Install .NET runtime
sudo apt update
sudo apt install -y aspnetcore-runtime-8.0
```

#### Configure Apache Virtual Host

```bash
# Copy the Apache configuration
sudo cp apache/aspdotnetapi.conf /etc/apache2/sites-available/

# Edit the configuration to set your domain name
sudo nano /etc/apache2/sites-available/aspdotnetapi.conf

# Enable the site
sudo a2ensite aspdotnetapi.conf
sudo a2dissite 000-default.conf  # Optional: disable default site

# Restart Apache
sudo systemctl restart apache2
```

#### Create Application Directory

```bash
# Create application directory
sudo mkdir -p /var/www/html/aspdotnetapi
sudo chown -R www-data:www-data /var/www/html/aspdotnetapi
```

## 🔄 Deployment Process

### Automatic Deployment (GitHub Actions)

The deployment happens automatically when you:

1. Push code to `main` or `master` branch
2. Create a pull request to these branches

The workflow will:
- ✅ Build the .NET application
- ✅ Run tests
- ✅ Create deployment package
- ✅ Deploy to your Ubuntu server
- ✅ Start/restart the application service

### Manual Deployment

You can also deploy manually using the deployment script:

```bash
# On your local machine, create deployment package
dotnet publish --configuration Release --output ./publish
cd publish && tar -czf ../deployment.tar.gz * && cd ..

# Copy to server
scp deployment.tar.gz user@your-server:/tmp/

# SSH to server and deploy
ssh user@your-server
sudo /path/to/deploy-server.sh deploy /tmp/deployment.tar.gz
```

## 🛠️ Server Management Commands

Use the deployment script for common server management tasks:

```bash
# Check application status
./deploy-server.sh status

# View application logs
./deploy-server.sh logs

# View last 100 log lines
./deploy-server.sh logs 100

# Restart application
sudo ./deploy-server.sh restart

# Reconfigure Apache
sudo ./deploy-server.sh apache
```

## 🌐 Accessing Your Application

After successful deployment, your application will be available at:

- **HTTP**: `http://your-domain.com` or `http://your-server-ip`
- **API Documentation**: `http://your-domain.com/swagger`
- **Health Check**: `http://your-domain.com/health` (if implemented)

## 📊 Monitoring and Logs

### Application Logs

```bash
# View real-time logs
sudo journalctl -u aspdotnetapi -f

# View last 50 lines
sudo journalctl -u aspdotnetapi -n 50

# View logs from specific time
sudo journalctl -u aspdotnetapi --since "2024-01-01 10:00:00"
```

### Apache Logs

```bash
# View Apache access logs
sudo tail -f /var/log/apache2/aspdotnetapi_access.log

# View Apache error logs
sudo tail -f /var/log/apache2/aspdotnetapi_error.log
```

### Service Status

```bash
# Check application service status
sudo systemctl status aspdotnetapi

# Check Apache status
sudo systemctl status apache2

# Check if port 5001 is listening
sudo netstat -tlnp | grep :5001
```

## 🔒 Security Considerations

### Firewall Configuration

```bash
# Allow HTTP and HTTPS traffic
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp

# Allow SSH (adjust port if needed)
sudo ufw allow 22/tcp

# Enable firewall
sudo ufw enable
```

### SSL/HTTPS Setup (Recommended)

For production, set up SSL using Let's Encrypt:

```bash
# Install Certbot
sudo apt install -y certbot python3-certbot-apache

# Obtain SSL certificate
sudo certbot --apache -d your-domain.com

# Auto-renewal is set up automatically
```

### Database Security

- Ensure your database server has proper firewall rules
- Use strong passwords for database connections
- Consider using connection string encryption
- Regularly update database credentials

## 🐛 Troubleshooting

### Common Issues

1. **Application not starting**
   ```bash
   # Check service status
   sudo systemctl status aspdotnetapi
   
   # Check logs for errors
   sudo journalctl -u aspdotnetapi -n 50
   ```

2. **Apache proxy not working**
   ```bash
   # Check Apache configuration
   sudo apache2ctl configtest
   
   # Check if modules are enabled
   sudo a2enmod proxy proxy_http
   sudo systemctl restart apache2
   ```

3. **Database connection issues**
   ```bash
   # Test database connectivity from server
   # Check connection string in appsettings.Production.json
   ```

4. **Port 5001 not listening**
   ```bash
   # Check if application is running
   sudo systemctl status aspdotnetapi
   
   # Check port usage
   sudo netstat -tlnp | grep :5001
   ```

### Getting Help

- Check application logs: `sudo journalctl -u aspdotnetapi -f`
- Check Apache logs: `sudo tail -f /var/log/apache2/error.log`
- Verify service status: `sudo systemctl status aspdotnetapi`
- Test database connection from the server

## 📁 File Structure

After deployment, your server will have:

```
/var/www/html/aspdotnetapi/          # Application files
├── TestAPI.dll                      # Main application
├── appsettings.Production.json      # Production configuration
├── wwwroot/                         # Static files (if any)
└── backup/                          # Previous deployment backup

/etc/systemd/system/
└── aspdotnetapi.service             # Systemd service file

/etc/apache2/sites-available/
└── aspdotnetapi.conf                # Apache virtual host

/var/log/apache2/
├── aspdotnetapi_access.log          # Apache access logs
└── aspdotnetapi_error.log           # Apache error logs
```

## 🔄 Updates and Maintenance

### Updating the Application

Simply push your changes to the main branch, and GitHub Actions will automatically deploy the update.

### Manual Updates

```bash
# Create new deployment package
dotnet publish --configuration Release --output ./publish
cd publish && tar -czf ../deployment.tar.gz * && cd ..

# Deploy to server
scp deployment.tar.gz user@your-server:/tmp/
ssh user@your-server "sudo /path/to/deploy-server.sh deploy /tmp/deployment.tar.gz"
```

### Database Migrations

Database migrations are automatically applied during deployment. For manual migration:

```bash
# On the server, navigate to application directory
cd /var/www/html/aspdotnetapi

# Run migrations
sudo -u www-data dotnet ef database update
```

---

## 📞 Support

If you encounter issues:

1. Check the troubleshooting section above
2. Review application and Apache logs
3. Verify all GitHub secrets are correctly set
4. Ensure server prerequisites are met

Happy deploying! 🚀