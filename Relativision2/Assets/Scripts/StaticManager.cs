using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticManager : MonoBehaviour
{
    public bool isMover = false;
    public Vector3 initPos;
    public Vector3 initScale;

    public float defRotSpeed = 20;
    public float rotSpeed = 20;
    public static float sharedTime = 0;

    private float extraRot = 0;

    private void Start ()
    {
        initPos = transform.position;
        initScale = transform.lossyScale;
    }

    private void FixedUpdate()
    {
        RotatePropeller();

        MoveStaticMover();
    }

    private void RotatePropeller()
    {
        if (transform.name.Contains("Windmill"))
        {
            transform.GetChild(0).GetChild(0).transform.localRotation = Quaternion.Euler(sharedTime * rotSpeed - extraRot, 0, 0);
        }
    }

    // Set the rotation that someone sees (takes time for the light to reach our eyes)
    public void SetVisibleRotation(float timeToReach)
    {
        extraRot = timeToReach * rotSpeed;
    }

    public void UpdateTime(float tickRate = 0.02f)
    {
        sharedTime += tickRate;
    }

    private void MoveStaticMover()
    {
        if (isMover == true)
        {
            transform.position += new Vector3(0, 0, 5 * 0.02f);
        }
    }
}
