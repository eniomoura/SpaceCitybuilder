using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*	Esta classe é usada para implementar funções do cursor do mouse			*
 *	dentro da visão de construção dos planetas, especialmente as funções	*
 *	relacionadas com o sistema de 'ploppar' construções em si.				*/
public class PlaceBuilding: MonoBehaviour {

	//Variáveis do editor
	[SerializeField] Camera mainCamera;
	[SerializeField] Light mouseLight;

	//Variáveis gerais do programa --mover?
	public GameObject[] buildings;
	public GameObject ploppedBuildingInfo;
	public UnityEngine.UI.Image bulldozeButton;
	public static bool bulldozeMode = false;
	public static GameObject building;

	//Variáveis internas
	Ray ray; RaycastHit hit;
	bool cursorLanded;

	void Start() {
		//Inicializações
		BuildingBehaviour.tooltipPanel = ploppedBuildingInfo;
	}

	void Update() {
		//O cursor está sobre um planeta?
		ray = mainCamera.ScreenPointToRay(Input.mousePosition);
		cursorLanded = Physics.Raycast(ray, out hit, mainCamera.farClipPlane, -257);

		MouseLight(); //Ilumine o cursor

		if (building != null) { //Se há construção selecionada
			if (cursorLanded) {
				//Atualize posição da nova construção
				building.transform.position = hit.point;
				building.transform.LookAt(hit.transform.position);

				if (Input.GetMouseButtonDown(0)) { //Left Click
					if (hit.transform.gameObject.CompareTag("Planet")) {
						if (hit.transform.gameObject.GetComponent<PlanetStats>().NewBuilding(building)) {
							//Construção criada com sucesso, tornando-a permanente
							building.layer = 0;
							building.transform.parent = hit.transform;
							building = null;
						} else {
							//Se os requerimentos da construção não obtiveram sucesso
							hit.transform.gameObject.GetComponent<PlanetStats>().AlertMoney();
							Destroy(building);
						}
					} else if (hit.transform.gameObject.name.Split('(')[0].Equals("Building")) {
						//Se uma construção for construída sobre outra, evolua a construção original
						hit.transform.GetComponent<BuildingBehaviour>().UpgradeBuilding();
						Destroy(building);
						building = null;
					}
				}
			} else { //Cursor está fora do planeta
				building.transform.position = Vector3.zero;

			}
		} else if (Input.GetMouseButtonDown(0)) { //Left Click, se nao ha construcao selecionada
			if (cursorLanded && hit.transform.tag.Equals("Ploppable") && bulldozeMode) { //se modo 'demolir' ativado
				Destroy(hit.transform.gameObject);
			}
		}
		if (Input.GetKeyDown(KeyCode.B)) {
			ToggleBulldozeMode();
		}
	}

	void MouseLight() { //Função controla a luz do mouse e highlight sobre construções
		if (cursorLanded && hit.collider.transform.parent == null) {
			mouseLight.gameObject.SetActive(true);
			mouseLight.range = hit.transform.localScale.magnitude/10;
			mouseLight.transform.position = hit.point - ray.direction * mouseLight.range *0.05f;
		} else {
			mouseLight.gameObject.SetActive(false);
		}
	}

	public void SelectBuilding(int id) { //Implementa os botões de construção da UI
		BuildingBehaviour.HideInfoTooltip();
		BuildingBehaviour.highlight.SetColor("_EmissionColor", Color.cyan);
		bulldozeButton.color = Color.white;
		bulldozeMode = false;
		if (building == null) {
			building = buildings[id];
			building = Instantiate(building, Vector3.zero, Quaternion.identity);
		} else {
			Destroy(building);
		}
	}

	public void ToggleBulldozeMode() {
		if (building != null) Destroy(building);
		building = null;
		BuildingBehaviour.HideInfoTooltip();
		if (bulldozeMode) {
			BuildingBehaviour.highlight.SetColor("_EmissionColor", Color.cyan);
			bulldozeButton.color = Color.white;
			bulldozeMode = false;
		} else {
			BuildingBehaviour.highlight.SetColor("_EmissionColor", Color.red);
			bulldozeButton.color = Color.red;
			bulldozeMode = true;
		}
	}
}