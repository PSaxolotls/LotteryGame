using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class OutputNumGenerator : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField numberInput;
    public Toggle selectAllToggle;
    public Toggle straightToggle;
    public Toggle boxToggle;
    public Toggle frontPairToggle;
    public Toggle backPairToggle;
    public Toggle splitPairToggle;
    public Toggle anyPairToggle;

    public List<Toggle> allToggles;

    [Header("Prefab Setup")]
    public GameObject resultPrefab;  // Prefab with 2 children: 0 = PlayType, 1 = Number
    public Transform resultsParent;  // Parent where prefabs will spawn


    private void Start()
    {
        selectAllToggle.onValueChanged.AddListener(isOn => SelectDeselectAllToggles(isOn));
    }

    public void GenerateResults()
    {
        string input = numberInput.text.Trim();

        if (input.Length != 3 || !int.TryParse(input, out _))
        {
            Debug.LogWarning("Please enter a valid 3-digit number.");
            return;
        }

        string d1 = input[0].ToString();
        string d2 = input[1].ToString();
        string d3 = input[2].ToString();

        // Clear old results
        foreach (Transform child in resultsParent)
        {
            Destroy(child.gameObject);
        }

        // Store results as (PlayType, Number)
        List<(string, string)> results = new List<(string, string)>();

        if (straightToggle.isOn)
            results.Add(("Straight", input));

        if (boxToggle.isOn)
            results.Add(("Box", input));

        if (frontPairToggle.isOn)
            results.Add(("Front Pair", d1 + d2));

        if (backPairToggle.isOn)
            results.Add(("Back Pair", d2 + d3));

        if (splitPairToggle.isOn)
            results.Add(("Split Pair", d1 + d3));

        if (anyPairToggle.isOn)
        {
            results.Add(("Any Pair", d1 + d2));
            results.Add(("Any Pair", d2 + d3));
            results.Add(("Any Pair", d1 + d3));
        }

        // Spawn prefabs
        foreach (var result in results)
        {
            GameObject obj = Instantiate(resultPrefab, resultsParent);
            obj.transform.GetChild(1).GetComponent<TMP_Text>().text = result.Item1; // Play type
            obj.transform.GetChild(2).GetComponent<TMP_Text>().text = result.Item2; // Number
        }

        numberInput.text = "";
        
    }


    public void SelectDeselectAllToggles(bool state)
    {
        foreach(var obj in allToggles)
        {
            obj.isOn = state;
        }
    }
}
