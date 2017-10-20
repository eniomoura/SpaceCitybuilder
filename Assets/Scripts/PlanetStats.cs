using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*	Esta classe é responsável por armazenar todos os dados específicos		*
 *	de um planeta, e é a classe responsável por implementar a relação entre	*
 *  esses dados - o sistema de 'citybuilding' em si.						*/
public class PlanetStats: MonoBehaviour {

	//Identificação do planeta
	public string planetName;
	public UnityEngine.UI.Text statDisplay;
	public List <BuildingBehaviour> buildings; //Em algum ponto, pode ser útil usar essa lista para recalcular valores.
	public GameObject nextPlanet;
	public GameObject prevPlanet;

	//Características físicas
	public float yearLength;
	public float dayLength;

	//Recursos acumulados (Citybuilder)
	public int population;
	public int ships=0; //placeholder
	public float money;
	public float metal;

	//Capacidades utilizadas (Citybuilder)
	public int energySpent;
	public int energyProduced;
	public bool overCapacity=false;

	//Recursos contínuos (Citybuilder)
	public int prod;
	public float metalPorS;
	public float taxes;
	public float revenue;
	public float maintenance;

	//Valores Internos
	private string statBar;
	private float lackMoneyTimer;

	void Awake() {
		foreach (BuildingBehaviour b in GetComponentsInChildren<BuildingBehaviour>()) {
			money += b.costMoney;
			metal += b.costMetal;
			NewBuilding(b.gameObject);
		}
	}

	void Start() {
		foreach(BuildingBehaviour b in buildings) {
			b.OnMouseExit();
		}
	}

	void Update() {
		if (energySpent > energyProduced && !overCapacity) {
			overCapacity = true;
			maintenance = maintenance * 2;
		} else if (overCapacity == true && energyProduced > energySpent) {
			overCapacity = false;
			maintenance = maintenance / 2;
		}
		if (lackMoneyTimer > 0) { lackMoneyTimer -= Time.deltaTime; }

		//Recalcular recursos contínuos
		metal += metalPorS * Time.deltaTime * GameData.get.gameSpeed;
		money += (revenue - maintenance) * Time.deltaTime * GameData.get.gameSpeed;
		revenue = population * prod * taxes;
	}

	public void UpdateStatBar() { //Atualiza a UI no topo da tela - Método chamado pela câmera que orbita o planeta atual
		statBar = "<b>Setor " + planetName + "</b>  " +
			"	|	Pop. " + population + "	|	";

		if (lackMoneyTimer<=0) {
			statBar +=(int)money + "$ (" + (revenue - maintenance).ToString("+0;-#") + ")	|	" + (int)metal + " Metais (" + (metalPorS).ToString("+0;-#") + ")";
		} else {
			statBar += "<color=red><b>"+(int)money + "$ (" + (revenue - maintenance).ToString("+0;-#") + ")	|	" + (int)metal + " Metais ("+ (metalPorS).ToString("+0;-#") + ")	</b></color>";
		}

		statBar += "	|	" + prod + " Produtividade	|	";
		if (energySpent > energyProduced) {
			statBar += "<color=red>"+ energySpent + "/" + energyProduced + " Energia"+"</color>";
		} else {
			statBar += energySpent + "/" + energyProduced + " Energia";
		}
		if (ships > 0) { //placeholder
			statBar += "	|	" + ships + " Naves";
		}
		statDisplay.text = statBar;
	}

	public bool NewBuilding(GameObject building) {
		BuildingBehaviour req = building.GetComponent<BuildingBehaviour>();
		bool cantBuild = ((money < req.costMoney && req.costMoney!=0) || (metal < req.costMetal && req.costMetal != 0));
		if (!cantBuild) {
			money -= req.costMoney;
			metal -= req.costMetal;
			metalPorS -= req.metalInput;
			population += req.pop;
			energyProduced += req.powProd;
			energySpent += req.powSpent;
			prod += req.prod;
			metalPorS += req.metalOutput;
			req.TurnOn();
			if (overCapacity) {
				maintenance += req.maintenance * 2;
			} else {
				maintenance += req.maintenance;
			}
			buildings.Add(req);
		}
		return !cantBuild;
	}

	public void RemoveBuilding(GameObject building) {
		BuildingBehaviour req = building.GetComponent<BuildingBehaviour>();
		if (overCapacity) {
			maintenance -= req.maintenance * 2;
		} else {
			maintenance -= req.maintenance;
		}
		population -= req.pop;
		energyProduced -= req.powProd;
		energySpent -= req.powSpent;
		prod -= req.prod;
		metalPorS -= req.metalOutput;
		metalPorS += req.metalInput;
		buildings.Remove(req);
	}

	public void AlertMoney() {
		lackMoneyTimer = .5f;
	}
}
