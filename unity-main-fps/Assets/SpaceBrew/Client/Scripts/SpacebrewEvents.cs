using UnityEngine;

public class SpacebrewEvents : MonoBehaviour {

    public MessageReceiver messageReceiver;

    SpacebrewClient sbClient;

    // Use this for initialization
    void Start() {
        // TODO: could get name from object in the scene (from type?)
        GameObject go = GameObject.Find ("SpaceBrewClient"); // the name of your client object
        sbClient = go.GetComponent<SpacebrewClient>();

        // TODO: could get list of parameters by script & automatically add listeners?
        sbClient.addEventListener(this.gameObject, "subBool");
        sbClient.addEventListener(this.gameObject, "subRange");
        sbClient.addEventListener(this.gameObject, "subString");
    }

    public void OnSpacebrewEvent(SpacebrewClient.SpacebrewMessage _msg) {
        //////// ADAPTATION - BEGIN
        print("Received Spacebrew Message (from: " + _msg.clientName + ")");
        print("name: " + _msg.name + ", type: " + _msg.type + ", value: " + _msg.value);
        messageReceiver.Receive(_msg);
        //////// ADAPTATION - END
    }


    // Sends a boolean message to SpaceBrew
    public void SendBool(bool value) {
        string strValue = value ? "true" : "false";
        print("Sending Spacebrew Message: " + strValue + " (BOOLEAN)");
        sbClient.sendMessage("pubBool", "boolean", strValue);
    }

    // Sends a range message to SpaceBrew
    public void SendRange(int value) {
        string strValue = value.ToString();
        print("Sending Spacebrew Message: " + strValue + " (RANGE)");
        sbClient.sendMessage("pubRange", "range", strValue);
    }

    // Sends a string message to SpaceBrew
    public void SendString(string value) {
        print ("Sending Spacebrew Message: " + value + " (STRING)");
        sbClient.sendMessage("pubString", "string", value);
    }

}
