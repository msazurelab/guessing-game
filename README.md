# guessing-game


### Game Rules

This is a console guessing game which allow users to a set of question about the solution. Users are allow to ask 5 Yes-or-No questions in each game, and is given 5 attempts to guess the solution. If the user got the answer right within 5 attempts, then the user wins.


### Pre-req

1. An Azure account.
https://azure.microsoft.com

2. Visual Studio 2017. Community version is fine.
https://www.visualstudio.com/downloads/

3. Windows OS to run Visual Studio 2017.


### Setup

1. Create a Azure Service Bus Namesapce. Choose standard tier.
Follow this https://docs.microsoft.com/en-us/azure/service-bus-messaging/service-bus-create-namespace-portal.

2. Update the following app.configs with Service Bus Namespace connection string.

```
.\Deployment\App.config
.\GameClient\App.config
.\GameServer\App.config
```

```
<add key="Microsoft.ServiceBus.ConnectionString"
  value="Endpoint=sb://guessinggame.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=h41Tu/CTd95bSXYym/EYoNNa0OGNBlbYTzdkpnFzFGw="/>
```

3. To provision the queues, compile GuessingGame.sln and run Deployment.exe.


### Running the apps

Compile GuessingGame.sln and run the following exe's.

```
GameClient.exe
GameServer.exe
```

### Design

Here are the main components of the solution:

Apps
* GameServer - this is the server of the game that process request message and send back a response message
* GameClient - this is the game client which users will use

Azure Service Bus Queues
* request-queue - queue to send request messages to
* response-queue - queue to send response messages to

The server supports concurrent gameplay with sessions. Client will start a game session by submitting a StartGameRequest message with SessionId to request-queue, and the server will respond if the session is created successfully. Subsequent response messages within the game session will be sent to the request-queue with SessionId set to the Id of the game session.

The client keeps track of the current game session. When it retrives message from the request-queue, it will specify the sessionId, so that the client will only receive message that's related to the current session, and not other sessions in other client instances.

GameServer send messages like below:
```
            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            var jsonMessage = JsonConvert.SerializeObject(messageBody);
            var message = new BrokeredMessage(jsonMessage);
            if(!string.IsNullOrWhiteSpace(sessionId))
            {
                message.SessionId = sessionId;
            }
            client.Send(message);
```

GameClient receive messages like below:
```
    var queueClient = QueueClient.CreateFromConnectionString(_connectionString, ApplicationConstants.ResponseQueueName, ReceiveMode.ReceiveAndDelete);
    var session = queueClient.AcceptMessageSession(sessionId);
```


### Remarks

1. Tests were not written due to time constraint. However, if tests were written, it'd be best written to test the contract, which are the messages that are exchanged on the queues. For example, if client submit a StartGameRequest to the request-queue, then the server is expected to submit a StartGameResponse back. Some refactoring is required to create the interfaces to allow this contract testing.

2. Scaling is not implemented intentionally to keep it simple. However, it's possible. IRepository pattern is used for storing game session. So currently GameSerer is a stateful app. To scale out horizontally, we can use competing consumer pattern with repository moved to external persistence to make GameServer stateless or set up some session affinity mechanism. Vertically scaling can be done by processing multiple message with threads.

3. GameServer doesn't track game result. This can be done easily, but skipped due to time constraint.

