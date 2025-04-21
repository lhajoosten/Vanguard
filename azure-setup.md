# Login to Azure

az login

# Create a resource group

az group create --name `resource-group-prod` --location `geo-location`

# Create SQL Server

az sql server create --name `sql-server-prod` --resource-group `resource-group-prod` --location `geo-location` --admin-user `Your-Admin-User-ID` --admin-password "`Your-Password`"

# Create SQL Database (Basic tier)

az sql db create --resource-group `resource-group-prod` --server `sql-server-prod` --name `sql-db-prod` --service-objective Basic

# Create App Service Plan for API (Free tier)

az appservice plan create --name `prod-api-plan` --resource-group `resource-group-prod` --sku Free

# Create App Service for API

az webapp create --resource-group `resource-group-prod` --plan `prod-api-plan` --name vanguard-api

# Create Static Web App for Teacher Portal

az staticwebapp create --name "vanguard-teacher-portal" --resource-group `resource-group-prod` \
 --location "`geo-location`" \
 --sku Free

# Create Static Web App for Student Portal

az staticwebapp create --name "vanguard-student-portal" \
 --resource-group `resource-group-prod` \
 --location "`geo-location`" \
 --sku Free
