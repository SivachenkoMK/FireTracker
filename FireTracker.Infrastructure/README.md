## Deployment process:

export AZURE_RESOURCE_GROUP=...

az deployment group create \
  --name StorageDeployment \
  --resource-group $AZURE_RESOURCE_GROUP \
  --parameters prod.parameters.bicepparam