using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterLibraryData", menuName = "ScriptableObjects/3DCharacterLibrary", order = 1)]
public class CharacterLibrary : ScriptableObject
{
    public string prefabName;

    public int numberOfPrefabsToCreate;
    public Vector3[] spawnPoints;
    public bool useSpawnEffect;
    [Space]
    public int activePlayers = 1;
    [Space,Space]
    public CharacterModelSetup[] characterModelTypes;
    public GameObject[] characterModelSuperTypes;
    public Sprite[] characterModelHUDImages;
    [Space, Space]
    public CharacterModelSetup[] enemyModelTypes;
    [Space,Space]
    public GameObject[] playerNumNaviHUD;
    
    public Dictionary<int, int> selectedCharacters = new Dictionary<int, int>();

    private void Start()
    {
        for (int i = 0; i < characterModelTypes.Length; i++)
        {
            selectedCharacters.Add(i+1, 0);
        }
    }
}