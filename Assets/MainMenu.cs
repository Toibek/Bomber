using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] Text NameText;
    [SerializeField] Text IdentifierText;
    public void UpdateName()
    {
        NameText.text = LeaderboardManager.Instance?.playerEntry.Name;
        IdentifierText.text = Application.version + " ; " + LeaderboardManager.Instance?.playerEntry.ID;
    }
    private void OnEnable()
    {
        UpdateName();
    }
}
