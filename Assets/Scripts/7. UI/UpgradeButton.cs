using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class UpgradeButton : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image iconImage;
    //[SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    public UpgradeData upgradeData;
    private UpgradeScreen upgradeScreen;

    [SerializeField] private Image frame;

    public Sprite offensiveFrame;
    public Sprite deffensiveFrame;
    public Sprite activeFrame;

    public TMP_Text typeText;
    
    private Vector3 originalScale;
    private Coroutine scaleCoroutine;
    
    private Coroutine wobbleCoroutine;

    [SerializeField] private float wobbleAngle = 10f;
    [SerializeField] private float wobbleSpeed = 2f;
    private float wobblePhaseOffset;
    
    private void Awake()
    {
        originalScale = transform.localScale;
        wobblePhaseOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    public void Setup(UpgradeData data, UpgradeScreen screen, PlayerController player)
    {
        upgradeData = data;
        upgradeScreen = screen;

        iconImage.sprite = data.icon;
        //nameText.text = data.upgradeName;
        if (player.currentControlScheme == "Gamepad")
        {
            descriptionText.text = data.descriptionIfGamepad;
        }
        else
        {
            descriptionText.text = data.descriptionIfKeyboard;
        }

        if (data.effectPrefab.isActiveAbility)
        {
            frame.sprite = activeFrame;
            typeText.text = "Active";
        }
        else if (!data.effectPrefab.isActiveAbility)
        {
            if (data.effectPrefab.offensive)
            {
                frame.sprite = offensiveFrame;
                typeText.text = "Offensive";
            }
            else
            {
                frame.sprite = deffensiveFrame;
                typeText.text = "Defensive";
            }
        }

        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (upgradeData != null && upgradeScreen != null)
        {
            upgradeScreen.SelectUpgrade(upgradeData);
        }
        else if (upgradeData == null && upgradeScreen != null)
        {
            Debug.Log("No upgrade data!!!");
        }
        else if (upgradeData != null && upgradeScreen == null)
        {
            Debug.Log("No upgrade screen!!!");
        }
        else if (upgradeData == null && upgradeScreen == null)
        {
            Debug.Log("No upgrade data AND upgrade screen!!!");
        }
        else
        {
            Debug.Log("Something's totally wrong");
        }
    }

    public UpgradeData GetUpgradeData()
    {
        return upgradeData;
    }
    
    public void OnSelect(BaseEventData eventData)
    {
        StartScaleAnimation(originalScale * 1.2f, 0.2f);
        StartWobble();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        StartScaleAnimation(originalScale, 0.2f);
        StopWobble();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StartScaleAnimation(originalScale * 1.2f, 0.2f);
        StartWobble();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartScaleAnimation(originalScale, 0.2f);
        StopWobble();
    }
    
    private void StartScaleAnimation(Vector3 targetScale, float duration)
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(ScaleCoroutine(targetScale, duration));
    }

    private IEnumerator ScaleCoroutine(Vector3 targetScale, float duration)
    {
        Vector3 startScale = transform.localScale;
        float time = 0f;

        while (time < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, time / duration);
            time += Time.unscaledDeltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }
    
    private void StartWobble()
    {
        if (wobbleCoroutine != null)
            StopCoroutine(wobbleCoroutine);

        wobbleCoroutine = StartCoroutine(WobbleCoroutine());
    }
    
    public void StopWobble()
    {
        if (wobbleCoroutine != null)
        {
            StopCoroutine(wobbleCoroutine);
            wobbleCoroutine = null;
        }
        
        transform.rotation = Quaternion.identity;
    }

    private IEnumerator WobbleCoroutine()
    {
        while (true)
        {
            float angle = Mathf.Sin(Time.unscaledTime * wobbleSpeed * Mathf.PI * 2f + wobblePhaseOffset) * wobbleAngle;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            yield return null;
        }
    }
}