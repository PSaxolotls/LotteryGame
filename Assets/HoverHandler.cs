using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameObject targetGameObject;
    private Coroutine disableCoroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
            disableCoroutine = null;
        }

        if (targetGameObject != null)
        {
            targetGameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetGameObject != null)
        {
            // Start a coroutine to disable the target after a small delay
            // This gives the cursor time to enter the target object
            disableCoroutine = StartCoroutine(DisableWithDelay());
        }
    }

    private IEnumerator DisableWithDelay()
    {
        // Wait for a very short time (e.g., 0.1 seconds)
        yield return new WaitForSeconds(0.1f);

        // Check if the cursor is currently over a UI element.
        // This is a simple but effective way to ensure the cursor is not on the target object.
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            targetGameObject.SetActive(false);
        }
        else
        {
            // If the cursor is over a UI element, we'll check again
            // to see if it's over the target object.
            // This is a more robust way to handle the logic.
            if (!IsPointerOverTargetObject())
            {
                targetGameObject.SetActive(false);
            }
        }
    }

    private bool IsPointerOverTargetObject()
    {
        // A more advanced check that confirms if the cursor is over the target UI element.
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == targetGameObject)
            {
                return true;
            }
        }

        return false;
    }
}