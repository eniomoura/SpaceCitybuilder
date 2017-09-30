using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*	Esta classe é responsável por controlar as órbitas dos planetas			*
 *	e movimentos relacionados dentro da visão de mapa do sistema solar.		*/
public class PlanetOrbit : MonoBehaviour {

	//Variáveis do editor
    [SerializeField] GameObject parentBody;
	[SerializeField] bool tidallyLocked;

	//Variáveis internas
	PlanetStats stats;
	float orbitDistance;


    void Start() {
		//Inicializações
        stats = GetComponent<PlanetStats>();
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
	}
}
