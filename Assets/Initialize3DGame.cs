using UnityEngine;

public class Initialize3DGame : MonoBehaviour
{
    public GameManager mgr;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(mgr.FetchUserData());
        StartCoroutine(mgr.FetchResultsOnStart());
        mgr.GetTimer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
