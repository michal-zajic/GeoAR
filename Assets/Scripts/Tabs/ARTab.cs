using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;
using UnityEngine.UI;

public enum LockMode {
    unlocked, locked
}

public class ARTab : TabView
{
    [SerializeField] Button _lockButton = null;
    [SerializeField] Button _unlockButton = null;
    [SerializeField] Button _updateButton = null;
    [SerializeField] Slider _zoomSlider = null;

    [SerializeField] PlaceMapOnARPlane _placer = null;
    [SerializeField] DeviceLocationProvider _locationProvider = null;
    [SerializeField] AbstractMap _map = null;
 
    LockMode _lockMode = LockMode.unlocked;

    bool _firstUpdate = false;
    bool _mapInitialized = false;
    
    // Start is called before the first frame update
    void Start()
    {
        _unlockButton.onClick.AddListener(() => {            
            SetLockModeTo(LockMode.locked);
        });
        _lockButton.onClick.AddListener(() => {
            SetLockModeTo(LockMode.unlocked);
        });
        _updateButton.onClick.AddListener(UpdateMapCenter);

        _zoomSlider.onValueChanged.AddListener(ZoomChanged);
                
        SetLockModeTo(LockMode.unlocked);

        _map.OnInitialized += () => {
            _mapInitialized = true;
            _zoomSlider.value = _zoomSlider.minValue + (_zoomSlider.maxValue - _zoomSlider.minValue) * Settings.instance.GetDefaultZoom();
            if (_firstUpdate) {
                UpdateMapCenter();                
            }
        };
    }

    void SetLockModeTo(LockMode mode) {
        _lockMode = mode;
        _lockButton.gameObject.SetActive(_lockMode == LockMode.locked);
        _unlockButton.gameObject.SetActive(_lockMode == LockMode.unlocked);
        _updateButton.gameObject.SetActive(_lockMode == LockMode.locked);
        _zoomSlider.gameObject.SetActive(_lockMode == LockMode.locked);
        _placer.LockStateChangeTo(_lockMode);
    }

    void ZoomChanged(float zoom) {
        _map.SetZoom(zoom);
        _map.UpdateMap();
    }

    void UpdateMapCenter() {
        Vector2d location;
        if (AppState.instance.markerPlaced) {
            location = AppState.instance.markerLocation;
        }
        else if (_locationProvider.CurrentLocation.IsLocationServiceEnabled && Settings.instance.IsGPSDefault()) {
            location = _locationProvider.CurrentLocation.LatitudeLongitude;
        } else {
            location = AppState.instance.currentMapCenter;
        }

        _map.SetCenterLatitudeLongitude(location);
        _map.UpdateMap();
    }

    protected override void OnTabSelection() {
        base.OnTabSelection();
        if(!_firstUpdate) {
            if(_mapInitialized)
                UpdateMapCenter();
            _firstUpdate = true;
        }
        ImagerySourceType type = (bool)Settings.instance.GetValue(Settings.Setting.useSatellite) ? ImagerySourceType.MapboxSatelliteStreet : ImagerySourceType.MapboxStreets;
        _map.ImageLayer.SetProperties(type, true, false, true);
    }
}
