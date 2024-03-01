using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    public Camera cam;

    public void Awake() {
        Instance = this;
    }

    public void Update() {
        float ratio = 16 / 9f;
        this.transform.position = new Vector3(
            Mathf.Clamp(PlayerMovement.Instance.transform.position.x, 0 + cam.orthographicSize * ratio, TileMapManager.Instance.LevelData.width - cam.orthographicSize * ratio),
            Mathf.Clamp(PlayerMovement.Instance.transform.position.y, 0 + cam.orthographicSize, TileMapManager.Instance.LevelData.height - cam.orthographicSize), -10);
    }
}
