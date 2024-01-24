// Environment parameter for the web app
@description('Environment of the web app')
param environment string = 'dev'

// Location parameter for services
@description('Location of services')
param location string = resourceGroup().location

// Generate a unique name for the web app
var webAppName = '${uniqueString(resourceGroup().id)}-${environment}'

// Generate a unique name for the app service plan
var appServicePlanName = '${uniqueString(resourceGroup().id)}-mpnp-asp'

// Generate a unique name for the log analytics workspace
var logAnalyticsName = '${uniqueString(resourceGroup().id)}-mpnp-la'

// Generate a unique name for the application insights
var appInsightsName = '${uniqueString(resourceGroup().id)}-mpnp-ai'

// SKU for the app service plan
var sku = 'S1'

// Generate a unique name for the container registry
var registryName = '${uniqueString(resourceGroup().id)}mpnpreg'

// SKU for the container registry
var registrySku = 'Standard'

// Image name for the Docker container
var imageName = 'techboost/dotnetcoreapp'

// Startup command for the web app
var startupCommand = ''

// Resource for the log analytics workspace
resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2021-12-01-preview' = {
  name: logAnalyticsName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 90
    workspaceCapping: {
      dailyQuotaGb: 1
    }
  }
}

// Resource for the application insights
resource appInsights 'Microsoft.Insights/components@2020-02-02-preview' = {
  name: appInsightsName
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
    WorkspaceResourceId: logAnalyticsWorkspace.id
  }
}

// Resource for the container registry
resource containerRegistry 'Microsoft.ContainerRegistry/registries@2020-11-01-preview' = {
  name: registryName
  location: location
  sku: {
    name: registrySku
  }
  properties: {
    adminUserEnabled: true
  }
}

// Resource for the app service plan
resource appServicePlan 'Microsoft.Web/serverFarms@2020-12-01' = {
  name: appServicePlanName
  location: location
  kind: 'linux'
  properties: {
    reserved: true
  }
  sku: {
    name: sku
  }
}

// Resource for the app service app (web app)
resource appServiceApp 'Microsoft.Web/sites@2020-12-01' = {
  name: webAppName
  location: location
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    clientAffinityEnabled: false
    siteConfig: {
      linuxFxVersion: 'DOCKER|${containerRegistry.name}.azurecr.io/${uniqueString(resourceGroup().id)}/${imageName}'
      http20Enabled: true
      minTlsVersion: '1.2'
      appCommandLine: startupCommand
      appSettings: [
        {
          name: 'WEBSITES_ENABLE_APP_SERVICE_STORAGE'
          value: 'false'
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_URL'
          value: 'https://${containerRegistry.name}.azurecr.io'
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_USERNAME'
          value: containerRegistry.name
        }
        {
          name: 'DOCKER_REGISTRY_SERVER_PASSWORD'
          value: containerRegistry.listCredentials().passwords[0].value
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
      ]
    }
  }
}

// Output for the application name
output application_name string = appServiceApp.name

// Output for the application URL
output application_url string = appServiceApp.properties.hostNames[0]

// Output for the container registry name
output container_registry_name string = containerRegistry.name
