using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Name : MonoBehaviour
{
    public string PlayerName = "aaa";
    [SerializeField] string AvaliableChars = "abcdefghijklmnopqrstuvwxyz0123456789 ";
    public GameObject[] LetterPickers;
    char[] characters;
    private void Start()
    {
        characters = new char[LetterPickers.Length];
        for (int i = 0; i < characters.Length; i++)
            characters[i] = AvaliableChars[Random.Range(0, AvaliableChars.Length)];

        PlayerName = "";
        for (int i = 0; i < characters.Length; i++)
            PlayerName += characters[i];
    }
    private void OnEnable()
    {
        for (int i = 0; i < LetterPickers.Length; i++)
        {
            GameObject go = LetterPickers[i];
            go.GetComponentInChildren<Text>().text = PlayerName[Mathf.Clamp(i, 0, PlayerName.Length)].ToString();

            GameObject bup = go.GetComponentsInChildren<Button>()[0].gameObject;
            bup.name = i.ToString();
            bup.GetComponent<Button>().onClick.RemoveAllListeners();
            bup.GetComponent<Button>().onClick.AddListener(() => CharUp(int.Parse(bup.name)));

            GameObject bdown = go.GetComponentsInChildren<Button>()[1].gameObject;
            bdown.name = i.ToString();
            bdown.GetComponent<Button>().onClick.RemoveAllListeners();
            bdown.GetComponent<Button>().onClick.AddListener(() => CharDown(int.Parse(bdown.name)));
        }
    }
    public void CharUp(int character)
    {
        int pos = AvaliableChars.IndexOf(characters[character]) + 1;
        if (pos < 0)
            pos = AvaliableChars.Length-1;
        else if (pos > AvaliableChars.Length-1)
            pos = 0;

        characters[character] = AvaliableChars[pos];
        LetterPickers[character].GetComponentInChildren<Text>().text = characters[character].ToString();
        PlayerName = "";
        for (int i = 0; i < characters.Length; i++)
            PlayerName += characters[i];
    }
    public void CharDown(int character)
    {
        int pos = AvaliableChars.IndexOf(characters[character]) - 1;
        if (pos < 0)
            pos = AvaliableChars.Length-1;
        else if (pos > AvaliableChars.Length-1)
            pos = 0;

        characters[character] = AvaliableChars[pos];
        LetterPickers[character].GetComponentInChildren<Text>().text = characters[character].ToString();
        PlayerName = "";
        for (int i = 0; i < characters.Length; i++)
            PlayerName += characters[i];
    }
    public void SetName()
    {
        LeaderboardManager.Instance.playerEntry.Name = PlayerName;
    }

}
