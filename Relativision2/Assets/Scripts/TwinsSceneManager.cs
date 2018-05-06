using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TwinsSceneManager : MonoBehaviour
{
    public Transform sun;
    public Transform planet;
    public Transform viewer;
    public Transform planetTwin;
    public Transform viewerTwin;
    public TextMesh planetTimeText;
    public TextMesh viewerTimeText;

    public Button restartB;
    public Text planetMassT;
    public Slider planetMassS;
    public Text distFromPlanetT;
    public Slider distFromPlanetS;
    public Text planetRadiusT;
    public Text spaceshipVelocityT;
    public Text planetTwinVelocityT;
    public InputField timescaleI;
    public Toggle PauseToggle;
    public Button carSceneB;
    public Button spaceSceneB;

    // Light speed
    private float c = 2.998e+8f; // m / s

    // Gravitation constant
    private float G = 6.674e-11f; // N * m^2 / kg^2

    // Planet mass
    private float M = 5.972e+24f; // kg

    // Planet radius
    private float R = 6.371e+6f; // m
    private float minRadius;

    // Spaceship distance from planet center
    private float r = 7.275e+6f; // m

    // Spaceship velocity
    private float v; // m / s

    private float viewerTime;
    private float planetTime;

    private float timeScale = 1;
    private float planetTwinVelocity;
    private float degreesPerSecond;
    private float planetDegreesPerSecond = 0.0042f;
    private float planetDegreesPerSecondAroundSun = 1.14E-5f;

    private void Awake()
    {
        minRadius = R;

        // Update planet twin velocity
        UpdatePlanetTwinVelocity();

        // Update spaceship velocity
        UpdateSpaceshipVelocity();

        Initialize();
    }

    private void Initialize()
    {
        spaceshipVelocityT.text = "Spaceship Velocity: " + v.ToString("F0") + " m / v";
        planetMassS.minValue = M;
        planetMassS.maxValue = M * 10000000;
        planetMassS.value = M;
        planetMassT.text = "Planet Mass: " + M + " kg";
        distFromPlanetS.minValue = R * 1.2f;
        distFromPlanetS.maxValue = R * 10;
        distFromPlanetS.value = r;
        distFromPlanetT.text = "Distance From Planet Center:\n" + r + " m";
        planetRadiusT.text = "Planet Radius " + R + " m";

        restartB.onClick.AddListener(delegate{ SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); OnSceneChange(); });

        planetMassS.onValueChanged.AddListener(delegate
        {
            M = planetMassS.value;
            planetMassT.text = "Planet Mass: " + M + " kg";

            // Update planet scale
            // NewScale = ((Value - Min) / (Max - Min)) * (ScaleMax - ScaleMin) + ScaleMin
            float changeRatio = (planetMassS.value - planetMassS.minValue) / (planetMassS.maxValue - planetMassS.minValue);
            float newScale = changeRatio * (20 - 10) + 10;
            planet.localScale = Vector3.one * newScale;

            // Update actual planet radius
            R = changeRatio * (10 * minRadius - minRadius) + minRadius;
            planetRadiusT.text = "Planet Radius " + R + " m";

            // Update min and max possible distances from the planet
            distFromPlanetS.minValue = R * 1.2f;
            distFromPlanetS.maxValue = R * 10;

            // Update the distance from the planet
            if (r < distFromPlanetS.minValue)
                r = distFromPlanetS.minValue;
            else if (r > distFromPlanetS.maxValue)
                r = distFromPlanetS.maxValue;

            distFromPlanetS.value = r;
            distFromPlanetT.text = "Distance From Planet Center:\n" + r + " m";

            // Change the visuals of the spaceship distance from planet
            float distChangeRatio = (distFromPlanetS.value - distFromPlanetS.minValue) / (distFromPlanetS.maxValue - distFromPlanetS.minValue);
            float newDist = distChangeRatio * (2 - (-3)) + (-3);
            Vector3 viewerPos = viewer.GetChild(0).localPosition;
            Vector3 viewerTimePos = viewer.GetChild(1).localPosition;
            viewer.GetChild(0).localPosition = new Vector3(newDist, viewerPos.y, viewerPos.z);
            viewer.GetChild(1).localPosition = new Vector3(newDist, viewerTimePos.y, viewerTimePos.z);

            // Update planet twin velocity
            UpdatePlanetTwinVelocity();

            // Update spaceship velocity
            UpdateSpaceshipVelocity();
        });

        distFromPlanetS.onValueChanged.AddListener(delegate
        {
            r = distFromPlanetS.value;
            distFromPlanetT.text = "Distance From Planet Center:\n" + r + " m";

            // Change the visuals of the spaceship distance from planet
            float distChangeRatio = (distFromPlanetS.value - distFromPlanetS.minValue) / (distFromPlanetS.maxValue - distFromPlanetS.minValue);
            float newDist = distChangeRatio * (2 - (-3)) + (-3);
            Vector3 viewerPos = viewer.GetChild(0).localPosition;
            Vector3 viewerTimePos = viewer.GetChild(1).localPosition;
            viewer.GetChild(0).localPosition = new Vector3(newDist, viewerPos.y, viewerPos.z);
            viewer.GetChild(1).localPosition = new Vector3(newDist, viewerTimePos.y, viewerTimePos.z);

            // Update planet twin velocity
            UpdatePlanetTwinVelocity();

            // Update spaceship velocity
            UpdateSpaceshipVelocity();
        });

        timescaleI.onEndEdit.AddListener(delegate
        {
            float value = float.MinValue;
            string timescaleString = timescaleI.text;

            value = float.TryParse(timescaleString, out value) == true ? value : float.NaN;

            if (float.IsNaN(value))
            {
                timescaleString = timescaleString.Substring(11);
                value = float.TryParse(timescaleString, out value) == true ? value : float.NaN;
            }
            
            if (float.IsNaN(value))
            {
                timescaleI.text = "Please enter a value (0.1 - 100)";
            }
            else
            {
                if (value < 0.1f)
                    value = 0.1f;
                else if (value > 100)
                    value = 100;

                if (value < 1)
                    Time.fixedDeltaTime = 0.02f * value;
                else
                    if (Time.fixedDeltaTime != 0.02f)
                        Time.fixedDeltaTime = 0.02f;

                if (PauseToggle.isOn == true)
                    PauseToggle.isOn = false;

                timeScale = value;
                Time.timeScale = value;
                timescaleI.text = "Timescale: " + value;
            }
        });

        PauseToggle.onValueChanged.AddListener(delegate 
        {
            if (PauseToggle.isOn == true)
            {
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = timeScale;
            }
        });

        carSceneB.onClick.AddListener(delegate { SceneManager.LoadScene(1); OnSceneChange(); });
        spaceSceneB.onClick.AddListener(delegate { SceneManager.LoadScene(0); OnSceneChange(); });
    }

    private void UpdatePlanetTwinVelocity()
    {
        float planetRadius = 2 * Mathf.PI * R;
        planetTwinVelocity = planetRadius / (24 * 3600);
        planetTwinVelocityT.text = "Planet Twin Velocity: " + planetTwinVelocity.ToString("F0") + " m / v";
    }

    private void UpdateSpaceshipVelocity()
    {
        v = Mathf.Sqrt((G * M) / r);
        spaceshipVelocityT.text = "Spaceship Velocity: " + v.ToString("F0") + " m / v";
        float circleLength = 2 * Mathf.PI * r;
        float distCoverTime = circleLength / v;
        float distCoveredPerSecond = circleLength / distCoverTime;
        degreesPerSecond = (distCoveredPerSecond / circleLength) * 360;
    }

    private void FixedUpdate()
    {
        // Rotate the planet around itself
        planet.Rotate(Vector3.up * Time.fixedDeltaTime * planetDegreesPerSecond, Space.World);

        // Rotate the viewer around the planet
        viewer.RotateAround(planet.transform.position, planet.transform.up, Time.fixedDeltaTime * degreesPerSecond);

        // Rotate the planet and the spaceship around the sun
        planet.RotateAround(sun.transform.position, sun.transform.up, Time.fixedDeltaTime * planetDegreesPerSecondAroundSun);
        viewer.RotateAround(sun.transform.position, sun.transform.up, Time.fixedDeltaTime * planetDegreesPerSecondAroundSun);

        GetObjectTimes();
    }

    private void OnSceneChange()
    {
        if (Time.fixedDeltaTime != 0.02f)
            Time.fixedDeltaTime = 0.02f;

        if (Time.timeScale != 1)
            Time.timeScale = 1;
    }

    private void GetObjectTimes()
    {
        viewerTime += Time.fixedDeltaTime;
        viewerTimeText.text = "Time: " + viewerTime + " s";

        planetTime += Time.fixedDeltaTime * Formulas.GetGamma(v - planetTwinVelocity, c);
        planetTimeText.text = "Time: " + planetTime + " s";
    }
}
