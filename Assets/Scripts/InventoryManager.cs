using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private Image[] toolImages;
    [SerializeField] private Image[] flowerImages;
    [SerializeField] private Image[] corpseImages;
    [SerializeField] private TextMeshProUGUI wateringCanUsageText;
    [SerializeField] private TextMeshProUGUI flowerPouchUsageText;
    private PlayerTools playerTools;

    private void Awake() {
        playerTools = FindObjectOfType<PlayerTools>();

        foreach (var image in toolImages) {
            image.transform.parent.localRotation = Quaternion.Euler(0, 0, Random.Range(-5f, 5f));
        }
    }

    private void Update() {
        for (int i = 0; i < toolImages.Length; i++) {
            var tool = playerTools.tools[i];
            if (tool == null) {
                toolImages[i].enabled = false;
                continue;
            }
            if (playerTools.equippedTool != Tools.No && tool == playerTools.tools[(int)playerTools.equippedTool]) {
                toolImages[i].transform.parent.localScale = new Vector3(1.15f, 1.15f, 1.15f);
            } else {
                toolImages[i].transform.parent.localScale = new Vector3(1, 1, 1);
            }

            SpriteRenderer toolImage = tool.GetComponent<SpriteRenderer>();

            if (toolImage != null) {
                toolImages[i].enabled = true;
                toolImages[i].sprite = toolImage.sprite;
                foreach (var img in flowerImages) img.enabled = false;
                foreach (var img in corpseImages) img.enabled = false;
                toolImages[4].transform.parent.localScale = new Vector3(1, 1, 1);
                toolImages[5].transform.parent.localScale = new Vector3(1, 1, 1);
            } else if (tool == playerTools.tools[4]) {
                foreach (var img in flowerImages) img.enabled = true;
                flowerImages[0].sprite = tool.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
                flowerImages[1].sprite = tool.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite;
                flowerImages[1].color = tool.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color;
                flowerImages[2].sprite = tool.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite;
            } else if (tool == playerTools.tools[5]) {
                foreach (var img in corpseImages) img.enabled = true;
                corpseImages[0].sprite = tool.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
                corpseImages[0].color = tool.transform.GetChild(0).GetComponent<SpriteRenderer>().color;
                corpseImages[1].sprite = tool.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite;
                corpseImages[2].sprite = tool.transform.GetChild(0).GetChild(1).GetComponent<SpriteRenderer>().sprite;
            } 
        }

        int wateringCanUsages = playerTools.tools[1].GetComponent<WateringCan>().usages;
        int flowerPouchSeeds = playerTools.tools[2].GetComponent<FlowerPouch>().seedCount;

        if (wateringCanUsages == 0) wateringCanUsageText.color = Color.red;
        else wateringCanUsageText.color = Color.white;

        if (flowerPouchSeeds == 0) flowerPouchUsageText.color = Color.red;
        else flowerPouchUsageText.color = Color.white;

        wateringCanUsageText.text = wateringCanUsages.ToString();
        flowerPouchUsageText.text = flowerPouchSeeds.ToString();
    }
}
