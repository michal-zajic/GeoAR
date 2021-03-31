using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

public abstract class ModuleVisualizer : MonoBehaviour
{
    protected List<GameObject> objects = new List<GameObject>();
    [HideInInspector] public Vector2d lastCoord;
    public abstract void Draw(ModuleDataLoader data, AbstractMap map);
    public abstract void Enable();
    public abstract void Disable();

    protected void DestroyObjects() {
        if (objects.Count >= 1) {
            for (int i = objects.Count - 1; i >= 0; i--) {
                Destroy(objects[i]);
                objects.RemoveAt(i);
            }
        }
    }
}
