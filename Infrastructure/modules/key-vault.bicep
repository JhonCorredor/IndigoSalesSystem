// ============================================
// Key Vault Module (Secrets Management)
// ============================================

@description('Azure region')
param location string

@description('Key Vault name (globally unique)')
@minLength(3)
@maxLength(24)
param keyVaultName string

@description('SQL Connection String')
@secure()
param sqlConnectionString string

@description('Blob Storage Connection String')
@secure()
param blobStorageConnectionString string

@description('JWT Secret Key')
@secure()
param jwtSecretKey string

@description('Resource tags')
param tags object

// ============================================
// Key Vault
// ============================================

resource keyVault 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  location: location
  tags: tags
  properties: {
    sku: {
      family: 'A'
      name: 'standard'
    }
    tenantId: subscription().tenantId
    enabledForDeployment: true
    enabledForTemplateDeployment: true
    enabledForDiskEncryption: false
    enableRbacAuthorization: false
    enableSoftDelete: true
    softDeleteRetentionInDays: 7
    accessPolicies: []
    networkAcls: {
      bypass: 'AzureServices'
      defaultAction: 'Allow'
      ipRules: []
      virtualNetworkRules: []
    }
  }
}

// ============================================
// Secrets
// ============================================

resource secretSqlConnectionString 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'SqlConnectionString'
  properties: {
    value: sqlConnectionString
    contentType: 'text/plain'
  }
}

resource secretBlobStorageConnectionString 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'BlobStorageConnectionString'
  properties: {
    value: blobStorageConnectionString
    contentType: 'text/plain'
  }
}

resource secretJwtKey 'Microsoft.KeyVault/vaults/secrets@2023-07-01' = {
  parent: keyVault
  name: 'JwtSecretKey'
  properties: {
    value: jwtSecretKey
    contentType: 'text/plain'
  }
}

// ============================================
// Outputs
// ============================================

output keyVaultName string = keyVault.name
output keyVaultUri string = keyVault.properties.vaultUri
output sqlConnectionStringSecretUri string = secretSqlConnectionString.properties.secretUri
output blobStorageConnectionStringSecretUri string = secretBlobStorageConnectionString.properties.secretUri
output jwtSecretKeySecretUri string = secretJwtKey.properties.secretUri
