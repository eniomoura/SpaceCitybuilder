using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

/* Esta classe é responsável por especificar o tipo de dado BuildingBehaviour,	*
 * que especifica o comportamento de instâncias de construções do citybuilder.	*/
public class BuildingBehaviour : MonoBehaviour {
	//Dados de uma construção - Valores int e float aparecem na tooltip de custo da barra de construção.
	List<Material> originalMaterials = new List<Material> ();
	public static GameObject tooltipPanel;
	public static BuildingBehaviour holdsTooltip;
	public static Material highlight;
	[SerializeField] public UnityEvent[] actions;
	UnityEngine.EventSystems.EventSystem uiManager;
	public string label;
	public int pop;
	public float costMoney;
	public float costMetal;
	public int powSpent;
	public int powProd;
	public int prod;
	public float metalInput;
	public float metalOutput;
	public float maintenance;
	private bool isOnline = false;
	private bool justBuilt = false;

	private void Awake () {
		MeshRenderer[] originalMeshes = GetComponentsInChildren<MeshRenderer> ();
		uiManager = GameObject.Find ("EventSystem").GetComponent<UnityEngine.EventSystems.EventSystem> ();
		for (int i = 0; i < originalMeshes.Length; i++) {
			originalMaterials.Add (originalMeshes[i].material);
		}
	}

	public void UpgradeBuilding () { //Aumenta o tamanho de um prédio. Placeholder.
		switch (name.Split ('(') [0]) {
			case "Building":
				if (transform.localScale.z < 0.08f) {
					transform.localScale += Vector3.forward * 0.03f;
				}
				break;
		}
	}

	private void OnDestroy () {
		if (isOnline) {
			transform.parent.GetComponent<PlanetStats> ().RemoveBuilding (gameObject);
		}
	}

	public void OnMouseOver () {
		if (Camera.main.transform.parent == transform.parent && isOnline) {
			foreach (MeshRenderer m in GetComponentsInChildren<MeshRenderer> ()) {
				m.material = highlight;
			}
			if (Input.GetMouseButtonUp (0) && !justBuilt && !PlaceBuilding.bulldozeMode) {
				ShowInfoTooltip ();
			}
		}
	}

	public void ShowInfoTooltip () {
		if (!uiManager.IsPointerOverGameObject ()) {
			if (this.Equals (holdsTooltip)) {
				HideInfoTooltip ();
			} else {
				HideInfoTooltip ();
				int tLines = 1;
				string info;
				info = "<b>" + label + "</b>\n";
				if (pop != 0) { info += "População: " + pop + "\n"; tLines++; }
				if (powSpent != 0) { info += "Energia gasta: " + powSpent + "Gj\n"; tLines++; }
				if (powProd != 0) { info += "Energia prod.: " + powProd + "Gj\n"; tLines++; }
				if (prod != 0) { info += "Produtividade: " + prod + "\n"; tLines++; }
				if (metalInput != 0) { info += "Metal gasto: " + metalInput + "/s\n"; tLines++; }
				if (metalOutput != 0) { info += "Metal minado: " + metalOutput + "/s\n"; tLines++; }
				if (maintenance != 0) { info += "Manutenção: " + maintenance + "$/s\n"; tLines++; }
				tooltipPanel.GetComponentInChildren<Text> ().text = info;
				DisplayTooltipActions ();
				holdsTooltip = this;
				tooltipPanel.GetComponent<RectTransform> ().SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, (tLines * 20) + (actions.Length * 35) + 5);
				tooltipPanel.SetActive (true);
			}
		}
	}

	public void DisplayTooltipActions () {
		if (actions.Length > 0) {
			Button actionButtonTemplate = tooltipPanel.GetComponentInChildren<Button> (true);
			for (int i = 0; i < actions.Length; i++) {
				Button newButton = Instantiate (actionButtonTemplate, tooltipPanel.transform, true);
				newButton.transform.position += (Vector3.up * i * 30);
				newButton.onClick.AddListener (actions[i].Invoke);
				switch (actions[i].GetPersistentMethodName (0)) {
					case "LaunchShip":
						newButton.name = "Lançar Nave";
						break;
					default:
						newButton.name = GameData.ParseCamel (actions[i].GetPersistentMethodName (0));
						break;
				}
				newButton.GetComponentInChildren<Text> ().text = newButton.name;
				newButton.gameObject.SetActive (true);
			}
		}
	}

	public void OnMouseExit () {
		MeshRenderer[] newMeshes = GetComponentsInChildren<MeshRenderer> ();
		for (int i = 0; i < newMeshes.Length; i++) {
			newMeshes[i].material = originalMaterials[i];
		}
		justBuilt = false;
	}

	public void Update () {
		if (this.Equals (holdsTooltip) && !Input.GetMouseButton (0) && Camera.main) {
			tooltipPanel.transform.position = Camera.main.WorldToScreenPoint (transform.position);
		}
		if (Input.GetMouseButton (0) && !uiManager.IsPointerOverGameObject ()) {
			HideInfoTooltip ();
		}
	}

	public static void HideInfoTooltip () {
		if (holdsTooltip != null) {
			Button[] buttons = tooltipPanel.GetComponentsInChildren<Button> ();
			for (int i = 0; i < buttons.Length; i++) {
				Destroy (buttons[i].gameObject);
			}
		}
		holdsTooltip = null;
		tooltipPanel.SetActive (false);
	}

	public void TurnOn () {
		isOnline = true;
		justBuilt = true;
	}
}