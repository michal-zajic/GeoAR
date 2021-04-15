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
    [SerializeField] Button _updateButton = null;
    [SerializeField] Button _onboardingButton = null;

    bool _firstUpdate = false;
    bool _pressedDown = false;

    float _requiredLongPressTime = 0.7f;
    float _longPressTime = 0;

    Vector2 _pressStartLocation;
    Vector2 _pressLastLocation;

    Vector2d _corner1 = new Vector2d(48.5476636, 11.6568511);
    Vector2d _corner2 = new Vector2d(51.2190594, 19.0396636);
    Vector2d _markerPosition;

    void Start()
    {
        _map.OnUpdated += OnMapUpdated;
        _marker.gameObject.SetActive(false);
        _locationButton.onClick.AddListener(CenterOnUserLocation);
        _updateButton.onClick.AddListener(() => {
            AppState.instance.allowMapConnectionAlert = true;
            Finder.instance.moduleMgr.VisualizeOnMap(_map);
        });
        _onboardingButton.onClick.AddListener(() => {
            Finder.instance.uiMgr.ShowOnboarding(true);
        });
        SendLocationToState(_map.CenterLatitudeLongitude);
        UpdateUpdateButton();        
    }

    protected override void OnTabSelection() {
        base.OnTabSelection();
        ResetLongPress();

        ImagerySourceType type = (bool)Settings.instance.GetValue(Settings.Setting.useSatellite) ? ImagerySourceType.MapboxSatelliteStreet : ImagerySourceType.MapboxStreets;
        _map.ImageLayer.SetProperties(type, true, false, true);
        Finder.instance.uiMgr.SetModulePanel(true);
        Finder.instance.uiMgr.SetLoadingImage(true);
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

    void UpdateUpdateButton() {
        if (Finder.instance.moduleMgr.activeModule == null) {
            _updateButton.interactable = false;
        } else {
            _updateButton.interactable = _map.Zoom > Finder.instance.moduleMgr.activeModule.GetMinZoom();
        }
    }

    void UpdateLocationButton() {
        _locationButton.gameObject.SetActive(_locationProvider.CurrentLocation.IsLocationServiceEnabled);
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

    void ResetLongPress() {
        _pressedDown = false;
        _longPressTime = 0;
    }

    void UpdateUI() {
        UpdateUserLocationIcon(_locationProvider.CurrentLocation);
        UpdateMarkerLocationIcon();
        UpdateUpdateButton();
        UpdateLocationButton();
    }

    void Update()
    {
        UpdateUI();
        InputCheck();

        if(_pressedDown) {
            _longPressTime += Time.deltaTime;
            if(_longPressTime >= _requiredLongPressTime) {
                //ProcessLongPress();
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
