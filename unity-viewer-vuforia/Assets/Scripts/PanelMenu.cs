using UnityEngine;
using UnityEngine.SceneManagement;

public class PanelMenu : MonoBehaviour {

    public PanelServer panelServer;
    public string testScene = "messages";

    SessionManager sessionManager;


    void Awake() {

        sessionManager = this.gameObject.GetComponentInChildren<SessionManager>();
        if (sessionManager == null) { 
            //ERROR!
        }

        panelServer.Init(sessionManager.GetServerAddressSetValue(), sessionManager.GetServerPortSetValue(),
                sessionManager.GetServerAddressDefaultValue(), sessionManager.GetServerPortDefaultValue());

        // load other scene asynchornously? => Only ok if 1 scene, and maybe not necessary... (small perf gain)
        //...
    }

/*
    void Start() {
    }

    void Update() {
    }
*/

    public void Go() {
        string serverAddress = panelServer.GetAddress();
        if (serverAddress != "") {
            sessionManager.SetServerAddress(serverAddress);
        }
        else {
            sessionManager.SetServerAddressDefault();
        }

        string serverPort = panelServer.GetPort();
        if (serverPort != "") {
            sessionManager.SetServerPort(serverPort);
        }
        else {
            sessionManager.SetServerPortDefault();
        }

        SceneManager.LoadScene(testScene);
    }

    public void Clear() {
        sessionManager.SetServerAddress("");
        sessionManager.SetServerPort("");
        panelServer.Clear();
    }

    public void Quit() {
        Application.Quit();
    }

}
