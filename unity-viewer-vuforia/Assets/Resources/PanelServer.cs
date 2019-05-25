using UnityEngine;
using UnityEngine.UI;

public class PanelServer : MonoBehaviour {

    string defaultAddress;
    string defaultPort;


    public void Init(string address, string port, string defaultAddress, string defaultPort) {
        this.defaultAddress = defaultAddress;
        this.defaultPort = defaultPort;

        SetAddressInputField(address, true);
        SetPortInputField(port, true);
    }


    void SetAddressInputField(string address, bool init) {
        InputField inputAddress = this.gameObject.transform.Find("InputFieldAddress").gameObject.GetComponent<InputField>();
        if (inputAddress != null) {
            inputAddress.text = address;
            if (init) {
                inputAddress.placeholder.GetComponent<Text>().text = "Default: " + defaultAddress;
            }
        }
    }

    void SetPortInputField(string port, bool init) {
        InputField inputPort = this.gameObject.transform.Find("InputFieldPort").gameObject.GetComponent<InputField>();
        if (inputPort != null) {
            inputPort.text = port;
            if (init) {
                inputPort.placeholder.GetComponent<Text>().text = "Default: " + defaultPort;
            }
        }
    }

    public void Clear() {
        SetAddressInputField("", false);
        SetPortInputField("", false);
    }

    public string GetAddress() {
        InputField inputAddress = this.gameObject.transform.Find("InputFieldAddress").gameObject.GetComponent<InputField>();
        if (inputAddress != null) {
            return inputAddress.text;
        }
        return "";
    }

    public string GetPort() {
        InputField inputPort = this.gameObject.transform.Find("InputFieldPort").gameObject.GetComponent<InputField>();
        if (inputPort != null) {
            return inputPort.text;
        }
        return "";
    }

}
