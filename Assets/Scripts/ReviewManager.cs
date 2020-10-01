using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviewManager : MonoBehaviour
{
    public float timeBeforeAsking;
    public string IOSAppID = "";
    string URL;
    float timer;
    readonly string IOSurl = "itms://itunes.apple.com/us/app/apple-store/{0}?mt=8";
    readonly string GPurl = "http://play.google.com/store/apps/details?id={0}";
    bool blockReview;
    #region singleton
    public static ReviewManager Instance;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }
    private void OnDestroy()
    {
        if (Instance == this)
        {
            PlayerPrefs.SetFloat("ReviewTime", timer);
            Instance = null;
        }
    }
    #endregion

    void Start()
    {
#if UNITY_IOS
        setIos();
#elif UNITY_ANDROID
        setAndroid();
#endif
        if (PlayerPrefs.GetInt("BlockReview") != 0)
            blockReview = true;
        else
            timer = PlayerPrefs.GetFloat("ReviewTime");
       GameManager.Instance.StartCoroutine(Counter());
    }
    private IEnumerator Counter()
    {
        while (!blockReview)
        {
            yield return new WaitForSeconds(1);
            timer++;
        }
    }
    void setIos()
    {
        if (IOSAppID != "")
            URL = IOSurl.Replace("{0}", IOSAppID);
        else
            Debug.LogError("Theres no ios app id in the ReviewManager");
    }
    void setAndroid() => URL = GPurl.Replace("{0}", Application.identifier);

    public bool CheckForReview() => (!blockReview && timer > timeBeforeAsking* 60);
    public void OpenReview()
    {
        if (URL != "")
        {
            Application.OpenURL(URL);
            blockReview = true;
            PlayerPrefs.SetInt("BlockReview", 1);
        }
        else
            Debug.LogError("Unable to open review, invalid OS?");
    }
    public void DenyReview()
    {
        timer = 0;
    }
    public void NeverAskAgain()
    {
        blockReview = true;
        PlayerPrefs.SetInt("BlockReview", 1);
    }
}
