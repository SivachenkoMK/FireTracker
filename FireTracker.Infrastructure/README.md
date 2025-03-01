## Deployment process:

export AZURE_RESOURCE_GROUP=...
export AZURE_CLUSTER_NAME=...
export AZURE_ACR_NAME=...

# Storage Deployment

az deployment group create \
  --name StorageDeployment \
  --template-file ./storage.bicep \
  --resource-group $AZURE_RESOURCE_GROUP \
  --parameters prod.storage.bicepparam

## Service Bus Deployment

az deployment group create \
  --name ServiceBusDeployment \
  --template-file ./servicebus.bicep \
  --resource-group $AZURE_RESOURCE_GROUP \
  --parameters prod.servicebus.bicepparam

## AKS Deployment

az provider register --namespace Microsoft.OperationsManagement

az provider register --namespace Microsoft.OperationalInsights

az deployment group create \
  --name AksDeployment \
  --template-file ./aks.bicep \
  --resource-group $AZURE_RESOURCE_GROUP \
  --parameters prod.aks.bicepparam

az role assignment create --assignee $(az aks show --resource-group $AZURE_RESOURCE_GROUP --name $AZURE_CLUSTER_NAME --query identityProfile.kubeletidentity.clientId --output tsv) --role AcrPull --scope $(az acr show --name $AZURE_ACR_NAME --query id --output tsv)

kubectl apply -f everything you need

kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/main/deploy/static/provider/cloud/deploy.yaml

### Kubectl Cheat-sheet

kubectl config current-context
az aks get-credentials --resource-group $AZURE_RESOURCE_GROUP --name $AZURE_CLUSTER_NAME

kubectl get nodes
kubectl get pods

kubectl describe ...
kubectl logs ...

kubectl get svc -n ingress-nginx