using UnityEngine;
using UnityEngine.Tilemaps;

public class CorpseTool : Tool
{
    private TileBase dirtTile;
    private TileBase wateredTile;
    private TileBase bloodiedTile;

    private PlayerTools playerTools;

    private void Awake() {
        playerTools = FindObjectOfType<PlayerTools>();

        dirtTile = TileManager.Instance.dirtTile;
        wateredTile = TileManager.Instance.wateredTile;
        bloodiedTile = TileManager.Instance.bloodiedTile;
    }

    public override void PrimaryAction() {
        if (PrepareTileAction(dirtTile, bloodiedTile, "bury", Sounds.Bury) ||
            PrepareTileAction(wateredTile, bloodiedTile, "bury", Sounds.Bury)) { 

            TileManager.Instance.AddTile((Vector3Int)storedTilePos);
            TileManager.Instance.SetFlowerColor((Vector3Int)storedTilePos, transform.GetChild(0).GetComponent<SpriteRenderer>().color);
        }
    }

    public override void SecondaryAction() {
        return;
    }

    protected override bool CheckToolUsage() {
        if (!base.CheckToolUsage()) return false;

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tilemap.WorldToCell(mousePos);
        TileBase targetedTile = tilemap.GetTile(tilePos);

        return targetedTile == dirtTile || targetedTile == wateredTile;
    }

    public override void ApplyTileChange() {
        base.ApplyTileChange();
        playerTools.corpsePickedUp = null;
        playerTools.tools[(int)Tools.Corpse] = null;
        playerTools.SetTool(Tools.No);
        Destroy(gameObject);
    }
}
