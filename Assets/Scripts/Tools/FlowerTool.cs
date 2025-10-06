using UnityEngine;
using UnityEngine.Tilemaps;

public class FlowerTool : Tool {
    private TileBase grassTile;
    private TileBase pentagramTile;

    private PlayerTools playerTools;
    public Color flowerColor;

    private void Awake() {
        playerTools = FindObjectOfType<PlayerTools>();
        grassTile = TileManager.Instance.grassTile;
        pentagramTile = TileManager.Instance.pentagramTile;
    }

    public override void PrimaryAction() {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tilemap.WorldToCell(mousePos);
        TileBase targetedTile = tilemap.GetTile(tilePos);
        Vector3 centerPlayerPos = player.transform.position + new Vector3(0, 0.4f, 0);
        Vector3 centerTilePos = tilePos + new Vector3(0.5f, 0.5f, 0);
        float dist = Vector2.Distance(centerTilePos, centerPlayerPos);
        if (dist > useDistance) return;

        if (targetedTile == grassTile) {
            AudioManager.Instance.PlaySound(Sounds.Place);
            transform.parent = null;
            transform.position = centerTilePos + new Vector3(Random.Range(-0.2f, 0.2f), Random.Range(-0.2f, 0.2f), 0);
            playerTools.flowerPickedUp = null;
            playerTools.tools[(int)Tools.Flower] = null;
            playerTools.SetTool(Tools.No);
            TileManager.Instance.AddTile(tilePos);
            TileManager.Instance.SetFlower(tilePos, gameObject);
            SpriteRenderer[] flowerRenderers = transform.GetComponentsInChildren<SpriteRenderer>();
            //foreach (var renderer in flowerRenderers) {
            //    renderer.sortingLayerName = "Objects";
            //}
            PreGameManager.Instance.flowersHarvested++;
            Destroy(this);
        }
    }

    public override void SecondaryAction() {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tilemap.WorldToCell(mousePos);
        TileBase targetedTile = tilemap.GetTile(tilePos);
        Vector3 centerPlayerPos = player.transform.position + new Vector3(0, 0.4f, 0);
        Vector3 centerTilePos = tilePos + new Vector3(0.5f, 0.5f, 0);
        float dist = Vector2.Distance(centerTilePos, centerPlayerPos);
        if (dist > useDistance) return;

        if (targetedTile == pentagramTile) {
            AudioManager.Instance.PlaySound(Sounds.Place);
            PentagramManager.Instance.AcceptFlower(playerTools.flowerPickedUp);
            playerTools.flowerPickedUp = null;
            playerTools.tools[(int)Tools.Flower] = null;
            playerTools.SetTool(Tools.No);
            Destroy(gameObject);
        }
    }

    protected override bool CheckToolUsage() {
        if (!base.CheckToolUsage()) return false;

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tilemap.WorldToCell(mousePos);
        TileBase targetedTile = tilemap.GetTile(tilePos);

        return targetedTile == grassTile || targetedTile == pentagramTile;
    }
}
