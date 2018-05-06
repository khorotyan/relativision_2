using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReferenceVars : MonoBehaviour
{
    public Camera mainCam;

    public Transform mover;
    public Transform staticsParent;
    public Transform ground;

    public TextMesh redCarSpeedText;
    public Text timeText;
    public Text VelocityText;

    public GameObject[] worldObjs;

    private void Awake()
    {
        StaticsInfoUpdate();
    }

    // Setup static object info for scaling
    public void StaticsInfoUpdate()
    {  
        Transform minTrans = staticsParent.GetChild(0);
        Transform maxTrans = staticsParent.GetChild(0);
        float minPosZ = 1000;
        float maxPosZ = -1000;

        for (int i = 0; i < staticsParent.childCount; i++)
        {
            float statPos = staticsParent.GetChild(i).localPosition.z;

            if (statPos < minPosZ)
            {
                minPosZ = statPos;
                minTrans = staticsParent.GetChild(i);
            }
            else if (statPos > maxPosZ)
            {
                maxPosZ = statPos;
                maxTrans = staticsParent.GetChild(i);
            }
        }

        minPosZ -= minTrans.GetComponent<Collider>().bounds.size.z;
        maxPosZ += maxTrans.GetComponent<Collider>().bounds.size.z;

        staticsParent.GetComponent<BoxCollider>().center = new Vector3(0, 0, (maxPosZ + minPosZ) / 2);
        staticsParent.GetComponent<BoxCollider>().size = new Vector3(1, 1, (maxPosZ - minPosZ));
    }
}
