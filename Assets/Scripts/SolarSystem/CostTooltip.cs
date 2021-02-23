using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/* Esta classe é usada para exibir uma tooltip ao construir algo pelo menu	*
 * do citybuilder.															*/
public class CostTooltip : MonoBehaviour {
	[SerializeField] GameObject building;
	[SerializeField] UnityEngine.UI.Text tooltip;
	private BuildingBehaviour behaviour;
	private FieldInfo[] bData;
	private string tooltipText;

	void Start() {
		RecalculateCosts();
	}

	void RecalculateCosts() {
		tooltipText = "";
		behaviour = building.GetComponent<BuildingBehaviour>();
		bData = behaviour.GetType().GetFields();
		foreach (FieldInfo attr in bData) {
			if (!attr.IsDefined(typeof(System.ObsoleteAttribute), true) &&
				 (attr.FieldType.Equals(0.GetType()) || attr.FieldType.Equals(0f.GetType())) &&
				 !attr.GetValue(behaviour).ToString().Equals(0.ToString())) {
				switch (attr.Name) {
					case "pop":
						tooltipText += "Abriga " + attr.GetValue(behaviour) + " residentes.\n";
						break;
					case "costMoney":
						tooltipText += "Custa " + attr.GetValue(behaviour) + "$\n";
						break;
					case "costMetal":
						tooltipText += "Custa " + attr.GetValue(behaviour) + " metais.\n";
						break;
					case "powSpent":
						tooltipText += "Consome " + attr.GetValue(behaviour) + " unidades de energia.\n";
						break;
					case "powProd":
						tooltipText += "Produz " + attr.GetValue(behaviour) + " unidades de energia.\n";
						break;
					case "prod":
						tooltipText += "Adiciona " + attr.GetValue(behaviour) + " produtividade.\n";
						break;
					case "metalInput":
						tooltipText += "Consome " + attr.GetValue(behaviour) + " metais por segundo.\n";
						break;
					case "metalOutput":
						tooltipText += "Produz " + attr.GetValue(behaviour) + " metais por segundo.\n";
						break;
					case "maintenance":
						tooltipText += "Custa " + attr.GetValue(behaviour) + "$ em manutenção por segundo.\n";
						break;
					default:
						tooltipText += attr.Name + ": " + attr.GetValue(behaviour);
						break;
				}
			}
		}
	}

	public void EnableTooltip() {
		tooltip.text = tooltipText;
		tooltip.enabled = true;
	}

	public void DisableTooltip() {
		tooltip.enabled = false;
	}
}
