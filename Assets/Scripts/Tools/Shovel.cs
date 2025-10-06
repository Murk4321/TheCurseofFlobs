using UnityEngine;
using UnityEngine.Tilemaps;

public class Shovel : Tool {
    [SerializeField] private TileBase dirtTile;
    [SerializeField] private TileBase wateredTile;
    [SerializeField] private TileBase bloodyTile;
    [SerializeField] private TileBase grassTile;

    public override void PrimaryAction() {
        PrepareTileAction(grassTile, dirtTile, "dig", Sounds.Dig);
    }

    public override void SecondaryAction() {
        PrepareTileAction(dirtTile, grassTile, "bury", Sounds.Bury);
        PrepareTileAction(wateredTile, grassTile, "bury", Sounds.Bury);
        PrepareTileAction(bloodyTile, grassTile, "bury", Sounds.Bury);
    }

    protected override bool CheckToolUsage() {
        if (!base.CheckToolUsage()) return false;

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tilemap.WorldToCell(mousePos);
        TileBase targetedTile = tilemap.GetTile(tilePos);

        return targetedTile == dirtTile || targetedTile == grassTile || targetedTile == wateredTile || targetedTile == bloodyTile;
    }

    public override void ApplyTileChange() {
        if (pendingToTile == dirtTile) PreGameManager.Instance.holesDug++;
        base.ApplyTileChange();
    }

}
