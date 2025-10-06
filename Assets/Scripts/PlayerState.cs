using Cinemachine;
using System.Collections;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cam;
    private CinemachineBasicMultiChannelPerlin camNoise;

    public static PlayerState Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        camNoise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    [SerializeField] private SpriteRenderer playerBody;
    [SerializeField] private SpriteRenderer playerEyes;

    [SerializeField] private Sprite neutralBody;
    [SerializeField] private Sprite neutralEyes;

    [SerializeField] private Sprite speechlessBody;
    [SerializeField] private Sprite[] madnessEyesSequence;

    public bool madnessSet;
    private int madnessEyesIndex = 0;

    private bool gameEnded;

    private void Update() {
        if ((GameflowManager.Instance.kills == 0 && PentagramManager.Instance.stage == 2) || PentagramManager.Instance.lost) {
            playerBody.sprite = neutralBody;
            playerEyes.sprite = neutralEyes;
        } else if (GameflowManager.Instance.kills >= 1 && !gameEnded) {
            playerBody.sprite = speechlessBody;
            camNoise.m_FrequencyGain = GameflowManager.Instance.kills;
            PostprocessRegulator.Instance.SetVignette((float)GameflowManager.Instance.kills / 10);
            if (!madnessSet) {
                madnessSet = true;
                StartCoroutine(MadnessEyes());
            }
        }

        if (GameflowManager.Instance.kills >= 10 && !gameEnded) {
            AudioManager.Instance.PlaySound(Sounds.Endgame);
            DialogSystem.Instance.EndgameFade("Flobby went insane");
            gameEnded = true;
        }
    }

    private IEnumerator MadnessEyes() {
        while (madnessSet) {
            playerEyes.sprite = madnessEyesSequence[madnessEyesIndex];
            madnessEyesIndex = Random.Range(0, madnessEyesSequence.Length);
            yield return new WaitForSeconds(0.5f / GameflowManager.Instance.kills);
        }
    }
}
