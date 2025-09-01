using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

[System.Serializable]
public class SlotsResponse
{
    public string status;
    public List<string> slots;
}

public class AdvanceTime : MonoBehaviour
{
   
    public GameObject prefab;   // Prefab with TMP_Text as 0th child
    public Transform content;   // Parent container (e.g. ScrollView content)

    public TMP_Text drawTime;
    public TMP_Text previousTime;

    public List<Toggle> toggleList;

    private void OnToggleChanged(bool isOn, string _txt, Toggle currentToggle)
    {
        // ?? Turn OFF all other toggles without retriggering their events
        foreach (var toggle in toggleList)
        {
            if (toggle != currentToggle)
            {
                toggle.onValueChanged.RemoveAllListeners();
                toggle.isOn = false;
                string toggleText = toggle.gameObject.transform.parent.transform.GetChild(1).GetComponent<TMP_Text>().text;
                toggle.onValueChanged.AddListener((state) => OnToggleChanged(state, toggleText, toggle));
            }
        }

        // ?? Now update UI safely
        if (isOn)
        {
            Debug.Log("? Toggle is ON - Fire your action here!");
            drawTime.text = _txt;
        }
        else
        {
            Debug.Log("? Toggle is OFF");
            drawTime.text = previousTime.text;
        }
    }




    void Start()
    {
        drawTime.text = previousTime.text;
        StartCoroutine(FetchSlotsFromAPI());
    }

    private IEnumerator FetchSlotsFromAPI()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(GameAPIs.advanceTimeAPi))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("API Error: " + www.error);
            }
            else
            {
                string json = www.downloadHandler.text;
                Debug.Log("API Response: " + json);

                // Parse JSON into SlotsResponse
                SlotsResponse response = JsonUtility.FromJson<SlotsResponse>(json);

                if (response != null && response.status == "success")
                {
                    PopulateSlots(response.slots);
                }
                else
                {
                    Debug.LogWarning("Invalid response from API");
                }
            }
        }
    }

    private void PopulateSlots(List<string> slots)
    {
        // Optional: clear old entries
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (string slot in slots)
        {
           
            GameObject obj = Instantiate(prefab, content);
            TMP_Text txt = obj.transform.GetChild(1).GetComponent<TMP_Text>();
            txt.text = slot;
            obj.transform.GetChild(0).GetComponent<Toggle>().onValueChanged.AddListener((isOn) => OnToggleChanged(isOn, txt.text, obj.transform.GetChild(0).GetComponent<Toggle>()));
            toggleList.Add(obj.transform.GetChild(0).GetComponent<Toggle>());
        }
    }
}
