using './storage.bicep'

@description('Storage account name')
param storageAccountName = 'examplestorage'

@description('Container name')
param containerName = 'example-container'

@description('Tags for the storage account')
param tags = {
  environment: 'production'
}
