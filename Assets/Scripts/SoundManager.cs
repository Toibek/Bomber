using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioClip[] BackgroundMusic;
    [SerializeField] GameObject Prefab_Player;
    [SerializeField] int AmountOfPlayers;
    AudioSource MusicPlayer;
    List<AudioSource> Players = new List<AudioSource>();
    #region singleton
    public static SoundManager Instance;
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
            Instance = null;
    }
    #endregion

    public static void Play(AudioClip clip) => Instance?.PlaySound(clip);
    void Start()
    {
        MusicPlayer = GetComponent<AudioSource>();
        MusicPlayer.clip = BackgroundMusic[Random.Range(0,BackgroundMusic.Length)];
        for (int i = 0; i < AmountOfPlayers; i++)
            Players.Add(Instantiate(Prefab_Player, transform).GetComponent<AudioSource>());
    }
    void Update()
    {
        if (MusicPlayer.enabled && !MusicPlayer.isPlaying)
        {
            Debug.Log("Starting Music");
            MusicPlayer.clip = BackgroundMusic[Random.Range(0, BackgroundMusic.Length)];
            MusicPlayer.Play();
        }
    }
    public void PlaySound(AudioClip clip)
    {
        AudioSource source = GetAvaliable();

        source.clip = clip;
        source.Play();

        if (Players.IndexOf(source) > AmountOfPlayers)
        {
            Players.Remove(source);
            Destroy(source.gameObject, clip.length);
        }
    }
    public AudioSource GetAvaliable()
    {
        for (int i = 0; i < Players.Count; i++)
        {
            if (!Players[i].isPlaying)
                return Players[i];
        }
        AudioSource Source = Instantiate(Prefab_Player, transform).GetComponent<AudioSource>();
        Players.Add(Source);
        return Source;

    }
}
