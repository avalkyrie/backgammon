using UnityEngine;
using System.Collections;
using UnityEngine.UI;


public enum GameState {
	Waiting,
	Rolling,
	Moving,
};

public class Game : MonoBehaviour {
	public Player playerPrefab;
	public Board board;
	public Text statusText;
	public Button rollButton;
	public Text rollButtonText;
	
	private Player player1;
	private Player player2;
	
	private GameState gameState = GameState.Waiting;
	private bool isFirstPlayerTurn = true;
	
	private ArrayList moves = new ArrayList();
	
	private Chip chipBeingMoved;
	private Vector3 chipDelta; // diff between mouse and chip origin

	//private int[] moves = new int[4];

	// Use this for initialization
	void Start () {
		player1 = Instantiate(playerPrefab);
		player2 = Instantiate(playerPrefab);
		
		// TESTING MOVEMENT
		moves.Add(1);
		moves.Add(2);
		moves.Add(3);
		moves.Add(4);
		gameState = GameState.Moving;
		
		
		UpdateUI();
	}
	
	// Update is called once per frame
	void Update () {
		if (gameState == GameState.Rolling) {			
			int die1 = board.dice[0].Value();
			int die2 = board.dice[1].Value();
			
			if (die1 != -1 && die2 != -1) {
				moves.Add(die1);
				moves.Add(die2);
				
				if (die1 == die2) {
					// doubles
					moves.Add(die1); 
					moves.Add(die1);
				}
				
				gameState = GameState.Moving;
				UpdateUI();
				board.HideDice();

			}	
		} else if (gameState == GameState.Moving) {
			if (Input.GetMouseButtonDown(0)) {
				if (chipBeingMoved == null) {
					RaycastHit hit;
					
					if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)) {
						chipBeingMoved = hit.transform.gameObject.GetComponent<Chip>();
						
						if (chipBeingMoved == null) {
							print ("didn't click anything");
						} else {
							chipDelta = hit.point - chipBeingMoved.transform.position;	
						}
					}
				} else {
					RaycastHit hit;
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					var layerMask = 1 << 8;

					if (Physics.Raycast(ray, out hit, 100, layerMask)) {						
						board.MoveChipToBoardLocation(chipBeingMoved, hit.transform.position);
						print (hit.transform.gameObject);
						
						chipBeingMoved = null;
						chipDelta = Vector3.zero;
					}
				}
			}
			
			if (chipBeingMoved != null) {
				RaycastHit hit;
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				if (Physics.Raycast(ray, out hit, 100)) {
										
					var vector = hit.point - chipDelta;
					// don't go under the board
					vector.y = chipBeingMoved.transform.position.y;
					chipBeingMoved.transform.position = vector; 
				}
			}
 		}
	}
	
	public void RollDice () {
		gameState = GameState.Rolling;
		board.RollDice();
		UpdateUI();
	}
	
	void UpdateUI() {
		switch (gameState)
		{
			case GameState.Waiting:
				if (isFirstPlayerTurn) {
					statusText.text = "Player 1's turn";
				} else {
					statusText.text = "Player 1's turn";
				}
				rollButtonText.text = "Roll dice";
				rollButton.interactable = true;
			break;
			
			case GameState.Rolling:
				statusText.text = "Rolling";
				rollButton.interactable = false;
			break;
			
			case GameState.Moving:
				var movesString = "Remaining:";
				
				foreach (var move in moves) {
					movesString = movesString + " " + move;
				}
				
				statusText.text = movesString;
				rollButton.interactable = false;
			break;
		}
	}
}
