using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapManager : MonoBehaviour
{
    public static TileMapManager Instance { get; private set; }

    [SerializeField] private Tilemap ground;
    [SerializeField] private Tilemap walls;

    [SerializeField] private TileBase groundTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase goldTile;

    [SerializeField] private Sprite[] crackingSprites;


    public LevelDataObject LevelData { get; private set; }
    

    public void Awake() {
        Instance = this;
    }

    public void Apply(LevelDataObject data) {
        ground.ClearAllTiles();
        walls.ClearAllTiles();

        LevelData = data;

        for (int ix = 0; ix < data.width; ix++) {
            for (int iy = 0; iy < data.height; iy++) {
                Vector3Int pos = new Vector3Int(ix, iy, 0);
                ground.SetTile(pos, groundTile);
                if (data.tiles[ix, iy] == TileType.WALL)
                    walls.SetTile(pos, wallTile);
                if (data.tiles[ix, iy] == TileType.GOLD)
                    walls.SetTile(pos, goldTile);
            }
        }

        ground.CompressBounds();
        walls.CompressBounds();

    }

   

    public LinkedList<Vector2Int> GetWallTilesInCircle(Vector2 center, float radius) {
        LinkedList<Vector2Int> results = new LinkedList<Vector2Int> ();

        float radiusSq = radius * radius;

        for(int ix = Mathf.Max(Mathf.FloorToInt(center.x - radius), 0); ix < Mathf.Min(Mathf.CeilToInt(center.x + radius), LevelData.width); ix++) {
            for (int iy = Mathf.Max(Mathf.FloorToInt(center.y - radius), 0); iy < Mathf.Min(Mathf.CeilToInt(center.y + radius), LevelData.height); iy++) {
                if (LevelData.tiles[ix, iy].IsWallTile() && (new Vector2(ix + .5f, iy + .5f) - center).sqrMagnitude < radiusSq) {
                    results.AddLast(new Vector2Int(ix, iy));
                }
            }
        }

        //Debug.Log($"Found {results.Count} in ({Mathf.Max(Mathf.FloorToInt(center.x - radius), 0)}, {Mathf.Max(Mathf.FloorToInt(center.y - radius), 0)}) To ({Mathf.Min(Mathf.CeilToInt(center.x + radius), levelData.width)}, {Mathf.Min(Mathf.CeilToInt(center.y + radius), levelData.height)})");
        return results;
    }

    public void Drill(Vector2 center, float radius, float damage) {
        LinkedList<Vector2Int> drillingPositions = GetWallTilesInCircle(center, radius);
        foreach (Vector2Int pos in drillingPositions) {
            if (LevelData.wallDamageData[pos.x, pos.y] == 0) {
                LevelData.wallDamageRenderer[pos.x, pos.y] = CreateCrackingAnimation(new Vector3(pos.x + .5f, pos.y + .5f));
            }
            LevelData.wallDamageData[pos.x, pos.y] += damage;

            if (LevelData.wallDamageData[pos.x, pos.y] >= 1) {
                BreakWallTile(pos.x, pos.y);
            } else
                LevelData.wallDamageRenderer[pos.x, pos.y].sprite = crackingSprites[(int)(LevelData.wallDamageData[pos.x, pos.y] * crackingSprites.Length)];
        }
    }

    public void BreakWallTile(int x, int y) {
        CreateBreakingVFX(x, y);
        LevelData.tiles[x, y].GetRessourceFromBreakingTile();


        walls.SetTile(new Vector3Int(x, y, 0), null);
        LevelData.tiles[x, y] = TileType.GROUND;
        Destroy(LevelData.wallDamageRenderer[x, y].gameObject);
        LevelData.wallDamageRenderer[x, y] = null;
    }

    private void CreateBreakingVFX(int x, int y) {
        GameObject prefab;
        switch (LevelData.tiles[x, y]) {
            case TileType.GOLD:
                prefab = Factory.Instance.goldDebritsParticleSystemPrefab;
                break;
            default:
                prefab = Factory.Instance.debritsParticleSystemPrefab;
                break;
        }

        Instantiate(prefab, new Vector3(x + .5f, y + .5f), Quaternion.identity);
    }

    private SpriteRenderer CreateCrackingAnimation(Vector3 pos) {
        GameObject crack = new GameObject("Cracking Anim");
        crack.transform.position = pos;
        crack.transform.parent = walls.transform;
        SpriteRenderer renderer = crack.AddComponent<SpriteRenderer>();
        renderer.sortingOrder = 2;
        renderer.sprite = crackingSprites[0];

        return renderer;
    }
}
