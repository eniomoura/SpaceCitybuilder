using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*	Esta classe é responsável pela movimentação da câmera em torno			*
 *	do planeta selecionado no mapa e implementação de funcões relacionadas	*
 *	a visualização do mapa do sistema solar.								*/
public class CameraOrbit : MonoBehaviour {

	[SerializeField] int sensitivity;
	[SerializeField] int maxAltitude;
	[SerializeField] int systemBoundaries;
	[SerializeField] float distance;
	[SerializeField] List<GameObject> magnetTargets;
	[SerializeField] Text positionMarker;
	public GameObject targetPlanet; //Deve sempre ser o transform pai desta câmera.
	public int leavingPlanet;
	public Material gridMaterial;
	public GameObject sectorMap;
	GameObject buildBar;
	PlanetStats stats;
	Camera thisCamera;
	Camera farCamera;

	void Start () {
		//Inicializações
		if (targetPlanet == null) { targetPlanet = GameObject.Find ("Earth"); } //debug
		thisCamera = gameObject.GetComponent<Camera> ();
		farCamera = thisCamera.GetComponentsInChildren<Camera> () [1];
		buildBar = GameObject.Find ("Build");
		Target (targetPlanet);
		magnetTargets.AddRange (GameObject.FindGameObjectsWithTag ("Planet"));
		magnetTargets.AddRange (GameObject.FindGameObjectsWithTag ("Sector"));
	}

	void LateUpdate () {
		//Seletor de Planetas
		if (Input.GetKeyDown (KeyCode.Period)) {
			Target (targetPlanet.GetComponent<PlanetStats> ().nextPlanet);
		} else if (Input.GetKeyDown (KeyCode.Comma)) {
			Target (targetPlanet.GetComponent<PlanetStats> ().prevPlanet);
		}

		//Câmera Orbital Planetária
		if (Input.GetMouseButton (1) || Input.GetAxis ("Horizontal") != 0 || Input.GetAxis ("Vertical") != 0) {
			if (leavingPlanet > 2) {
				transform.position += transform.right * (Input.GetAxis ("Horizontal") * sensitivity / 10 * Mathf.Sqrt (distance));
				transform.position += transform.up * (Input.GetAxis ("Vertical") * sensitivity / 10 * Mathf.Sqrt (distance));
				if (Input.GetMouseButton (1)) {
					transform.RotateAround (Vector3.zero, transform.forward, Input.GetAxis ("Mouse X") * sensitivity);
				}
			} else if (leavingPlanet == 0) {
				Vector3 centerPoint = targetPlanet.transform.position + (transform.position.y * Vector3.up);
				transform.RotateAround (centerPoint, targetPlanet.transform.up, (Input.GetAxis ("Mouse X") + Input.GetAxis ("Horizontal")) * sensitivity);
				bool canMoveNorthSouth = (transform.localPosition.y < 0.9f && (Input.GetAxis ("Mouse Y") + Input.GetAxis ("Vertical")) < 0) ||
					(transform.localPosition.y > -0.9f && (Input.GetAxis ("Mouse Y") + Input.GetAxis ("Vertical")) > 0);
				if (canMoveNorthSouth) {
					transform.RotateAround (targetPlanet.transform.position, -transform.right, (Input.GetAxis ("Mouse Y") + Input.GetAxis ("Vertical")) * sensitivity);
				}
			}
		}
		//Zoom controlado por FOV para a câmera de curto alcance:
		float newAltitudeValue = thisCamera.fieldOfView + -Input.GetAxis ("Mouse ScrollWheel") * sensitivity * 5;
		if (newAltitudeValue > 10 && newAltitudeValue < maxAltitude && leavingPlanet == 0) { //Zoom Planetário
			thisCamera.fieldOfView += -Input.GetAxis ("Mouse ScrollWheel") * sensitivity * 10;
		} else if (newAltitudeValue > maxAltitude || leavingPlanet == 2) { //Zoom Interplanetário
			ApplyPlanetaryZoomOut ();
		}
		if (Input.GetAxis ("Mouse ScrollWheel") > 0 && leavingPlanet > 2) {
			ApplyPlanetaryZoomIn ();
		}

		//Câmera interplanetária início
		if (leavingPlanet == 0) {
			SlideInUIObjectsOnPlanetMap ();
			stats.UpdateStatBar (); //Atualiza UI
		} else {
			stats.statDisplay.text = "<b> Sistema Solar </b> (Duplo clique para selecionar um planeta)";
		}
		if (leavingPlanet == 3) {
			float espessura = 0.002f * Mathf.Sqrt (thisCamera.transform.position.y);
			float distancia = 50;
			for (int i = 0; i < systemBoundaries / Mathf.Sqrt (thisCamera.transform.position.y); i++) {
				Graphics.DrawMesh (GameData.get.meshLibrary[(int) PrimitiveType.Plane], Matrix4x4.TRS (Vector3.right * i * distancia, Quaternion.identity, new Vector3 (espessura, 1, systemBoundaries)), gridMaterial, 0);
				Graphics.DrawMesh (GameData.get.meshLibrary[(int) PrimitiveType.Plane], Matrix4x4.TRS (Vector3.left * i * distancia, Quaternion.identity, new Vector3 (espessura, 1, systemBoundaries)), gridMaterial, 0);
				Graphics.DrawMesh (GameData.get.meshLibrary[(int) PrimitiveType.Plane], Matrix4x4.TRS (Vector3.forward * i * distancia, Quaternion.identity, new Vector3 (systemBoundaries, 1, espessura)), gridMaterial, 0);
				Graphics.DrawMesh (GameData.get.meshLibrary[(int) PrimitiveType.Plane], Matrix4x4.TRS (Vector3.back * i * distancia, Quaternion.identity, new Vector3 (systemBoundaries, 1, espessura)), gridMaterial, 0);
			}
			Vector3 cursor = farCamera.ScreenToWorldPoint (Input.mousePosition + Vector3.forward * farCamera.transform.position.y) / 5;
			positionMarker.text = "(" + cursor.x.ToString ("+0.0;-0.0;0") + " , " + cursor.z.ToString ("+0.0;-0.0;0") + ")";
			positionMarker.enabled = true;
		} else {
			positionMarker.enabled = false;
		}
	}

	public void ApplyPlanetaryZoomOut () { //Zoom out em modo interplanetário
		distance = (transform.position - targetPlanet.transform.position).magnitude;
		Vector3 planetHover = targetPlanet.transform.position + 10 * Vector3.up * targetPlanet.transform.lossyScale.y;
		if (leavingPlanet == 0) { //Preparação para zoom interplanetário
			if (transform.localPosition.y < 0.8f && Input.GetAxis ("Mouse ScrollWheel") < 0) {
				transform.RotateAround (targetPlanet.transform.position, -transform.right, Input.GetAxis ("Mouse ScrollWheel") * sensitivity * 30);
			} else {
				leavingPlanet = 1;
			}
		}
		if (leavingPlanet == 1) { //Primeiro frame em zoom interplanetário, adapta mapa
			if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
				BuildingBehaviour.HideInfoTooltip ();
				if (PlaceBuilding.building != null) Destroy (PlaceBuilding.building);
				PlaceBuilding.building = null;
				thisCamera.transform.position -= (targetPlanet.transform.lossyScale.y + 1) * transform.forward;
				thisCamera.nearClipPlane = 0.1f;
				thisCamera.farClipPlane = 0.2f;
				farCamera.nearClipPlane = 0.1f;
				leavingPlanet = 2;
			} else {
				leavingPlanet = 0;
			}
		}
		if (leavingPlanet == 2) { //Zoom interplanetário automático
			if (Quaternion.Angle (transform.rotation, Quaternion.LookRotation (Vector3.down)) > 1f || transform.position.y < planetHover.y) {
				transform.position = Vector3.MoveTowards (transform.position, planetHover, distance / 15);
				transform.LookAt (targetPlanet.transform.position, Vector3.up);
				buildBar.transform.localPosition -= Vector3.up * 5; //desce build bar
			} else { //Saindo do planeta
				transform.rotation = Quaternion.LookRotation (Vector3.down);
				buildBar.SetActive (false); //some build bar
				transform.SetParent (null);
				sectorMap.SetActive (true);
				leavingPlanet = 3;
			}
		}
		if (leavingPlanet == 3) { //Zoom interplanetário manual - possível bug: pode acontecer no mesmo último frame do zoom automático
			if (Input.GetAxis ("Mouse ScrollWheel") < 0) {
				transform.position = Vector3.MoveTowards (transform.position, GameData.get.transform.position + (Vector3.up * systemBoundaries), -Input.GetAxis ("Mouse ScrollWheel") * sensitivity * Mathf.Sqrt (distance) * 3);
			}
		}
	}

	public void ApplyPlanetaryZoomIn () { //Zoom in em modo sistema solar (magnet)
		GameObject closestTarget = targetPlanet;
		float closestTargetDistance = 9999;
		Vector3 magnet = farCamera.ScreenToWorldPoint (Input.mousePosition + Vector3.forward * farCamera.transform.position.y);
		foreach (GameObject t in magnetTargets) {
			float newDistance = Vector3.Distance (t.transform.position, magnet);
			if (newDistance < closestTargetDistance) {
				closestTargetDistance = newDistance;
				closestTarget = t;
			}
		}
		magnet = transform.position;
		if (Vector3.Angle (transform.position, closestTarget.transform.position) > 45) {
			magnet = Vector3.MoveTowards (magnet, closestTarget.transform.position + Vector3.up * (magnet.y * 0.8f), Input.GetAxis ("Mouse ScrollWheel") * sensitivity * Mathf.Sqrt (distance) * 3);
		} else {
			magnet = Vector3.MoveTowards (magnet, closestTarget.transform.position, Input.GetAxis ("Mouse ScrollWheel") * sensitivity * Mathf.Sqrt (distance) * 3);
		}
		if ((closestTarget.transform.position - magnet).magnitude > closestTarget.transform.lossyScale.magnitude) {
			transform.position = magnet;
		} else if (closestTarget.GetComponent<PlanetStats> () != null) {
			Target (closestTarget);
		}
	}

	public void Target (GameObject newTarget) { //Muda alvo da câmera orbital (visão de planeta)
		leavingPlanet = 0;
		BuildingBehaviour.HideInfoTooltip ();
		transform.parent = newTarget.transform;
		targetPlanet = newTarget;
		transform.localPosition = Vector3.back;
		thisCamera.farClipPlane = newTarget.transform.lossyScale.z + 1;
		farCamera.nearClipPlane = newTarget.transform.lossyScale.z * 2;
		thisCamera.fieldOfView = 75;
		farCamera.fieldOfView = 30;
		transform.LookAt (newTarget.transform);
		stats = newTarget.GetComponent<PlanetStats> ();
		sectorMap.SetActive (false);
	}

	public void SlideInUIObjectsOnPlanetMap () {
		if (!buildBar.activeSelf) {
			buildBar.GetComponent<RectTransform> ().anchoredPosition = Vector3.down * 42;
			buildBar.SetActive (true);
		}
		if (buildBar.GetComponent<RectTransform> ().anchoredPosition.y < -5) {
			buildBar.GetComponent<RectTransform> ().anchoredPosition += Vector2.up * 5;
		} else {
			buildBar.GetComponent<RectTransform> ().anchoredPosition = Vector3.zero;
		}
	}
}