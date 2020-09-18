using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class EnableOnlyDuringState : MonoBehaviour
{
    public bool HideOnStart = false;
    public EnumStates StateForEnable = default;
    private void Start()
    {
        PlaystateManager.Instance.GetState(StateForEnable).StateEnter_Start += Show;
        PlaystateManager.Instance.GetState(StateForEnable).StateExit_Start += Hide;
        if (HideOnStart)
            gameObject.SetActive(false);
    }
    void Show() => gameObject.SetActive(true);
    void Hide() => gameObject.SetActive(false);
    private void OnDestroy()
    {
        PlaystateManager.Instance.GetState(StateForEnable).StateEnter_Start -= Show;
        PlaystateManager.Instance.GetState(StateForEnable).StateExit_Start -= Hide;
    }
}
