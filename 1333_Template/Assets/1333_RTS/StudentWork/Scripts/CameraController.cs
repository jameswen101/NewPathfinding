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

    // Start is called before the first frame update
    void Start()
    {
        pos = transform.position;
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) // forward (Z+)
            transform.position += transform.forward * panSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.S)) // backward (Z-)
            transform.position -= transform.forward * panSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) // left (X-)
            transform.position -= transform.right * panSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.D)) // right (X+)
            transform.position += transform.right * panSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.Q))
        {
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView - 20f * Time.deltaTime, 20f, 60f);
        }
        if (Input.GetKey(KeyCode.E))
        {
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView + 20f * Time.deltaTime, 20f, 60f);
        }
    }
}
