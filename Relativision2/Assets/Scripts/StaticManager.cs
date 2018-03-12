using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticManager : MonoBehaviour
{
    public Vector3 initPos;
    public Vector3 initScale;

	void Start ()
    {
        initPos = transform.position;
        initScale = transform.lossyScale;
    }
}
