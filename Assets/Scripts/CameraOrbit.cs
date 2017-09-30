using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*	Esta classe é responsável pela movimentação da câmera em torno			*
 *	do planeta selecionado no mapa e implementação de funcões relacionadas	*
 *	a visualização do mapa do sistema solar.								*/
public class CameraOrbit: MonoBehaviour {

	[SerializeField] int sensitivity;
	[SerializeField] int minZoom;
	[SerializeField] GameObject targetPlanet; //Deve sempre ser o transform pai desta câmera.
	PlanetStats stats;
	Camera thisCamera;

	void Start() {
		//Inicializações
		thisCamera = gameObject.GetComponent<Camera>();
		Target(targetPlanet);
	}

	void LateUpdate() {
		if (Input.GetKeyDown(KeyCode.Period)) { //planeta anterior
			Target(targetPlanet.GetComponent<PlanetStats>().nextPlanet);
		} else if (Input.GetKeyDown(KeyCode.Comma)) { //próximo planeta
			Target(targetPlanet.GetComponent<PlanetStats>().prevPlanet);
		}
		if (Input.GetMouseButton(1)) { //movimenta câmera em torno do planeta
			transform.RotateAround(targetPlanet.transform.position, transform.up, Input.GetAxis("Mouse X") * sensitivity);
			transform.RotateAround(targetPlanet.transform.position, -transform.right, Input.GetAxis("Mouse Y") * sensitivity);
		}
		//Zoom controlado por FOV para a câmera de curto alcance:
		float newZoomValue = thisCamera.fieldOfView + -Input.GetAxis("Mouse ScrollWheel") * sensitivity * 10;
		if (newZoomValue > 10 && newZoomValue < minZoom) {
			thisCamera.fieldOfView += -Input.GetAxis("Mouse ScrollWheel") * sensitivity * 10;
		}
		stats.UpdateStatBar(); //Atualiza UI
	}

	public void Target(GameObject newTarget) { //Muda alvo da câmera orbital
		BuildingBehaviour.HideInfoTooltip();
		transform.parent = newTarget.transform;
		targetPlanet = newTarget;
		transform.localPosition = Vector3.back;
		thisCamera.farClipPlane = newTarget.transform.lossyScale.z + 1;
		thisCamera.GetComponentsInChildren<Camera>()[1].nearClipPlane = newTarget.transform.lossyScale.z *2;
		transform.LookAt(newTarget.transform);
		stats = newTarget.GetComponent<PlanetStats>();
	}

	public GameObject GetTargetPlanet() { //placeholder (!)
		return targetPlanet;
	}
}
