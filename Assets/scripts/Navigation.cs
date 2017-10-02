using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour {
    private bool lastTouchValid = false;
    private Vector2 lastTouchPosition;
    public float panSpeed = 0.1f;
    private Rigidbody cameraRigidbody;
    public float zoom = 1.0f;
    public AnimationCurve cameraHeightZoom;
    public AnimationCurve cameraRotationZoom;
    public LineRenderer minimapGuideLines;

    void Awake()
    {
        cameraRigidbody = Camera.main.GetComponent<Rigidbody>();
    }

	// Use this for initialization
	void Start () {
        updateMinimapGuideLines();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonUp(0))
            lastTouchValid = false;

        if(Input.GetMouseButton(0))
        {
            if(!lastTouchValid)
            {
                lastTouchPosition = Input.mousePosition;
                lastTouchValid = true;
            } else {
                Vector2 deltaMousePosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - lastTouchPosition;

                //Camera.main.transform.Translate;
                cameraRigidbody.velocity += new Vector3(-deltaMousePosition.x * panSpeed, 0.0f, -deltaMousePosition.y * panSpeed);

                lastTouchPosition = Input.mousePosition;
            }

            updateMinimapGuideLines();
        }

        if(Input.mouseScrollDelta.magnitude != 0.0f)
        {
            zoom -= Input.mouseScrollDelta.y;

            if (zoom < 1.0f) zoom = 1.0f;
            if (zoom > 15f) zoom = 15.0f;

            Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, cameraHeightZoom.Evaluate(zoom), Camera.main.transform.localPosition.z);
            Camera.main.transform.localRotation = Quaternion.Euler(cameraRotationZoom.Evaluate(zoom), Camera.main.transform.localRotation.y, Camera.main.transform.localRotation.z);

            updateMinimapGuideLines();
        }
	}
    

    void FixedUpdate()
    {
        if(cameraRigidbody.velocity.magnitude > 0.0f)
        {
            cameraRigidbody.velocity /= 2.0f;
        }
    }

    public void updateMinimapGuideLines()
    {
        RaycastHit bottomLeftHit, bottomRightHit, topLeftHit, topRightHit;


        Ray bottomLeft = Camera.main.ScreenPointToRay(new Vector2(0.0f, 0.0f));
        Ray bottomRight = Camera.main.ScreenPointToRay(new Vector2(Screen.width, 0.0f));
        Ray topLeft = Camera.main.ScreenPointToRay(new Vector2(0.0f, Screen.height));
        Ray topRight = Camera.main.ScreenPointToRay(new Vector2(Screen.width, Screen.height));

        Physics.Raycast(bottomLeft, out bottomLeftHit);
        Physics.Raycast(bottomRight, out bottomRightHit);
        Physics.Raycast(topLeft, out topLeftHit);
        Physics.Raycast(topRight, out topRightHit);

        minimapGuideLines.positionCount = 4;
        minimapGuideLines.SetPosition(0, bottomLeftHit.point + new Vector3(0.0f, 60.0f, 0.0f));
        minimapGuideLines.SetPosition(1, bottomRightHit.point + new Vector3(0.0f, 60.0f, 0.0f));
        minimapGuideLines.SetPosition(2, topRightHit.point + new Vector3(0.0f, 60.0f, 0.0f));
        minimapGuideLines.SetPosition(3, topLeftHit.point + new Vector3(0.0f, 60.0f, 0.0f));

    }
}
