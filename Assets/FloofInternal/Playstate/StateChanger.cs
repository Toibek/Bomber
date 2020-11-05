using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateChanger : MonoBehaviour
{
    public EnumStates state = default;
    public void ChangeState()
    {
            PlaystateManager.Instance.ChangeState(state);
    }
}
