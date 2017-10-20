using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*	Esta classe é responsável por controlar as órbitas dos planetas			*
 *	e movimentos relacionados dentro da visão de mapa do sistema solar.		*/
public class PlanetOrbit : MonoBehaviour {

	//Variáveis do editor
    [SerializeField] GameObject parentBody;
	[SerializeField] bool tidallyLocked;
	[SerializeField] UnityEngine.UI.Text tooltip; //Tooltip do planeta realçado

	//Variáveis internas
	Behaviour halo; //Realça um planeta distante sob o mouse
	PlanetStats stats;
	float orbitDistance;


    void Start() {
		//Inicializações
        stats = GetComponent<PlanetStats>();
		halo = (Behaviour)GetComponent("Halo");
		orbitDistance = (transform.position - parentBody.transform.position).magnitude;
	}

	void FixedUpdate () {
		//Rotaciona planeta
		if (!tidallyLocked) {
			transform.Rotate(Vector3.up, (360 / stats.dayLength) * Time.deltaTime * GameData.get.gameSpeed);
		}
		//Corrige erros acumulados de atitude causados por referenciais móveis
		transform.position = parentBody.transform.position
								+ (transform.position - parentBody.transform.position).normalized * orbitDistance;
		//Translaciona o planeta
		transform.RotateAround(	parentBody.transform.position,
								Vector3.up, (360 / (stats.yearLength * 24)) * Time.deltaTime * GameData.get.gameSpeed);
		//Caso rotação total deva ser 0 (Tidal Lock)
		if (tidallyLocked) {
			transform.LookAt(parentBody.transform.position);
		}

		//Limpa halos
		if (halo.enabled || tooltip.enabled) {
			if (Camera.main.GetComponent<CameraOrbit>().leavingPlanet==0) {
				halo.enabled = false;
				tooltip.enabled = false;
			}
		}
	}

	void OnMouseOver() {
		if (transform != Camera.main.transform.parent) { //Quando o mouse está sobre um planeta distante
			halo.enabled = true;
			tooltip.text = GetComponent<PlanetStats>().planetName;
			tooltip.transform.position = new Vector2(Input.mousePosition.x + 10, Input.mousePosition.y);
			tooltip.enabled = true;
			if (GameData.get.DoubleClick()) {
				Camera.main.GetComponent<CameraOrbit>().Target(gameObject);
			}
		}
	}

	private void OnMouseExit() {
		halo.enabled = false;
		tooltip.enabled = false;
	}
}
