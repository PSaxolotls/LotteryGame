using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuantityPointsManager : MonoBehaviour
{

    public List<GameObject> quantity;
    public List<GameObject> points;
    public float multiplier;

    public SeriesManager seriesMgr;

    public TMP_Text quantityTotalTxt;
    public TMP_Text PointsTotalTxt;

    private void OnEnable()
    {
        SeriesManager.OnQuantityAdded += OnQuantityAdded;
    }

    private void OnDisable()
    {
        SeriesManager.OnQuantityAdded += OnQuantityAdded;
    }

    private void Start()
    {
        for (int i = 0; i <= quantity.Count - 1; i++)
        {
            quantity[i].transform.GetChild(0).GetComponent<TMP_Text>().text = "0";
        }
        for (int i = 0; i <= points.Count - 1; i++)
        {
            points[i].transform.GetChild(0).GetComponent<TMP_Text>().text = "0";
        }
    }



    void OnQuantityAdded(int amount, List<int> series, List<int> range)
    {
        if (!seriesMgr.isSingleRangeSelected)
        {
            float aftercalculation;
            foreach (var indx in range)
            {
                int currentvalue = int.Parse(quantity[indx].transform.GetChild(0).GetComponent<TMP_Text>().text);
                aftercalculation = (/*currentvalue +*/ amount) * series.Count;
                quantity[indx].transform.GetChild(0).GetComponent<TMP_Text>().text = aftercalculation.ToString();
                OnPointsAdded(aftercalculation, series, range);
            }
            UpdateFinalTotal();
        }


    }


    void OnPointsAdded(float amount, List<int> series, List<int> range)
    {
        float aftercalculation;
        foreach (var indx in range)
        {
            aftercalculation = amount * multiplier;
            points[indx].transform.GetChild(0).GetComponent<TMP_Text>().text = aftercalculation.ToString();
        }
        UpdateFinalTotal();

    }

    void UpdateFinalTotal()
    {
        int total = 0;
        for (int i = 0; i <= quantity.Count - 1; i++)
        {
            total += int.Parse(quantity[i].transform.GetChild(0).GetComponent<TMP_Text>().text);
        }
        quantityTotalTxt.text = total.ToString();

        int totalpoints = 0;
        for (int i = 0; i <= points.Count - 1; i++)
        {
            totalpoints += int.Parse(points[i].transform.GetChild(0).GetComponent<TMP_Text>().text);
        }
        PointsTotalTxt.text = (total * multiplier).ToString();
    }

    public void ClearData()
    {
        for (int i = 0; i <= quantity.Count - 1; i++)
        {
            quantity[i].transform.GetChild(0).GetComponent<TMP_Text>().text = "0";
        }
        for (int i = 0; i <= points.Count - 1; i++)
        {
            points[i].transform.GetChild(0).GetComponent<TMP_Text>().text = "0";
        }


        UpdateFinalTotal();

    }

}
