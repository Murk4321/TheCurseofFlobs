using UnityEngine;

public enum Tools {
    No = -1,
    Shovel = 0,
    WateringCan = 1,
    Pouch = 2,
    Knife = 3,
    Flower = 4,
    Corpse = 5
}

public class PlayerTools : MonoBehaviour {
    public Tools equippedTool = Tools.No;

    public Tool[] tools; 
    [SerializeField] private Sprite[] toolCursors;

    private PlayerMovement player;

    public Transform toolHolder;
    public GameObject flowerPickedUp;
    public GameObject corpsePickedUp;

    private void Start() {
        player = GetComponent<PlayerMovement>();

        foreach (var t in tools) {
            if (t == null) continue;
            t.gameObject.SetActive(false);
        }
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) UseToolPrimary();
        if (Input.GetMouseButtonDown(1)) UseToolSecondary();

        if (player.canMove) HandleToolChange();
    }

    private void HandleToolChange() {
        if (Input.GetKeyDown(KeyCode.Alpha1)) SetTool(Tools.Shovel);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SetTool(Tools.WateringCan);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SetTool(Tools.Pouch);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SetTool(Tools.Knife);
        if (Input.GetKeyDown(KeyCode.Alpha5) && flowerPickedUp != null) SetTool(Tools.Flower);
        if (Input.GetKeyDown(KeyCode.Alpha6) && corpsePickedUp != null) SetTool(Tools.Corpse);

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f) {
            int minTool = (int)Tools.Shovel;
            int maxTool = (int)Tools.Knife;
            if (flowerPickedUp != null) maxTool = (int)Tools.Flower;
            if (corpsePickedUp != null) maxTool = (int)Tools.Corpse;

            int currentIndex = (int)equippedTool;

            if (equippedTool == Tools.No) {
                currentIndex = scroll > 0 ? minTool : maxTool;
            } else {
                if (scroll < 0f) {
                    currentIndex++;
                    if ((Tools)currentIndex == Tools.Flower && flowerPickedUp == null) currentIndex++;
                    if (currentIndex > maxTool) currentIndex = minTool;
                } else if (scroll > 0f) {
                    currentIndex--;
                    if ((Tools)currentIndex == Tools.Flower && flowerPickedUp == null) currentIndex--;
                    if (currentIndex < minTool) currentIndex = maxTool;
                }
            }
            SetTool((Tools)currentIndex);
        }
    }

    public void SetTool(Tools tool) {
        if (tool == equippedTool) {
            tools[(int)equippedTool].gameObject.SetActive(false);
            equippedTool = Tools.No;
            MouseSettings.Instance.SetCursorSprite(null);
            TilemapHighlighter.Instance.SetHighlightTile(HighlightTileType.Disable);
            return;
        }

        if (tool == Tools.No) {
            foreach (var t in tools) {
                if (t != null) t.gameObject.SetActive(false);
            }
            equippedTool = Tools.No;
            MouseSettings.Instance.SetCursorSprite(null);
            TilemapHighlighter.Instance.SetHighlightTile(HighlightTileType.Disable);
            return;
        }

        if (equippedTool != Tools.No) {
            tools[(int)equippedTool].gameObject.SetActive(false);
        }

        tools[(int)tool].gameObject.SetActive(true);
        MouseSettings.Instance.SetCursorSprite(toolCursors[(int)tool]);
        equippedTool = tool;
    }

    private void UseToolPrimary() {
        if (equippedTool != Tools.No) {
            tools[(int)equippedTool].PrimaryAction();
        }
    }

    private void UseToolSecondary() {
        if (equippedTool != Tools.No) {
            tools[(int)equippedTool].SecondaryAction();
        }
    }

}
