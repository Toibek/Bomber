using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Reward : MonoBehaviour
{
    [SerializeField] int waitTime = default;
    [SerializeField] Text Counter = default;
    [SerializeField] bool PlaySkippableAnyway = default;
    Coroutine Counting;
    bool adStarted;
    void Start()
    {
        PlaystateManager.Instance.GetState(EnumStates.Reward).StateEnter_End = startCounter;
    }
    void startCounter()
    {
        Counting = StartCoroutine(Countdown());
    }
    IEnumerator Countdown()
    {
        for (int i = waitTime; i >= 0; i--)
        {
            Counter.text = i.ToString();
            yield return new WaitForSeconds(1);
        }
        if (!adStarted)
        {
            if (PlaySkippableAnyway)
            {
                PlaystateManager.Instance.GetState(EnumStates.Ad).View.GetComponent<Ad>().SetState(EnumStates.Death);
                PlaystateManager.Instance.ChangeState(EnumStates.Ad);
            }
            else
                PlaystateManager.Instance.ChangeState(EnumStates.Death);
            GameManager.Instance.HandleDeath();
        }
        else
            yield return new WaitForSeconds(2);
        if (adStarted)
            rewardEnd(false);
    }

    public void StartReward()
    {
        adStarted = true;
        AdManager.Instance.CompletedRewarded = rewardEnd;
        AdManager.Instance.PlayRewarded();
    }
    void rewardEnd(bool skipped)
    {
        adStarted = false;
        if (skipped)
        {
            PlaystateManager.Instance.ChangeState(EnumStates.Death);
            GameManager.Instance.HandleDeath();
        }
        else
            PlaystateManager.Instance.ChangeState(EnumStates.Pause);
    }
}
