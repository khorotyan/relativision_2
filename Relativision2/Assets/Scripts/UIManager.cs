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
    public InputField velocityIn;
    public Toggle accelerationToggle;

    private void Awake()
    {
        restartB.onClick.AddListener(delegate { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); });

        if (isSpaceScene == true)
            viewerSpace.vel = lightSpeedS.value * viewerSpace.velPercentage;
        else
            viewer.vel = lightSpeedS.value * viewer.velPercentage;

        lightSpeedS.onValueChanged.AddListener(delegate 
        {
            if (isSpaceScene == true)
                viewerSpace.vel = lightSpeedS.value * viewerSpace.velPercentage;
            else
                viewer.vel = lightSpeedS.value * viewer.velPercentage;

            Formulas.lightSpeed = lightSpeedS.value;
            lightSpeedT.text = "Light Speed: " + lightSpeedS.value;
            accelerationS.minValue = lightSpeedS.value * 0.002f;
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

        velocityIn.onEndEdit.AddListener(delegate
        {
            string input = velocityIn.text;

            if (input[velocityIn.text.Length - 1] == '%')
                input = input.Remove(input.Length - 1);

            float vel = float.MaxValue;

            try
            {
                vel = float.Parse(input);
            }
            catch (System.Exception)
            {
                velocityIn.text = "Input is invalid";
            }             

            if (vel != float.MaxValue)
            {
                if (isSpaceScene == true)
                    viewerSpace.vel = vel * Formulas.lightSpeed / 100;
                else
                    viewer.vel = vel * Formulas.lightSpeed / 100;
            }

            if (vel > 98) vel = 98;
            else if (vel < -98) vel = -98;

            velocityIn.text = "Velocity: " + vel + "%";
        });

        accelerationToggle.onValueChanged.AddListener(delegate 
        {
            if (isSpaceScene == true)
                viewerSpace.alwaysAccelerate = accelerationToggle.isOn;
            else
                viewer.alwaysAccelerate = accelerationToggle.isOn;
        });

        if (isSpaceScene == true)
            nextSceneB.onClick.AddListener(delegate { SceneManager.LoadScene(1); });
        else
            nextSceneB.onClick.AddListener(delegate { SceneManager.LoadScene(0); });
    }
}
