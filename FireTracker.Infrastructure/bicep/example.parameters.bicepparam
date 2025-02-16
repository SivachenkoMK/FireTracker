using './storage.bicep'

@description('Storage account name')
param storageAccountName

@description('Location of the resource')
param location

@description('Container name')
param containerName

@description('Tags for the storage account')
param tags = {
  environment:
}
