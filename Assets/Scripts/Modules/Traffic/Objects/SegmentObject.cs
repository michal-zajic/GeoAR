using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

public class SegmentObject : MonoBehaviour
{
    [SerializeField] GameObject _carObject = null;

    List<Car> _cars = new List<Car>();
    List<Vector2d> _segmentPoints = new List<Vector2d>();
    AbstractMap _map;
    int _carLimit;

    float _jamFactor;
    float _speed;

    public void Init(List<Vector2d> segmentPoints, AbstractMap map, float jamFactor, float speed, int carLimit) {
        _map = map;
        _segmentPoints = segmentPoints;
        _jamFactor = jamFactor;
        _speed = speed;
        _carLimit = carLimit;
        StopAllCoroutines();
        StartCoroutine(SpawnCars());
    }

    public void OnDestroy() {
        for(int i = _cars.Count - 1; i >= 0; i--) {
            Destroy(_cars[i].gameObject);
        }
    }

    public void SetActive(bool active) {
        StopAllCoroutines();
        if (active) {
            StartCoroutine(SpawnCars());
        }
        _cars.ForEach(car => { car.SetActive(active); });
    }

    IEnumerator SpawnCars() {
        while (true) {
            if (_cars.Count < _carLimit) {
                GameObject carObj = Instantiate(_carObject, _map.transform);
                Car car = carObj.GetComponent<Car>();
                car.Init(_segmentPoints, _map, _speed, _jamFactor);
                car.onDestroy += () => { _cars.Remove(car); };
                _cars.Add(car);
            }
            //the slower the cars go, the slower the spawner should spawn, so the cars arent overlapping too much
            yield return new WaitForSeconds(Random.Range(3, 8) + _jamFactor / 2 );  
        }
    }
}
