using UnityEngine;
using System.Collections;
using MongoDB.Bson;
using System.Collections.Generic;
using UnityEngine.UI;

public class PrinterDataDisplay : MonoBehaviour
{
    public MongoDBManager mongoManager;
    public float updateInterval = 5f;

    public InputField inputNozzleTemp;
    public InputField inputBedTemp;
    public InputField inputX;
    public InputField inputY;
    public InputField inputZ;
    public InputField inputAnomaly;

    private void Start()
    {
        StartCoroutine(UpdatePrinterData());
    }

    private IEnumerator UpdatePrinterData()
    {
        while (true)
        {
            List<BsonDocument> latestData = mongoManager.GetLatestData(1);
            if (latestData.Count > 0)
            {
                BsonDocument data = latestData[0];
                DisplayData(data);
            }
            yield return new WaitForSeconds(updateInterval);
        }
    }

    private void DisplayData(BsonDocument data)
    {
        float nozzleTemp = (float)data["Nozzle Temperature"].AsDouble;
        float bedTemp = (float)data["Bed Temperature"].AsDouble;
        float xPos = (float)data["X Axis"].AsDouble;
        float yPos = (float)data["Y Axis"].AsDouble;
        float zPos = (float)data["Z Axis"].AsDouble;
        string anomaly = data.Contains("anomaly") ? data["anomaly"].AsString : "";

        inputNozzleTemp.text = nozzleTemp.ToString();
        inputBedTemp.text = bedTemp.ToString();
        inputX.text = xPos.ToString();
        inputY.text = yPos.ToString();
        inputZ.text = zPos.ToString();
        inputAnomaly.text = anomaly;
    }
}