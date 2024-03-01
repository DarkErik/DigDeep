using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Factory : MonoBehaviour
{
    public static Factory Instance { get; private set; }

    public LayerMask walls;


    public GameObject debritsParticleSystemPrefab;
    public GameObject goldDebritsParticleSystemPrefab;

    private void Awake() {
        Instance = this;
    }

    private void OnDrawGizmosSelected() {
        if (SpacialGrouping.currentGrouping == null || !Application.isPlaying) {
            return;
        } else {
            SpacialGrouping.currentGrouping.DrawGizmos();
        }

        
    }
}
