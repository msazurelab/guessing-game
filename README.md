# guessing-game


### Game Rules

This is a console guessing game which allow users to a set of question about the solution. Users are allow to ask 5 Yes-or-No questions in each game, and is given 5 attempts to guess the solution. If the user got the answer right within 5 attempts, then the user wins.

![Alt text](/GameScreenshot.jpg?raw=true "Optional Title")


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

The client and the server exchanges message via request-queue and response-queue. Client sends request messages to request-queue, which server polls periodically and sends the response back to response-queue.

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

2. Scaling on the server side is not implemented intentionally to keep it simple. However, it's possible. IRepository pattern is used for storing game sessions. Currently the repository is a in-memory repository, hence GameSerer is a stateful app. To scale out horizontally, we can use competing consumer pattern (Please see diagram below. Consumer is GameServer in our context) with repository moved to external persistence to make GameServer stateless or set up some session affinity mechanism. Vertically scaling can be done by processing multiple message with threads.

![Alt text](http://www.enterpriseintegrationpatterns.com/img/CompetingConsumers.gif?raw=true "Optional Title")
http://www.enterpriseintegrationpatterns.com/patterns/messaging/CompetingConsumers.html


3. GameServer doesn't track game result. This can be done easily, but skipped due to time constraint.

4. We can host the server on Azure WebApp. Again, this can be done, but skipped due to time constraint.

5. Comments are basically non-existent due to time constraint :)

6. As an afterthought, a possible better messaging mechanism can be done like this.

In request message, client specify where it wants to receive the response message. It can specify this in a ReplyTo field like so.

```
RequestMessage
- ReplyTo: <queue-name> <sessionId>
```

Then, once the message is processed, the server sends a response message to the specific queue with the session Id. This is neater as it follow the request-reply pattern in Enterprise Integration Patterns. Currently, all response messages within the same session are sent with the same sessionId, if there is more than two messages in the queue for any reason (bug, etc), it would cause issue. By disassociating message session with game session, we can have one session per request-response. This would help with correlating response to the request with much better confidence.

![Alt text](http://www.enterpriseintegrationpatterns.com/img/RequestReply.gif?raw=true "Optional Title")
http://www.enterpriseintegrationpatterns.com/patterns/messaging/RequestReply.html


7. Another afterthought, it's be much cooler if we can have sort of PubSub with the Queues. Azure Service Bus offers Topics which has subscriptions, however, it still requires the subscriber to poll from the subscription queue. It's still not a nice PubSub model. Azure Service Fabrics Reliable Actor has nice framework for this. There is a open source PubSub actor project that's built on top on ASF. https://github.com/loekd/ServiceFabric.PubSubActors. Basically, a subscribing actor can pass on its reference to the PubSub actor and register. When the publishing actor publishes a message to the PubSub actor, the PubActor actor would forward that message to the subscribing actor.


