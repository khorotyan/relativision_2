﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewer : MonoBehaviour
{
    public ReferenceVars rv;

    private float acceleration = 0.3f;

    public float vel;
    public float time;
    public float ownTime;
    private float timeTicks = 0.02f;
    private int accelerating = 0;

    private void FixedUpdate()
    {
        GetVelocity();

        ManageTime();

        ApplyStaticObjPos();

        SpawnObjects();
    }

    private void ManageTime()
    {
        time += timeTicks;
        
        if (vel > 0)
        {
            ownTime += timeTicks / Formulas.GetGamma(vel);
        }
        else if (vel < 0)
        {
            ownTime += timeTicks / Formulas.GetGamma(vel);
        }
        else
        {
            ownTime += timeTicks;
        }

        rv.timeText.text = "t = " + (time).ToString("F1") + " / " + "t' = " + (ownTime).ToString("F1");
    }

    // Calculate body velocity and move it
    private void GetVelocity()
    {
        ManageAcceleration();

        // Limit the movement speed
        if (Mathf.Abs(vel) > 0.97f * Formulas.lightSpeed)
        {
            vel = 0.97f * Formulas.lightSpeed * Mathf.Sign(vel);
        }

        // Apply the changed position to the mover and the ground
        transform.position += new Vector3(0, 0, vel * timeTicks);

        rv.VelocityText.text = (vel / Formulas.lightSpeed).ToString("F2") + "% of light speed";
    }
    
    // Apply static object positions and display some data
    public void ApplyStaticObjPos()
    {
        for (int i = 0; i < rv.staticsParent.childCount; i++)
        {
            float prevLen = rv.staticsParent.GetChild(i).localScale.z;
            float newLen = 1;

            float prevZSize = 0;
            float newZSize = 0;

            if (vel > 0)
            {
                newLen = Formulas.GetStaticsLength(1, vel);
            }
            else if (vel < 0)
            {
                newLen = Formulas.GetMoversLength(1, vel);
            }

            // Change object position
            rv.staticsParent.GetChild(i).position -= new Vector3(0, 0, vel * timeTicks * Formulas.GetGamma(vel));

            // Apply the changed scale
            prevZSize = rv.staticsParent.GetChild(i).GetComponent<Collider>().bounds.size.z;
            rv.staticsParent.GetChild(i).localScale = new Vector3(1, 1, newLen);
            newZSize = rv.staticsParent.GetChild(i).GetComponent<Collider>().bounds.size.z;

            // Position change due to scale (do proper scale)
            float posChange = Mathf.Abs(newZSize - prevZSize) / 2;

            if (accelerating == 1)
            {
                rv.staticsParent.GetChild(i).position -= new Vector3(0, 0, posChange);
            }
            else if (accelerating == -1)
            {
                rv.staticsParent.GetChild(i).position += new Vector3(0, 0, posChange);
            }
            
            // Change in object distance
            Vector3 statInitPos = rv.staticsParent.GetChild(i).GetComponent<StaticManager>().initPos;
            float objDist = statInitPos.z - transform.position.z;
            
            if (vel > 0)
            {
                float objDistDiff = objDist - objDist / Formulas.GetGamma(vel);
                rv.staticsParent.GetChild(i).position = statInitPos - new Vector3(0, 0, objDistDiff);
            }
            else if (vel < 0)
            {
                float objDistDiff = objDist * Formulas.GetGamma(vel) - objDist;
                rv.staticsParent.GetChild(i).position = statInitPos + new Vector3(0, 0, objDistDiff);
            }
        }     
    }

    private void ManageAcceleration()
    {
        KeyCode up;
        KeyCode down;

        up = KeyCode.W;
        down = KeyCode.S;

        if (Input.GetKey(up))
        {
            vel += acceleration * Time.fixedDeltaTime;
            accelerating = 1;
        }
        else if (Input.GetKey(down))
        {
            vel -= acceleration * Time.fixedDeltaTime;
            accelerating = -1;
        }
        // If not accelerating/decelerating, slow down untill stopped
        else if (!Input.GetKey(up) && !Input.GetKey(down))
        {
            if (vel > 0)
            {
                vel -= 0.2f * acceleration * Time.fixedDeltaTime;
                accelerating = -1;

                if (vel < 0)
                    vel = 0;
            }
            else if (vel < 0)
            {
                vel += 0.2f * acceleration * Time.fixedDeltaTime;
                accelerating = 1;

                if (vel > 0)
                    vel = 0;
            }
        }
    }

    private void SpawnObjects()
    {
        List<Transform> destroyables = new List<Transform>();
        float maxPosZ = -10000;

        for (int i = rv.staticsParent.childCount - 1; i >= 0; i--)
        {
            //float statPosZ = rv.staticsParent.GetChild(i).position.z;
            float statPosZ = rv.staticsParent.GetChild(i).GetComponent<StaticManager>().initPos.z;

            if (statPosZ > maxPosZ)
            {
                maxPosZ = statPosZ;
            }
            
            // Destroy the objects that are far away
            if (Mathf.Abs(transform.position.z - statPosZ) > 500 && statPosZ < transform.position.z)
            {
                destroyables.Add(rv.staticsParent.GetChild(i));
            }
        }

        if (Mathf.Abs(transform.position.z - maxPosZ) < 250)
        {
            int obj1ID = Random.Range(0, rv.worldObjs.Length);
            int obj2ID = Random.Range(0, rv.worldObjs.Length);

            Transform obj1 = Instantiate(rv.worldObjs[obj1ID], rv.staticsParent).transform;
            Transform obj2 = Instantiate(rv.worldObjs[obj2ID], rv.staticsParent).transform;

            float yPos1 = rv.worldObjs[obj1ID].transform.position.y;
            float yPos2 = rv.worldObjs[obj2ID].transform.position.y;

            float xPos1 = Random.Range(-25f, -35f);
            float xPos2 = Random.Range(25f, 35f);

            float zPos1 = Random.Range(30f, 35f);
            float zPos2 = Random.Range(30f, 35f);

            obj1.transform.position = new Vector3(xPos1, yPos1, maxPosZ + zPos1);
            obj2.transform.position = new Vector3(xPos2, yPos2, maxPosZ + zPos2);

            rv.StaticsInfoUpdate();
        }

        if (destroyables.Count > 0)
        {
            for (int i = 0; i < destroyables.Count; i++)
            {
                Destroy(destroyables[i].gameObject);
            }
        }       
    }
}
