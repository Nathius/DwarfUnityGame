using UnityEngine;
using System.Collections;

public class ProductionNumber : MonoBehaviour {

    private static float TotalLifeTime = 1f;
    private static float DriftSpeed = 1.5f;

    private float TimeRemaining;


	// Use this for initialization
	void Start () {
        TimeRemaining = TotalLifeTime;
	}
	
	// Update is called once per frame
	void Update () {

        TimeRemaining -= Time.deltaTime;

        if (TimeRemaining > 0)
        {
            var last = this.transform.position;
            var drift = Time.deltaTime * DriftSpeed;
            this.transform.position = new Vector3(last.x, last.y + drift, last.z);
        }
        else
        {
            Destroy(this.gameObject);
            Destroy(this);
        }
	}
}
