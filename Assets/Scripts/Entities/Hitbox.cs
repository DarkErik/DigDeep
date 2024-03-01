using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{

    private Transform _transform;
    [SerializeField] private float radius = 1f;
    [SerializeField] private Vector3 offset;


    private void Awake() {
        _transform = transform;
    }
    public bool PointInside(Vector3 point) {
        return Vector3.Distance(point, GetCenter()) <= radius;
    }

    public bool Collide(Hitbox hitbox) {
        return Vector3.Distance(hitbox.GetCenter(), GetCenter()) <= radius + hitbox.radius;
    }

    public Vector3 GetCenter() {
        return _transform.position + offset;
    }


    public float GetRadius() {
        return radius;
    }

    public void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position + offset, radius);
    }
}
/// <summary>
/// ASSUMING CELLW AND CELLH ARE BIGGER THAN ANY HITBOX DIAMETER!
/// </summary>
public class SpacialGrouping {
    public static SpacialGrouping currentGrouping = null;

    private int cellSize;
    private int gridW, gridH;

    private LinkedList<Hitbox>[,] buckets;
    public SpacialGrouping(int cellSize, LevelDataObject lvlData) {
        this.cellSize = cellSize;

        gridW = Mathf.CeilToInt(lvlData.width / (float)cellSize);
        gridH = Mathf.CeilToInt(lvlData.height/ (float)cellSize);

        buckets = new LinkedList<Hitbox>[gridW, gridH];
        for (int ix = 0; ix < gridW; ix++) {
            for (int iy = 0; iy < gridH; iy++) {
                buckets[ix, iy] = new LinkedList<Hitbox>();
            }
        }
    }

    public (int, int) GetBucket(Hitbox hitbox) {
        Vector3 center = hitbox.GetCenter();
        return ((int)(Mathf.Clamp(center.x, 0, cellSize * (gridW - 1)) / cellSize), (int)(Mathf.Clamp(center.y, 0, cellSize * (gridH - 1)) / cellSize));
    }

    public (int, int) GetBucket(Vector3 point) {
        return ((int)(Mathf.Clamp(point.x, 0, cellSize * (gridW - 1)) / cellSize), (int)(Mathf.Clamp(point.y, 0, cellSize * (gridH - 1)) / cellSize));
    }

    public (int bucketX, int bucketY) Add(Hitbox hitbox) {
        (int bucketX, int bucketY) = GetBucket(hitbox);

        //Debug.Log($"UPDATE TO: ({bucketX}, {bucketY})");
        buckets[bucketX, bucketY].AddLast(hitbox);
        return (bucketX, bucketY);
    }

    public void Remove(Hitbox hitbox) {
        (int bucketX, int bucketY) = GetBucket(hitbox);
        buckets[bucketX, bucketY].Remove(hitbox);
    }

    public void Remove(Hitbox hitbox, (int bucketX, int bucketY) data) {
        buckets[data.bucketX, data.bucketY].Remove(hitbox);
    }

    public LinkedList<Hitbox> CollisionWithAll(Hitbox hitbox) {
        LinkedList<Hitbox> res = new LinkedList<Hitbox>();
        (int bucketX, int bucketY) = GetBucket(hitbox);
        var buckets = GetNeigbourBuckets(bucketX, bucketY, 1 + (int)(hitbox.GetRadius() / cellSize));
        foreach (var bucket in buckets) {
            foreach (Hitbox h in bucket) {
                if (h.Collide(hitbox)) {
                    res.AddLast(h);
                }
            }
        }

        return res;
    }

    public Hitbox GetClosestHitbox(Vector3 point, float maxRadius) {

        Hitbox bestHitbox = null;
        float distance = maxRadius;


        (int x, int y) bucketData = GetBucket(point);
        for(int i = 0; i < Mathf.CeilToInt(maxRadius / (cellSize / 2f)); i++) {
            LinkedList<LinkedList<Hitbox>> allBuckets = new LinkedList<LinkedList<Hitbox>>();
            
            if (i == 0) {
                allBuckets.AddLast(buckets[bucketData.x, bucketData.y]);
            } else {
                //Select Only Buckets in Range i (2, 3, 4, 5, ...)
                int maxIX = Mathf.Min(gridW, bucketData.x + i);
                int maxIY = Mathf.Min(gridH, bucketData.y + i);
                int minIY = Mathf.Max(0, bucketData.y - i);
                int minIX = Mathf.Max(0, bucketData.x - i);
                //TOP&BOT
                for (int ix = minIX; ix < maxIX; ix++) {
                    allBuckets.AddLast(buckets[ix, minIY]);
                    //Debug.Log($"buckets[ix, maxIY] {ix}, {maxIY}");
                    allBuckets.AddLast(buckets[ix, maxIY]);
                }
                //LEFT&RIGHT
                for (int iy = minIY + 1; iy < maxIY - 1; iy++) {
                    allBuckets.AddLast(buckets[minIX, iy]);
                    allBuckets.AddLast(buckets[maxIX, iy]);
                }
            }

            foreach (var bucket in allBuckets) {
                foreach (Hitbox hitbox in bucket) {
                    float currentDistance = (hitbox.GetCenter() - point).magnitude - hitbox.GetRadius();
                    if (currentDistance < distance) {
                        distance = currentDistance;
                        bestHitbox = hitbox;
                    }
                }
            }
            if (bestHitbox != null) break;
        }
        return bestHitbox;
    }


    public Hitbox CollisionWith(Hitbox hitbox) {
        (int bucketX, int bucketY) = GetBucket(hitbox);
        var bucketsList = GetNeigbourBuckets(bucketX, bucketY, 1 + (int)(hitbox.GetRadius() / cellSize));
        foreach (var bucket in bucketsList) {
            foreach (Hitbox h in bucket) {
                if (h.Collide(hitbox)) {
                    return h;
                }
            }
        }
        return null;
    }

    private LinkedList<LinkedList<Hitbox>> GetNeigbourBuckets(int x, int y, int range) {
        LinkedList<LinkedList<Hitbox>> res = new LinkedList<LinkedList<Hitbox>>();

        int maxIX = Mathf.Min(gridW, x + range);
        int maxIY = Mathf.Min(gridH, y + range);
        int minIY = Mathf.Max(0, y - range);
        for(int ix = Mathf.Max(0, x - range); ix < maxIX; ix++) {
            for(int iy = minIY; iy < maxIY; iy ++) {
                res.AddLast(buckets[ix, iy]);
            }
        }
        return res;
    }

    public (int bucketX, int bucketY) UpdateHitboxBucket((int bucketX, int bucketY) oldData, Hitbox hitbox) {
        (int bucketX, int bucketY) = GetBucket(hitbox);
        if (oldData.bucketX != bucketX || oldData.bucketY != bucketY) {
            Remove(hitbox, oldData);
            //Debug.Log($"UPDATE TO: ({data.bucketX}, {data.bucketY})");
            return Add(hitbox);
        }
        return oldData;
    }

    public void DrawGizmos() {
        for (int ix = 0; ix < gridW; ix++) {
            for (int iy = 0; iy < gridH; iy++) {
                Gizmos.color = new Color(Mathf.Sin(ix) * .5f + 0.5f, Mathf.Cos(iy) * .5f + 0.5f, ((ix + iy) / 2) / ((gridH + gridW) * .5f));
                Gizmos.DrawWireCube(new Vector3(ix + .5f, iy + .5f) * cellSize, new Vector3(cellSize, cellSize, .1f) * 0.95f);
                foreach (Hitbox hb in buckets[ix, iy]) {
                    hb.OnDrawGizmosSelected();
                }
            }
        }
    }
}
