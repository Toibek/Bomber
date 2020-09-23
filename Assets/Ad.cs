using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ad : MonoBehaviour
{
    [SerializeField] float waitTime = default;
    [SerializeField] EnumStates StateAfter = default;
    EnumStates OrgState;
    Coroutine adrout;
    // Start is called before the first frame update
    void Start()
    {
        PlaystateManager.Instance.GetState(EnumStates.Ad).StateEnter_End += waitForAd;
        OrgState = StateAfter;
    }
    public void SetState() => SetState(OrgState);
    public void SetState(EnumStates StateAfter)
    {
        this.StateAfter = StateAfter;
    }
    void waitForAd()
    {
        adrout = StartCoroutine(AdRoutine());
    }
    IEnumerator AdRoutine()
    {
        yield return new WaitForSeconds(waitTime);
        playAd();
        yield return new WaitForSeconds(waitTime);
        AdPlayed(true);
    }
    void playAd()
    {
        AdManager.Instance.CompletedVideo = AdPlayed;
        AdManager.Instance.PlayVideo();
    }
    void AdPlayed(bool skipped)
    {
        PlaystateManager.Instance.ChangeState(StateAfter);
    }
}
