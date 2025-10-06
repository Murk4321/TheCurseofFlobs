using UnityEngine;

public class PreGameManager : MonoBehaviour
{
    public static PreGameManager Instance { get; private set; }

    private void Awake() {
        if (Instance == null) {
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }

    [SerializeField] private GameObject followCamera;
    [SerializeField] private Vector3 spawnPos;

    [SerializeField] private string[] grandpaFirst;
    [SerializeField] private string[] grandpaAfterDig;
    [SerializeField] private string[] grandpaAfterWater;
    [SerializeField] private string[] grandpaAfterPlant;
    [SerializeField] private string[] grandpaAfterHarvest;

    [SerializeField] private GameObject flob;
    public bool isSmall = true;
    public bool gameStarted = false;

    private int dialogueState = -2;

    public int holesDug = 0;
    public int holesWatered = 0;
    public int flowersPlanted = 0;
    public int flowersHarvested = 0;

    private void Start() {
        DialogSystem.Instance.StartFadeOut(1.5f);
        followCamera.SetActive(false);
    }

    private void Update() {
        if (gameStarted) return;
        if (Input.GetKeyDown(KeyCode.Z)) {
            dialogueState = 5;
            DialogSystem.Instance.dialogueCompleted = true;
        }
        if (DialogSystem.Instance.fadeEnded && dialogueState == -2) {
            AudioManager.Instance.PlayMusic(Music.Pregame);
            dialogueState = 0;
        }
        switch (dialogueState) {
            case 0:
                DialogSystem.Instance.SetDialogue(grandpaFirst, "Grandpa Flob");
                dialogueState = 1; 
                break;
            case 1:
                if (DialogSystem.Instance.dialogueCompleted)
                    DialogSystem.Instance.SetObjective("Dig 4 holes with your shovel");
                if (holesDug >= 4)
                {
                    DialogSystem.Instance.SetDialogue(grandpaAfterDig, "Grandpa Flob");
                    dialogueState = 2;
                }
                break;
            case 2:
                if (DialogSystem.Instance.dialogueCompleted)
                    DialogSystem.Instance.SetObjective("Fill watering can and \n\twater your holes");
                if (holesWatered >= 4) {
                    DialogSystem.Instance.SetDialogue(grandpaAfterWater, "Grandpa Flob");
                    dialogueState = 3;
                }
                break;
            case 3:
                if (DialogSystem.Instance.dialogueCompleted)
                    DialogSystem.Instance.SetObjective("Gather 4 wild flowers\n\tand plant them");
                if (flowersPlanted >= 4) {
                    DialogSystem.Instance.SetDialogue(grandpaAfterPlant, "Grandpa Flob");
                    dialogueState = 4;
                }
                break;
            case 4:
                if (DialogSystem.Instance.dialogueCompleted)
                    DialogSystem.Instance.SetObjective("Wait for flowers to grow\n\tand place them near grandpa");
                if (flowersHarvested >= 4) {
                    DialogSystem.Instance.SetDialogue(grandpaAfterHarvest, "Grandpa Flob");
                    dialogueState = 5;
                }
                break;
            case 5:
                if (DialogSystem.Instance.dialogueCompleted) {
                    DialogSystem.Instance.StartFade(3, "Nowadays...");
                    Invoke(nameof(BeginGame), 1.5f);
                    dialogueState = -1;
                }
                break;
        }
    }

    private void BeginGame() {
        followCamera.SetActive(true);
        flob.transform.position = spawnPos;
        flob.transform.localScale = Vector3.one;
        isSmall = false;
        PentagramManager.Instance.SetTimer(90);
        gameStarted = true;
        AudioManager.Instance.PlayMusic(Music.Default);
        PentagramManager.Instance.timerActive = true;
        TileManager.Instance.DestroySpawnArea();
    }
}
