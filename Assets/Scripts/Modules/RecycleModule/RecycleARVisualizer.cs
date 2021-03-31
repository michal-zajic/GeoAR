using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

public class RecycleARVisualizer : ModuleARVisualizer
{
    [SerializeField] GameObject _containerObject = null;

    public override void Disable() {
        objects.ForEach(obj => {
            obj.GetComponent<ContainerObject>().SetActive(false);
        });
    }

    public override void Enable() {
        objects.ForEach(obj => {
            obj.GetComponent<ContainerObject>().SetActive(true);
        });
    }

    public override void Draw(ModuleDataLoader data, AbstractMap map) {
        DestroyObjects();
        objects = new List<GameObject>();
        RecycleDataLoader loader = data as RecycleDataLoader;
        foreach(Container container in loader.arContainers) {
            GameObject obj = Instantiate(_containerObject);
            obj.transform.localScale = 0.03f * map.transform.localScale;
            obj.transform.position = map.GeoToWorldPosition(new Vector2d(container.coordinates.y, container.coordinates.x));
            Vector3 pos = obj.transform.position;
            obj.transform.position = new Vector3(pos.x, pos.y + obj.transform.localScale.y / 2, pos.z);
            obj.transform.SetParent(map.transform);
            obj.GetComponent<ContainerObject>().Init(container);
            objects.Add(obj);
        }
    }
}
