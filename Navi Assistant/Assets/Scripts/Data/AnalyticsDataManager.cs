using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AnalyticsDataManager : MonoBehaviour
{
    public AnalyticsData analyticsData;
    private string formUrl = "https://docs.google.com/forms/u/2/d/e/1FAIpQLSezVFD-CAkzTR2Wm7xqi4hnxBQwilB4CIkQrE35QK-bBPuiQA/formResponse";

    private void Awake()
    {
        analyticsData = new AnalyticsData();
        analyticsData.deviceModel = SystemInfo.deviceModel;
        analyticsData.deviceOS = SystemInfo.operatingSystem;
        analyticsData.deviceRAM = SystemInfo.systemMemorySize.ToString();
        analyticsData.deviceLanguage = Application.systemLanguage.ToString();
        analyticsData.QRrelocalizationCounts = 0;
    }

    public void SubmitFeedback()
    {   // Submit feedback data to Google Form
        Debug.Log("Submitting feedback data...");
        StartCoroutine(PostAnalyticsData(analyticsData));
    }

    private IEnumerator PostAnalyticsData(AnalyticsData _analyticsData)
    {   // Post analytics data to Google Form
        WWWForm form = new WWWForm();
        form.AddField("entry.1679684700", _analyticsData.deviceModel);
        form.AddField("entry.122009798", _analyticsData.deviceOS);
        form.AddField("entry.247487186", _analyticsData.deviceRAM);
        form.AddField("entry.1347588557", _analyticsData.deviceLanguage);

        form.AddField("entry.53284847", _analyticsData.timeToLoadJSONMap);
        form.AddField("entry.1576113477", _analyticsData.timeToGenerateMapRender);
        form.AddField("entry.1241879741", _analyticsData.timeToCalculatePath);

        print("QR relocalization counts: " + _analyticsData.QRrelocalizationCounts);
        form.AddField("entry.1509236469", _analyticsData.QRrelocalizationCounts);

        //if (!string.IsNullOrEmpty(_analyticsData.contactInfo))
        //    form.AddField("entry.ZZZZZ", _analyticsData.contactInfo);

        // Post feedback data to Google Form using a web request
        using (UnityWebRequest www = UnityWebRequest.Post(formUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Feedback submitted successfully.");
            }
            else
            {
                Debug.LogError("Error in feedback submission: " + www.error);
            }
        }
    }
}

public struct AnalyticsData
{
    public string deviceModel;
    public string deviceOS;
    public string deviceRAM;
    public string deviceLanguage;

    public string timeToLoadJSONMap;
    public string timeToGenerateMapRender;
    public string timeToCalculatePath;

    public int QRrelocalizationCounts;


}
