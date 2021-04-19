using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityARInterface;
using UnityEngine;
using UnityEngine.UI;

public enum LockMode {
    unlocked, locked
}

public class ARTab : TabView
{
    [SerializeField] Button _lockButton = null;
    [SerializeField] Button _unlockButton = null;
    [SerializeField] Slider _zoomSlider = null;
    [SerializeField] GameObject _placementHintPanel = null;
    [SerializeField] GameObject _alertPopup = null;

    [SerializeField] PlaceMapOnARPlane _placer = null;
    [SerializeField] DeviceLocationProvider _locationProvider = null;
    [SerializeField] AbstractMap _map = null;
    [SerializeField] ARPlaneVisualizer _planeVis = null;
 
    LockMode _lockMode = LockMode.unlocked;

    bool _mapInitialized = false;
    bool _firstLock = false;

    // Start is called before the first frame update
    void Start()
    {
        _unlockButton.onClick.AddListener(() => {            
            SetLockModeTo(LockMode.locked);
        });
        _lockButton.onClick.AddListener(() => {
            SetLockModeTo(LockMode.unlocked);
        });

        _zoomSlider.onValueChanged.AddListener(ZoomChanged);
                
        SetLockModeTo(LockMode.unlocked);
        Finder.instance.uiMgr.SetModulePanel(true);        

        _map.OnInitialized += () => {
            _mapInitialized = true;
            _zoomSlider.value = _zoomSlider.minValue + (_zoomSlider.maxValue - _zoomSlider.minValue) * Settings.instance.GetDefaultZoom();
            UpdateMapCenter();
        };
    }

    void SetLockModeTo(LockMode mode) {
        if (!_firstLock && mode == LockMode.locked) {
            VisualizeData();
            AppState.instance.allowARConnectionAlert = true;
            _firstLock = true;
        }
        _lockMode = mode;
        _lockButton.gameObject.SetActive(_lockMode == LockMode.locked);
        _unlockButton.gameObject.SetActive(_lockMode == LockMode.unlocked);
        _zoomSlider.gameObject.SetActive(_lockMode == LockMode.locked);
        _planeVis.TogglePlanes(_lockMode == LockMode.unlocked);
        _placementHintPanel.SetActive(_lockMode == LockMode.unlocked);
        _placer.LockStateChangeTo(_lockMode);

        Finder.instance.uiMgr.SetModulePanel(_lockMode == LockMode.locked);
    }

    void ZoomChanged(float zoom) {
        _map.SetZoom(zoom);
        _map.UpdateMap();
    }

    void VisualizeData() {
        Finder.instance.moduleMgr.VisualizeAROnMap(_map);
    }

    void UpdateMapCenter() {
        if (UpdateCheck()) {
            _map.SetCenterLatitudeLongitude(AppState.instance.transferLocation);
            _map.UpdateMap();
            if (_firstLock) {
                if (Finder.instance.moduleMgr.activeModule)
                    Finder.instance.moduleMgr.activeModule.arVisualizer.Disable();
                VisualizeData();
            }
        }
    }

    bool UpdateCheck() {
        return !_map.CenterLatitudeLongitude.Equals(AppState.instance.transferLocation);
    }

    protected override void OnTabSelection() {
        base.OnTabSelection();
        
        ImagerySourceType type = (bool)Settings.instance.GetValue(Settings.Setting.useSatellite) ? ImagerySourceType.MapboxSatelliteStreet : ImagerySourceType.MapboxStreets;
        _map.ImageLayer.SetProperties(type, true, false, true);

        Finder.instance.uiMgr.SetModulePanel(_lockMode == LockMode.locked);
        Finder.instance.uiMgr.SetLoadingImage(true);

        if (_mapInitialized) {
            UpdateMapCenter();
        }
    }
}
