using UnityEngine;
using UnityEngine.Tilemaps;

public class FlowerPouch : Tool {
    [SerializeField] private TileBase flowerTile;
    [SerializeField] private TileBase grassTile;
    [SerializeField] private TileBase wateredTile;
    [SerializeField] private TileBase blodiedTile;
    [SerializeField] private TileBase plantedTile;
    [SerializeField] private TileBase blodiedPlantedTile;
    [SerializeField] private TileBase spiritflowerTile;

    [SerializeField] private GameObject flowerPrefab;
    [SerializeField] private GameObject demonFlowerPrefab;
    [SerializeField] private GameObject spiritFlowerPrefab;

    [SerializeField] private string[] spiritGrandpaDialog;

    public int seedCount = 0;
    public bool spirited = false;
    private bool pendingCollect;
    private bool pendingCollectSpirit;
    private bool pendingPlant;
    private bool pendingDemonPlant;
    private bool pendingSpiritPlant;

    public override void PrimaryAction() {
        if (!canUse || seedCount <= 0) return;

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tilemap.WorldToCell(mousePos);
        TileBase targetedTile = tilemap.GetTile(tilePos);
        Vector3 centerPlayerPos = player.transform.position + new Vector3(0, 0.4f, 0);
        Vector3 centerTilePos = tilePos + new Vector3(0.5f, 0.5f, 0);
        float dist = Vector2.Distance(centerTilePos, centerPlayerPos);
        if (dist > useDistance) return;
        
        if (targetedTile == wateredTile) {
            anim.SetTrigger("plant");
            canUse = false;
            player.canMove = false;

            storedTilePos = tilePos;

            pendingFromTile = wateredTile;
            pendingToTile = plantedTile;
            if (!spirited) pendingPlant = true;
            else pendingSpiritPlant = true;
        }

        if (targetedTile == blodiedTile) {
            anim.SetTrigger("plant");
            canUse = false;
            player.canMove = false;

            storedTilePos = tilePos;
            pendingFromTile = blodiedTile;
            pendingToTile = blodiedPlantedTile;
            pendingDemonPlant = true;
        }
    }

    public override void SecondaryAction() {
        if (!canUse) return;

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 centerPlayerPos = player.transform.position + new Vector3(0, 0.4f, 0);
        Vector3Int tilePos = tilemap.WorldToCell(mousePos);
        TileBase targetedTile = tilemap.GetTile(tilePos);
        Vector3 centerTilePos = tilePos + new Vector3(0.5f, 0.5f, 0);
        float dist = Vector2.Distance(centerTilePos, centerPlayerPos);
        if (dist > useDistance) return;


        if (targetedTile == flowerTile) {
            anim.SetTrigger("collect");
            canUse = false;
            player.canMove = false;

            storedTilePos = tilePos;
            pendingFromTile = flowerTile;
            pendingToTile = grassTile;
            pendingCollect = true;
        }

        if (targetedTile == spiritflowerTile) {
            anim.SetTrigger("collect");
            canUse = false;
            player.canMove = false;

            storedTilePos = tilePos;
            pendingFromTile = spiritflowerTile;
            pendingToTile = grassTile;
            pendingCollectSpirit = true;
        }
    }

    public override void ApplyTileChange() {
        if (storedTilePos.HasValue) {
            tilemap.SetTile(storedTilePos.Value, pendingToTile);

            if (pendingCollect) {
                seedCount++;
                pendingCollect = false;
                AudioManager.Instance.PlaySound(Sounds.Gather);

                TileManager.Instance.DecreaseFlowerCount();
            }

            if (pendingCollectSpirit) {
                seedCount++;
                pendingCollectSpirit = false;
                AudioManager.Instance.PlaySound(Sounds.Gather);

                PentagramManager.Instance.SetTimer(500);
                DialogSystem.Instance.SetDialogue(spiritGrandpaDialog, "Grandpa's Spirit");
                AudioManager.Instance.PlayMusic(Music.No);
                DialogSystem.Instance.SetObjective("Follow Grandpa's instructions\n\tlike in the old good times");
                spirited = true;
            }

            if (pendingPlant) {
                seedCount--;
                pendingPlant = false;
                PreGameManager.Instance.flowersPlanted++;
                AudioManager.Instance.PlaySound(Sounds.Plant);

                Vector3 plantPos = (Vector3)storedTilePos +
                                    new Vector3(Random.Range(0.3f, 0.7f), Random.Range(0.3f, 0.7f), 0);

                var flowerInstance = Instantiate(flowerPrefab, plantPos, Quaternion.identity)
                                    .GetComponent<FlowerPlant>();

                flowerInstance.growingOn = storedTilePos.Value;
                TileManager.Instance.AddTile(storedTilePos.Value);
                TileManager.Instance.SetFlower(storedTilePos.Value, flowerInstance.gameObject);
            }

            if (pendingSpiritPlant) {
                seedCount--;
                pendingSpiritPlant = false;
                spirited = false;
                AudioManager.Instance.PlaySound(Sounds.Plant);

                Vector3 plantPos = (Vector3)storedTilePos +
                                    new Vector3(Random.Range(0.3f, 0.7f), Random.Range(0.3f, 0.7f), 0);

                var flowerInstance = Instantiate(spiritFlowerPrefab, plantPos, Quaternion.identity)
                                    .GetComponent<FlowerPlant>();

                flowerInstance.growingOn = storedTilePos.Value;
                TileManager.Instance.AddTile(storedTilePos.Value);
                TileManager.Instance.SetFlower(storedTilePos.Value, flowerInstance.gameObject);
                DialogSystem.Instance.SetObjective("Wait for plant to grow\n\tand give it to Florbar");
            }

            if (pendingDemonPlant) {
                seedCount--;
                pendingDemonPlant = false;
                AudioManager.Instance.PlaySound(Sounds.Plant);

                Vector3 plantPos = (Vector3)storedTilePos +
                                    new Vector3(Random.Range(0.3f, 0.7f), Random.Range(0.3f, 0.7f), 0);

                var flowerInstance = Instantiate(demonFlowerPrefab, plantPos, Quaternion.identity)
                                    .GetComponent<FlowerPlant>();

                flowerInstance.growingOn = storedTilePos.Value;
                TileManager.Instance.SetFlower(storedTilePos.Value, flowerInstance.gameObject);
                flowerInstance.flowerColor = TileManager.Instance.GetFlowerColor(storedTilePos.Value).Value;
                flowerInstance.petalSprite.color = flowerInstance.flowerColor;
            }

            storedTilePos = null;
        }
    }

    protected override bool CheckToolUsage() {
        if (!base.CheckToolUsage()) return false;

        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int tilePos = tilemap.WorldToCell(mousePos);
        TileBase targetedTile = tilemap.GetTile(tilePos);

        return (seedCount > 0 && (targetedTile == wateredTile || targetedTile == blodiedTile)) || (targetedTile == flowerTile || targetedTile == spiritflowerTile);
    }
}
