using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Formulas : MonoBehaviour
{
    public static float lightSpeed = 9;

    // Get the Lorenz factor
    public static float GetGamma(float velocity)
    {
        return 1 / (Mathf.Sqrt(1 - Mathf.Pow(velocity, 2) / Mathf.Pow(lightSpeed, 2)));
    }

    public static float GetGamma(float velocity, float lightSpeed)
    {
        return 1 / (Mathf.Sqrt(1 - Mathf.Pow(velocity, 2) / Mathf.Pow(lightSpeed, 2)));
    }

    // Get the position of a moving body - x'
    public static float GetMoversPos(float statPos, float statTime, float velocity)
    {
        return GetGamma(velocity) * (statPos - velocity * statTime);
    }

    // Get the position of a static body - x
    public static float GetStaticsPos(float movePos, float moveTime, float velocity)
    {
        return GetGamma(velocity) * (movePos + velocity * moveTime);
    }

    // Get the time of a moving body - t'
    public static float GetMoversTime(float statPos, float statTime, float velocity)
    {
        return GetGamma(velocity) * (statTime - (velocity * statPos) / Mathf.Pow(lightSpeed, 2));
    }

    // Get the time of a static body - t
    public static float GetStaticsTime(float movePos, float moveTime, float velocity)
    {
        return GetGamma(velocity) * (moveTime + (velocity * movePos) / Mathf.Pow(lightSpeed, 2));
    }

    // Get the length of a moving body L'
    public static float GetMoversLength(float statLen, float velocity)
    {
        return statLen * GetGamma(velocity);
    }

    // Get the length of a Static body L
    public static float GetStaticsLength(float moveLen, float velocity)
    {
        return moveLen / GetGamma(velocity);
    }
}
