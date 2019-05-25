using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour {

    public GameController gameController;

    /*
    float x = 0.0f;
    float y = 0.0f;
    float z = 0.0f;

    public void changePosition() {
        gameController.updatePosition(x, y, z);
        x += 0.1f;
        y += 0.0f;
        z += 0.5f;
    }
    */

    //// LASER - MID
    public void shoot() {
        gameController.shootLaser();
    }
    //// LASER - END

}
