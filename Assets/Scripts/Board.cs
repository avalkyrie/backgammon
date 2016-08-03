using UnityEngine;
using System.Collections.Generic; 


public class Board : MonoBehaviour {
	public Die diePrefab;
	public Chip whiteChip;
	public Chip redChip;

	public Die[] dice = new Die[2];

	// Clockwise counting from bottom right corner
	const int LocationsCount = 4;
	const int SpacesPerLocation = 6; 
	const int NumberOfLocations = LocationsCount * SpacesPerLocation;
	
	// Current locations of chips on board
	List<List<Chip>> locations = new List<List<Chip>>();
	List<Vector3> transformForLocation = new List<Vector3>();
	
	const float XOffset = 2.6f;
	const float ZOffset = 2.1f; 
	const float BridgeOffset = 1.0f;
	
	void Start() {
		//Random.seed = System.Environment.TickCount;
		Random.seed = (int)System.DateTime.Now.Ticks;
		print("Seed:" + Random.seed);
		
		// Count of each chip in each position to start		
		var Counts = new List<int>() {
			2, 0, 0, 0, 0, 5,
			0, 3, 0, 0, 0, 5,
			5, 0, 0, 0, 3, 0,
			5, 0, 0, 0, 0, 2,	
		};
		var IsRed = new List<bool>() {
			false, false, false, false, false, true,
			false, true, false, false, false, false,
			true, false, false, false, false, false,
			false, false, false, false, false, true,
		};
		
		var InitialPosition = transform.position;
		InitialPosition.y = 0.4f;
		InitialPosition.x = 16.0f;
		InitialPosition.z = -9.8f;
		
		var counter = 0;
		while (counter < NumberOfLocations) {
			transformForLocation.Add(InitialPosition);

			var sign = SignForLocation(counter);

			if (counter == 5 || counter == 17) {
				InitialPosition.x -= sign * BridgeOffset;
			}
			
			// Flip to opposite side of board
			if (counter == 11) {
				InitialPosition.z += 20.0f;
			} else {
				InitialPosition.x -= sign * XOffset;	
			}
			
			locations.Add(new List<Chip>());

			counter++;
		}
		
		counter = 0; 
		while (counter < NumberOfLocations) {
			var chipCount = Counts[counter];
			while (chipCount > 0) {
				Chip chip;
				if (IsRed[counter]) {
					chip = Instantiate(redChip);
				} else {
					chip = Instantiate(whiteChip);
				}
				
				PlaceChipAtLocation(chip, counter);						
				chipCount--;
			}
			counter++;
		}
	}
	
	public void PlaceChipAtLocation(Chip chip, int location) {
		var chips = locations[location];
		var position = transformForLocation[location];
		position.z += SignForLocation(location) * ZOffset * chips.Count;
		chip.transform.position = position;		locations[location].Add(chip);
	}
	
	public void UpdateChipAtLocation(Chip chip, int location) {
		var chips = locations[location];
		var position = transformForLocation[location];
		var chipPosition = chips.IndexOf(chip);
		position.z += SignForLocation(location) * ZOffset * chipPosition;
		chip.transform.position = position;
	}
	
	public void MoveChipToBoardLocation(Chip chip, Vector3 location) {
		RemoveChip(chip);
		Vector3 chipCenter = chip.transform.position;
		PlaceChipAtLocation(chip, 0);
	}
	
	public void RemoveChip(Chip chip) {
		if (chip == null) {
			return;
		}
		
		var location = 0;
		bool didRemoveChip = false;
		for (; location < locations.Count; location++) {
			var chips = locations[location];
			if (chips.Contains(chip)) {
				chips.Remove(chip);
				didRemoveChip = true;
				break;
			}
		}
		
		// Lazy mode… reset positions of all other chips in stack
		if (didRemoveChip && location < locations.Count) {
			foreach (var updatedChip in locations[location]) {
				UpdateChipAtLocation(updatedChip, location);
			}
		}
	}
	
	static int SignForLocation(int location) {
		return (location > 11) ? -1 : 1;
	}

	public void RollDice() {
		// TODO : Figure out the right way to hide things
		dice[0] = Instantiate(diePrefab);
		dice[1] = Instantiate(diePrefab);

		RollDie(dice[0]);
		RollDie(dice[1]);	
	}
	
	public void HideDice() {
		Destroy(dice[0].gameObject, 1.0f);
		Destroy(dice[1].gameObject, 1.0f);
		dice[0] = null;
		dice[1] = null;
	}
	
	void RollDie(Die die) {
		// TODO: Care more about randomness
		die.transform.position = new Vector3(Random.Range(-3.0f, 3.0f), Random.Range(15.0f, 25.0f), Random.Range(-3.0f, 3.0f));
		die.transform.rotation = Random.rotation;
		die.GetComponent<Rigidbody>().velocity = new Vector3(Random.Range(-3.0f, 3.0f), -10, Random.Range(-3.0f, 3.0f));
	}
}
