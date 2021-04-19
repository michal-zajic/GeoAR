using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

public class TrafficMapVisualizer : ModuleMapVisualizer
{
    [SerializeField] Transform _featureParent = null;
    AbstractMap _map = null;

    Vector2d _initialLoc;

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

    public override void Draw(ModuleDataLoader data, AbstractMap map) {
        if (_map == null) {
            _map = map;
            _initialLoc = _map.WorldToGeoPosition(_featureParent.position);
            OnMapUpdated();
            _map.OnUpdated += OnMapUpdated;
        }        
        TrafficDataLoader loader = data as TrafficDataLoader;
        StopAllCoroutines();
        DestroyObjects();
        StartCoroutine(DrawLines(loader.mapSegments));
    }

    //uses Unity's native Line Renderer, allowes color change through material and size change
    //gps segment points are converted to unity space and passed to line renderer
    IEnumerator DrawLines(List<TrafficSegment> segments) {
        int i = 0;
        foreach(TrafficSegment segment in segments) {
            GameObject obj = new GameObject("line");
            obj.transform.SetParent(_featureParent);
            LineRenderer renderer = obj.AddComponent<LineRenderer>();
            renderer.startWidth = 1.000f;
            renderer.endWidth = 1.000f;
            renderer.positionCount = segment.coordinateList.Count;
            renderer.useWorldSpace = false;
            Material material = new Material(Shader.Find("Unlit/Color"));
            material.color = TrafficSegment.GetColorFromJamFactor(segment.jamFactor);
            renderer.material = material;

            var positions = segment.coordinateList.Select(coord => {                
                return _map.GeoToWorldPosition(coord) + new Vector3(0, 0.05f, 0);
            }).ToArray();

            renderer.SetPositions(positions);
            objects.Add(obj);
            i++;
            if(i%10 == 0) //freeze prevention
                yield return null;
        }

        yield return null;
    }

    //lines are scaled with map
    private void OnMapUpdated() {
        _featureParent.position = _map.GeoToWorldPosition(_initialLoc);
        float scale = _map.transform.localScale.x * _map.transform.parent.localScale.x;
        _featureParent.localScale = new Vector3(scale, scale, scale);
    }

}
