using UnityEngine;

public abstract class MessageReceiver : MonoBehaviour {

    public abstract void Receive(SpacebrewClient.SpacebrewMessage message);

}
