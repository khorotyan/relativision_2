using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticManager : MonoBehaviour
{
    public Vector3 initPos;
    public Vector3 initScale;

    public float defRotSpeed = 20;
    public float rotSpeed = 20;
    public float time = 0;

    private void Start ()
    {
        initPos = transform.position;
        initScale = transform.lossyScale;
    }

    private void FixedUpdate()
    {
        RotatePropeller();
    }

    private void RotatePropeller()
    {
        if (transform.name.Contains("Windmill"))
        {
            time += 0.02f;

            transform.GetChild(0).GetChild(0).transform.localRotation = Quaternion.Euler(time * rotSpeed, 0, 0);
        }
    }
}
