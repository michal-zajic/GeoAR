using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

//Visualizer for recycle module on 2D map
public class RecycleVisualizer : ModuleMapVisualizer
{
    [SerializeField] Transform _featureParent = null;
    [SerializeField] GameObject _mapRecycleObject = null;

    Vector2d _initialLoc;

    AbstractMap _map;

    public override void Disable() {
        objects.ForEach(obj => {
            obj.SetActive(false);
        });
    }

    public override void Enable() {
        objects.ForEach(obj => {
            obj.SetActive(true);
        });
    }

    //instantiates recycle map objects on its coordinates
    IEnumerator DrawCoroutine(List<Container> containers) {
        DestroyObjects();
        int i = 1;
        foreach (Container container in containers) {
            GameObject obj = Instantiate(_mapRecycleObject);
            obj.transform.localScale = 0.03f * _map.transform.localScale.x * _map.transform.parent.localScale;
            obj.transform.position = _map.GeoToWorldPosition(new Vector2d(container.coordinates.y, container.coordinates.x));
            //z(y)-fighting prevention
            obj.transform.position += new Vector3(0, (i * 0.001f) + 0.01f, 0);
            obj.transform.SetParent(_featureParent);

            var colors = container.trashTypes.Select(type => {
                return Container.GetColorFromTrashType(type);
            }).Distinct().ToList();
            var accessColor = Container.GetColorFromAccessibility(container.accessibility);
            obj.GetComponent<PieChart>().Init(colors, accessColor, true);
            objects.Add(obj);

            i++;
        }
        yield return null;
    }

    public override void Draw(ModuleDataLoader data, AbstractMap map) {
        if (_map == null) {
            _map = map;
            _initialLoc = _map.WorldToGeoPosition(_featureParent.position);
            OnMapUpdated();
            _map.OnUpdated += OnMapUpdated;
        }
        RecycleDataLoader loader = data as RecycleDataLoader;
        StartCoroutine(DrawCoroutine(loader.containers));
    }

    //similar to pollen visualizer, feature parent is scaled based on map, children here are not scaled back, so they get bigger with map zoom
    private void OnMapUpdated() {
        _featureParent.position = _map.GeoToWorldPosition(_initialLoc);
        float scale = _map.transform.localScale.x * _map.transform.parent.localScale.x;
        _featureParent.localScale = new Vector3(scale, scale, scale);
    }   
}
