////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: MapTab.cs
//Author : Michal Zajíc
//Year : 2021
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Location;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//Controller for map tab
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

    Vector2d _corner1 = new Vector2d(48.5476636, 11.6568511);   //handpicked corner locations of bounding box encapsulating Czech Republic
    Vector2d _corner2 = new Vector2d(51.2190594, 19.0396636);
    Vector2d _currentARPosition = Vector2d.zero;

    void Start()
    {
        _marker.gameObject.SetActive(false);

        TransferMapCenterToAR();
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
            TransferMapCenterToAR();
        });
    }

    //when clicking on this tab, map needs to be updated if user changed settings (satellite imagery)
    //also map specific UI is enabled
    protected override void OnTabSelection() {
        base.OnTabSelection();

        ImagerySourceType type = (bool)Settings.instance.GetValue(Settings.Setting.useSatellite) ? ImagerySourceType.MapboxSatelliteStreet : ImagerySourceType.MapboxStreets;
        _map.ImageLayer.SetProperties(type, true, false, true);
        Finder.instance.uiMgr.SetModulePanel(true);
        Finder.instance.uiMgr.SetLoadingImage(true);

        if(Finder.instance.moduleMgr.activeModule != null) {
            Finder.instance.moduleMgr.activeModule.mapVisualizer.Enable();
        }
    }

    void CenterOnUserLocation() {
        Vector2d loc = _locationProvider.CurrentLocation.LatitudeLongitude;
        _map.SetCenterLatitudeLongitude(loc);
        _map.SetZoom(15);
        _map.UpdateMap();
    }

    void SendTransferLocationToState(Vector2d location) {
        _currentARPosition = location;
        AppState.instance.UpdateTransferLocation(location);
    }

    void TransferMapCenterToAR() {
        SendTransferLocationToState(_map.CenterLatitudeLongitude);
    }

    //update button is interactable based on chosen module and map zoom
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

    Vector2 GeoToScreenPoint(Vector2d latlon) {
        Vector3 position = _map.GeoToWorldPosition(latlon);
        return camera.WorldToScreenPoint(position);        
    }

    //enables user location button based on gps availability
    //when getting gps for the first time, it also centers map on user location
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
    //shows zoom instruction when map zoom is lesser than module's minimal zoom
    void UpdateZoomAlert() {
        if(Finder.instance.moduleMgr.activeModule != null) {
            if(_map.Zoom < Finder.instance.moduleMgr.activeModule.GetMinZoom()) {
                _zoomAlert.SetActive(true);
                return;
            } 
        }
        _zoomAlert.SetActive(false);
    }
    //shows transfer button when map is further than 200m from last transfered position
    void UpdateTransferButton() {
        Vector2d first = _currentARPosition;
        Vector2d second = _map.CenterLatitudeLongitude;
        _transferButton.gameObject.SetActive(Tools.GetDistanceBetweenPoints(first.x, first.y, second.x, second.y) > 200);
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
