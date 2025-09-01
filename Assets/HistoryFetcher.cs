using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro; // if you're using TextMeshPro instead of legacy Text

[System.Serializable]
public class HistoryResponse
{
    public string status;
    public HistoryData[] data;
}

[System.Serializable]
public class HistoryData
{
    public string id;
    public string userid;
    public string barcode;
    public string game_name;
    public string ticket_time;
    public string game_no;
    public string draw_time;
    public string play_point;
    public string claim_point;
    public string status;
}

public class HistoryFetcher : MonoBehaviour
{
    [Header("Prefab and Content Holder")]
    public GameObject historyitemPrefab;
    public Transform content;


    [System.Serializable]
    public class HistoryRequest
    {
        public string userid;
    }

    void Start()
    {
        StartCoroutine(FetchHistory());
    }

    IEnumerator FetchHistory()
    {
        // Create form and add fields
        WWWForm form = new WWWForm();
        form.AddField("id", PlayerPrefs.GetInt("UserId").ToString());

        using (UnityWebRequest www = UnityWebRequest.Post(GameAPIs.fetchHistoryAPi, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                string json = www.downloadHandler.text;
                Debug.Log("Response: " + json);

                HistoryResponse response = JsonUtility.FromJson<HistoryResponse>(json);

                if (response != null && response.data != null)
                {
                    foreach (var entry in response.data)
                    {
                        GameObject item = Instantiate(historyitemPrefab, content);

                        // First child (index 0) has timer text
                        Transform timerObj = item.transform.GetChild(0);
                        SetText(timerObj, entry.ticket_time);

                        // Second child (index 1) has multiple result items
                        Transform resultItems = item.transform.GetChild(1);

                        if (item.transform.childCount > 0) SetText(item.transform.GetChild(0), entry.barcode);
                        if (item.transform.childCount > 1) SetText(item.transform.GetChild(1), entry.game_name );
                        if (item.transform.childCount > 2) SetText(item.transform.GetChild(2), entry.play_point);
                        if (item.transform.childCount > 3) SetText(item.transform.GetChild(3), entry.claim_point);
                        if (item.transform.childCount > 4) SetText(item.transform.GetChild(4), entry.status);
                    }
                }
                else
                {
                    Debug.LogWarning("No data found in response");
                }
            }
        }
    }

    private void SetText(Transform target, string value)
    {
        TMP_Text text = target.GetComponent<TMP_Text>();
        if (text != null) text.text = value;
    }

}
