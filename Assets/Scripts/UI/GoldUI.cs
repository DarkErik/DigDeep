using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    public static GoldUI Instance { get; private set; }
    [HideInInspector] public int Gold {
        get {
            return _gold;
        }
        set {
            _gold = value;
            number.SetText(""+_gold);
        }
    }
    private int _gold;
    [SerializeField] private TMPro.TextMeshProUGUI number;

    public void Awake() {
        Instance = this;
    }


}
