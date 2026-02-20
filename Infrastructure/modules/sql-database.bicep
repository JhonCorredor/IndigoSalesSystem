// ============================================
// SQL Database Module (Serverless)
// ============================================

@description('Azure region')
param location string

@description('SQL Server name')
param serverName string

@description('Database name')
param databaseName string

@description('SQL Admin username')
@secure()
param adminUsername string

@description('SQL Admin password')
@secure()
param adminPassword string

@description('Environment')
param environment string

@description('Resource tags')
param tags object

// ============================================
// SQL Server
// ============================================

resource sqlServer 'Microsoft.Sql/servers@2023-05-01-preview' = {
  name: serverName
  location: location
  tags: tags
  properties: {
    administratorLogin: adminUsername
    administratorLoginPassword: adminPassword
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
}

// Firewall Rules
resource firewallRuleAllowAzureServices 'Microsoft.Sql/servers/firewallRules@2023-05-01-preview' = {
  parent: sqlServer
  name: 'AllowAllWindowsAzureIps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// Allow your current IP (optional - for development)
resource firewallRuleAllowMyIp 'Microsoft.Sql/servers/firewallRules@2023-05-01-preview' = if (environment == 'dev') {
  parent: sqlServer
  name: 'AllowMyIp'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '255.255.255.255'
  }
}

// ============================================
// SQL Database (Serverless)
// ============================================

resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-05-01-preview' = {
  parent: sqlServer
  name: databaseName
  location: location
  tags: tags
  sku: {
    name: 'GP_S_Gen5'
    tier: 'GeneralPurpose'
    family: 'Gen5'
    capacity: 1
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 34359738368 // 32 GB
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    zoneRedundant: false
    readScale: 'Disabled'
    autoPauseDelay: 60 // Auto-pause after 60 minutes
    minCapacity: json('0.5') // Minimum 0.5 vCore
    isLedgerOn: false
  }
}

// ============================================
// Outputs
// ============================================

output serverName string = sqlServer.name
output databaseName string = sqlDatabase.name
output serverFqdn string = sqlServer.properties.fullyQualifiedDomainName
output connectionString string = 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${sqlDatabase.name};Persist Security Info=False;User ID=${adminUsername};Password=${adminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
