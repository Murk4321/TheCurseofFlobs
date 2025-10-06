using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class FlobSpawner : MonoBehaviour {
    [SerializeField] private GameObject flobPrefab;
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase grassTile;
    [SerializeField] private int maxFlobs = 15;
    [SerializeField] private float spawnInterval = 5f;

    public List<GameObject> spawnedFlobs = new List<GameObject>();
    private Camera cam;

    private bool spawnStarted;

    private void Start() {
        cam = Camera.main;
        
    }

    private void Update() {
        if (PreGameManager.Instance != null) {
            if (!PreGameManager.Instance.gameStarted) return;
        }
        if (!spawnStarted) {
            InvokeRepeating(nameof(SpawnFlob), spawnInterval, spawnInterval);
            spawnStarted = true;
        }
    }

    private void SpawnFlob() {
        if (spawnedFlobs.Count >= maxFlobs) return;

        BoundsInt bounds = tilemap.cellBounds;
        for (int attempt = 0; attempt < 20; attempt++) { 
            Vector3Int pos = new Vector3Int(
                Random.Range(bounds.xMin, bounds.xMax),
                Random.Range(bounds.yMin, bounds.yMax),
                0
            );

            TileBase tile = tilemap.GetTile(pos);
            if (tile == grassTile) {
                Vector3 worldPos = tilemap.CellToWorld(pos) + new Vector3(0.5f, 0.5f, 0f);

                if (!IsVisibleToCamera(worldPos)) {
                    GameObject flob = Instantiate(flobPrefab, worldPos, Quaternion.identity);
                    spawnedFlobs.Add(flob);

                    flob.GetComponent<FlobCitizen>().SetState(FlobCitizen.State.Passive);
                    return;
                }
            }
        }
    }

    private bool IsVisibleToCamera(Vector3 worldPos) {
        Vector3 viewportPos = cam.WorldToViewportPoint(worldPos);
        return viewportPos.x >= 0 && viewportPos.x <= 1 &&
               viewportPos.y >= 0 && viewportPos.y <= 1 &&
               viewportPos.z > 0;
    }
}
