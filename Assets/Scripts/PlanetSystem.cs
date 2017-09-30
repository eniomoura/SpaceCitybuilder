using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*	Essa classe é responsável pela implementação de sistemas usados			*
 *	pelo mapa do sistema solar, e integração deste com o resto do jogo.		*/
public class PlanetSystem : MonoBehaviour {

	Behaviour halo; //Realça um planeta distante sob o mouse
	[SerializeField] UnityEngine.UI.Text tooltip; //Tooltip do planeta realçado

	void Start() {
		//Inicializadores
		halo = (Behaviour) GetComponent("Halo");
	}

	void OnMouseOver() {
		if (transform != Camera.main.transform.parent) { //Quando o mouse está sobre um planeta distante
			halo.enabled = true;
			tooltip.text = GetComponent<PlanetStats>().planetName;
			tooltip.transform.position = new Vector2(Input.mousePosition.x+10,Input.mousePosition.y);
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
