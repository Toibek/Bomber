using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnableOnlyDuringState : MonoBehaviour
{
    public bool HideOnStart = false;
    public EnumStates[] StateForEnable = default;
    private void Start()
    {
        for (int i = 0; i < StateForEnable.Length; i++)
        {
        PlaystateManager.Instance.GetState(StateForEnable[i]).StateEnter_Start += Show;
        PlaystateManager.Instance.GetState(StateForEnable[i]).StateExit_Start += Hide;
        }
        if (HideOnStart)
            gameObject.SetActive(false);
    }
    void Show() => gameObject.SetActive(true);
    void Hide() => gameObject.SetActive(false);
    private void OnDestroy()
    {
        if (PlaystateManager.Instance != null)
        {
            for (int i = 0; i < StateForEnable.Length; i++)
            {
            PlaystateManager.Instance.GetState(StateForEnable[i]).StateEnter_Start -= Show;
            PlaystateManager.Instance.GetState(StateForEnable[i]).StateExit_Start -= Hide;
            }
        }
    }
}
