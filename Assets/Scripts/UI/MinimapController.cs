using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapController : MonoBehaviour
{
    public enum MinimapBehaviour
    {
        Static,
        RotateMap,
        RotatePlayerIcon
    }

    public CharacterTrigger player;
    public Transform playerIndicator;
    public Camera minimapCamera;

    [Header("Minimap Settings")]
    public MinimapBehaviour minimapBehaviour;
    public bool invertPlayerRotation;

    private void LateUpdate()
    {
        UpdateMinimapView();
    }

    private void UpdateMinimapView()
    {
        if (!player || !minimapCamera) return;

        Vector3 playerRotation = player.transform.eulerAngles;
        Vector3 playerPosition = player.transform.position;
        playerPosition.y = minimapCamera.transform.position.y;

        minimapCamera.transform.position = playerPosition;

        if (minimapBehaviour == MinimapBehaviour.RotateMap) minimapCamera.transform.rotation = Quaternion.Euler(90f, player.transform.eulerAngles.y, 0f);
        if (minimapBehaviour == MinimapBehaviour.RotatePlayerIcon && playerIndicator) playerIndicator.transform.rotation = invertPlayerRotation ? Quaternion.Inverse(Quaternion.Euler(0f, 0f, playerRotation.y)) : Quaternion.Euler(0f, 0f, playerRotation.y);
    }

    public void AssignPlayer(CharacterTrigger _player)
    {
        player = _player;
    }
}
