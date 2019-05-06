using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour {

    [SerializeField]
    float scaleYMax;

    float scaleY;

	// Use this for initialization
	void Awake () {
        scaleY = Random.Range(1, scaleYMax + 1);
        transform.localScale = new Vector3(transform.localScale.x, scaleY, transform.localScale.z);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
