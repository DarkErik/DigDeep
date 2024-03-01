using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util
{
    public static T TryXTimes<T>(int maxTrys, System.Func<T> doFunc, System.Func<T, bool> whileFunc) {
        for (int i = 0; i < maxTrys; i++) {
            T t = doFunc();
            if (!whileFunc(t)) return t;
        }
        throw new GenerationFailedException();
    }
}

public class DoEveryXFrame {
    private int x;
    private int currentFrame;
    private System.Action action;
    
    public DoEveryXFrame(int x, System.Action action) {
        this.x = x;
        this.currentFrame = x;

        this.action = action;
    }

    public bool UpdateFrame() {
        if (++currentFrame >= x) {
            action();
            return true;
        }
        return false;
    }
}

[System.Serializable]
public class Generator {
    public int amount;
    public float cooldown;

    private float currentCooldown;

    public Generator(int amount, float cooldown) {
        this.amount = amount;
        this.cooldown = cooldown;
    }

    public int Work(float deltaTime) {
        currentCooldown += deltaTime;
        if (currentCooldown > cooldown) {
            currentCooldown = 0;
            return amount;
        }
        return 0;
    }
}

public class GenerationFailedException : System.Exception {
}



