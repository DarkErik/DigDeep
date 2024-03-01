using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicGeneration : MonoBehaviour {
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private float goldSpawnPercent = 0.01f;
    [SerializeField] private float scale = 0.1f;
    [SerializeField] private float wallThreshhold = 0.5f;

    public void Start() {
        Apply(Generate());
    }

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Apply(Generate());
        }
    }
    public LevelDataObject Generate() {
        LevelDataObject levelData = new LevelDataObject(width, height);
        
        float noiseOffsetX = Random.Range(0.0f, 1000f);
        float noiseOffsetY = Random.Range(0.0f, 1000f);

        for (int ix = 0; ix < width; ix++) {
            for (int iy = 0; iy < height; iy++) {
                float noiseValue = Mathf.PerlinNoise(ix * scale + noiseOffsetX, iy * scale + noiseOffsetY);

                if (noiseValue > wallThreshhold) {
                    levelData.tiles[ix, iy] = TileType.WALL;
                } else {
                    levelData.tiles[ix, iy] = TileType.GROUND;
                }
            }
        }

        int goldSpawns = (int)(width * height * goldSpawnPercent);

        while (goldSpawns > 0) {
            int spawns = Random.Range(1, Mathf.Min(3, goldSpawns));
            List <Vector2Int> pos = GetRandomPositonsGoop(GetRandomWallPosition(levelData), spawns, levelData);
            foreach (Vector2Int position in pos) {
                levelData.tiles[position.x, position.y] = TileType.GOLD;
            }
            goldSpawns -= spawns;
        }

        return levelData;
    }

    private Vector2Int GetRandomWallPosition(LevelDataObject lvlData) {
        Vector2Int pos;
        do {
            pos = new Vector2Int(Random.Range(0, lvlData.width), Random.Range(0, lvlData.height));
        } while (lvlData.tiles[pos.x, pos.y] != TileType.WALL);
        return pos;
    }

    private List<Vector2Int> GetRandomPositonsGoop(Vector2Int start, int number, LevelDataObject lvlData) {
        List<Vector2Int> positions = new List<Vector2Int>();
        const int maxTrys = 100;
        positions.Add(start);
        for(int i = 0; i < maxTrys; i++) {
            Vector2Int basePos = positions[Random.Range(0, positions.Count)];
            switch(Random.Range(0, 4)) {
                case 0:
                    basePos += Vector2Int.right;
                    break;
                case 1:
                    basePos += Vector2Int.down;
                    break;
                case 2:
                    basePos += Vector2Int.left;
                    break;
                case 3:
                    basePos += Vector2Int.up;
                    break;
            }
            if (!lvlData.IsInBounds(basePos) || positions.Contains(basePos) || lvlData.tiles[basePos.x, basePos.y] != TileType.WALL)
                continue;
            positions.Add(basePos);
            if (positions.Count >= number) {
                return positions;
            }
        }
        return positions;
    }
    public void Apply(LevelDataObject levelData) {
        TileMapManager.Instance.Apply(levelData);
        SpacialGrouping.currentGrouping = new SpacialGrouping(5, levelData);
    }

}

public class LevelDataObject {
    public int width, height;
    public TileType[,] tiles;
    public float[,] wallDamageData;
    public SpriteRenderer[,] wallDamageRenderer;

    public LevelDataObject(int width, int height) {
        this.width = width;
        this.height = height;
        tiles = new TileType[width, height];
        wallDamageData = new float[width, height];
        wallDamageRenderer = new SpriteRenderer[width, height];
    }

    public bool IsInBounds(Vector3 xy) {
        return xy.x >= 0 && xy.y >= 0 && xy.x < width && xy.y < height;   
    }

    public bool IsInBounds(Vector2Int xy) {
        return xy.x >= 0 && xy.y >= 0 && xy.x < width && xy.y < height;
    }

    public bool IsInBounds(int x, int y) {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    public bool IsTile(Vector3 xy, TileType type) {
        return IsInBounds(xy) && tiles[Mathf.FloorToInt(xy.x), Mathf.FloorToInt(xy.y)] == type;
    }

}

public enum TileType { GROUND, WALL, GOLD }

public static class TileTypeExtension {
    public static bool IsWallTile(this TileType tileType) {
        return tileType != TileType.GROUND;
    }

    public static void GetRessourceFromBreakingTile(this TileType tileType) {
        switch (tileType) {
            case TileType.GOLD:
                GoldUI.Instance.Gold++;
                break;
        }
    }
}