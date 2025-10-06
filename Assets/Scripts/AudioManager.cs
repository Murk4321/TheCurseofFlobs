using UnityEngine;

public enum Sounds {
    No = -1,
    Dig = 0,
    Bury = 1,
    Water = 2, 
    Fill = 3,
    Plant = 4,
    Gather = 5,
    Cut = 6,
    Attack = 7,
    Wash = 8,
    Place = 9,
    Dialog = 10,
    Endgame = 11
}

public enum Music {
    No = -1,
    Default = 0,
    Pregame = 1
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;

    [SerializeField] private AudioClip[] soundDB;
    [SerializeField] private AudioClip[] musicDB;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    private void Update() {
        if (GameflowManager.Instance != null) {
            if (GameflowManager.Instance.kills != 0) {
                float t = Mathf.Clamp01(GameflowManager.Instance.kills / 10f);
                float pitchModifier = Mathf.Lerp(1.0f, 0.1f, t);
                SetMusicPitch(pitchModifier);
            }
        }
    }

    public void PlaySound(Sounds sound, float volume = 0.5f) {
        if (sound == Sounds.No) return;
        sfxSource.pitch = Random.Range(0.9f, 1.1f);
        sfxSource.volume = volume;
        sfxSource.PlayOneShot(soundDB[(int)sound]);
    }

    public void PlayMusic(Music music) {
        if (music == Music.No) {
            musicSource.clip = null;
            return;
        }
        musicSource.clip = musicDB[(int)music];
        musicSource.Play();
    }

    public void SetMusicPitch(float pitch) {
        musicSource.pitch = pitch;
    }
}
