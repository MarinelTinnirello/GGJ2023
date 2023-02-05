using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSetup : MonoBehaviour
{
    public CharacterLibrary characterLibrary;
    [Space]
    public CharacterType characterType;
    private CharacterType previousCharacterType;
    public MultiplayerCharacterSettings multiplayerCharacterSettings;
    public MovementType characterRenderType;
    public CharacterAnimationController characterAnimationController;
    public Transform characterContainer;
    public CharacterFootstepSystem characterFootstepSystem;
    [HideInInspector]
    public CharacterModelSetup currentModel;
    private GameObject currentModelGameObject;
    public int currentModelID;
    [Space]
    public bool destroyOnKnockOut = true;

    [Header("Character Controller Types")]
    public CharacterInputController characterInputController;
    public NPCController npcController;
    public EnemyController enemyController;
    private CharacterTrigger currentCharacterController;

    [Space]
    public GameObject mainCharacterSpotlight;

    private void Start()
    {
        if (!characterInputController) characterInputController = GetComponent<CharacterInputController>();
        if (!npcController) npcController = GetComponent<NPCController>();
        if (!enemyController) enemyController = GetComponent<EnemyController>();
        if (!characterFootstepSystem) characterFootstepSystem = GetComponent<CharacterFootstepSystem>();

        //AddCharacterModelByID(currentModelID);
    }
    private void Update()
    {
        if (previousCharacterType != characterType) SetCharacterType(characterType);
    }

    public void AddCharacterModelByID(int modelID, CharacterType setCharacterType = CharacterType.None)
    {
        CharacterType currentCharacterType = setCharacterType != CharacterType.None ? setCharacterType : characterType;
        currentModelID = modelID;

        if (characterLibrary) currentModel = characterType != CharacterType.Enemy ? characterLibrary.characterModelTypes[currentModelID] : characterLibrary.enemyModelTypes[currentModelID];
        
        if (currentModel)
        {
            GameObject obj = Instantiate(currentModel.gameObject);

            if(characterContainer) foreach(Transform child in characterContainer) GameObject.Destroy(child.gameObject);

            obj.transform.parent = characterContainer? characterContainer : transform;
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;

            currentModelGameObject = obj;
            currentModelGameObject.SetActive(true);

            currentModel = currentModelGameObject.GetComponent<CharacterModelSetup>();
        }

        SetCharacterType(currentCharacterType);
    }

    public void SetCharacterType(CharacterType setCharacterType)
    {
        characterType = previousCharacterType = setCharacterType;

        if (characterInputController) characterInputController.enabled = characterType == CharacterType.MainPlayer;
        if (npcController) npcController.enabled = characterType == CharacterType.NPC;
        if (enemyController) enemyController.enabled = characterType == CharacterType.Enemy;

        switch (characterType)
        {
            case CharacterType.Enemy:
                currentCharacterController = enemyController;
                UpdateTagAndLayerName("Enemy", "Enemy");
                break;
            case CharacterType.MainPlayer:
                currentCharacterController = characterInputController;
                UpdateTagAndLayerName("Player", "Player");

                if (mainCharacterSpotlight) mainCharacterSpotlight.gameObject.SetActive(true);

                break;
            case CharacterType.NPC:
                currentCharacterController = npcController;
                UpdateTagAndLayerName("NPC", "NPC");
                break;
            default:
                break;
        }

        switch (multiplayerCharacterSettings)
        {
            case MultiplayerCharacterSettings.OnlineCoOp:
                break;
            case MultiplayerCharacterSettings.OnlineEnemy:
                break;
            default:
                break;
        }

        currentCharacterController.destroyOnKnockOut = destroyOnKnockOut;
        currentCharacterController?.SetCharacterRenderType(characterRenderType);

        if (characterAnimationController) currentCharacterController.characterAnimationController = characterAnimationController;

        if (characterFootstepSystem)
        {
            if (currentCharacterController.characterEffects) characterFootstepSystem.characterEffects = currentCharacterController.characterEffects;
            if (currentCharacterController.characterSoundEffects) characterFootstepSystem.soundEffects = currentCharacterController.characterSoundEffects;

            currentCharacterController.characterFootstepSystem = characterFootstepSystem;
        }

        if (currentModel) currentCharacterController?.SetModelInformation(currentModel);
        if (currentCharacterController.characterAnimationController) currentCharacterController.characterAnimationController.characterContainer = characterContainer;
        
        if (characterType == CharacterType.MainPlayer)
        {
            if(characterAnimationController && characterAnimationController.headLookAt)
            {
                characterAnimationController.headLookAt.overrideAnimation = false;
                characterAnimationController.headLookAt.target = null;
            }

            GameManager.Instance?.SetMainCharacter(currentCharacterController);
        }

        currentCharacterController.characterIsSetup = true;
    }

    public CharacterTrigger GetCharacterTriggerType()
    {
        switch (characterType)
        {
            case CharacterType.Enemy:
                if (enemyController) return enemyController;
                break;
            case CharacterType.MainPlayer:
                if (characterInputController) return characterInputController;
                break;
            case CharacterType.NPC:
                if (npcController) return npcController;
                break;
            default:
                break;
        }

        return null;
    }

    public void UpdateTagAndLayerName(string tagName, string layerName)
    {
        gameObject.tag = tagName;
        gameObject.layer = LayerMask.NameToLayer(layerName);
    }
}
