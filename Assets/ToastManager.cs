using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ToastManager : MonoBehaviour
{
    public static ToastManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private GameObject toastObject;
    [SerializeField] private bool useTextMeshPro = true;

    [Header("Settings")]
    [SerializeField] private float toastDuration = 1.2f;

    private Coroutine toastCoroutine;
    private Image toastImage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Make sure the root object persists
        DontDestroyOnLoad(transform.root.gameObject);

        if (toastObject != null)
            toastImage = toastObject.GetComponent<Image>();

        if (toastImage != null)
            SetAlpha(0f); // Start hidden
    }



    public void ShowToast(string message)
    {
        if (toastObject == null) return;

        // Stop any running coroutine
        if (toastCoroutine != null)
            StopCoroutine(toastCoroutine);

        // Activate and set text
        toastObject.SetActive(true);
        SetAlpha(1f);

        Transform firstChild = toastObject.transform.GetChild(0);
        firstChild.gameObject.SetActive(true);
        if (firstChild != null)
        {
            if (useTextMeshPro && firstChild.TryGetComponent(out TMP_Text tmp))
            {
                tmp.text = message;
            }
            else if (firstChild.TryGetComponent(out Text txt))
            {
                txt.text = message;
            }
        }

        // Start auto-hide
        toastCoroutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(toastDuration);
        HideToast();
    }

    public void HideToast()
    {
        if (toastObject != null)
        {
            SetAlpha(0f);
            toastObject.transform.GetChild(0).gameObject.SetActive(false);
            // toastObject.SetActive(false);
        }
    }

    private void SetAlpha(float alpha)
    {
        if (toastImage != null)
        {
            Color c = toastImage.color;
            c.a = alpha;
            toastImage.color = c;
        }
    }
}
