using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JMRDemoLocalRigExample : MonoBehaviour
{
    public int width = 10;

    private Vector3 rotationVector = Vector3.zero;
    private float speed = 3f;

    private void Start()
    {
    }

    private void Update()
    {
        rotationVector.y = speed * Time.deltaTime;
        transform.Rotate(rotationVector, Space.World);
        transform.position = new Vector3(transform.position.x, transform.position.y, Mathf.PingPong(Time.time * 2, width) - width / 2f);
    }
}