using UnityEngine;
using UnityEngine.Tilemaps;

public class WateringCan : Tool {
    [SerializeField] private TileBase dirtTile;
    [SerializeField] private TileBase wateredTile;
    [SerializeField] private TileBase waterTile;

    [SerializeField] private int maxUsages = 3;
    public int usages = 0;
    private bool pendingFill;

    public override void PrimaryAction() {
        if (usages <= 0) return;
        if (PrepareTileAction(dirtTile, wateredTile, "pour", Sounds.Water, 0.2f)) { 
            usages--;
        }
    }

    public override void SecondaryAction() {
        if (!canUse) return;

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tilemap.WorldToCell(mousePos);
        TileBase targetedTile = tilemap.GetTile(tilePos);
        Vector3 centerPlayerPos = player.transform.position + new Vector3(0, 0.4f, 0);
        Vector3 centerTilePos = tilePos + new Vector3(0.5f, 0.5f, 0);
        float dist = Vector2.Distance(centerTilePos, centerPlayerPos);
        if (dist > useDistance) return;

        if (targetedTile == waterTile) {
            anim.SetTrigger("fill");
            AudioManager.Instance.PlaySound(Sounds.Fill, 0.2f);
            canUse = false;
            player.canMove = false;
            pendingFill = true;
        }
    }

    public override void ApplyTileChange() {
        if (pendingToTile == wateredTile && !pendingFill) PreGameManager.Instance.holesWatered++;

        base.ApplyTileChange();

        if (pendingFill) {
            usages = maxUsages;
            pendingFill = false;
        }
    }

    protected override bool CheckToolUsage() {
        if (!base.CheckToolUsage()) return false;

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tilemap.WorldToCell(mousePos);
        TileBase targetedTile = tilemap.GetTile(tilePos);

        return (usages > 0 && targetedTile == dirtTile) || (targetedTile == waterTile);
    }
}
