using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*	Esta classe é responsável por controlar as órbitas dos planetas			*
 *	e movimentos relacionados dentro da visão de mapa do sistema solar.		*/
public class PlanetOrbit : MonoBehaviour {

	//Variáveis do editor
	public GameObject parentBody;
	public bool tidallyLocked;
	public UnityEngine.UI.Text tooltip; //Tooltip do planeta realçado
	public string planetName;
	public float yearLength;
	public float dayLength;
	public bool isSector;

	//Variáveis internas
	Behaviour halo; //Realça um planeta distante sob o mouse
	float orbitDistance;

	void Start () {
		//Inicializações
		PlanetStats stats = GetComponent<PlanetStats> ();
		if (stats != null) {
			planetName = stats.planetName;
			yearLength = stats.yearLength;
			dayLength = stats.dayLength;
		}
		halo = (Behaviour) GetComponent ("Halo");
		orbitDistance = (transform.position - parentBody.transform.position).magnitude;
	}

	void FixedUpdate () {
		//Rotaciona planeta
		if (!tidallyLocked) {
			transform.Rotate (Vector3.up, (360 / dayLength) * Time.deltaTime * GameData.get.gameSpeed);
		}
		//Corrige erros acumulados de atitude causados por referenciais móveis
		transform.position = parentBody.transform.position +
			(transform.position - parentBody.transform.position).normalized * orbitDistance;
		//Translaciona o planeta
		transform.RotateAround (parentBody.transform.position,
			Vector3.up, (360 / (yearLength * 24)) * Time.deltaTime * GameData.get.gameSpeed);
		//Caso rotação total deva ser 0 (Tidal Lock)
		if (tidallyLocked) {
			transform.LookAt (parentBody.transform.position);
		}

		//Limpa halos
		if (!isSector && (halo.enabled || tooltip.enabled)) {
			if (Camera.main && Camera.main.GetComponent<CameraOrbit> ().leavingPlanet == 0) {
				halo.enabled = false;
				tooltip.enabled = false;
			}
		}
	}

	void OnMouseOver () {
		if (ToggleMode.mode == "map" && transform != Camera.main.transform.parent) { //Quando o mouse está sobre um planeta distante
			halo.enabled = true;
			tooltip.text = planetName;
			tooltip.transform.position = new Vector2 (Input.mousePosition.x + 10, Input.mousePosition.y);
			tooltip.enabled = true;
			if (GameData.get.DoubleClick () && !isSector) {
				Camera.main.GetComponent<CameraOrbit> ().Target (gameObject);
			}
		}
	}

	private void OnMouseExit () {
		if (!isSector) halo.enabled = false;
		tooltip.enabled = false;
	}
}