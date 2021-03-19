using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;
using UnityEngine.UI;

public class MapTab : TabView
{
    [SerializeField] AbstractMap _map = null;
    [SerializeField] DeviceLocationProvider _locationProvider = null;
    [SerializeField] Image _userLocationImage = null;
    [SerializeField] RectTransform _marker = null;
    [SerializeField] Button _locationButton = null;

    bool _firstUpdate = false;

    Vector2d corner1 = new Vector2d(48.5476636, 11.6568511);
    Vector2d corner2 = new Vector2d(51.2190594, 19.0396636);
    
    // Start is called before the first frame update
    void Start()
    {
        _map.OnUpdated += OnMapUpdated;
        _locationButton.onClick.AddListener(CenterOnUserLocation);
        SendLocationToState(_map.CenterLatitudeLongitude);
    }

    protected override void OnTabSelection() {
        base.OnTabSelection();
    }

    void CenterOnUserLocation() {
        Vector2d loc = _locationProvider.CurrentLocation.LatitudeLongitude;
        _map.SetCenterLatitudeLongitude(loc);
        OnMapUpdated();
        _map.SetZoom(15);
        _map.UpdateMap();
    }

    void SendLocationToState(Vector2d location) {
        AppState.instance.UpdateMapCenter(location);
    }

    void OnMapUpdated() {
        SendLocationToState(_map.CenterLatitudeLongitude);
    }

    Vector2d ScreenToGeoPoint(Vector2 screenPos) {
        Vector3 position = camera.ScreenToWorldPoint(screenPos);
        return _map.WorldToGeoPosition(position);
    }

    Vector2 GeoToScreenPoint(Vector2d latlon) {
        Vector3 position = _map.GeoToWorldPosition(latlon);
        return camera.WorldToScreenPoint(position);        
    }

    void UpdateUserLocationIcon(Location location) {
        if (!location.IsLocationServiceEnabled || location.IsLocationServiceInitializing) {
            _userLocationImage.gameObject.SetActive(false);
            return;
        }
        Vector2d latlon = location.LatitudeLongitude;
        if (!_firstUpdate && latlon.x > corner1.x && latlon.x < corner2.x && latlon.y > corner1.y && latlon.y < corner2.y) {
            _firstUpdate = true;
            CenterOnUserLocation();
        }
        if(!_userLocationImage.gameObject.activeInHierarchy)
            _userLocationImage.gameObject.SetActive(true);

        Vector2 screenPos = GeoToScreenPoint(latlon);
        _userLocationImage.rectTransform.position = screenPos;


    }

    // Update is called once per frame
    void Update()
    {
        UpdateUserLocationIcon(_locationProvider.CurrentLocation);
    }
}
