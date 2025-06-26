using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float panBorderThickness = 10f;
    public Vector2 panLimit;
    public float scrollSpeed = 20f;
    public float minY = 20f;
    public float maxY = 120f;
    Vector3 pos;
    public Camera cam;
    public Vector3 defaultPosition;
    public Vector3 topDownPosition;
    public bool isTopDown = false;

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (isTopDown)
                transform.position += panSpeed * Time.deltaTime * Vector3.forward; // Z+
            else
                transform.position += panSpeed * Time.deltaTime * transform.forward;
        }

        if (Input.GetKey(KeyCode.S))
        {
            if (isTopDown)
                transform.position -= panSpeed * Time.deltaTime * Vector3.forward; // Z-
            else
                transform.position -= panSpeed * Time.deltaTime * transform.forward;
        }


        if (Input.GetKey(KeyCode.A)) // left (X-)
            transform.position -= panSpeed * Time.deltaTime * transform.right;

        if (Input.GetKey(KeyCode.D)) // right (X+)
            transform.position += panSpeed * Time.deltaTime * transform.right;

        if (Input.GetKey(KeyCode.Q))
        {
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - 20f * Time.deltaTime, 20f, 60f);
        }
        if (Input.GetKey(KeyCode.E))
        {
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView + 20f * Time.deltaTime, 20f, 60f);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            isTopDown = !isTopDown;

            if (isTopDown)
            {
                cam.transform.SetPositionAndRotation(topDownPosition, Quaternion.Euler(90f, 0f, 0f));
            }
            else
            {
                cam.transform.SetPositionAndRotation(defaultPosition, Quaternion.Euler(0f, 0f, 0f));
            }
        }
    }
}
