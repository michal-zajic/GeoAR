using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

public class RecycleARVisualizer : ModuleARVisualizer
{
    List<GameObject> objects;
    public override void Disable() {
        DestroyObjects();
    }

    public override void Enable() {
        throw new System.NotImplementedException();
    }

    public override void Prepare(ModuleDataLoader data, AbstractMap map) {
        DestroyObjects();
        objects = new List<GameObject>();
        RecycleDataLoader loader = data as RecycleDataLoader;
        foreach(Container container in loader.containers) {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obj.transform.localScale = 0.05f * map.transform.localScale;
            obj.transform.position = map.GeoToWorldPosition(new Vector2d(container.coordinates.y, container.coordinates.x));
            obj.transform.SetParent(map.transform);
            obj.tag = "ModuleObject";
            objects.Add(obj);
        }
    }

    private void DestroyObjects() {
        if (objects != null && objects.Count >= 1) {
            for (int i = objects.Count - 1; i >= 0; i--) {
                Destroy(objects[i]);
                objects.RemoveAt(i);
            }
        }
    }
}
