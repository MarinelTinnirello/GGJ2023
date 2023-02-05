using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class CharacterHealth : MonoBehaviour
{
    [Header("Health Bar Settings")]
    public Canvas healthBarCanvas;
    public Image healthBarDisplay;
    public RenderMode renderMode;
    public float maxHP = 150f;
    public float currentHP = 150f;

    [Space, Range(0f,1f)]
    public float currentHealth;
    
    [Header("Health Bar Settings")]
    public Image knockOutIcon;
    public GameObject knockOutObject;

    [Space]
    public bool isAlive;
    
    private Camera cam;
    private Quaternion originalRotation;
    private Transform parentTransform;
    private void Start()
    {
        isAlive = true;

        knockOutIcon?.rectTransform.DOScale(0f, 0f);

        cam = GameManager.Instance ? GameManager.Instance.UICamera : Camera.main;

        originalRotation = transform.rotation;
    }

    private void Update()
    {
        //currentHP = currentHP - (10 * Time.deltaTime); //for testing
        currentHealth = currentHP / maxHP;
        CheckIfIsAlive();

        if (healthBarDisplay && isAlive) healthBarDisplay.fillAmount = GetHealthPercentage();

        if (!isAlive) OnKnockOut();
    }
    private void LateUpdate()
    {
        //if (!healthBarCanvas || !cam) return;

        if (renderMode == RenderMode.Billboard && parentTransform) transform.localRotation = Quaternion.Inverse(parentTransform.localRotation);

        //if (renderMode == RenderMode.Billboard) healthBarCanvas.transform.LookAt(cam.transform.position);

        /*bool isReverse = false;
        Vector3 targetPos = healthBarCanvas.transform.position + cam.transform.rotation * (isReverse ? Vector3.back : Vector3.forward);
        Vector3 targetOrientation = cam.transform.rotation * Vector3.up;
        healthBarCanvas.transform.LookAt(targetPos, targetOrientation);*/

        //healthBarCanvas.transform.rotation = cam.transform.rotation * originalRotation;

        //healthBarCanvas.transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }

    public void SetParentTransform(Transform targetTransform)
    {
        parentTransform = targetTransform;
    }

    public void SetMaxHealth(float ammount)
    {
        currentHP = maxHP = ammount;
    }

    public void TakeDamage(float ammount)
    {
        currentHP = currentHP - ammount;

        CheckIfIsAlive();
    }
    public void AddHealthByPercent(float percentage)
    {
        percentage = maxHP * percentage;

        currentHP = currentHP + percentage;
        if (currentHP > maxHP) currentHP = maxHP;

        CheckIfIsAlive();
    }
    public void AddHealth(float ammount)
    {
        currentHP = currentHP + ammount;
        if (currentHP > maxHP) currentHP = maxHP;

        CheckIfIsAlive();
    }

    private void CheckIfIsAlive()
    {
        if (currentHP <= 0f) isAlive = false;
    }

    public float GetHealthPercentage()
    {
        if (!isAlive) return 0f;

        return currentHealth;
    }

    public string GetHealthPercentageFormatted()
    {
        return Mathf.Round(GetHealthPercentage()*100) + "%";
    }

    public void OnKnockOut()
    {
        if (healthBarCanvas) healthBarCanvas.enabled = false;

        knockOutIcon?.rectTransform.DOScale(2f, 1f).SetEase(Ease.OutElastic);

        if (knockOutObject) knockOutObject.SetActive(true);
    }

    public void SetSortingLayer(string targetLayer)
    {
        if(healthBarCanvas) healthBarCanvas.sortingLayerName = targetLayer;
    }
}
