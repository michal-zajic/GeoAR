using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

public abstract class ModuleDataLoader : MonoBehaviour {
    protected Vector2d location;
    protected float range;
    protected bool ar;

    public virtual void Init(AbstractMap map, bool ar = false) {
        this.ar = ar;
        range = ar ? 580 : 1000;
        location = map.CenterLatitudeLongitude;
    }

    public abstract void GetData(Action onFinish = null);
}
