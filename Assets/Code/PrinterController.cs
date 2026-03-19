using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrinterController : MonoBehaviour
{
    public GameObject XAxis; // Drag the XAxis part here in the inspector
    public GameObject YAxis; // Drag the YAxis part here in the inspector
    public GameObject ZAxis; // Drag the ZAxis part here in the inspector

    public InputField inputX; // Assign the InputX field in the inspector
    public InputField inputY; // Assign the InputY field in the inspector
    public InputField inputZ; // Assign the InputZ field in the inspector
    public InputField inputNozzleTemp; // New - Assign Nozzle Temp InputField in the inspector
    public InputField inputBedTemp;    // New - Assign Bed Temp InputField in the inspector
    public InputField inputAnomaly; // New - Assign Anomaly InputField in the inspector
    public Button enterButton; // Assign the EnterButton in the inspector
    public Button autoHomeButton; // Assign the AutoHome button in the inspector

    // New Variables for temperature-based color change
    private Material xAxisMaterial;
    private Material zAxisMaterial;

    // Color gradient settings
    private Color coolColor = Color.blue;
    private Color optimalColor = Color.green;
    private Color hotColor = Color.red;

    // Default positions (AutoHome positions)
    private Vector3 XAxisDefaultPos = new Vector3(45, -25, 0);  // Default position for XAxis (z is fixed at 0)
    private Vector3 YAxisDefaultPos = new Vector3(0, -25, 0);  // Default position for YAxis
    private Vector3 ZAxisDefaultPos = new Vector3(0, 0, -55);   // Default position for ZAxis

    // Movement speed (affects how quickly objects return to home position)
    public float moveSpeed = 5f;

    // Flags for smooth homing
    private bool isHoming = false;

    void Start()
    {
        // Assign materials of XAxis and ZAxis parts
        xAxisMaterial = XAxis.GetComponent<Renderer>().material;
        zAxisMaterial = ZAxis.GetComponent<Renderer>().material;

        // Add listeners to temperature inputs
        inputNozzleTemp.onEndEdit.AddListener(delegate { UpdateNozzleTemp(); });
        inputBedTemp.onEndEdit.AddListener(delegate { UpdateBedTemp(); });
        enterButton.onClick.AddListener(OnEnterPressed);
        autoHomeButton.onClick.AddListener(OnAutoHomePressed);
    }

    // Update nozzle temperature based on user input
    void UpdateNozzleTemp()
    {
        float nozzleTemp = Mathf.Clamp(float.Parse(inputNozzleTemp.text), 30f, 250f);
        Color newColor = CalculateColor(nozzleTemp, 30f, 180f, 200f, 250f);
        StartCoroutine(SmoothColorTransition(xAxisMaterial, newColor));
    }

    // Update bed temperature based on user input
    void UpdateBedTemp()
    {
        float bedTemp = Mathf.Clamp(float.Parse(inputBedTemp.text), 30f, 90f);
        Color newColor = CalculateColor(bedTemp, 30f, 50f, 60f, 90f);
        StartCoroutine(SmoothColorTransition(zAxisMaterial, newColor));
    }

    // Calculate color based on temperature
    Color CalculateColor(float temp, float minTemp, float belowOpt, float aboveOpt, float maxTemp)
    {
        if (temp < belowOpt) return Color.Lerp(coolColor, optimalColor, (temp - minTemp) / (belowOpt - minTemp));
        if (temp > aboveOpt) return Color.Lerp(optimalColor, hotColor, (temp - aboveOpt) / (maxTemp - aboveOpt));
        return optimalColor;
    }

    // Smoothly transition color over time
    IEnumerator SmoothColorTransition(Material material, Color targetColor)
    {
        Color startColor = material.color;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 2f; // Adjust speed of transition if needed
            material.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
        material.color = targetColor;
    }

    void Update()
    {
        if (isHoming)
        {
            // Smoothly move XAxis to its default position
            XAxis.transform.localPosition = Vector3.Lerp(XAxis.transform.localPosition, XAxisDefaultPos, Time.deltaTime * moveSpeed);

            // Smoothly move YAxis to its default position
            YAxis.transform.localPosition = Vector3.Lerp(YAxis.transform.localPosition, YAxisDefaultPos, Time.deltaTime * moveSpeed);

            // Smoothly move ZAxis to its default position
            ZAxis.transform.localPosition = Vector3.Lerp(ZAxis.transform.localPosition, ZAxisDefaultPos, Time.deltaTime * moveSpeed);

            // Check if all objects have reached their default positions
            if (Vector3.Distance(XAxis.transform.localPosition, XAxisDefaultPos) < 0.01f &&
                Vector3.Distance(YAxis.transform.localPosition, YAxisDefaultPos) < 0.01f &&
                Vector3.Distance(ZAxis.transform.localPosition, ZAxisDefaultPos) < 0.01f)
            {
                isHoming = false; // Stop homing once all axes are close enough to their default positions
            }
        }
    }

    void OnEnterPressed()
    {
        // Parse input values
        float enteredX = Mathf.Clamp(float.Parse(inputX.text), 0f, 220f);
        float enteredY = Mathf.Clamp(float.Parse(inputY.text), 0f, 250f);
        float enteredZ = Mathf.Clamp(float.Parse(inputZ.text), 0f, 250f);

        // Convert input ranges to Unity model ranges and move parts accordingly
        float targetX = Mathf.Lerp(45f, -45f, (enteredX - 0f) / (220f - 0f));
        float targetY = Mathf.Lerp(-25f, 90f, (enteredY - 0f) / (250f - 0f));
        float targetZ = Mathf.Lerp(-55f, 35f, (enteredZ - 0f) / (250f - 0f));

        // Move YAxis
        YAxis.transform.localPosition = new Vector3(0, targetY, 0);

        // Move XAxis
        XAxis.transform.localPosition = new Vector3(targetX, YAxis.transform.localPosition.y, 0);

        // Move ZAxis
        ZAxis.transform.localPosition = new Vector3(0, 0, targetZ);
    }

    void OnAutoHomePressed()
    {
        isHoming = true; // Start the homing process
    }

    public void UpdatePrinterData(float nozzleTemp, float bedTemp, float xPos, float yPos, float zPos, string anomaly)
    {
        inputNozzleTemp.text = nozzleTemp.ToString();
        inputBedTemp.text = bedTemp.ToString();
        inputX.text = xPos.ToString();
        inputY.text = yPos.ToString();
        inputZ.text = zPos.ToString();
        inputAnomaly.text = anomaly;
    }
}