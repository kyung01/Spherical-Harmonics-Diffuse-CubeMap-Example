using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        this.transform.rotation = Quaternion.Euler(0, this.transform.rotation.eulerAngles.y+Time.deltaTime*10, 0);	
	}
}
