# guessing-game


### Pre-req
1. An Azure account
https://azure.microsoft.com

2. Visual Studio 2017. Community version is fine.
https://www.visualstudio.com/downloads/

3. Windows OS to run Visual Studio 2017


### Setup
1. Create a Azure Service Bus Namesapce. Choose standard tier.
Follow this https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-create-namespace-portal.

2. Update the following app.configs with Service Bus Namespace connection string

```
.\Deployment\App.config
.\GameClient\App.config
.\GameServer\App.config
```

```
      <add key="Microsoft.ServiceBus.ConnectionString"
          value="Endpoint=sb://guessinggame.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=h41Tu/CTd95bSXYym/EYoNNa0OGNBlbYTzdkpnFzFGw="/>
```

3. To provision the queues, compile GuessingGame.sln and run Deployment.exe


### Running the apps

Compile GuessingGame.sln and run the following exe's 

```
GameClient.exe
GameServer.exe
```