using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMode : MonoBehaviour {
    public Camera[] cameras;
    void Update () {
        if (Input.GetKeyDown (KeyCode.M)) {
            foreach (var camera in cameras) {
                camera.gameObject.SetActive (!camera.gameObject.activeSelf);
            }
        }
    }
}