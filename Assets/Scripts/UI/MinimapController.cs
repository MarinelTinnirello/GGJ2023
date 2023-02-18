using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    public static MinimapController _instance;

    public enum MinimapBehaviour
    {
        Static,
        RotateMap,
        RotatePlayerIcon,
        StationaryMap
    }

    public CharacterTrigger player;
    public Transform playerIndicator;
    public Camera minimapCamera;
    public RawImage minimapDisplay;

    [Header("Minimap Settings")]
    public MinimapBehaviour minimapBehaviour;
    [Range(0, 255)]
    public byte opacity = 255;
    private byte setOpacity = 255;
    public bool invertPlayerRotation;

    [Header("Marker Settings")]
    public MinimapMarkerIcon markerIconPrefab;
    public Transform markersContainer;

    [Space]
    public List<MinimapMarkerIcon> markerIcons;

    private Dictionary<int, MinimapMarkerIcon> markerData;
    private int nextMarkerID = 0;
    private Sprite[] markerImages;

    private bool init;

    public static MinimapController Instance
    {
        get { return _instance; }
    }

    private void Awake()
    {
        _instance = this;

        markerData = new Dictionary<int, MinimapMarkerIcon>();
        markerIcons = new List<MinimapMarkerIcon>();

        MatchPlayerPosition();
    }

    private void LateUpdate()
    {
        UpdateMinimapView();
    }

    public void AssignMainPlayer(CharacterTrigger _player)
    {
        player = _player;
    }

    private void MatchPlayerPosition()
    {
        if (!player || init || minimapBehaviour == MinimapBehaviour.StationaryMap ) return;

        transform.position = player.transform.position;
        init = true;
    }

    private void UpdateMinimapView()
    {
        if (!player || !minimapCamera) return;

        MatchPlayerPosition();
        SetMapOpacity();

        Vector3 playerRotation = player.transform.eulerAngles;
        Vector3 playerPosition = player.transform.position;
        playerPosition.y = minimapCamera.transform.position.y;

        if (minimapBehaviour != MinimapBehaviour.StationaryMap)
        {
            minimapCamera.transform.position = playerPosition;
        } else
        {
            RectTransform playerIco = (RectTransform)playerIndicator;
            Vector3 screenPos = minimapCamera.WorldToScreenPoint(player.transform.position);
            playerIndicator.transform.localPosition = new Vector3(screenPos.x- (playerIco.rect.width/2), screenPos.y - (playerIco.rect.height / 2), 0f);;
        }

        if (minimapBehaviour == MinimapBehaviour.RotateMap) minimapCamera.transform.rotation = Quaternion.Euler(90f, player.transform.eulerAngles.y, 0f);
        if (minimapBehaviour == MinimapBehaviour.RotatePlayerIcon || minimapBehaviour == MinimapBehaviour.StationaryMap) if(playerIndicator) playerIndicator.transform.rotation = invertPlayerRotation ? Quaternion.Inverse(Quaternion.Euler(0f, 0f, playerRotation.y)) : Quaternion.Euler(0f, 0f, playerRotation.y);
    }

    private void SetMapOpacity()
    {
        if (!minimapDisplay || setOpacity == opacity) return;

        Color32 _color = minimapDisplay.color;
        _color.a = opacity;
        minimapDisplay.color = _color;

        

        setOpacity = opacity;
    }

    public void RegisterMarker(MinimapMarker _marker)
    {
        if (!markerIconPrefab) return;

        MinimapMarkerIcon _icon = InstantiateMarkerPrefab(markerIconPrefab, Vector3.zero, markersContainer);

        _icon?.AssignMarker(_marker, minimapCamera);

        markerIcons.Add(_icon);
        markerData.Add(nextMarkerID, _icon);
        nextMarkerID++;
    }

    private MinimapMarkerIcon InstantiateMarkerPrefab(MinimapMarkerIcon prefab, Vector3 position = default, Transform parent = null)
    {
        if (prefab == null)
            return null;

        MinimapMarkerIcon obj = Instantiate(prefab);

        obj.transform.parent = parent;
        obj.transform.localScale = Vector3.one;
        obj.transform.localPosition = position;

        obj.gameObject.SetActive(true);

        return obj;
    }

    /*public void AssignPlayer(CharacterTrigger _player)
    {
        player = _player;
    }*/


}
