using UnityEngine;

/*
TODO: this script shouldn't be attached to the camera, but to the character, and finding the camera attached to the character.
*/

public class CharacterMouseController : MonoBehaviour {

    public float sensitivity = 5.0f;
    public float smoothing = 2.0f;
    public float minLookDown = -75.0f;
    public float maxLookUp = 75.0f;

    Vector2 mouseLook;
    Vector2 smoothV;

    GameObject character;

    void Start() {
//TODO: can't just use "gameObject" straight away?
        character = this.transform.parent.gameObject;
    }

    void Update() {
        Vector2 movement = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        movement = Vector2.Scale(movement, new Vector2(sensitivity * smoothing, sensitivity * smoothing));

        smoothV.x = Mathf.Lerp(smoothV.x, movement.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, movement.y, 1f / smoothing);

        mouseLook += smoothV;
        mouseLook.y = Mathf.Clamp(mouseLook.y, minLookDown, maxLookUp);

        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);
    }
}
