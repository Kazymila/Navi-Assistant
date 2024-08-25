using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class AnalyticsDataManager : MonoBehaviour
{
    public AnalyticsData analyticsData;
    private string formUrl = "https://docs.google.com/forms/u/2/d/e/1FAIpQLSezVFD-CAkzTR2Wm7xqi4hnxBQwilB4CIkQrE35QK-bBPuiQA/formResponse";

    private void Awake()
    {
        analyticsData = new AnalyticsData();
        analyticsData.deviceName = SystemInfo.deviceName;
        analyticsData.deviceModel = SystemInfo.deviceModel;
        analyticsData.deviceOS = SystemInfo.operatingSystem;
        analyticsData.deviceRAM = SystemInfo.systemMemorySize.ToString();
        analyticsData.deviceLanguage = Application.systemLanguage.ToString();
        analyticsData.QRrelocalizationCount = 0;
    }

    public void SubmitFeedback()
    {   // Submit feedback data to Google Form
        Debug.Log("[Analytics Manager] Submitting feedback data...");
        StartCoroutine(PostAnalyticsData(analyticsData));
    }

    private IEnumerator PostAnalyticsData(AnalyticsData _analyticsData)
    {   // Post analytics data to Google Form
        WWWForm form = new WWWForm();
        form.AddField("entry.580131976", _analyticsData.deviceName);
        form.AddField("entry.1679684700", _analyticsData.deviceModel);
        form.AddField("entry.122009798", _analyticsData.deviceOS);
        form.AddField("entry.247487186", _analyticsData.deviceRAM);
        form.AddField("entry.1347588557", _analyticsData.deviceLanguage);

        form.AddField("entry.53284847", _analyticsData.timeToLoadJSONMap);
        form.AddField("entry.1576113477", _analyticsData.timeToGenerateMapRender);

        form.AddField("entry.1971169010", _analyticsData.startPosition);
        form.AddField("entry.1503912756", _analyticsData.destinationPoint);
        form.AddField("entry.1394761568", _analyticsData.pathDistance);
        form.AddField("entry.1241879741", _analyticsData.timeToCalculatePath);
        form.AddField("entry.301613269", _analyticsData.timeTakenToCompletePath);

        form.AddField("entry.1509236469", _analyticsData.QRrelocalizationCount);
        form.AddField("entry.2104579494", _analyticsData.assistantCalledCount);
        form.AddField("entry.1981129000", _analyticsData.problemSolvingCount);
        form.AddField("entry.948606556", _analyticsData.changeDestinationCount);
        form.AddField("entry.787445292", _analyticsData.cannotCalculatePathErrorCount);

        // Post feedback data to Google Form using a web request
        using (UnityWebRequest www = UnityWebRequest.Post(formUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("[Analytics Manager] Feedback submitted successfully.");
            }
            else
            {
                Debug.LogError("[Analytics Manager] Error in feedback submission: " + www.error);
            }
        }
    }
}

public struct AnalyticsData
{
    public string deviceName;
    public string deviceModel;
    public string deviceOS;
    public string deviceRAM;
    public string deviceLanguage;

    public string timeToLoadJSONMap;
    public string timeToGenerateMapRender;

    public string startPosition;
    public string destinationPoint;
    public string pathDistance;
    public string timeToCalculatePath;
    public string timeTakenToCompletePath;

    public int QRrelocalizationCount;
    public int assistantCalledCount;
    public int problemSolvingCount;
    public int changeDestinationCount;
    public int cannotCalculatePathErrorCount;
}
