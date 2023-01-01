using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    //Stores the image for the player health bar
    [Header("Player Health")]
    [SerializeField]
    Image playerHealthBar;

    //Sets the object as an instance for the singleton structure
    public static PlayerUI instance;

    //Stores the list of the player's buff names
    [Header("Class System UI - Buffs")]
    public List<string> playerBuffNames;

    //Stores a reference to the two UI groups for the classe
    [Header("Class System UI - UI Groups")]
    public GameObject simpleClassGameplayUI;
    public GameObject dynamicClassGameplayUI;

    //Stores the images for the ability cooldowns
    [Header("Class System UI - Cooldown layer")]
    public Image[] UICooldownLayers;

    //Dynamic class - text for the weapon's ammo, remaining ammo, etc.
    [Header("Class System UI - Weapon Text")]
    public TextMeshProUGUI magAmmoText;
    public TextMeshProUGUI remainingAmmoText;

    //Stores the text objects for the buffs
    public TextMeshProUGUI dynamicBuffText;
    public TextMeshProUGUI simpleBuffText;

    // Start is called before the first frame update
    void Start()
    {
        //Set object instance, accessed by all objects
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //Updates the player's health bar fill and colour
        playerHealthBar.fillAmount = PlayerBase.instance.health / PlayerBase.instance.maxHealth;

        if(playerHealthBar.fillAmount < .4f)
        {
            playerHealthBar.color = Color.red;
        }
        else
        {
            playerHealthBar.color = Color.green;
        }

        //Calls the respective UI update methods depending on which is active
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
        //References the different abilities that are selected within the simple class
        SimpleClassAbilities abilityReference = PlayerBase.instance.GetComponent<SimpleClassAbilities>();
        //This cooldown display is represent by a greyed out image that slowly becomes smaller
        UICooldownLayers[0].fillAmount = abilityReference.buffCooldown / abilityReference.maxBuffCooldown;
        UICooldownLayers[1].fillAmount = abilityReference.superJumpLimit / abilityReference.superJumpLimitVal;
        UICooldownLayers[2].fillAmount = abilityReference.dodgeCooldown / abilityReference.maxDodgeCooldown;
    }

    public void UpdateGameplayAbilityUI(bool dynamicUI = false)
    {
        //Method called when switching class in the menu or in gameplay
        if (dynamicUI)
        {
            dynamicClassGameplayUI.GetComponent<DynamicGameplayUI>().updateUI();
        }
        else
        {
            simpleClassGameplayUI.GetComponent<SimpleGameplayUI>().updateUI();
        }
    }

    //References the abilities, cooldowns and weapon values in the dynamic class and weapon script
    public void UpdateDynamicClassUI()
    {
        DynamicClassAbilities abilityReference = PlayerBase.instance.GetComponent<DynamicClassAbilities>();
        //Updates the cooldowns for the dynamic class (Once again, greyed out image that becomes smaller)
        UICooldownLayers[3].fillAmount = abilityReference.concentratedShotCooldown / abilityReference.concentratedCooldownVal;
        UICooldownLayers[4].fillAmount = abilityReference.projectileCooldown / abilityReference.maxProjectileCooldown;
        //Records the amount of ammo currently in the mag, and the amount of remaining ammo that is held
        magAmmoText.text = "" + abilityReference.weapon.AmmoCount + " / " + abilityReference.weapon.MaxAmmoCount;
        remainingAmmoText.text = "" + abilityReference.weapon.AmmoHeld;
    }


    //Called to set the currently active gameplay UI (Refer to earlier defined bug)
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
