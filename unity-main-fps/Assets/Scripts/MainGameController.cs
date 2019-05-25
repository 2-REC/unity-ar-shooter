using UnityEngine;
using System.Collections.Generic;
using SimpleJSON;

public class MainGameController : MonoBehaviour {

    public Transform position_listener;
    public float speed = 1.0f;
    public MessageSender server;

    //// LASER - MID
    public Rigidbody arrow;
    private List<Transform> arrows;
    //// LASER - END

    private Vector3 old_position;
    private Vector3 old_orientation;

    private void Awake() {
        arrows = new List<Transform>();
    }


/*
    void Start() {
    }
*/

    void Update() {


var msg = new JSONClass();
bool send_data = false;


// TODO: SEND BY TIME INTERVAL INSTEAD OF CONTINUOUSLY!
        if (position_listener.hasChanged) {
//            print("sphere position changed - sending to server");
            position_listener.hasChanged = false;

            // check if changed "enough" to send to server
            bool position_changed = ChangedPosition();
            bool orientation_changed = ChangedOrientation();
            if (position_changed || orientation_changed) {
//                sendPosition(position_changed, orientation_changed);
                sendPosition(position_changed, orientation_changed, msg);

                // "non-accumulative deltas" (only update old values when difference is big enough and have been handled
                old_position = position_listener.position;
                old_orientation = position_listener.forward;
send_data = true;
            }
        }

        //// LASER - MID
//        SendArrows();
/*
        if (SendArrows(msg)) {
            send_data = true;
        }
*/
        //// LASER - END

if (send_data) {
    server.Send(msg.ToString());
}

    }

    private bool ChangedPosition() {

        // compare position
//TODO: maybe better to use magnitude (ok as long as have "non-accumulative deltas")
/*
        Vector3 delta = old_position - position_listener.position;
//TODO: set offset as parameter
        if (delta.magnitude >= 0.01f) {
            return true;
        }
*/
        Vector3 new_position = position_listener.position;
//TODO: set offsets as parameters
        if ((Mathf.Abs(new_position.x - old_position.x) > 0.01f)
                || (Mathf.Abs(new_position.y - old_position.y) > 0.01f)
                || (Mathf.Abs(new_position.z - old_position.z) > 0.01f)) {
//print("SENDING POSITION");
            return true;
        }

        return false;
    }

    private bool ChangedOrientation() {

        // compare orientation (only up/vertical axis)
        Vector3 delta = old_orientation - position_listener.forward;
//TODO: set offset as parameter
        if (delta.magnitude >= 0.01f) {
//print("SENDING ORIENTATION");
            return true;
        }

        return false;
    }

    //// LASER - MID
    public void processMessage(string message) {
        var msg = JSONNode.Parse(message);

//TODO: make more checks?
        if (msg["SRC"].ToString() == "\"client\"") {
//            print("received message from client");

            if (msg["TYPE"].ToString() == "\"arrow\"") {

// TODO: also check that "direction" exists!
                if (msg["position"] != null) {
//print("arrow position: " + msg["position"].ToString()); // could check that has the desired form
                    float x = int.Parse(msg["position"][0].Value) / 100.0f;
                    float y = int.Parse(msg["position"][1].Value) / 100.0f;
                    float z = int.Parse(msg["position"][2].Value) / 100.0f;
/*
print("x : " + x);
print("y : " + y);
print("z : " + z);
*/
                    Vector3 position = new Vector3(x, y, z);

//                    print("arrow direction: " + msg["direction"].ToString()); // could check that has the desired form
                    float a = int.Parse(msg["direction"][0].Value) / 100.0f;
                    float b = int.Parse(msg["direction"][1].Value) / 100.0f;
                    float c = int.Parse(msg["direction"][2].Value) / 100.0f;
/*
print("a : " + a);
print("b : " + b);
print("c : " + c);
*/
                    Vector3 direction = new Vector3(a, b, c);

//                    ShootArrow(position, position + (direction * 100.0f));
//                    ShootArrow(position, (direction * 100.0f));
                    ShootArrow(position, direction);
                }
            }

        }

    }

    void ShootArrow(Vector3 position, Vector3 direction) {

//TODO: keep track of lines to be able to destroy them (should be attached to arrow instance)
        DrawLine(position, position + (direction * 100.0f), Color.red);

        Rigidbody arrow_instance;

        Quaternion orientation = Quaternion.LookRotation(direction, Vector3.up);
        orientation *= Quaternion.AngleAxis(90.0f, Vector3.right);

        arrow_instance = Instantiate(arrow, position, orientation);
        arrow_instance.velocity = direction * 10.0f;

        arrows.Add(arrow_instance.transform);
    }

//    void DrawLine(Vector3 origin, Vector3 end, Color color, float duration = 0.2f) {
    void DrawLine(Vector3 origin, Vector3 end, Color color) {
        GameObject myLine = new GameObject();
        myLine.transform.position = origin;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
//        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = color;
        lr.endColor = color;

        lr.startWidth = .01f;
        lr.endWidth = .01f;

        lr.SetPosition(0, origin);
        lr.SetPosition(1, end);

//        GameObject.Destroy(myLine, duration);
    }
    //// LASER - END


/*
    void sendPosition(bool position, bool orientation) {
        var msg = new JSONClass();
        msg.Add("SRC", "server");
        msg.Add("TYPE", "player");

        if (position) {
            msg["position"]["x"] = ((int)(position_listener.position.x * 100)).ToString();
            msg["position"]["y"] = ((int)(position_listener.position.y * 100)).ToString();
            msg["position"]["z"] = ((int)(position_listener.position.z * 100)).ToString();
        }
        if (orientation) {
            msg["orientation"]["x"] = ((int)(position_listener.rotation.x * 100)).ToString();
            msg["orientation"]["y"] = ((int)(position_listener.rotation.y * 100)).ToString();
            msg["orientation"]["z"] = ((int)(position_listener.rotation.z * 100)).ToString();
            msg["orientation"]["w"] = ((int)(position_listener.rotation.w * 100)).ToString();
        }

        server.Send(msg.ToString());
    }
*/
    void sendPosition(bool position, bool orientation, JSONClass msg) {
        msg.Add("SRC", "server");
        msg.Add("TYPE", "player");

        if (position) {
            msg["position"]["x"] = ((int)(position_listener.position.x * 100)).ToString();
            msg["position"]["y"] = ((int)(position_listener.position.y * 100)).ToString();
            msg["position"]["z"] = ((int)(position_listener.position.z * 100)).ToString();
        }
        if (orientation) {
            msg["orientation"]["x"] = ((int)(position_listener.rotation.x * 100)).ToString();
            msg["orientation"]["y"] = ((int)(position_listener.rotation.y * 100)).ToString();
            msg["orientation"]["z"] = ((int)(position_listener.rotation.z * 100)).ToString();
            msg["orientation"]["w"] = ((int)(position_listener.rotation.w * 100)).ToString();
        }
    }
/*
    private void SendArrows() {

        var msg = new JSONClass();
        msg.Add("SRC", "server");
        msg.Add("TYPE", "arrows");

        int length = 0;
/*
        foreach (Transform transform in arrows) {
print("ID: " + transform.GetInstanceID());
//TODO: make deeper checks...
            if (transform.hasChanged) {
//TODO: send each one separately or all at once?
                msg["arrows"][length]["position"]["x"] = ((int)(transform.position.x * 100)).ToString();
                msg["arrows"][length]["position"]["y"] = ((int)(transform.position.y * 100)).ToString();
                msg["arrows"][length]["position"]["z"] = ((int)(transform.position.z * 100)).ToString();

                msg["arrows"][length]["orientation"]["x"] = ((int)(transform.rotation.x * 100)).ToString();
                msg["arrows"][length]["orientation"]["y"] = ((int)(transform.rotation.y * 100)).ToString();
                msg["arrows"][length]["orientation"]["z"] = ((int)(transform.rotation.z * 100)).ToString();
                msg["arrows"][length]["orientation"]["w"] = ((int)(transform.rotation.w * 100)).ToString();

                ++length;
            }
        }
*/
/*        msg.Add("length", length.ToString());

//        if (length > 0) {
++count;
if (count > 100) {
    server.Send(msg.ToString());
    count = 0;
}
//        }
    }
static int count = 0;
*/
    private bool SendArrows(JSONClass msg) {

//        msg.Add("SRC", "server");
//        msg.Add("TYPE", "arrows");

        int length = 0;

        foreach (Transform transform in arrows) {
//TODO: should have an ID provided by the clients!
// OR USE A POOL OF OBJECTS!
//print("ID: " + transform.GetInstanceID());
//TODO: make deeper checks...
            if (transform.hasChanged) {
                transform.hasChanged = false;

//TODO: send each one separately or all at once?
                msg["arrows"][length]["position"]["x"] = ((int)(transform.position.x * 100)).ToString();
                msg["arrows"][length]["position"]["y"] = ((int)(transform.position.y * 100)).ToString();
                msg["arrows"][length]["position"]["z"] = ((int)(transform.position.z * 100)).ToString();

                msg["arrows"][length]["orientation"]["x"] = ((int)(transform.rotation.x * 100)).ToString();
                msg["arrows"][length]["orientation"]["y"] = ((int)(transform.rotation.y * 100)).ToString();
                msg["arrows"][length]["orientation"]["z"] = ((int)(transform.rotation.z * 100)).ToString();
                msg["arrows"][length]["orientation"]["w"] = ((int)(transform.rotation.w * 100)).ToString();

                ++length;
            }
        }

        msg.Add("length", length.ToString());

        return (length > 0);
    }

}
