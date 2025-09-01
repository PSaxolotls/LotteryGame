using UnityEngine;

public class ReportsToggle : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; // Assign in Inspector

    // Call this from Button OnClick()
    public void ToggleTarget()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(!targetObject.activeSelf);
        }
    }
}
