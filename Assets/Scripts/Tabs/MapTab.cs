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
    [SerializeField] GameObject _zoomAlert = null;
    [SerializeField] RectTransform _marker = null;
    [SerializeField] Button _locationButton = null;
    [SerializeField] Button _updateButton = null;
    [SerializeField] Button _onboardingButton = null;
    [SerializeField] Button _transferButton = null;

    bool _firstUpdate = false;

    Vector2d _corner1 = new Vector2d(48.5476636, 11.6568511);
    Vector2d _corner2 = new Vector2d(51.2190594, 19.0396636);
    Vector2d _currentARPosition = Vector2d.zero;

    void Start()
    {
        _marker.gameObject.SetActive(false);       

        SendTransferLocationToState(_map.CenterLatitudeLongitude);
        InitButtons();       
    }

    void InitButtons() {
        _locationButton.onClick.AddListener(CenterOnUserLocation);
        _updateButton.onClick.AddListener(() => {
            AppState.instance.allowMapConnectionAlert = true;
            Finder.instance.moduleMgr.VisualizeOnMap(_map);
        });
        _onboardingButton.onClick.AddListener(() => {
            Finder.instance.uiMgr.ShowOnboarding(true);
        });
        _transferButton.onClick.AddListener(() => {
            _currentARPosition = _map.CenterLatitudeLongitude;
            AppState.instance.UpdateTransferLocation(_currentARPosition);
        });
    }

    protected override void OnTabSelection() {
        base.OnTabSelection();

        ImagerySourceType type = (bool)Settings.instance.GetValue(Settings.Setting.useSatellite) ? ImagerySourceType.MapboxSatelliteStreet : ImagerySourceType.MapboxStreets;
        _map.ImageLayer.SetProperties(type, true, false, true);
        Finder.instance.uiMgr.SetModulePanel(true);
        Finder.instance.uiMgr.SetLoadingImage(true);
    }

    void CenterOnUserLocation() {
        Vector2d loc = _locationProvider.CurrentLocation.LatitudeLongitude;
        _map.SetCenterLatitudeLongitude(loc);
        _map.SetZoom(15);
        _map.UpdateMap();
    }

    void SendTransferLocationToState(Vector2d location) {
        AppState.instance.UpdateTransferLocation(location);
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
            SendTransferLocationToState(latlon);
            CenterOnUserLocation();
        }
        if(!_userLocationImage.gameObject.activeInHierarchy)
            _userLocationImage.gameObject.SetActive(true);

        Vector2 screenPos = GeoToScreenPoint(latlon);
        _userLocationImage.rectTransform.position = screenPos;


    }

    void UpdateZoomAlert() {
        if(Finder.instance.moduleMgr.activeModule != null) {
            if(_map.Zoom < Finder.instance.moduleMgr.activeModule.GetMinZoom()) {
                _zoomAlert.SetActive(true);
                return;
            } 
        }
        _zoomAlert.SetActive(false);
    }

    void UpdateTransferButton() {
        _transferButton.gameObject.SetActive(!_currentARPosition.Equals(_map.CenterLatitudeLongitude));
    }

    void UpdateUI() {
        UpdateUserLocationIcon(_locationProvider.CurrentLocation);
        UpdateUpdateButton();
        UpdateLocationButton();
        UpdateTransferButton();
        UpdateZoomAlert();
    }

    void Update()
    {
        UpdateUI();        
    }
}
