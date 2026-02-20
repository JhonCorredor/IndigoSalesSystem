// ============================================
// App Service Module (Web App + Plan)
// ============================================

@description('Azure region')
param location string

@description('App Service Plan name')
param appServicePlanName string

@description('Web App name')
param webAppName string

@description('Environment')
param environment string

@description('Resource tags')
param tags object

// ============================================
// App Service Plan
// ============================================

var skuMap = {
  dev: {
    name: 'F1' // Free tier for development
    tier: 'Free'
    capacity: 1
  }
  staging: {
    name: 'B1' // Basic tier
    tier: 'Basic'
    capacity: 1
  }
  prod: {
    name: 'P1v2' // Premium tier for production
    tier: 'PremiumV2'
    capacity: 2
  }
}

resource appServicePlan 'Microsoft.Web/serverfarms@2023-01-01' = {
  name: appServicePlanName
  location: location
  tags: tags
  sku: skuMap[environment]
  kind: 'linux'
  properties: {
    reserved: true // Required for Linux
  }
}

// ============================================
// Web App (API)
// ============================================

resource webApp 'Microsoft.Web/sites@2023-01-01' = {
  name: webAppName
  location: location
  tags: tags
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|8.0'
      alwaysOn: environment == 'prod' ? true : false
      ftpsState: 'Disabled'
      minTlsVersion: '1.2'
      http20Enabled: true
      cors: {
        allowedOrigins: [
          'http://localhost:4200'
          'https://localhost:4200'
        ]
        supportCredentials: true
      }
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: environment == 'prod' ? 'Production' : 'Development'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
      ]
    }
    clientAffinityEnabled: false
  }
}

// ============================================
// Outputs
// ============================================

output webAppName string = webApp.name
output webAppUrl string = 'https://${webApp.properties.defaultHostName}'
output webAppPrincipalId string = webApp.identity.principalId
output appServicePlanId string = appServicePlan.id
