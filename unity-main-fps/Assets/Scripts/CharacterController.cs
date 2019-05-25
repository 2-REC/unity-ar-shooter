using UnityEngine;

public class CharacterController : MonoBehaviour {

    public float speed = 10.0f;

    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update() {
        float walk = Input.GetAxis("Vertical") * speed;
        float strafe = Input.GetAxis("Horizontal") * speed;

        walk *= Time.deltaTime;
        strafe *= Time.deltaTime;

        transform.Translate(strafe, 0, walk);

//TODO: should go in kind of menu ui
        if (Input.GetKeyDown("escape")) {
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
