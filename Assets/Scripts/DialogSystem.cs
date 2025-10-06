using System.Collections;
using TMPro;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class DialogSystem : MonoBehaviour
{
    public static DialogSystem Instance { get; private set; }

    [SerializeField] private GameObject bigSplashScreen;
    [SerializeField] private GameObject gameOverText;
    [SerializeField] private TextMeshProUGUI objectiveText;
    [SerializeField] private float fadeDuration = 1.5f;
    private TMP_Text splashText;
    public CanvasGroup canvasGroup;
    private Coroutine fadeCoroutine;
    private PlayerMovement player;

    [SerializeField] private GameObject dialogWindow;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private float typeSpeed = 0.03f;
    private string[] dialogueLines;
    [SerializeField] private GameObject inventory;
    [SerializeField] private GameObject toolTips;

    private int currentLine = 0;
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private bool textFullyShown = false;
    public bool dialogueCompleted;
    public bool fadeEnded;

    [SerializeField] private CinemachineVirtualCamera followCamera;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        canvasGroup = bigSplashScreen.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = bigSplashScreen.AddComponent<CanvasGroup>();

        splashText = bigSplashScreen.GetComponentInChildren<TMP_Text>();
    }

    private void Start() {
        player = FindObjectOfType<PlayerMovement>();
    }

    private void Update() {
        if (Input.anyKeyDown) {
            if (isTyping) {
                SkipTyping();
            } else if (textFullyShown) {
                ShowNextLine();
            }
        }
    }

    private void ShowNextLine() {
        if (currentLine >= dialogueLines.Length) {
            dialogueText.text = "";
            inventory.SetActive(true);
            toolTips.SetActive(true);
            dialogWindow.SetActive(false);
            player.inDialog = false;
            dialogueCompleted = true;
            followCamera.Follow = player.transform;
            return;
        }
        player.inDialog = true;
        textFullyShown = false;
        dialogueText.text = "";
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(dialogueLines[currentLine]));
        currentLine++;
    }

    private IEnumerator TypeText(string text) {
        isTyping = true;
        dialogueText.text = text;
        dialogueText.maxVisibleCharacters = 0;

        for (int i = 0; i <= text.Length; i++) {
            dialogueText.maxVisibleCharacters = i;
            AudioManager.Instance.PlaySound(Sounds.Dialog);
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
        textFullyShown = true;
    }

    private void SkipTyping() {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.maxVisibleCharacters = dialogueText.text.Length;
        isTyping = false;
        textFullyShown = true;
    }

    public void SetDialogue(string[] newLines, string speakerName, Transform target = null) {
        if (target != null) {
            followCamera.gameObject.SetActive(true);
            followCamera.Follow = target;
        }
        inventory.SetActive(false);
        toolTips.SetActive(false);
        dialogWindow.SetActive(true);
        dialogueCompleted = false;
        player.inDialog = true;
        speakerText.text = speakerName;
        dialogueLines = newLines;
        currentLine = 0;
        ShowNextLine();
    }

    public void EndgameFade(string text) {
        fadeEnded = false;
        gameOverText.SetActive(true);
        FadeIn(text);
        PostprocessRegulator.Instance.SetVignette(0);
        Invoke(nameof(RestartGame), 20);
    }

    public void SetObjective(string obj) {
        objectiveText.text = $"Current task: \n\t{obj}";
    }

    private void RestartGame() {
        SceneManager.LoadScene(0);
    }

    public void StartFade(float time, string text) {
        fadeEnded = false;
        FadeIn(text);
        Invoke(nameof(FadeOut), time);
    }

    public void StartFadeOut(float time) {
        fadeEnded = false;
        Invoke(nameof(FadeOut), time);
    }

    public void FadeIn(string text) {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        bigSplashScreen.SetActive(true);
        splashText.text = text;

        fadeCoroutine = StartCoroutine(FadeRoutine(0f, 1f));
    }

    public void FadeOut() {
        if (fadeCoroutine != null)
            StopCoroutine(fadeCoroutine);

        fadeCoroutine = StartCoroutine(FadeRoutine(1f, 0f, disableOnEnd: true));
    }

    private IEnumerator FadeRoutine(float from, float to, bool disableOnEnd = false) {
        float t = 0f;
        canvasGroup.alpha = from;

        while (t < fadeDuration) {
            t += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = to;
        fadeEnded = true;

        if (disableOnEnd && to == 0f)
            bigSplashScreen.SetActive(false);
    }
}