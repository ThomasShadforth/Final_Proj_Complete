using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [Header("Player Health")]
    [SerializeField]
    Image playerHealthBar;

    public static PlayerUI instance;

    [Header("Class System UI - Buffs")]
    public List<string> playerBuffNames;

    [Header("Class System UI - UI Groups")]
    public GameObject simpleClassGameplayUI;
    public GameObject dynamicClassGameplayUI;

    [Header("Class System UI - Cooldown layer")]
    public Image[] UICooldownLayers;

    [Header("Class System UI - Weapon Text")]
    public TextMeshProUGUI magAmmoText;
    public TextMeshProUGUI remainingAmmoText;


    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerHealthBar.fillAmount = PlayerBase.instance.health / PlayerBase.instance.maxHealth;

        if(playerHealthBar.fillAmount < .4f)
        {
            playerHealthBar.color = Color.red;
        }
        else
        {
            playerHealthBar.color = Color.green;
        }

        if (simpleClassGameplayUI.activeInHierarchy)
        {
            UpdateSimpleClassUI();
        }

        if (dynamicClassGameplayUI.activeInHierarchy)
        {
            UpdateDynamicClassUI();
        }
    }

    public void UpdateSimpleClassUI()
    {
        SimpleClassAbilities abilityReference = PlayerBase.instance.GetComponent<SimpleClassAbilities>();
        UICooldownLayers[0].fillAmount = abilityReference.buffCooldown / abilityReference.maxBuffCooldown;
        UICooldownLayers[1].fillAmount = abilityReference.superJumpLimit / abilityReference.superJumpLimitVal;
        UICooldownLayers[2].fillAmount = abilityReference.dodgeCooldown / abilityReference.maxDodgeCooldown;
    }

    public void UpdateGameplayAbilityUI(bool dynamicUI = false)
    {
        if (dynamicUI)
        {
            dynamicClassGameplayUI.GetComponent<DynamicGameplayUI>().updateUI();
        }
        else
        {
            simpleClassGameplayUI.GetComponent<SimpleGameplayUI>().updateUI();
        }
    }

    public void UpdateDynamicClassUI()
    {
        DynamicClassAbilities abilityReference = PlayerBase.instance.GetComponent<DynamicClassAbilities>();
        UICooldownLayers[3].fillAmount = abilityReference.concentratedShotCooldown / abilityReference.concentratedCooldownVal;
        UICooldownLayers[4].fillAmount = abilityReference.projectileCooldown / abilityReference.maxProjectileCooldown;
        magAmmoText.text = "" + abilityReference.weapon.AmmoCount + " / " + abilityReference.weapon.MaxAmmoCount;
        remainingAmmoText.text = "" + abilityReference.weapon.AmmoHeld;
    }

    public void SetClassGameplayUI(bool dynamic = false)
    {
        if (dynamic)
        {
            simpleClassGameplayUI.SetActive(false);
            dynamicClassGameplayUI.SetActive(true);
        }
        else
        {
            simpleClassGameplayUI.SetActive(true);
            dynamicClassGameplayUI.SetActive(false);
        }
    }
}
