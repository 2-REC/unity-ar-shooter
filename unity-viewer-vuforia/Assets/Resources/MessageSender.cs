using UnityEngine;

public class MessageSender : MonoBehaviour {

    public SpacebrewEvents spacebrewClientEvents;

    public void Send(string message) {
        spacebrewClientEvents.SendString(message);
    }

}
