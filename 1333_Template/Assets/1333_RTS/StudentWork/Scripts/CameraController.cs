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
    [SerializeField] private float rotationSpeed = 5f;  // Adjust for smoothness
    private float targetRoll = 0f;

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

        if (Input.GetKeyDown(KeyCode.Q))
            transform.Rotate(0f, 0f, -90f, Space.Self);

        if (Input.GetKeyDown(KeyCode.E))
            transform.Rotate(0f, 0f, 90f, Space.Self);

        Vector3 euler = transform.rotation.eulerAngles;
        float currentRoll = euler.z;
        float newRoll = Mathf.MoveTowardsAngle(currentRoll, targetRoll, rotationSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(euler.x, euler.y, newRoll);

        // Mouse scroll-wheel zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            cam.fieldOfView -= scroll * scrollSpeed * 5f;
            cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, minY, maxY);
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
