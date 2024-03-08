using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float scrollSpeed = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.mouseScrollDelta.y<0)
        {
            var position = transform.position;
            transform.position = new Vector3(position.x, position.y + scrollSpeed, position.z);
        }
        
        if (Input.mouseScrollDelta.y>0 && transform.position.y > 0)
        {
            var position = transform.position;
            transform.position = new Vector3(position.x, position.y - scrollSpeed, position.z);
        }
        
    }
}
