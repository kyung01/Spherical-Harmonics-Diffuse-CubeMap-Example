using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraRotation : MonoBehaviour {
    
    float angle = 0;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        angle += Time.deltaTime;
        this.transform.position = new Vector3(Mathf.Cos(angle) * 4, 0.7f,Mathf.Sin(angle)*4);
        this.transform.LookAt(new Vector3());
	}
}
