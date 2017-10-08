using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigation : MonoBehaviour {
    private bool lastTouchValid = false;
    private Vector2 lastTouchPosition;
    public float panSpeed = 0.1f;
    private Rigidbody cameraRigidbody;
    public float zoom = 1.0f;
    private float lastZoom = 1.0f;
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
                cameraRigidbody.velocity += new Vector3(deltaMousePosition.x * panSpeed, 0.0f, deltaMousePosition.y * panSpeed);

                lastTouchPosition = Input.mousePosition;
            }

            updateMinimapGuideLines();
        }

        //float zoomedValue = calculateZoomedValue();

        //if(zoomedValue != 0.0f)
        //{
        //    zoom -= zoomedValue;

        //    if (zoom < 1.0f) zoom = 1.0f;
        //    if (zoom > 9.0f) zoom = 9.0f;

        //    cameraRigidbody.velocity += new Vector3(0.0f, (zoom - lastZoom) * 100.0f, 0.0f);

        //    lastZoom = zoom;

        //    //Camera.main.transform.localPosition = new Vector3(Camera.main.transform.localPosition.x, cameraHeightZoom.Evaluate(zoom), Camera.main.transform.localPosition.z);
        //    Vector3 localRotation = Camera.main.transform.localRotation.eulerAngles;
        //    Camera.main.transform.localRotation = Camera.main.transform.localRotation = Quaternion.Euler(cameraRotationZoom.Evaluate(zoom), localRotation.y, localRotation.z);

        //    updateMinimapGuideLines();
        //}
	}

//    private float lastTouchDistance = -1.0f;

//    private float calculateZoomedValue()
//    {
//#if UNITY_ANDROID
//        if (Input.touchCount != 2)
//        {
//            lastTouchDistance = -1.0f;
//            return 0.0f;
//        }

//        if (lastTouchDistance < 0.0f) {
//            lastTouchDistance = (Input.GetTouch(0).rawPosition - Input.GetTouch(1).rawPosition).magnitude;
//            return 0.0f;
//        } else
//        {
//            float distance = (Input.GetTouch(0).rawPosition - Input.GetTouch(1).rawPosition).magnitude - lastTouchDistance;
//            lastTouchDistance = (Input.GetTouch(0).rawPosition - Input.GetTouch(1).rawPosition).magnitude;

//            return distance / 10.0f;

//        }

//#else
//        return Input.mouseScrollDelta.y;
//#endif
//    }


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


        int layerMask = ~LayerMask.GetMask("Bee", "Ignore Raycast", "Outlined", "Flower");


        Physics.Raycast(bottomLeft, out bottomLeftHit, 100.0f, layerMask);
        Physics.Raycast(bottomRight, out bottomRightHit, 100.0f, layerMask);
        Physics.Raycast(topLeft, out topLeftHit, 100.0f, layerMask);
        Physics.Raycast(topRight, out topRightHit, 100.0f, layerMask);

        minimapGuideLines.positionCount = 4;
        minimapGuideLines.SetPosition(0, bottomLeftHit.point + new Vector3(0.0f, 60.0f, 0.0f));
        minimapGuideLines.SetPosition(1, bottomRightHit.point + new Vector3(0.0f, 60.0f, 0.0f));
        minimapGuideLines.SetPosition(2, topRightHit.point + new Vector3(0.0f, 60.0f, 0.0f));
        minimapGuideLines.SetPosition(3, topLeftHit.point + new Vector3(0.0f, 60.0f, 0.0f));

    }
}
