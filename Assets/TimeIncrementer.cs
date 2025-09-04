using System;
using UnityEngine;
using TMPro;

public class TimeIncrementer : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text timeText;   // assign in inspector

    [Header("Start Time")]
    public string startTimeString = "2025-08-24 00:43:00"; // input format

    private DateTime currentTime;
    public TMP_Text startTimeTxt;

    private void Start()
    {
  
    }
    public void StartHeaderCurrentTime()
    {
        startTimeString = startTimeTxt.text;
        // Parse your given string into DateTime
        if (DateTime.TryParse(startTimeString, out currentTime))
        {
          //  Debug.Log("? Parsed start time: " + currentTime);
            InvokeRepeating(nameof(IncrementTime), 1f, 1f); // call every 1 second
        }
        else
        {
            Debug.LogError("? Failed to parse time string: " + startTimeString);
        }
    }
    private void IncrementTime()
    {
        // Add 1 second
        currentTime = currentTime.AddSeconds(1);

        // Update UI (format same as input)
        if (timeText != null)
            timeText.text = currentTime.ToString("dd-MM-yyyy HH:mm:ss");

       // Debug.Log("Updated Time: " + currentTime);
    }
}
