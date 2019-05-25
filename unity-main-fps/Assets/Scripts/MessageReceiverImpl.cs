//using SimpleJSON;

public class MessageReceiverImpl : MessageReceiver {

    public MainGameController gameController;


    override public void Receive(SpacebrewClient.SpacebrewMessage message) {
/*
        print("RECEIVED MESSAGE");
        print("clientName: " + message.clientName);
        print("name: " + message.name);
        print("type: " + message.type);
        print("value: " + message.value);
*/
        if (message.type == "string") {
            gameController.processMessage(message.value);
        }
    }

}
