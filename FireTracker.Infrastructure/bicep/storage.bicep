param storageAccountName string
param containerName string
param location string = resourceGroup().location
param tags object = {}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  tags: tags
  properties: {
    accessTier: 'Hot'
  }
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-05-01' = {
  name: 'default'
  parent: storageAccount
  properties: {
    deleteRetentionPolicy: {
      enabled: true
      days: 1
    }
  }
}

resource encryptionScope 'Microsoft.Storage/storageAccounts/encryptionScopes@2023-05-01' = {
  name: 'blobScope' // Encryption scope name
  parent: storageAccount
  properties: {
    source: 'Microsoft.Storage' // Use Microsoft-managed keys
    state: 'Enabled' // Enable the encryption scope
  }
}

resource blobContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01' = {
  name: containerName
  parent: blobService
  properties: {
    defaultEncryptionScope: 'blobScope'
    denyEncryptionScopeOverride: true
    publicAccess: 'None'
  }
}

output storageAccountId string = storageAccount.id
output containerId string = blobContainer.id
