using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapTab : TabView {
    [SerializeField] AbstractMap _map = null;
    [SerializeField] DeviceLocationProvider _locationProvider = null;
    [SerializeField] Image _userLocationImage = null;
    [SerializeField] RectTransform _marker = null;
    [SerializeField] Button _locationButton = null;

    bool _firstUpdate = false;
    bool _pressedDown = false;

    float _requiredLongPressTime = 0.7f;
    float _longPressTime = 0;

    Vector2 _pressStartLocation;
    Vector2 _pressLastLocation;

    Vector2d _corner1 = new Vector2d(48.5476636, 11.6568511);
    Vector2d _corner2 = new Vector2d(51.2190594, 19.0396636);
    Vector2d _markerPosition;

    
    // Start is called before the first frame update
    void Start()
    {
        _map.OnUpdated += OnMapUpdated;
        _marker.gameObject.SetActive(false);
        _locationButton.onClick.AddListener(CenterOnUserLocation);
        SendLocationToState(_map.CenterLatitudeLongitude);
    }

    protected override void OnTabSelection() {
        base.OnTabSelection();
        ResetLongPress();

        ImagerySourceType type = (bool)Settings.instance.GetValue(Settings.Setting.useSatellite) ? ImagerySourceType.MapboxSatelliteStreet : ImagerySourceType.MapboxStreets;
        _map.ImageLayer.SetProperties(type, true, false, true);
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
        if (!_firstUpdate && latlon.x > _corner1.x && latlon.x < _corner2.x && latlon.y > _corner1.y && latlon.y < _corner2.y) {
            _firstUpdate = true;
            CenterOnUserLocation();
        }
        if(!_userLocationImage.gameObject.activeInHierarchy)
            _userLocationImage.gameObject.SetActive(true);

        Vector2 screenPos = GeoToScreenPoint(latlon);
        _userLocationImage.rectTransform.position = screenPos;


    }

    void UpdateMarkerLocationIcon() {
        if (_marker.gameObject.activeInHierarchy)
            _marker.position = GeoToScreenPoint(_markerPosition) + new Vector2(0, 30);
    }

    void ProcessLongPress() {
        if(Vector2.Distance(_pressStartLocation, _pressLastLocation) < 10) {
            if (!_marker.gameObject.activeInHierarchy)
                _marker.gameObject.SetActive(true);
            _marker.position = _pressLastLocation + new Vector2(0, 30);
            _markerPosition = ScreenToGeoPoint(_pressLastLocation);
            AppState.instance.markerPlaced = true;
            AppState.instance.UpdateMarkerLocation(_markerPosition);
            PerformHapticFeedback();
        }

        ResetLongPress();
    }

    void PerformHapticFeedback() {
        if (Application.platform == RuntimePlatform.IPhonePlayer && !SystemInfo.deviceModel.StartsWith("iPad")) {
            IOSNative.StartHapticFeedback(HapticFeedbackTypes.LIGHT);
        }
    }

    void ResetLongPress() {
        _pressedDown = false;
        _longPressTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUserLocationIcon(_locationProvider.CurrentLocation);
        UpdateMarkerLocationIcon();

        InputCheck();

        if(_pressedDown) {
            _longPressTime += Time.deltaTime;
            if(_longPressTime >= _requiredLongPressTime) {
                ProcessLongPress();
            }
        }
    }

    void InputCheck() {
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;
        }
        foreach (var touch in Input.touches) {
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId)) {
                return;
            }
        }
        if (Input.GetMouseButtonDown(0)) {
            OnPointerDown(Input.mousePosition);
        } else if (Input.GetMouseButtonUp(0)) {
            OnPointerUp(Input.mousePosition);
        } else if (Input.GetMouseButton(0)) {
            OnDrag(Input.mousePosition);
        }
    }

    void OnPointerUp(Vector2 position) {
        if(_pressedDown && _longPressTime < Time.deltaTime * 5) {
            _marker.gameObject.SetActive(false);
            AppState.instance.markerPlaced = false;            
        }
        ResetLongPress();
    }

    void OnPointerDown(Vector2 position) {
        _pressedDown = true;
        _pressStartLocation = position;
        _pressLastLocation = _pressStartLocation;
    }

    void OnDrag(Vector2 position) {
        _pressLastLocation = position;
    }
}
