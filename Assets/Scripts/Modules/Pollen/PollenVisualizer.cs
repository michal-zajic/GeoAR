using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

//Visualizer for pollen on 2D map
public class PollenVisualizer : ModuleMapVisualizer
{
    [SerializeField] Transform _featureParent = null;
    [SerializeField] GameObject _mapRecycleObject = null;

    Vector2d _initialLoc;

    AbstractMap _map;

    float initialScale;

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
        
    IEnumerator DrawCoroutine(List<PollenInfo> pollenInfos) {
        int i = 1;
        foreach (PollenInfo info in pollenInfos) {
            GameObject obj = Instantiate(_mapRecycleObject);
            obj.transform.localScale = new Vector3(10.0f, 10, 10);
            initialScale = obj.transform.localScale.x;
            obj.transform.position = _map.GeoToWorldPosition(new Vector2d(info.coordinates.x, info.coordinates.y));
            obj.transform.position += new Vector3(0, (i * 0.001f) + 0.01f, 0);
            obj.transform.SetParent(_featureParent);
            PollenMapObject pollenMO = obj.GetComponent<PollenMapObject>();
            pollenMO.SetActive(info.grassDanger != PollenInfo.Danger.low, info.treeDanger != PollenInfo.Danger.low, info.weedDanger != PollenInfo.Danger.low);
                        
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
        DestroyObjects();
        PollenDataLoader loader = data as PollenDataLoader;
        List<PollenInfo> infos = loader.pollenInfos;
        PollenInfo centerInfo = loader.NearestInfoTo(map.CenterLatitudeLongitude);
        centerInfo.coordinates = map.CenterLatitudeLongitude;
        StartCoroutine(DrawCoroutine(loader.pollenInfos));
    }

    //pollen sprites are placed under featureParent - transform, which scales with map, the sprites are scaled too so they remain constant size unlike recycle objects
    private void OnMapUpdated() {
        _featureParent.position = _map.GeoToWorldPosition(_initialLoc);
        float scale = _map.transform.localScale.x;
        _featureParent.localScale = new Vector3(scale, scale, scale);
        float size = initialScale / (scale);
        foreach (Transform child in _featureParent.transform) {
            child.localScale = new Vector3(size, size, size);
        }
    }
}
