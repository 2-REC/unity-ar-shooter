using UnityEngine;

public class SessionManager : MonoBehaviour, IServerManager {

    const string DEFAULT_SERVER_ADDRESS = "127.0.0.1";
    const string DEFAULT_SERVER_PORT = "9000";


    private const string KEY_SERVER_ADDRESS = "SB_SERVER_ADDRESS";
    private const string KEY_SERVER_PORT = "SB_SERVER_PORT";


    // IServerManager implementation
    public string GetServerAddress() {
        return PlayerPrefs.GetString(KEY_SERVER_ADDRESS, DEFAULT_SERVER_ADDRESS);
    }

    public string GetServerAddressDefaultValue() {
        return DEFAULT_SERVER_ADDRESS;
    }

    public string GetServerAddressSetValue() {
        return PlayerPrefs.GetString(KEY_SERVER_ADDRESS, "");
    }

    // IServerManager implementation
    public void SetServerAddress(string address) {
        if (address != "") {
            PlayerPrefs.SetString(KEY_SERVER_ADDRESS, address);
        }
        else {
            PlayerPrefs.DeleteKey(KEY_SERVER_ADDRESS);
        }
    }

    public void SetServerAddressDefault() {
        PlayerPrefs.SetString(KEY_SERVER_ADDRESS, DEFAULT_SERVER_ADDRESS);
    }


    // IServerManager implementation
    public string GetServerPort() {
        return PlayerPrefs.GetString(KEY_SERVER_PORT, "");
    }

    public string GetServerPortDefaultValue() {
        return DEFAULT_SERVER_PORT;
    }

    public string GetServerPortSetValue() {
        return PlayerPrefs.GetString(KEY_SERVER_PORT, "");
    }

    // IServerManager implementation
    public void SetServerPort(string port) {
        if (port != "") {
            PlayerPrefs.SetString(KEY_SERVER_PORT, port);
        }
        else {
            PlayerPrefs.DeleteKey(KEY_SERVER_PORT);
        }
    }

    public void SetServerPortDefault() {
        PlayerPrefs.SetString(KEY_SERVER_PORT, DEFAULT_SERVER_PORT);
    }

}
