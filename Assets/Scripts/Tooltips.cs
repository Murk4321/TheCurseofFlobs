using TMPro;
using UnityEngine;

public class Tooltips : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI LMBAction;
    [SerializeField] private TextMeshProUGUI RMBAction;

    private PlayerTools playerTools;

    private void Awake() {
        playerTools = FindObjectOfType<PlayerTools>();
    }

    private void Update() {
        switch (playerTools.equippedTool) {
            case Tools.No:
                ActionVisibility(false, false);
                break;
            case Tools.Shovel:
                ActionVisibility(true, true);
                LMBAction.text = "Dig";
                RMBAction.text = "Bury";
                break;
            case Tools.WateringCan:
                ActionVisibility(true, true);
                LMBAction.text = "Water";
                RMBAction.text = "Fill";
                break;
            case Tools.Pouch:
                ActionVisibility(true, true);
                LMBAction.text = "Plant";
                RMBAction.text = "Gather";
                break;
            case Tools.Knife:
                var knife = playerTools.tools[(int)Tools.Knife].GetComponent<Knife>();
                bool canKill = knife.canKill;
                bool hasFlower = playerTools.flowerPickedUp != null;
                bool hasCorpse = playerTools.corpsePickedUp != null;

                switch (true) {
                    case true when knife.isBloody:
                        LMBAction.text = hasFlower ? "Wash" : "Cut/Wash";
                        RMBAction.text = canKill && !hasCorpse ? "Kill" : "None";
                        ActionVisibility(!hasFlower, canKill && !hasCorpse);
                        break;

                    case true when hasFlower:
                        LMBAction.text = knife.isBloody ? "Wash" : "None";
                        RMBAction.text = canKill && !hasCorpse ? "Kill" : "None";
                        ActionVisibility(!hasFlower, canKill && !hasCorpse);
                        break;

                    case true when !hasFlower:
                        LMBAction.text = knife.isBloody ? "Cut/Wash" : "Cut";
                        RMBAction.text = canKill && !hasCorpse ? "Kill" : "None";
                        ActionVisibility(!hasFlower, canKill && !hasCorpse);
                        break;
                }

                break;


            case Tools.Flower:
                ActionVisibility(true, true);
                LMBAction.text = "Place";
                RMBAction.text = "Sacrifice";
                break;
            case Tools.Corpse:
                ActionVisibility(true, false);
                LMBAction.text = "Bury";
                break;
        }
    }

    private void ActionVisibility(bool lmb, bool rmb) {
        LMBAction.transform.parent.gameObject.SetActive(lmb);
        RMBAction.transform.parent.gameObject.SetActive(rmb);
    }
}
