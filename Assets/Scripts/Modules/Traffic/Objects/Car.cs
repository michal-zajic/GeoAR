using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

public class Car : HideableObject
{
    public delegate void onDestroyDelegate();
    public event onDestroyDelegate onDestroy;

    List<Vector2d> _segmentPoints = new List<Vector2d>();
    int _nextPoint = 1;
    AbstractMap _map;

    float _speed;

    float _carSize = 0.0036f;

    public void Init(List<Vector2d> segmentPoints, AbstractMap map, float speed, float jamFactor) {
        _segmentPoints = segmentPoints;
        _map = map;
        _speed = speed;
        renderer.material.color = TrafficSegment.GetColorFromJamFactor(jamFactor);
        transform.position = _map.GeoToWorldPosition(_segmentPoints[0]);
        transform.localScale = new Vector3(_carSize, _carSize, _carSize);
        _nextPoint = 1;
    }


    void Update()
    {
        if(_nextPoint >= _segmentPoints.Count) {
            Destroy(gameObject);
            return;
        }

        Vector3 targetPos = _map.GeoToWorldPosition(_segmentPoints[_nextPoint]);
        transform.LookAt(targetPos);
        Vector3 direction = (targetPos - transform.position).normalized;
        float unitySpeed = _speed * _map.transform.localScale.x * _map.transform.parent.localScale.x / 996 /*real to unity world speed conversion constant*/;
        transform.position += direction * unitySpeed * Time.deltaTime;

        if(Vector3.Distance(targetPos, transform.position) < 0.05f) {
            _nextPoint++;
        }
    }

    private void OnDestroy() {
        onDestroy();
    }
}
