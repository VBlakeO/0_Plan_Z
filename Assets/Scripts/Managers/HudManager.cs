using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;
using TMPro;
using PlanZ.Weapons.Data;

public class HudManager : MonoBehaviour
{
    public static HudManager Instance = null;

    [Header("Crosshair")]
    [SerializeField] private Image CrosshairImage = null;
    [Space]

    [Header("Life Bar")]
    public Image lifeBar = null;
    public TextMeshProUGUI lifeText = null;
    public PlayerInventory playerInventory = null;
    public ComplexLivingBeing playerLife = null;
    [Space]

    [Header("First Aid Kit")]
    public TextMeshProUGUI firstAidText = null;

    [Header("Interaction")]
    [SerializeField] private Image interectionBar = null;
    [SerializeField] private Image interectionBase = null;
    [SerializeField] private TextMeshProUGUI interectionText = null;



    [Header("Weapons")]
    [SerializeField] private Image weaponIcon = null;
    [SerializeField] private TextMeshProUGUI ammoText = null;

    float previousHealth = 0;

    protected void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Garante que não existam múltiplas instâncias
            return;
        }

        Instance = this;
    }

    void Start()
    {
        previousHealth = playerLife.GetCurrentLife();

        playerLife.OnReceiveDamage += UpdateLifeBar;
        playerLife.OnReceiveHealing += UpdateLifeBar;
        playerInventory.OnUpdateFAK += UpdateFirstAid;

        interectionBar.fillAmount = 0f;
        SetInteractionText("");

        SetInteractionBaseState(false);
        UpdateFirstAid();
        UpdateLifeBar();
        DOTween.Init();
    }

    public void SetInterectionBarProgress(float progress)
    {
        interectionBar.fillAmount = progress;
    }


    #region Weapons
    public void SetCrosshairState(bool state)
    {
        CrosshairImage.enabled = state;
    }

    public void SetCrosshairStyle(CrosshairData Data)
    {
        // CrosshairImage.sprite = Data.CrosshairSprite;
        // CrosshairImage.color = Data.CrosshairColor;
        // CrosshairImage.transform.localScale = new Vector3(Data.CrosshairSize, Data.CrosshairSize, Data.CrosshairSize);
    }

    public void SetWeaponIcon(Sprite _weaponIcon)
    {
        weaponIcon.sprite = _weaponIcon;
    }

    public void UpdateAmmo(int _currentAmmo, int _maxAmmo)
    {
        if (_currentAmmo < 10)
        {
            if (_maxAmmo < 10)
                ammoText.SetText($"0{_currentAmmo}/0{_maxAmmo}");
            else
                ammoText.SetText($"0{_currentAmmo}/{_maxAmmo}");
        }
        else
        {
            if (_maxAmmo < 10)
                ammoText.SetText($"{_currentAmmo}/0{_maxAmmo}");
            else
                ammoText.SetText($"{_currentAmmo}/{_maxAmmo}");
        }
    }

    public void SetInteractionBaseState(bool state)
    {
        interectionBase.gameObject.SetActive(state);
    }

    public void SetInteractionText(string _text)
    {
        interectionText.text = "E   " + _text;
    }

    #endregion Weapons

    #region PlayerLife
    public void UpdateFirstAid()
    {
        firstAidText.text = "0" + playerInventory.currentFirstAidAmount;
    }

    private void UpdateLifeBar()
    {
        if (playerLife)
        {
            lifeBar.DOFillAmount(playerLife.GetCurrentLife() * 0.01f, 1f);

            DOTween.To(() => previousHealth, x => previousHealth = x, playerLife.GetCurrentLife(), 1f).SetEase(Ease.InOutSine).OnUpdate(() =>
            {
                lifeText.text = $"{previousHealth:F0}%";
            });
        }
    }
    #endregion Weapons
}