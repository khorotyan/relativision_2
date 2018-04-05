using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {

    public bool isSpaceScene = true;
    public ViewerSpace viewerSpace;
    public Viewer viewer;

    [Space(10)]
    public Button restartB;
    public Text lightSpeedT;
    public Slider lightSpeedS;
    public Text accelerationT;
    public Slider accelerationS;
    public Text inBetDistT;
    public Slider inBetDistS;
    public Button nextSceneB;

    private void Awake()
    {
        restartB.onClick.AddListener(delegate { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); });

        lightSpeedS.onValueChanged.AddListener(delegate 
        {
            if (isSpaceScene == true)
                viewerSpace.vel = lightSpeedS.value * viewerSpace.velPercentage;
            else
                viewer.vel = lightSpeedS.value * viewer.velPercentage;

            Formulas.lightSpeed = lightSpeedS.value;
            lightSpeedT.text = "Light Speed: " + lightSpeedS.value;
            accelerationS.minValue = lightSpeedS.value * 0.01f;
            accelerationS.maxValue = lightSpeedS.value * 0.20f;
        });

        accelerationS.onValueChanged.AddListener(delegate
        {
            if (isSpaceScene == true)
                viewerSpace.acceleration = accelerationS.value;
            else
                viewer.acceleration = accelerationS.value;

            accelerationT.text = "Acceleration: " + accelerationS.value.ToString("F2");
        });

        inBetDistS.onValueChanged.AddListener(delegate
        {
            if (isSpaceScene == true)
                viewerSpace.inObjDist = inBetDistS.value;
            else
                viewer.inObjDist = inBetDistS.value;
            
            inBetDistT.text = "In Object Distance: " + inBetDistS.value; 
        });

        if (isSpaceScene == true)
            nextSceneB.onClick.AddListener(delegate { SceneManager.LoadScene(1); });
        else
            nextSceneB.onClick.AddListener(delegate { SceneManager.LoadScene(0); });
    }
}
