using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{

    public Transform target;

    public float smoothSpeed = 0.125f;
    public Vector3 offset;

    public float bottomEdgeYPos;
    
    // Start is called before the first frame update
    void Start()
    {
        Vector3 startPos = new Vector3(0, 5, -10);
        transform.position = startPos;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (target.transform.position.y >= transform.position.y) {
            Vector3 newPos = new Vector3(0, target.transform.position.y, -10);
            transform.position = newPos;
        }
        //Vector3 desiredPosition = target.position + offset;
        //Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        //transform.position = desiredPosition;
    }
}
