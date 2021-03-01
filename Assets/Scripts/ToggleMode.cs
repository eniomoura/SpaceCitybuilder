using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMode : MonoBehaviour {
    public static string mode = "map";

    public Camera[] cameras;
    public Canvas[] canvases;
    public Light[] lights;

    void Update () {
        if (Input.GetKeyDown (KeyCode.M)) {
            ToggleMapShipMode ();
        }
    }

    void ToggleMapShipMode () {
        if (mode == "map") {
            mode = "ship";
        } else {
            mode = "map";
        }
        //troca câmeras
        foreach (var camera in cameras) {
            camera.gameObject.SetActive (!camera.gameObject.activeSelf);
        }
        foreach (var canvas in canvases) {
            canvas.gameObject.SetActive (!canvas.gameObject.activeSelf);
        }
        foreach (var light in lights) {
            light.gameObject.SetActive (!light.gameObject.activeSelf);
        }
    }
}