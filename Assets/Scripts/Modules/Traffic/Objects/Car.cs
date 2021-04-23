using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

//Car object used in AR traffic visualization
public class Car : HideableObject
{
    public delegate void onDestroyDelegate();
    public event onDestroyDelegate onDestroy;

    List<Vector2d> _segmentPoints = new List<Vector2d>();
    int _nextPoint = 1;
    AbstractMap _map;

    float _speed;

    float _carSize = 0.0036f;

    //sets color, position and size
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
        //end of route reached, destroy the car
        if(_nextPoint >= _segmentPoints.Count) {
            Destroy(gameObject);
            return;
        }
        //otherwise move towards next segment point
        Vector3 targetPos = _map.GeoToWorldPosition(_segmentPoints[_nextPoint]);
        transform.LookAt(targetPos);
        Vector3 direction = (targetPos - transform.position).normalized;
        float unitySpeed = _speed * _map.transform.localScale.x * _map.transform.parent.localScale.x / 996 /*real to unity world speed conversion constant*/;
        transform.position += direction * unitySpeed * Time.deltaTime;

        if(Vector3.Distance(targetPos, transform.position) < 0.05f) {
            _nextPoint++;
        }
    }

    //the car needs to be removed from list in SegmentObject, so the car lets it know, that it is about to be destroyed
    //also the global car amout is decreased
    private void OnDestroy() {
        AppState.instance.carsSpawned -= 1;
        if(onDestroy != null)
            onDestroy();
    }
}
