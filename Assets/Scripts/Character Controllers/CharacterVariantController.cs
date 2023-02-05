using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CharacterVariantController : MonoBehaviour
{
    [Serializable]
    public class CharacterVariantOptions
    {
        public CharacterFaceOptions characterFace;
        public CharacterHairTypes characterHair;
        public CharacterSkinOptions characterSkin;
        [Space]
        public CharacterShirtTypes characterShirtType;
        public Color shirtColorFill = Color.white;
        public CharacterShirtLogoTypes characterShirtDecal;
        public Color shirtDecalFill = Color.white;
        [Space]
        public bool hasEarPiece;
        public bool hasPadAndPen;
    }

    public SpriteLibrary spriteLibrary;

    public Animator characterAnimator;
    private readonly string animatorLabelFaceID = "FaceID";
    private readonly string animatorLabelCharacterUpdated = "CharacterUpdated";

    public CharacterList currentVariantID;
    private int prevVariantID;

    [Space]
    public CharacterVariantOptions[] characterVariantOptions;
    private CharacterVariantOptions currentVariant;

    [Space]
    public SpriteRenderer Face;
    public SpriteRenderer FaceBlink;
    public SpriteRenderer FaceDazed;
    public SpriteRenderer Head;
    public SpriteRenderer Hair;
    public SpriteRenderer HairRear;
    public SpriteRenderer Torso;
    public SpriteRenderer Shirt;
    public SpriteRenderer ShirtLogo;
    public SpriteRenderer LeftUpperSleeve;
    public SpriteRenderer RightUpperSleeve;
    public SpriteRenderer Pelvis;
    public SpriteRenderer Neck;
    public SpriteRenderer LeftUpperArm;
    public SpriteRenderer LeftForearm;
    public SpriteRenderer LeftHand;
    public SpriteRenderer RightUpperArm;
    public SpriteRenderer RightForearm;
    public SpriteRenderer RightHand;
    public SpriteRenderer LeftThigh;
    public SpriteRenderer LeftLowerLeg;
    public SpriteRenderer LeftFoot;
    public SpriteRenderer RightThigh;
    public SpriteRenderer RightLowerLeg;
    public SpriteRenderer RightFoot;

    private Dictionary<SpriteRenderer, CharacterBodyParts> targetBodyPart;
    private Dictionary<CharacterBodyParts, Enum> targetVarientType;
    private Dictionary<SpriteRenderer, int> sortingLayerOrder;

    private List<SpriteRenderer> shirtSpriteRenderers;

    private Component[] spriteRenderers;

    private bool useShortingMarker;
    private int distanceFromMarker;
    private int prevDistanceFromMarker;

    private bool isSet;
    private bool isReady;

    private void Start()
    {
        if (!characterAnimator) characterAnimator = GetComponent<Animator>();

        if (targetBodyPart is null) CreateSpriteAtlas();

        targetVarientType = new Dictionary<CharacterBodyParts, Enum>();

        spriteLibrary?.SetData();

        if (GameManager.Instance)
            if (GameManager.Instance.SortIndexStartPoint) useShortingMarker = true;

        isReady = true;
    }

    private void OnEnable()
    {
        if (isReady) ApplyVariantByID((int)currentVariantID);
    }

    private void Update()
    {
        if ((int)currentVariantID != prevVariantID || !isSet)
        {
            ApplyVariantByID((int)currentVariantID);
        }

        if (useShortingMarker)
        {
            distanceFromMarker = Mathf.RoundToInt(GameManager.Instance.SortIndexStartPoint.position.z - transform.position.z);

            if(prevDistanceFromMarker != distanceFromMarker)
            {
                prevDistanceFromMarker = distanceFromMarker;
                UpdateSpriteOrderInLayer(distanceFromMarker);
            }
        }
    }

    private void CreateSpriteAtlas()
    {
        targetBodyPart = new Dictionary<SpriteRenderer, CharacterBodyParts>
        {
            { Face, CharacterBodyParts.Face },
            { FaceBlink, CharacterBodyParts.FaceBlink },
            { FaceDazed, CharacterBodyParts.FaceDazed },
            { Head, CharacterBodyParts.Head },
            { Hair, CharacterBodyParts.Hair },
            { HairRear, CharacterBodyParts.HairRear },
            { Neck, CharacterBodyParts.Neck },
            { Torso, CharacterBodyParts.Torso },
            { Shirt, CharacterBodyParts.Shirt },
            { ShirtLogo, CharacterBodyParts.ShirtLogo },
            { LeftUpperSleeve, CharacterBodyParts.LeftUpperShirtSleve },
            { RightUpperSleeve, CharacterBodyParts.RightUpperShirtSleve },
            { Pelvis, CharacterBodyParts.Pelvis },
            { LeftUpperArm, CharacterBodyParts.LeftUpperArm },
            { LeftForearm, CharacterBodyParts.LeftForearm },
            { LeftHand, CharacterBodyParts.LeftHand },
            { RightUpperArm, CharacterBodyParts.RightUpperArm },
            { RightForearm, CharacterBodyParts.RightForearm },
            { RightHand, CharacterBodyParts.RightHand },
            { LeftThigh, CharacterBodyParts.LeftThigh },
            { LeftLowerLeg, CharacterBodyParts.LeftLowerLeg },
            { LeftFoot, CharacterBodyParts.LeftFoot },
            { RightThigh, CharacterBodyParts.RightThigh },
            { RightLowerLeg, CharacterBodyParts.RightLowerLeg },
            { RightFoot, CharacterBodyParts.RightFoot }
        };

        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        shirtSpriteRenderers = new List<SpriteRenderer>();
        
        shirtSpriteRenderers.Add(Shirt);
        shirtSpriteRenderers.Add(LeftUpperSleeve);
        shirtSpriteRenderers.Add(RightUpperSleeve);

        sortingLayerOrder = new Dictionary<SpriteRenderer, int>();

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            sortingLayerOrder.Add(spriteRenderer, spriteRenderer.sortingOrder);
    }

    public void ApplyVariantByID(int _id)
    {
        if (_id >= characterVariantOptions.Length) return;

        prevVariantID = _id;
        currentVariant = characterVariantOptions[_id];

        UpdateVarientData();
        UpdateSpriteRenderers();
        //UpdateAnimator((int)currentVariant.characterFace);
        
        isSet = true;
    }

    private void UpdateVarientData()
    {
        if (targetVarientType != null && targetVarientType.Count > 0) targetVarientType.Clear();

        targetVarientType = new Dictionary<CharacterBodyParts, Enum>
        {
            { CharacterBodyParts.Face, currentVariant.characterFace},
            { CharacterBodyParts.FaceBlink, currentVariant.characterFace},
            { CharacterBodyParts.FaceDazed, currentVariant.characterFace},
            { CharacterBodyParts.Head, currentVariant.characterSkin},
            { CharacterBodyParts.Hair, currentVariant.characterHair},
            { CharacterBodyParts.HairRear, currentVariant.characterHair},
            { CharacterBodyParts.Torso, currentVariant.characterSkin},
            { CharacterBodyParts.Shirt, currentVariant.characterShirtType },
            { CharacterBodyParts.ShirtLogo, currentVariant.characterShirtDecal },
            { CharacterBodyParts.LeftUpperShirtSleve, currentVariant.characterShirtType},
            { CharacterBodyParts.RightUpperShirtSleve, currentVariant.characterShirtType},
            { CharacterBodyParts.Pelvis, currentVariant.characterSkin},
            { CharacterBodyParts.Neck, currentVariant.characterSkin},
            { CharacterBodyParts.LeftUpperArm, currentVariant.characterSkin},
            { CharacterBodyParts.LeftForearm, currentVariant.characterSkin},
            { CharacterBodyParts.LeftHand, currentVariant.characterSkin},
            { CharacterBodyParts.RightUpperArm, currentVariant.characterSkin},
            { CharacterBodyParts.RightForearm, currentVariant.characterSkin},
            { CharacterBodyParts.RightHand, currentVariant.characterSkin},
            { CharacterBodyParts.LeftThigh, currentVariant.characterSkin},
            { CharacterBodyParts.LeftLowerLeg, currentVariant.characterSkin},
            { CharacterBodyParts.LeftFoot, currentVariant.characterSkin},
            { CharacterBodyParts.RightThigh, currentVariant.characterSkin},
            { CharacterBodyParts.RightLowerLeg, currentVariant.characterSkin},
            { CharacterBodyParts.RightFoot, currentVariant.characterSkin}
        };
    }

    private void UpdateSpriteRenderers()
    {
        int sortingLayerOffset = (int)currentVariantID < 1 ? characterVariantOptions.Length + 1 - (int)currentVariantID : (int)currentVariantID;

        if (targetBodyPart is null) CreateSpriteAtlas();
        if (targetBodyPart.Count < 1) return;

        if (characterAnimator) characterAnimator.enabled = false;

        foreach (KeyValuePair<SpriteRenderer, CharacterBodyParts> charPart in targetBodyPart)
        {
            Sprite _sprite = spriteLibrary.GetAsset(charPart.Value, targetVarientType[charPart.Value].ToString());

            if (charPart.Key)
            {
                if (_sprite) charPart.Key.sprite = _sprite;
            }
        }

        UpdateSpriteOrderInLayer(sortingLayerOffset);
        UpdateSpriteColors();

        if (characterAnimator) characterAnimator.enabled = true;
    }

    public void UpdateSpriteOrderInLayer(int _offset)
    {
        #if UNITY_EDITOR
        if (Application.isEditor && !EditorApplication.isPlaying) return;
        #endif

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            spriteRenderer.sortingOrder = sortingLayerOrder[spriteRenderer] + (_offset * targetBodyPart.Count); 
    }

    private void UpdateSpriteColors()
    {
        foreach (SpriteRenderer spriteRenderer in shirtSpriteRenderers)
            spriteRenderer.color = currentVariant.shirtColorFill;

        ShirtLogo.color = currentVariant.shirtDecalFill;
    }
}
