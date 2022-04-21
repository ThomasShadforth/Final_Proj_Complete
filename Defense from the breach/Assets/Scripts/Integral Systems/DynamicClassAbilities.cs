using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class DynamicClassAbilities : MonoBehaviour
{
    //Stores data, properties and behaviours for the base dynamic class.

    //The abilities that are currently selected.
    [Header("Selected Abilities")]
    public string selectedProjectile = "Ordnance - Spinning Orb";
    public string buffOrMissile = "Concentrated Spec - Missile Barrage";
    public string selectedStatBoost = "Strengthen - Weapon Damage";
    
    //Cooldowns for each ability (Base), and specific cooldowns for selected abilities
    [Header("Ability Cooldowns")]
    public float projectileCooldown;
    public float maxProjectileCooldown;
    public float almightyWaveCooldown;
    public float splitterCooldown;
    public float spinningOrbCooldown;
    public float concentratedShotCooldown;
    public float concentratedCooldownVal;

    //Timers for abilities (Attacks, buffs, etc.)
    [Header("Ability Timers")]
    public float concentratedShotTimer;
    public float projectileBuffTimer;
    public float projBuffTimerVal;
    public float weaponBuffTimer;
    public float wepBuffTimerVal;

    //Stores the properties for specific abilities
    [Header("Ability Properties")]
    //Damage multiplier for concentrated shot, how often it fires
    public float concentratedShotMultiplier;
    public float concentratedRateVal;
    public float concentratedRateOfFire;
    //The prefab that is instantiated when firing concentrated shot
    [SerializeField] GameObject concentratedBullet;
    //Stores the value for the weapon buff (If that ability is selected)
    public float weaponBuffVal;
    public float weaponBuffModifier;
    //Projectile buff value granted if using the missile barrage
    public float projectileModifier;
    //Array that stores the different projectile objects, and a variable that stores the currently selected one
    [SerializeField] GameObject[] selectedProjectileObject;
    [SerializeField] GameObject projectileObject;
    //How many missiles the player can fire, and the delay between firing each one
    public int missileStacks;
    public float missileDelay;
    
    //The prefab for the missile
    [Header("Required Prefabs")]
    public GameObject missileObject;

    //Transforms for the missile spawn positions, and the projectile spawn position
    [Header("Object Creation Properties")]
    [SerializeField] Transform missileLauncherOne;
    [SerializeField] Transform missileLauncherTwo;
    [SerializeField] Transform projectileThrowPos;

    //Whether or not buffs or the concentrated shot is currently active (The concentrated shot bool is public so that it may be accessed by the weapon script)
    [Header("Timer Booleans")]
    bool projBuffTimer;
    bool wepBuffTimer;
    public bool concentratedShotActive;

    //Reference to the weapon
    public WeaponTypes weapon;

    //Stores a string of the names of buffs that are currently active (for the UI)
    public string BuffUIString;
    //Stores the stacks of the missile (If the ability is selected, depending on if the player has any stacks at all)
    public string MissileStackBuffString;
    //Stores a list of the timers for buffs (For the UI)
    public List<float> timers = new List<float>();
    //Reference to the Buff UI text
    public TextMeshProUGUI BuffUI;

    //Determines whether or not the buff timers have ended (Determines whether they should be removed from the Buff UI)
    bool projectileBuffTimerReach = true;
    bool wepBuffTimerReach = true;


    // Start is called before the first frame update
    void Start()
    {
        //Set the concentrated shotCooldown, the buffTimers to their respective default values
        concentratedShotCooldown = concentratedCooldownVal;
        weaponBuffTimer = wepBuffTimerVal;
        projectileBuffTimer = projBuffTimerVal;
        //Set the current projectile object (Avoids errors)
        SetProjectileObject();
    }


    // Update is called once per frame
    void Update()
    {
        //If paused, or the tutorial is currently active, then do not execute any of the update loop's code.
        if (GamePause.paused)
        {
            return;
        }

        if(TutorialSystem.instance != null && (TutorialSystem.instance.isTutorialActive && !TutorialSystem.instance.doesTutHaveConditions && !TutorialSystem.instance.uiDisappeared))
        {
            return;
        }
        else if (TutorialSystem.instance != null && (TutorialSystem.instance.isTutorialActive && TutorialSystem.instance.doesTutHaveConditions && !TutorialSystem.instance.uiDisappeared))
        {
            return;
        }

        //Input checks for the abilities
        CheckForConcentratedShot();
        CheckForProjectiles();
        CheckForMissile();

        //If the projectile buff is active, tick the timer down according to game time
        if (projBuffTimer)
        {
            //If the timer hasn't run out, keep counting down
            if(projectileBuffTimer > 0)
            {
                projectileBuffTimer -= 1 * GamePause.deltaTime;
                
            }
            //Otherwise, set the timer bool to false, the modifier back to the default, 
            else
            {
                projBuffTimer = false;
                projectileModifier = 1;
                if (!projectileBuffTimerReach)
                {
                    //Remove the buff text from the UI
                    RemoveBuffFromUI("Projectile Buff");
                    projectileBuffTimerReach = true;
                }
            }
        }


        //If weapon buff is active, tick the timer down while it is greater than 0
        if (wepBuffTimer)
        {
            if(weaponBuffTimer > 0)
            {
                weaponBuffTimer -= 1 * GamePause.deltaTime;
                
            }
            else
            {
                //Otherwise, set time bool to false and modifier to default
                wepBuffTimer = false;
                weaponBuffModifier = 1;
                if (!wepBuffTimerReach)
                {
                    //Remove the buff text
                    RemoveBuffFromUI("Weapon Damage Buff");
                    wepBuffTimerReach = true;
                }
            }
        }

        //Set the UI

        SetUI();
    }

    #region Checks for Abilities
    void CheckForConcentratedShot()
    {
        //If the cooldown has finished, check for input
        if(concentratedShotCooldown <= 0)
        {
            //if the ability isn't active (Which it won't be)
            if(Input.GetButtonDown("Dodge") && !concentratedShotActive)
            {
                //Set ability to active, set the timer
                concentratedShotActive = true;
                concentratedShotTimer = 12;
                //Add the buff to the UI and add the timer to the list
                AddBuffToUI("Concentrated Shot");
                concentratedShotMultiplier = 1f;
                timers.Add(concentratedShotTimer);

                //Reset the missile stacks or buff modifier if they haven't been used
                if (buffOrMissile.Contains("Missile"))
                {
                    missileStacks = 0;
                }
                else
                {
                    weaponBuffVal = 1;
                }
            }
        }
        //Count down the cooldown
        else
        {
            concentratedShotCooldown -= GamePause.deltaTime;
        }

        //When the ability is active, check for the input to fire the concentrated shot
        if (concentratedShotActive && concentratedShotTimer > 0)
        {
            if (concentratedRateOfFire <= 0)
            {

                if (Input.GetButtonDown("Fire1"))
                {
                    //Fire method
                    ConcentratedShot();
                    //Set the cooldown for the rate of fire
                    concentratedRateOfFire = concentratedRateVal;

                }
            }
            //Count down the shot cooldown
            else
            {
                concentratedRateOfFire -= 1 * GamePause.deltaTime;
            }
            //Count down the ability timer
            concentratedShotTimer -= 1 * GamePause.deltaTime;
            //If the timer is 0 or less
        } else if(concentratedShotTimer <= 0 && concentratedShotActive)
        {
            //Remove buff text from the UI, set the ability to false, the cooldown to the respective value
            RemoveBuffFromUI("Concentrated Shot");
            concentratedShotActive = false;
            concentratedShotCooldown = concentratedCooldownVal;
            //If the player has the weapon buff active, trigger it automatically
            if (buffOrMissile.Contains("Enhanced"))
            {
                WeaponAttackBuff();
            }
        }
    }

    //Checks for the projectile ability input
    void CheckForProjectiles()
    {
        //If the cooldown is 0 or less, trigger the projectile
        if (Input.GetButtonDown("Buff"))
        {
            if (projectileCooldown <= 0)
            {
                CreateProjectile();
            }
        }

        //If the cooldown hasn't reached 0, count down
        if(projectileCooldown > 0)
        {
            projectileCooldown -= 1 * GamePause.deltaTime;
        }
        
    }

    void CheckForMissile()
    {
        //If the player inputs the button for the missiles while the ability is selected and has at least 1 missile stack, call the fireMissiles method
        if (Input.GetButtonDown("SuperJump"))
        {
            if (buffOrMissile.Contains("Missile"))
            {
                if (missileStacks > 0)
                {
                    FireMissiles();
                }
            }
            else
            {
                
            }
            
        }
    }
    #endregion

    #region Offensive Abilities
    //Instantiates the concentrated shot bullet, sets the damage and the velocity (Dependent on the weapon's current forward transform based on rotation)
    public void ConcentratedShot()
    {
        GameObject shotBullet = Instantiate(concentratedBullet, weapon.muzzleEnd.position, weapon.muzzleEnd.rotation);
        shotBullet.GetComponent<ConcentratedShotBullet>().setDamage(20, concentratedShotMultiplier);
        shotBullet.GetComponent<Rigidbody>().velocity = weapon.transform.forward * 15;
    }

    //Calls the MissileBarrage coroutine
    public void FireMissiles()
    {

        StartCoroutine(MissileBarrage(missileStacks));
        

    }
    public IEnumerator MissileBarrage(int missileStack)
    {
        //Accepts the player's missile stack as an argument
        //While the variable is greater than 0
        while(missileStack > 0)
        {
            //First, instantiate a missile object at the first missile spawn position using it's rotation
            GameObject missile = Instantiate(missileObject, missileLauncherOne.position, missileLauncherOne.rotation);
            //Pass the projectile modifier to set the damage (It has a built-in default damage)
            missile.GetComponent<MissileBarrageProj>().SetDamage(projectileModifier);
            //Set the velocity
            missile.GetComponent<Rigidbody>().velocity = missile.transform.forward * 10;
            //Reduce missileStack by 1
            missileStack--;
            //Wait for the missile delay
            yield return new WaitForSeconds(missileDelay);

            //If there are no missiles left, restart the loop (Which will end as missileStack is no longer greater than 0)
            if(missileStack <= 0)
            {
                continue;
            }
            //Otherwise, spawn the next missile at the second position using the corresponding rotation
            else
            {
                missile = Instantiate(missileObject, missileLauncherTwo.position, missileLauncherTwo.rotation);
                missile.GetComponent<MissileBarrageProj>().SetDamage(projectileModifier);
                missile.GetComponent<Rigidbody>().velocity = missile.transform.forward * 10;
                //Set speed, damage, etc
                missileStack--;
                yield return new WaitForSeconds(missileDelay);
                //Either check again, or simply leave it to the loop condition
            }
        }
        //Set the projectile buff timer to true
        projBuffTimer = true;
        //Add the buff to the UI
        AddBuffToUI("Projectile Buff");
        //Set the timer to the corresponding value
        projectileBuffTimer = projBuffTimerVal;
        //Add the timer
        timers.Add(projectileBuffTimer);
        projectileBuffTimerReach = false;
        //Set missileStacks to 0
        missileStacks = 0;
        //Attack ends, start timer for projectile buff
    }
    #endregion

    #region Projectiles and Bombs
    //Instantiate the projectile based on the currently selected object
    void CreateProjectile()
    {
        //Set the velocity after instantiating
        GameObject spawnedProjectile = Instantiate(projectileObject, projectileThrowPos.position, projectileThrowPos.rotation);
        spawnedProjectile.GetComponent<Rigidbody>().velocity = transform.forward * 20f;

        //Depending on the selected projectile, set the damage and the cooldown accordingly
        switch (selectedProjectile)
        {
            case "Ordnance - Spinning Orb":
                spawnedProjectile.GetComponent<SpinningOrb>().damage = spawnedProjectile.GetComponent<SpinningOrb>().damage * projectileModifier;
                projectileCooldown = spinningOrbCooldown;
                break;
            case "Ordnance - Splitter Frag":
                spawnedProjectile.GetComponent<SplitterFragBase>().baseDamage = spawnedProjectile.GetComponent<SplitterFragBase>().baseDamage * projectileModifier;
                spawnedProjectile.GetComponent<SplitterFragBase>().fragPieceDamage = spawnedProjectile.GetComponent<SplitterFragBase>().fragPieceDamage * projectileModifier;
                projectileCooldown = splitterCooldown;
                break;
            case "Ordnance - Almighty Wave":
                spawnedProjectile.GetComponent<AlmightyWaveBase>().baseDamage = spawnedProjectile.GetComponent<AlmightyWaveBase>().baseDamage * projectileModifier;
                
                projectileCooldown = almightyWaveCooldown;
                break;
        }
    }
    #endregion

    #region Post Ability Buffs
    //Sets the weapon damage buff after concentrated shot ends (If selected)
    public void WeaponAttackBuff()
    {
        //Set the buff modifier (This directly impacts the weapon damage)
        weaponBuffModifier = weaponBuffVal;
        //Set the weapon's damage modifier
        weapon.damageModifier = weaponBuffModifier;
        //Set the bufftimer
        weaponBuffTimer = wepBuffTimerVal;
        //Add the buff to the UI and the timer to the list
        AddBuffToUI("Weapon Damage Buff");
        timers.Add(weaponBuffTimer);
        //Set buffTimer to true and timerReach to false
        wepBuffTimerReach = false;
        wepBuffTimer = true;
    }

    #endregion

    #region Ability Hit Methods
    //Increase the concentrated shot multiplier slightly (Affects overall damage)
    public void increaseConcentratedDamage()
    {
        concentratedShotMultiplier += .2f;
    }


    public void increaseMissileBuffStack()
    {
        //If the weapon buff is selected, increase the overall modifier
        //If the missile is selected, increase it by 1 per concentrated shot hit
        switch (buffOrMissile)
        {
            case "Concentrated Spec - Enhanced Might":
                
                weaponBuffVal += .2f;
                break;
            case "Concentrated Spec - Missile Barrage":
                missileStacks += 1;
                break;
            default:
                break;
        }
    }

    //Increase the projectile modifier on missile hit
    public void increaseProjectilesModifier()
    {
        projectileModifier += .2f;
    }

    //Sets abilities based on what is selected
    public void SetAbility(string abilityToSet)
    {
        //Sets the ability to the required variable based on the naming convention
        if(abilityToSet.Contains("Concentrated Spec"))
        {
            buffOrMissile = abilityToSet;
        } else if (abilityToSet.Contains("Strengthen"))
        {
            selectedStatBoost = abilityToSet;
        } else if (abilityToSet.Contains("Ordnance"))
        {
            //Set the projectile object after setting the string
            selectedProjectile = abilityToSet;
            SetProjectileObject();
        }

        //Updates the gameplay UI
        PlayerUI.instance.UpdateGameplayAbilityUI(true);
    }

    //Sets the projectile based on the selected projectile string
    public void SetProjectileObject()
    {
        //sets the object and the maxCooldown based on the string (maxProjectileCooldown is used in the gameplay UI)
        if (selectedProjectile == "Ordnance - Almighty Wave")
        {
            projectileObject = selectedProjectileObject[1];
            maxProjectileCooldown = almightyWaveCooldown;
        }
        else if (selectedProjectile == "Ordnance - Spinning Orb")
        {
            projectileObject = selectedProjectileObject[0];
            maxProjectileCooldown = spinningOrbCooldown;
        }
        else if(selectedProjectile == "Ordnance - Splitter Frag")
        {
            projectileObject = selectedProjectileObject[2];
            maxProjectileCooldown = splitterCooldown;
        }
    }
    #endregion

    #region Utility Methods
    public void SetUI()
    {
        //Set the buffUI text to blank every time (Prevents it from stacking endlessly)
        BuffUI.text = "";

        //If the missile stack is selected and the player has at least 1, place missile stacks at the start of the UI string
        if(buffOrMissile.Contains("Missile") && missileStacks > 0)
        {
            MissileStackBuffString = "Missile Stacks: " + missileStacks;
        }
        else
        {
            //Set it to blank
            MissileStackBuffString = "";
        }

        //If the normal buff string is not blank, execute this block of code
        if (BuffUIString != "" || MissileStackBuffString != "")
        {
            //Split the string into a string array using string.split

            string[] buffList = BuffUIString.Split(',');

            //Convert the array into a list
            List<string> buffs = buffList.ToList<string>();

            //If the missile stack string is not blank, add it to the start of the UI
            if(MissileStackBuffString != "")
            {
                BuffUI.text = BuffUI.text + MissileStackBuffString + "\n";
            }

            //For each buff, if buff isn't blank, add it and it's respective timer to the UI.
            foreach (string buff in buffs)
            {
                if (buff != "")
                {
                    BuffUI.text = BuffUI.text + buff + ": " + ReduceListTimer(BuffUIString, buff) + "\n";
                }
            }
        }
    }

    //Adds the buff name to the UIString followed by a comma (Used for splitting the array)
    public void AddBuffToUI(string stringToAdd)
    {

        BuffUIString += $"{stringToAdd},";
    }


    public void RemoveBuffFromUI(string stringToRemove)
    {
        //Split UIString into a string array, and convert it to a list
        string[] buffList = BuffUIString.Split(',');

        List<string> buffs = buffList.ToList<string>();



        /*
        for(int i = 0; i < buffList.Length; i++)
        {
            if(buffList[i] == stringToRemove)
            {
                Debug.Log("FOUND!");
                buffList[i].Remove(0);

                Debug.Log(buffList.Length);

                buffList = ShiftBuffNames(buffList);
            }
        }*/

        //If the buff at the index is the string being removed, remove it from the list
        for (int i = 0; i < buffs.Count; i++)
        {
            if (buffs[i] == stringToRemove)
            {
                buffs.Remove(stringToRemove);

                //Set the list to have the values shifted up (Pass in the list as an array and convert the return value to a list)
                buffs = ShiftBuffNames(buffs.ToArray<string>()).ToList<string>();
            }

        }

        //Convert the list into an array and turn it back into a string value
        BuffUIString = GetStringFromArray(buffs.ToArray<string>());
    }

    private string GetStringFromArray(string[] buffList)
    {
        //Initialises a string varaible
        string newBuffString;
        if (buffList.Length != 0 && buffList.Length > 1)
        {
            //If the length is greater than 1 and 0, set the newbuffString to the joined array
            newBuffString = string.Join(",", buffList);
        }
        else
        {
            //Otherwise, return a blank string
            newBuffString = "";
        }



        return newBuffString;
    }

    //Accepts the array of buff names as a parameter
    public string[] ShiftBuffNames(string[] names)
    {
        //If the length is greater than 1 and the first index is not blank
        if (names.Length != 1 && names[0] != "")
        {

            for (int i = 0; i < names.Length; i++)
            {
                //If the current entry is blank
                if (names[i] == "")
                {
                    //check if the next index is the array's length.
                    //If it is, break the loop
                    if (i + 1 == names.Length)
                    {
                        break;
                    }
                    //Otherwise, set the current index to the next index's value
                    names[i] = names[i + 1];
                    //Set the next index's value to be blank
                    names[i + 1] = "";
                }
                else
                {
                    continue;
                }
            }
        }

        //Return the array
        return names;
    }

    public float ReduceListTimer(string buffString, string buffName)
    {
        //Split the buffString into an array
        string[] listOfBuffs = buffString.Split(',');

        for (int i = 0; i < listOfBuffs.Length; i++)
        {
            Debug.Log(listOfBuffs[i]);
            //If the name is the same as the entry and the name isn't blank, reduced the timer by game time
            if (listOfBuffs[i] == buffName && buffName != "")
            {
                timers[i] -= GamePause.deltaTime;


                //If the timer at the index is 0, remove it from the timers list
                if (timers[i] <= 0)
                {
                    timers.Remove(timers[i]);
                }

                //Round the value for the UI (For clean UI)
                return Mathf.Round(timers[i]);
            }
        }

        //Otherwise, return 0
        return 0f;
    }

    

    #endregion
}
