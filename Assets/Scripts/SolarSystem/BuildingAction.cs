using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingAction : MonoBehaviour {
	public void LaunchShip() { //placeholder
		PlanetStats stats = Camera.main.GetComponent<CameraOrbit>().targetPlanet.GetComponent<PlanetStats>();
		if (stats.money > 10000) {
			stats.ships++;
			stats.money -= 10000;
		} else {
			stats.AlertMoney();
		}
	}
}
