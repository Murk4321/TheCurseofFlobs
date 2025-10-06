using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PentagramManager : MonoBehaviour {
    public static PentagramManager Instance { get; private set; }

    public bool timerActive;
    public int timeLeft;
    public int stage = 0;
    public int flowersSacrificed = 0;
    public int devilFlowersSacrificed = 0;
    private bool needColorful;
    private bool needDevil;
    private bool gameEnded;
    private bool brokePact;
    private bool newPactCreated;
    private bool binded;
    private bool happyend;
    public bool lost;

    private Color colorNeeded;
    private PlayerTools playerTools;

    private TextMeshProUGUI timeText;

    [SerializeField] private SpriteRenderer colorBlob;

    [SerializeField] private string[] timerOutFlorbarDialog;
    [SerializeField] private string[] flowersDeadFlorbarDialog;
    [SerializeField] private string[] florbarAfterDemonFlowerDialog;
    [SerializeField] private string[] florbarSpiritFlowerDialog;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }

        timeText = GetComponentInChildren<TextMeshProUGUI>();
        playerTools = FindObjectOfType<PlayerTools>();
    }

    private void Update() {
        if (!PreGameManager.Instance.gameStarted) return;

        if (stage == 0) {
            DialogSystem.Instance.SetObjective($"Grow and offer flowers to Florbar\n\tvia pentagram\n\t(placing a flower resets the timer)\n\t{flowersSacrificed}/12");
        }

        if (timerActive && timeLeft <= 0 && !brokePact && stage != 1) {
            DialogSystem.Instance.SetDialogue(timerOutFlorbarDialog, "Florbar", transform);
            AudioManager.Instance.PlayMusic(Music.No);
            lost = true;
            timerActive = false;
            brokePact = true;
        } else if (brokePact && DialogSystem.Instance.dialogueCompleted && !gameEnded && stage != 1) {
            AudioManager.Instance.PlaySound(Sounds.Endgame);
            DialogSystem.Instance.EndgameFade("Florbar wiped out every flob down to the last");
            gameEnded = true;
        } 

        if (timerActive && timeLeft <= 0 && !brokePact && stage == 1) {
            DialogSystem.Instance.SetDialogue(flowersDeadFlorbarDialog, "Florbar", transform);
            AudioManager.Instance.PlayMusic(Music.No);
            timerActive = false;
            brokePact = true;
        } else if (brokePact && DialogSystem.Instance.dialogueCompleted && stage == 1) {
            AudioManager.Instance.PlayMusic(Music.Default);
            needDevil = true;
            timeText.color = Color.red;
            SetTimer(300);
            stage = 2;
            playerTools.tools[(int)Tools.Knife].GetComponent<Knife>().canKill = true;
            brokePact = false;
            DialogSystem.Instance.SetObjective("Kill a flob, bury him\n\tgrow a flower on top\n\tand bring to Florbar");
        }

        if (stage == 2 && DialogSystem.Instance.dialogueCompleted && !newPactCreated && devilFlowersSacrificed != 0) {
            AudioManager.Instance.PlayMusic(Music.Default);
            stage = 3;
            newPactCreated = true;
            TileManager.Instance.ReplaceRandomGrassWithSpiritFlower();
            SetTimer(300);
            DialogSystem.Instance.SetObjective($"Grow and sacrifice\n\tdevil's flowers to Florbar\n\t{devilFlowersSacrificed}/12");
        }

        if (binded && DialogSystem.Instance.dialogueCompleted && !happyend) {
            DialogSystem.Instance.StartFade(1, "");
            happyend = true;
            Invoke(nameof(LoadEnding), 2);
        }
    }

    private void LoadEnding() {
        SceneManager.LoadScene(2);
    }

    private void Start() {
        timerActive = false;
    }

    public void SetTimer(int seconds) {
        if (timerActive) return;
        timeLeft = seconds;
        timerActive = true;
        timeText.text = FormatTime(timeLeft);
        Invoke(nameof(TimerFlow), 1);
    }

    private void TimerFlow() {
        if (timeLeft > 0 && timerActive) {
            timeLeft--;
            timeText.text = FormatTime(timeLeft);
            Invoke(nameof(TimerFlow), 1);
        } else {
            timerActive = false;
        }
    }

    private static string FormatTime(float seconds) {
        int minutes = Mathf.FloorToInt(seconds / 60f);
        int secs = Mathf.FloorToInt(seconds % 60f);
        return $"{minutes:00}:{secs:00}";
    }

    public void AcceptFlower(GameObject flower) {
        if (flower.GetComponent<FlowerPlant>().flowerType == FlowerType.Spirit) {
            timerActive = false;
            DialogSystem.Instance.SetDialogue(florbarSpiritFlowerDialog, "Florbar", transform);
            binded = true;
        }

        if (needColorful) {
            if (ColorUtils.CompareColors(flower.GetComponent<FlowerPlant>().flowerColor, colorNeeded) < 0.75f) {
                return;
            } else {
                colorBlob.gameObject.SetActive(false);
                needColorful = false;
            }
        }

        if (needDevil) {
            if (flower.GetComponent<FlowerPlant>().flowerType != FlowerType.Devil) {
                return;
            }
        }

        if (!needDevil) flowersSacrificed++;
        if (needDevil) devilFlowersSacrificed++;

        if (flowersSacrificed % 5 == 0 && stage <= 1) {
            timeLeft = 300;
            colorNeeded = ColorUtils.RandomColor();
            needColorful = true;
            colorBlob.color = colorNeeded;
            colorBlob.gameObject.SetActive(true);
        } else if (stage <= 1) timeLeft = 90;

        if (flowersSacrificed == 11 && stage == 0) {
            stage = 1;
            DialogSystem.Instance.SetObjective("???");
        }

        if (devilFlowersSacrificed == 1 && stage == 2) {
            DialogSystem.Instance.SetDialogue(florbarAfterDemonFlowerDialog, "Florbar", transform);
            AudioManager.Instance.PlayMusic(Music.No);
            timerActive = false;
        } 

        if (devilFlowersSacrificed % 5 == 0 && stage >= 3) {
            timeLeft = 600;
            colorNeeded = ColorUtils.RandomColor();
            needColorful = true;
            FlobSpawner flobSpawner = FindObjectOfType<FlobSpawner>();
            var randomFlob = flobSpawner.spawnedFlobs[Random.Range(0, flobSpawner.spawnedFlobs.Count)];
            colorBlob.color = randomFlob.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
            colorBlob.gameObject.SetActive(true);
        } else if (stage >= 3) timeLeft = 300;
    }
}

