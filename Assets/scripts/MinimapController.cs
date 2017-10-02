using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapController : MonoBehaviour {
    public Vector3 customPosition;
    public float padding = 5.1f;

	void Update () {
        float scaleFactor = ((float)Camera.main.pixelWidth) / ((float)Camera.main.pixelHeight);

        this.transform.localPosition = new Vector3((customPosition.y * scaleFactor) - this.transform.localScale.x * padding, customPosition.y - this.transform.localScale.z * padding, customPosition.z);
    }
}
