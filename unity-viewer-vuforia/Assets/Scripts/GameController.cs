using UnityEngine;
using SimpleJSON;

public class GameController : MonoBehaviour {

    //// LASER - MID
    public Transform ar_camera;
    public MessageSender server;
    public Rigidbody arrow;
    //// LASER - END

    public Transform plane;
    public Transform player;


/*
    void Start() {
    }
    void Update() {
    }
*/

    public void processMessage(string message) {
        var msg = JSONNode.Parse(message);

//TODO: make more checks?
        if (msg["SRC"].ToString() == "\"server\"") {
//            print("received message from server");

            if (msg["TYPE"].ToString() == "\"player\"") {

//                Vector3 new_position;
                if (msg["position"] != null) {
//                    print("new position: " + msg["position"].ToString()); // could check that has the desired form
                    float x = int.Parse(msg["position"][0].Value) / 100.0f;
                    float y = int.Parse(msg["position"][1].Value) / 100.0f;
                    float z = int.Parse(msg["position"][2].Value) / 100.0f;
/*
print("x : " + x);
print("y : " + y);
print("z : " + z);
*/
                    updatePosition(new Vector3(x, y, z));
//                    new_position = new Vector3(x, y, z);
                }

//                Quaternion new_orientation;
                if (msg["orientation"] != null) {
//                    print("new orientation: " + msg["orientation"].ToString()); // could check that has the desired form
                    float x = int.Parse(msg["orientation"][0].Value) / 100.0f;
                    float y = int.Parse(msg["orientation"][1].Value) / 100.0f;
                    float z = int.Parse(msg["orientation"][2].Value) / 100.0f;
                    float w = int.Parse(msg["orientation"][3].Value) / 100.0f;
/*
print("w : " + w);
print("x : " + x);
print("y : " + y);
print("z : " + z);
*/
                    updateOrientation(new Quaternion(x, y, z, w));
//                    new_orientation = new Quaternion(w, x, y, z);
                }

            }
            else if (msg["TYPE"].ToString() == "\"arrows\"") {

print("ARROWS");
print("length: " + msg["length"]);
print("arrows: " + msg["arrows"].ToString());

//TODO: should get the IDs, & determine which arrows are to create...
// OR USE A POOL OF OBJECTS!
//...

/*
// TODO: also check that "direction" exists!
                if (msg["position"] != null) {
//print("arrow position: " + msg["position"].ToString()); // could check that has the desired form
                    float x = int.Parse(msg["position"][0].Value) / 100.0f;
                    float y = int.Parse(msg["position"][1].Value) / 100.0f;
                    float z = int.Parse(msg["position"][2].Value) / 100.0f;

//print("x : " + x);
//print("y : " + y);
//print("z : " + z);
                    Vector3 position = new Vector3(x, y, z);

//                    print("arrow direction: " + msg["direction"].ToString()); // could check that has the desired form
                    float a = int.Parse(msg["direction"][0].Value) / 100.0f;
                    float b = int.Parse(msg["direction"][1].Value) / 100.0f;
                    float c = int.Parse(msg["direction"][2].Value) / 100.0f;
//print("a : " + a);
//print("b : " + b);
//print("c : " + c);
                    Vector3 direction = new Vector3(a, b, c);

//                    ShootArrow(position, position + (direction * 100.0f));
//                    ShootArrow(position, (direction * 100.0f));
                    ShootArrow(position, direction);
                }
*/
            }

        }

    }

    void updatePosition(Vector3 position) {
//TODO: should handle an offset (starting position)
        player.position = plane.position + position;
    }

    void updateOrientation(Quaternion orientation) {
        player.rotation = plane.rotation * orientation;
    }

    //// LASER - MID
    public void shootLaser() {

// TODO: should allow shooting only when have a tracker attacher!

        // get camera position
        Debug.Log("camera position: " + ar_camera.position);
        // get camera orientation
        Debug.Log("camera rotation: " + ar_camera.rotation);


        Vector3 target = ar_camera.position + (ar_camera.forward * 100.0f);
        /*
                if (Physics.Linecast(ar_camera.position, target)) {
                    Debug.Log("hit");
                } else {
                    Debug.Log("no hit");
                }
        */

        // trace line from camera position along orientation line (lookat?)
        DrawLine(ar_camera.position, target, Color.red);

Quaternion orientation = ar_camera.rotation * Quaternion.AngleAxis(90.0f, Vector3.right);
Rigidbody arrow_instance = Instantiate(arrow, ar_camera.position, orientation);
//arrow_instance.velocity = target/10.0f;
arrow_instance.velocity = ar_camera.forward * 10.0f;

        // send position & orientation to server
// TODO: need more information?
        sendArrow(ar_camera.position, ar_camera.forward);
    }

    void sendArrow(Vector3 position, Vector3 direction) {
        var msg = new JSONClass();
// TODO: set unique client name!
        msg.Add("SRC", "client");
        msg.Add("TYPE", "arrow");
        msg["position"]["x"] = ((int)(position.x * 100)).ToString();
        msg["position"]["y"] = ((int)(position.y * 100)).ToString();
        msg["position"]["z"] = ((int)(position.z * 100)).ToString();
        msg["direction"]["x"] = ((int)(direction.x * 100)).ToString();
        msg["direction"]["y"] = ((int)(direction.y * 100)).ToString();
        msg["direction"]["z"] = ((int)(direction.z * 100)).ToString();

        server.SendData(msg.ToString());
    }

    void DrawLine(Vector3 origin, Vector3 end, Color color, float duration = 0.5f) {
        GameObject myLine = new GameObject();
        myLine.transform.position = origin;
        myLine.AddComponent<LineRenderer>();
        LineRenderer lr = myLine.GetComponent<LineRenderer>();
//        lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        lr.startColor = color;
        lr.endColor = color;

        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;

        lr.SetPosition(0, origin);
        lr.SetPosition(1, end);

        GameObject.Destroy(myLine, duration);
    }
    //// LASER - END

}
