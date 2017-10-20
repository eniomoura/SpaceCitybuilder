using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*	Esta classe é responsável pelo armazenamento de dados necessários		*
 *	para o funcionamento dos sistemas do jogo, e implementação de funções	*
 *	que servem como base para outras mecânicas do jogo.						*/
public class GameData : MonoBehaviour {

	//GameObjects de sistemas
	[SerializeField] UnityEngine.UI.Text timeIndicator;
	[SerializeField] GameObject exitMenu;

	//Variáveis de sistemas
	public float gameSpeed;
	public float doubleClickSpeed;
	public float lastLeftClick;
	public Material highlight;
	
	//Acessadores
	public static GameData get;

	//Atributos internos
	private float previousSpeed=0; //velocidade do jogo antes de pausar

	void Start () {
		//Inicializações
        get = this; //Singleton deste objeto
		lastLeftClick = 0;
		doubleClickSpeed = 0.5f;
		BuildingBehaviour.highlight = this.highlight;
		BuildingBehaviour.highlight.SetColor("_EmissionColor", Color.cyan);
	}

	void Update () {
		//Sistemas críticos contínuos
		DoubleClickTimer();
		if (Input.GetKeyDown(KeyCode.Space)) {
			PauseGame();
		} else if (Input.GetKeyDown(KeyCode.KeypadPlus)) {
			AccelerateGame();
		} else if (Input.GetKeyDown(KeyCode.KeypadMinus)) {
			SlowGame();
		} else if (Input.GetKeyDown(KeyCode.Escape)) {
			if (PlaceBuilding.building!=null) {
				Destroy(PlaceBuilding.building);
				PlaceBuilding.building = null;
			} else if (BuildingBehaviour.tooltipPanel.activeSelf) {
				BuildingBehaviour.HideInfoTooltip();
			} else {
				if (exitMenu.activeSelf) {
					exitMenu.SetActive(false);
				} else {
					exitMenu.SetActive(true);
				}
			}
		}
	}

	public bool DoubleClick() { //retorna true se dois cliques foram dados em rápida sequência
		return Input.GetKeyDown(KeyCode.Mouse0) && lastLeftClick < doubleClickSpeed;
	}


	void DoubleClickTimer() { //temporiza os cliques do mouse
		if (Input.GetKeyDown(KeyCode.Mouse0)) {
			lastLeftClick = 0;
		} else {
			lastLeftClick += Time.deltaTime;
		}
	}

	public void PauseGame() { //pausa os sistemas dependentes da velocidade de simulação
		if (gameSpeed > 0) {
			previousSpeed = gameSpeed;
			gameSpeed = 0;
			timeIndicator.color = Color.red;
			timeIndicator.text = "||";
			timeIndicator.enabled = true;
		} else { //Restaura o indicador de velocidade
			gameSpeed = previousSpeed;
			if (gameSpeed == 1) {
				timeIndicator.enabled = false;
			} else {
				timeIndicator.color = Color.green;
				timeIndicator.text = "x" + gameSpeed;
			}
		}
	}

	public void AccelerateGame() { //acelera os sistemas dependentes da velocidade de simulação
		if (gameSpeed < 5) {
			gameSpeed++;
		}
		if (gameSpeed > 1) {
			timeIndicator.color = Color.green;
			timeIndicator.text = "x" + gameSpeed;
			timeIndicator.enabled = true;
		} else {
			timeIndicator.enabled = false;
		}
	}

	public void SlowGame() { //desacelera os sistemas dependentes da velocidade de simulação
		if (gameSpeed > 1) {
			gameSpeed--;
			timeIndicator.color = Color.green;
			timeIndicator.text = "x" + gameSpeed;
			timeIndicator.enabled = true;
		}
		if (gameSpeed==1){
			timeIndicator.enabled = false;
		}
	}

	public static string ParseCamel(string s) {
		var r = new System.Text.RegularExpressions.Regex(@"
            (?<=[A-Z])(?=[A-Z][a-z]) |
                (?<=[^A-Z])(?=[A-Z]) |
                (?<=[A-Za-z])(?=[^A-Za-z])", System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace);

		return r.Replace(s, " ");
	}

	public void DisplayHelp() {
		exitMenu.SetActive(true);
		string helpText =	"<b>Left Click</b>: Interagir\n" +
							"<b>Right Click</b>: Mover Câmera Orbital\n" +
							"<b>Mouse Scroll Wheel</b>: Zoom Orbital\n" +
							"<b>'>' ou '.'</b>: Próximo Planeta\n" +
							"<b>'<' ou ','</b>: Planeta Anterior\n" +
							"<b>Double Left Click(em um planeta)</b>: Selecionar Planeta\n" +
							"<b>Esc(enquanto construindo)</b>: Cancelar construção\n" +
							"<b>Numpad '+'</b>: Acelerar tempo\n" +
							"<b>Numpad '-'</b>: Desacelerar tempo\n" +
							"<b>Barra de espaço</b>: Pause / Unpause\n";
		UnityEngine.UI.Text text = exitMenu.GetComponentInChildren<UnityEngine.UI.Text>();
		RectTransform size = exitMenu.GetComponent<RectTransform>();
		UnityEngine.UI.Button[] buttons = exitMenu.GetComponentsInChildren<UnityEngine.UI.Button>(true);
		if (text.text.Equals("MENU")) {
			for (int i = 0; i < buttons.Length; i++) {
				UnityEngine.UI.Text bText = buttons[i].GetComponentInChildren<UnityEngine.UI.Text>();
				if (bText.name.Equals("Ajuda")) {
					bText.name = "VoltarAjuda";
					bText.text = "Voltar";
				} else {
					buttons[i].gameObject.SetActive(false);
				}
			}
			size.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.rect.width*3);
			text.rectTransform.position += new Vector3(25,-10,0);
			text.fontSize = 12;
			text.alignment = TextAnchor.UpperLeft;
			text.text = helpText.Replace("\n", System.Environment.NewLine);
		} else {
			for (int i = 0; i < buttons.Length; i++) {
				UnityEngine.UI.Text bText = buttons[i].GetComponentInChildren<UnityEngine.UI.Text>();
				if (bText.name.Equals("VoltarAjuda")) {
					bText.name = "Ajuda";
					bText.text = "Ajuda";
				} else {
					buttons[i].gameObject.SetActive(true);
				}
			}
			size.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.rect.width /3);
			text.rectTransform.position -= new Vector3(25, -10, 0);
			text.fontSize = 28;
			text.alignment = TextAnchor.MiddleCenter;
			text.text = "MENU";
		}
	}

	public void CloseMainMenu() {
		exitMenu.SetActive(false);
	}

	public void ExitGame() {
		Application.Quit();
		Debug.Log("Game Closed");
	}
}
