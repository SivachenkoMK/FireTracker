param clusterName string
param nodeCount int
param sshRSAPublicKey string
param azureuser string
param dnsPrefix string
param location string = resourceGroup().location
param workspaceName string
param acrName string

resource acr 'Microsoft.ContainerRegistry/registries@2023-07-01' existing = {
  name: acrName
}

resource logAnalyticsWorkspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: workspaceName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

param acrPullRoleId string = '7f951dda-4ed3-4680-a7ca-43fe172d538d'
resource aksAcrRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(aksCluster.id, 'acrpull')
  scope: acr
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', acrPullRoleId)
    principalId: aksCluster.identity.principalId
  }
}

// Still need to assign `AcrPull` to the Kubelet Managed Identity post-deployment

resource aksCluster 'Microsoft.ContainerService/managedClusters@2022-09-01' = {
  name: clusterName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    dnsPrefix: dnsPrefix
    agentPoolProfiles: [
      {
        name: 'agentpool'
        count: nodeCount
        vmSize: 'Standard_B2s'
        osType: 'Linux'
        mode: 'System'
        type: 'VirtualMachineScaleSets'
        enableAutoScaling: true
        minCount: 1
        maxCount: 5
      }
    ]
    linuxProfile: {
      adminUsername: azureuser
      ssh: {
        publicKeys: [
          {
            keyData: sshRSAPublicKey
          }
        ]
      }
    }
    enableRBAC: true
    networkProfile: {
      networkPlugin: 'azure'
      loadBalancerSku: 'standard'
    }
    addonProfiles: {
      omsagent: {
        enabled: true
        config: {
          logAnalyticsWorkspaceResourceID: logAnalyticsWorkspace.id
        }
      }
    }
    kubernetesVersion: '1.31.1'
  }
}

output kubeConfig string = aksCluster.properties.linuxProfile.adminUsername