using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChanger : MonoBehaviour
{
    public EnumStates state = default;
    public void ChangeState()
    {
        if (state == EnumStates.MainMenu && ReviewManager.Instance != null && ReviewManager.Instance.CheckForReview())
            PlaystateManager.Instance.ChangeState(EnumStates.Review);
        else
            PlaystateManager.Instance.ChangeState(state);
    }
}
