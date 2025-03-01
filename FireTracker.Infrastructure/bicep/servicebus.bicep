param location string = resourceGroup().location
param serviceBusNamespace string
param serviceBusTopicName string
@allowed([
  'Basic'
  'Standard'
  'Premium'
])
param serviceBusSku string
param enablePartitioning bool = false

resource serviceBus 'Microsoft.ServiceBus/namespaces@2024-01-01' = {
  name: serviceBusNamespace
  location: location
  sku: {
    name: serviceBusSku
    tier: serviceBusSku
  }
}

resource serviceBusTopic 'Microsoft.ServiceBus/namespaces/topics@2024-01-01' = {
  name: serviceBusTopicName
  parent: serviceBus
  properties: {
    enablePartitioning: enablePartitioning
  }
}

resource incidentSubscription 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = {
  name: 'incident'
  parent: serviceBusTopic
  properties: {
  }
}

resource incidentRule 'Microsoft.ServiceBus/namespaces/topics/subscriptions/rules@2024-01-01' = {
  name: 'IncidentFilter'
  parent: incidentSubscription
  properties: {
    filterType: 'SqlFilter'
    sqlFilter: {
      sqlExpression: 'routingKey = \'fire.gis\' OR routingKey = \'fire.location\''
    }
  }
}

resource photoSubscription 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = {
  name: 'photo'
  parent: serviceBusTopic
  properties: {
  }
}

resource photoRule 'Microsoft.ServiceBus/namespaces/topics/subscriptions/rules@2024-01-01' = {
  name: 'PhotoFilter'
  parent: photoSubscription
  properties: {
    filterType: 'SqlFilter'
    sqlFilter: {
      sqlExpression: 'routingKey = \'fire.photo\''
    }
  }
}

resource analysisSubscription 'Microsoft.ServiceBus/namespaces/topics/subscriptions@2024-01-01' = {
  name: 'analysis'
  parent: serviceBusTopic
  properties: {
  }
}

resource analysisRule 'Microsoft.ServiceBus/namespaces/topics/subscriptions/rules@2024-01-01' = {
  name: 'AnalysisFilter'
  parent: analysisSubscription
  properties: {
    filterType: 'SqlFilter'
    sqlFilter: {
      sqlExpression: 'routingKey = \'fire.analysis\''
    }
  }
}