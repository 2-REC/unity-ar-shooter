using UnityEngine;
using System;

public class SpacebrewEvents : MonoBehaviour {

    SpacebrewClient sbClient;
    public MessageReceiver messageReceiver;


    void Start() {
        sbClient = gameObject.GetComponentInParent<SpacebrewClient>();
        if (sbClient == null) {
            throw new Exception("SpacebrewEvent: SpacebrewClient not found in parent object!");
        }
/*
        foreach (SpacebrewClient.Subscriber subscriber in sbClient.subscribers) {
            print("Adding subscriber: " + subscriber.name);
            sbClient.addEventListener(this.gameObject, subscriber.name);
        }
*/
    }


    public void OnSpacebrewEvent(SpacebrewClient.SpacebrewMessage _msg) {
//TODO: REMOVE THIS PRINT - DEBUG PURPOSE
print("Received Spacebrew Message (from: " + _msg.clientName + ")");
print("name: " + _msg.name + ", type: " + _msg.type + ", value: " + _msg.value);
        messageReceiver.Receive(_msg);
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
