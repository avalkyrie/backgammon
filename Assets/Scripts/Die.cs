using UnityEngine;
using System.Collections;

public class Die : MonoBehaviour {
							
	int result = -1;

	public int Value () {
		return result;
	}
	
	public void Reset () {
		result = -1;
	}

	void Update () {
		Rigidbody body = this.GetComponent<Rigidbody>(); 
		float speed = body.velocity.magnitude;

		if (speed < float.Epsilon && body.constraints != RigidbodyConstraints.FreezeAll) {
			body.velocity = Vector3.zero;
			speed = 0;
			
			body.constraints = RigidbodyConstraints.FreezeAll;
			
			var x = transform.rotation.eulerAngles.x;
			var z = transform.rotation.eulerAngles.z;
			
			if (z >= 315 || z < 45) {
				if (x >= 315 || x < 45) {
					result = 2;					
				} else if (x >= 225) {
					result = 1;
				} else if (x >= 135) {
					result = 5;
				} else if (x >= 45) {				
					result = 6;
				} 
			} else if (z >= 225) {
				result = 4;
			} else if (z >= 135) {
				result = 5;
			} else if (z >= 45) {				
				result = 3;
			} 
		}
	}
}
