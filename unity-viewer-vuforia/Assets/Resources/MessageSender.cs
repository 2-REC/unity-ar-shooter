using UnityEngine;

public class MessageSender : MonoBehaviour {

    public SpacebrewEvents spacebrewClientEvents;
    public string pubNameData;


    public void SendData(string message) {
        spacebrewClientEvents.SendString(pubNameData, message);
    }

}
