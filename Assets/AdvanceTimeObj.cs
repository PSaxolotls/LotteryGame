using UnityEngine;
using UnityEngine.UI;

public class AdvanceTimeObj : MonoBehaviour
{
    public Toggle toggle;

    private void Start()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(OnToggleChanged);
        }
    }

    private void OnToggleChanged(bool isOn)
    {
        if (isOn)
        {
            Debug.Log(" Toggle is ON - Fire your action here!");
            //  Put your custom action here
        }
        else
        {
            Debug.Log(" Toggle is OFF");
        }
    }

    private void OnDestroy()
    {
        if (toggle != null)
        {
            toggle.onValueChanged.RemoveListener(OnToggleChanged);
        }
    }
}
