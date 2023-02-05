using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpiteLibraryData", menuName = "ScriptableObjects/2DSpriteLibrary", order = 1)]
public class SpriteLibrary : ScriptableObject
{
    public string prefabName;

    [Serializable]
    public struct SpriteAssets
    {
        public CharacterBodyParts groupName;
        public Sprite[] image;
        public string[] label;
    }

    /*[Serializable]
    public struct CharacterFaces
    {
        public CharacterFacialExpressions groupName;
        public Sprite[] image;
        public string[] label;
    }

    public CharacterFaces[] characterFaces;*/

    [Space]
    public SpriteAssets[] spriteGroups;

    public Dictionary<CharacterBodyParts, SpriteAssets> dataSet;

    public void SetData()
    {
        dataSet = new Dictionary<CharacterBodyParts, SpriteAssets>();

        for (int i = 0; i < spriteGroups.Length; i++)
        {
            dataSet.Add(spriteGroups[i].groupName, spriteGroups[i]);
        }
    }

    public Sprite GetAsset(CharacterBodyParts _type, string _name)
    {
        Sprite _sprite = null;

        if (!dataSet.ContainsKey(_type)) return _sprite;

        if (dataSet[_type].image != null)
        {
            for (int i = 0; i < dataSet[_type].image.Length; i++)
            {
                if (dataSet[_type].label[i] == _name) _sprite = dataSet[_type].image[i];
            }
        }

        return _sprite;
    }

    public Sprite GetAssetFromArray(Sprite[] _array, int _id)
    {
        Sprite _sprite = null;

        if (_array == null || _array.Length < _id || _array.Length <= 0) return _sprite;

        return _array[_id];
    }
}