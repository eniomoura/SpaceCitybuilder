using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour {
    public Camera camera;
    public double maxAcceleration;
    public double maxSpeed;
    public double acceleration;
    public double speed;
    public float sensitivity;
    public float turnRate;
    public Vector3 accelerationVector;
    public Vector3 speedVector;

    void FixedUpdate () {
        if (ToggleMode.mode == "ship") {
            MoveShip ();
            RotateShip ();
        }
    }
    void MoveShip () {
        acceleration = accelerationVector.magnitude;
        speed = speedVector.magnitude;
        accelerationVector = Vector3.ClampMagnitude (accelerationVector + new Vector3 (Input.GetAxis ("Horizontal") * sensitivity / 2, Input.GetAxis ("Vertical") * sensitivity / 2, Input.GetAxis ("Forward") * sensitivity), (float) maxAcceleration);
        if (Input.GetKey (KeyCode.Tab)) {
            accelerationVector = Vector3.zero;
        }
        speedVector = Vector3.MoveTowards (speedVector, Vector3.zero, (float) maxAcceleration / 10);
        speedVector = Vector3.ClampMagnitude (speedVector + accelerationVector, (float) maxSpeed);
        transform.localPosition += (transform.right * speedVector.x) + (transform.up * speedVector.y) + (transform.forward * speedVector.z);
    }

    void RotateShip () {
        transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.LookRotation (camera.ScreenToWorldPoint (Input.mousePosition + Vector3.forward * 10000)), turnRate);
    }

    void OnDrawGizmos () {
        Gizmos.DrawSphere (camera.ScreenToWorldPoint (Input.mousePosition + Vector3.forward), .5f);
    }
}