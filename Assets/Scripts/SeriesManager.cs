using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SeriesManager : MonoBehaviour
{
    [Header("Series Data (toggles)")]
    public List<Toggle> seriesToggles;   // Assign in Inspector
    public List<int> seriesValues = new List<int> { 10, 30, 50 }; // Match order with toggles
    public List<SeriesButton> seriesButtons = new List<SeriesButton>();
    public List<int> currentSeriesSelected = new List<int>();
    public int currentSeriesBase = 10; // 10 => 10xx, 30 => 30xx, etc.


    [Header("Range Buttons (10 items)")]
    public GameObject[] RangeGrp;   // each has Button + Text (TMP_Text or Text)
    public List<int> currentRangeSelected = new List<int>();
    public List<RangeData> rangeGroups; // Assign in Inspector


    [Header("Grid")]
    public GridManager gridManager;

    [Header("Quantity&Points")]
    public List<GameObject> quantity;
    public List<GameObject> points;
    public static Action<int, List<int>, List<int>> OnQuantityAdded;


    private bool rangeListenersHooked = false;

    public Dictionary<int, int> betNumbers = new Dictionary<int, int>();


    public List<string> rangeGrpColorHex;
    public GameObject mainGridBG;
    public int currentRangeIndx;
    void Start()
    {
        for (int i = 0; i < seriesToggles.Count; i++)
        {
            int seriesValue = seriesValues[i]; // capture value for closure
            seriesToggles[i].onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    SetSeries(seriesValue); //  Pass 10, 30, 50 instead of index
                }
                else
                {
                    OnSeriesDeselected(seriesValue);
                }
            });
        }
        foreach (var sb in seriesButtons)
        {
            int value = sb.seriesValue; // capture the value
            sb.button.onClick.AddListener(() =>
            {
                isSingleRangeSelected = true;
                OnSeriesBtnClicked(value, sb.index);
            });

        }



        // Default series
        SetSeries(10);
        rangeGroups[0].toggle.isOn = true;
        // Hook range buttons once

        foreach (var range in rangeGroups)
        {
            // Handle Toggle
            if (range.toggle != null)
            {
                range.toggle.onValueChanged.AddListener(isOn =>
                {
                    if (isOn)
                        OnRangeSelected(range.rangeValue);
                    else
                        OnRangeDeselected(range.rangeValue);
                });
            }

            // Handle Button
            if (range.button != null)
            {
                range.button.onClick.AddListener(() =>
                {
                    OnRangeBtnSelected(range.rangeValue);
                });
            }
        }




        // Default range (first 100-block)
        OnRangeSelected(0);
    }

    private void SetSeries(int seriesBase)
    {
        currentSeriesBase = seriesBase;
        if (!currentSeriesSelected.Contains(currentSeriesBase))
        {
            currentSeriesSelected.Add(currentSeriesBase);
        }
        // Update RangeGrp labels: (seriesBase + i) * 100 .. +99
        for (int i = 0; i < RangeGrp.Length; i++)
        {
            int start = (currentSeriesBase + i) * 100;
            int end = start + 99;

            var tmp = RangeGrp[i].GetComponentInChildren<TMP_Text>();
            if (tmp != null) tmp.text = $"{start}-{end}";
            else
            {
                var ugui = RangeGrp[i].GetComponentInChildren<TMP_Text>();
                if (ugui != null) ugui.text = $"{start}-{end}";
            }
        }




        // Also refresh grid for currently selected range (assume 0 if none)
        OnRangeSelected(currentRangeIndx);
    }

    private void OnSeriesBtnClicked(int seriesBase, int index)
    {
        foreach (var toggle in seriesToggles)
        {
            toggle.isOn = false;
        }
        currentSeriesSelected.Clear();
        gridManager.ClearMainInputs();
        gridManager.ClearBandF();
        currentSeriesBase = seriesBase;
        if (!currentSeriesSelected.Contains(currentSeriesBase))
        {
            currentSeriesSelected.Add(currentSeriesBase);
        }
        seriesToggles[index].isOn = true;
        // Update RangeGrp labels: (seriesBase + i) * 100 .. +99
        for (int i = 0; i < RangeGrp.Length; i++)
        {
            int start = (currentSeriesBase + i) * 100;
            int end = start + 99;

            var tmp = RangeGrp[i].GetComponentInChildren<TMP_Text>();
            if (tmp != null) tmp.text = $"{start}-{end}";
            else
            {
                var ugui = RangeGrp[i].GetComponentInChildren<TMP_Text>();
                if (ugui != null) ugui.text = $"{start}-{end}";
            }
        }

        gridManager.OnValueAddedInGridInputs();



        // Also refresh grid for currently selected range (assume 0 if none)
        OnRangeSelected(currentRangeIndx);
    }


    void OnSeriesDeselected(int seriesBase)
    {
        currentSeriesSelected.Remove(seriesBase);

    }

    public bool isRangeBtnClicked;
    private void OnRangeSelected(int rangeIndex)
    {
        if (!currentRangeSelected.Contains(rangeIndex))
        {
            currentRangeSelected.Add(rangeIndex);
        }

        if (currentRangeSelected.Count > 1)
        {
            foreach (int series in currentSeriesSelected)
            {
                foreach (int range in currentRangeSelected)
                {
                    gridManager.SaveCurrentGridData(series, range);
                }
            }
        }

        currentRangeIndx = rangeIndex;

        gridManager.isLoading = true;   // ? prevent blank propagation
        gridManager.LoadGridData(currentSeriesBase, rangeIndex);
        gridManager.UpdateGridNumbers(currentSeriesBase, rangeIndex);
        gridManager.isLoading = false;  // ? enable propagation back

        gridManager.OnValueAddedInGridInputs();
    }


    public bool isSingleRangeSelected;
    private void OnRangeBtnSelected(int rangeIndex)
    {

        // gridManager.ClearAll();

        // currentRangeSelected.Clear();
        foreach (var grp in rangeGroups)
        {
            grp.toggle.isOn = false;
        }
        // Save for all selected series/ranges
        //foreach (int series in currentSeriesSelected)
        //{
        //    foreach (int range in currentRangeSelected)
        //    {
        //        gridManager.SaveCurrentGridData(series, range);
        //    }
        //}
        currentRangeSelected.Clear();
        gridManager.ClearMainInputs();
        gridManager.ClearBandF();

        if (!currentRangeSelected.Contains(rangeIndex))
        {
            currentRangeSelected.Add(rangeIndex);
        }
        currentRangeIndx = rangeIndex;
        rangeGroups[rangeIndex].toggle.isOn = true;
        // currentRangeSelected.Add(rangeIndex);
        gridManager.isLoading = true;   // ? prevent blank propagation
        gridManager.LoadGridData(currentSeriesBase, rangeIndex);
        // seriesBase: 10 ? 1000s, 30 ? 3000s; rangeIndex: which 100-block (0..9)
        if (gridManager != null)
            gridManager.UpdateGridNumbers(currentSeriesBase, rangeIndex);

        //Color newColor;
        //if (ColorUtility.TryParseHtmlString(rangeGrpColorHex[rangeIndex], out newColor))
        //{
        //    mainGridBG.GetComponent<Image>().color = newColor;
        //}
        //else
        //{
        //    Debug.LogWarning("Invalid hex color: " + rangeGrpColorHex[rangeIndex]);
        //}

        gridManager.isLoading = false;   // ? prevent blank propagation

    }

    private void OnRangeDeselected(int rangeIndex)
    {
        rangeGroups[rangeIndex].toggle.isOn = false;
        currentRangeSelected.Remove(rangeIndex);
    }

    void OnBuyBtnClicked()
    {

    }


    public void SelectAllSeries()
    {
        // Check if all toggles are currently ON
        bool allOn = true;
        for (int i = 0; i < seriesToggles.Count; i++)
        {
            if (seriesToggles[i] != null && !seriesToggles[i].isOn)
            {
                allOn = false;
                break;
            }
        }

        // If all were ON ? turn OFF all
        if (allOn)
        {
            for (int i = 0; i < seriesToggles.Count; i++)
            {
                if (seriesToggles[i] != null)
                {
                    seriesToggles[i].isOn = false;
                    currentSeriesSelected.Remove(seriesValues[i]);
                    currentSeriesBase = 10;
                    Debug.Log($"Series {i + 1} deselected with value: {seriesValues[i]}");
                }
            }
        }
        // Otherwise ? turn ON all
        else
        {
            for (int i = 0; i < seriesToggles.Count; i++)
            {
                if (seriesToggles[i] != null)
                {
                    seriesToggles[i].isOn = true;
                    if (!currentSeriesSelected.Contains(seriesValues[i]))
                    {
                        currentSeriesSelected.Add(seriesValues[i]);
                    }
                    Debug.Log($"Series {i + 1} selected with value: {seriesValues[i]}");
                }
            }
        }
    }

    public void SelectAllRange()
    {
        bool allSelected = true;

        // Check if all are already selected
        for (int i = 0; i < rangeGroups.Count; i++)
        {
            if (rangeGroups[i].toggle != null && !rangeGroups[i].toggle.isOn)
            {
                allSelected = false;
                break;
            }
        }

        if (allSelected)
        {
            // Deselect all
            for (int i = 0; i < rangeGroups.Count; i++)
            {
                if (rangeGroups[i].toggle != null)
                {
                    rangeGroups[i].toggle.isOn = false;
                }
            }
            currentRangeSelected.Clear();
            Debug.Log("All ranges deselected");
        }
        else
        {
            // Select all
            for (int i = 0; i < rangeGroups.Count; i++)
            {
                if (rangeGroups[i].toggle != null)
                {
                    rangeGroups[i].toggle.isOn = true;

                    if (!currentRangeSelected.Contains(rangeGroups[i].rangeValue))
                    {
                        currentRangeSelected.Add(rangeGroups[i].rangeValue);
                    }

                    Debug.Log($"Series {i + 1} selected with value: {rangeGroups[i].rangeValue}");
                }
            }
        }
    }



    public void ClearAllSeriesAndRange()
    {
        currentRangeSelected.Clear();
        currentSeriesSelected.Clear();
        for (int i = 0; i < seriesToggles.Count; i++)
        {
            if (seriesToggles[i] != null)
            {
                seriesToggles[i].isOn = false;
            }
        }

        for (int i = 0; i < rangeGroups.Count; i++)
        {
            if (rangeGroups[i].toggle != null)
            {
                rangeGroups[i].toggle.isOn = false;
            }
        }

        currentSeriesSelected.Add(10);
        seriesToggles[0].isOn = true;
        rangeGroups[0].toggle.isOn = true;
    }


}
[System.Serializable]
public class SeriesButton
{
    public Button button;
    public int seriesValue;
    public int index;
}

[System.Serializable]
public class RangeData
{
    public Toggle toggle;
    public Button button;
    public int rangeValue; // e.g., 10, 30, 50
}