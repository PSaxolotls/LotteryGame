using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FamilyManager : MonoBehaviour
{
    public GridManager mgr;
    public Toggle familyToggle;
    private int rows;
    private int cols;

    void Start()
    {
      
    }

    public void GenerateAddListenerForFamily()
    {
        rows = mgr.gridInputs.GetLength(0);
        cols = mgr.gridInputs.GetLength(1);

        // Add listeners to every input
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject cell = mgr.gridInputs[r, c];
                if (cell == null) continue;

                TMP_Text numberText = cell.transform.GetChild(0).GetComponent<TMP_Text>();
                TMP_InputField input = cell.transform.GetChild(1).GetComponent<TMP_InputField>();

                if (numberText == null || input == null) continue;

                int baseNumber;
                if (int.TryParse(numberText.text, out baseNumber))
                {
                    int capturedNumber = baseNumber; // capture for closure
                    input.onValueChanged.AddListener(value => OnInputChanged(capturedNumber, value));
                }
            }
        }
    }

    private void OnInputChanged(int baseNumber, string value)
    {
        if (!familyToggle.isOn) return;

        // If we're in "loading" mode, ignore empty values to prevent wipe
        if (mgr.isLoading && string.IsNullOrEmpty(value))
            return;

        // Generate family
        HashSet<int> family = GenerateFamily(baseNumber);

        // Update all corresponding inputs in the family
        foreach (int num in family)
        {
            UpdateInputForNumber(num, value);
        }
    }


    private void UpdateInputForNumber(int number, string value)
    {
     //   if (mgr.skipFamilyUpdates) return; // ? ignore when FillRow/FillColumn is running

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject cell = mgr.gridInputs[r, c];
                if (cell == null) continue;

                TMP_Text numberText = cell.transform.GetChild(0).GetComponent<TMP_Text>();
                TMP_InputField input = cell.transform.GetChild(1).GetComponent<TMP_InputField>();

                if (numberText == null || input == null) continue;

                if (int.TryParse(numberText.text, out int num) && num == number)
                {
                    input.text = value; // ? only update when allowed
                  
                }
            }
        }
    //  StartCoroutine(mgr.DelayCall(number));
    }


    // --- FAMILY LOGIC ---
    private static readonly Dictionary<int, int> SwapMap = new Dictionary<int, int>
{
    { 0, 5 },
    { 5, 0 },
    { 1, 6 },
    { 6, 1 },
    { 2, 7 },
    { 7, 2 },
    { 3, 8 },
    { 8, 3 },
    { 4, 9 },
    { 9, 4 }
};

    public static HashSet<int> GenerateFamily(int baseNumber)
    {
        HashSet<int> family = new HashSet<int>();

        // Extract prefix and last two digits
        int prefix = baseNumber / 100;
        int tens = (baseNumber / 10) % 10;
        int units = baseNumber % 10;

        // Step 1: Add base itself
        family.Add(baseNumber);

        // Step 2: Apply swap rules on base number
        TryAddSwap(family, prefix, tens, units, swapTens: false, swapUnits: true);  // swap units
        TryAddSwap(family, prefix, tens, units, swapTens: true, swapUnits: false);  // swap tens
        TryAddSwap(family, prefix, tens, units, swapTens: true, swapUnits: true);   // swap both

        // Step 3: Inverse last 2 digits (xy -> yx)
        int invTens = units;
        int invUnits = tens;
        int inverse = prefix * 100 + invTens * 10 + invUnits;
        family.Add(inverse);

        // Step 4: Apply swap rules on the inverse
        TryAddSwap(family, prefix, invTens, invUnits, swapTens: false, swapUnits: true);
        TryAddSwap(family, prefix, invTens, invUnits, swapTens: true, swapUnits: false);
        TryAddSwap(family, prefix, invTens, invUnits, swapTens: true, swapUnits: true);

        return family;
    }

    private static void TryAddSwap(HashSet<int> family, int prefix, int tens, int units, bool swapTens, bool swapUnits)
    {
        int newTens = tens;
        int newUnits = units;

        if (swapTens && SwapMap.ContainsKey(tens))
            newTens = SwapMap[tens];

        if (swapUnits && SwapMap.ContainsKey(units))
            newUnits = SwapMap[units];

        int newNumber = prefix * 100 + newTens * 10 + newUnits;
        family.Add(newNumber);
    }

}
