using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSystem : MonoBehaviour
{
    public static TutorialSystem instance;

    public GameObject tutorialUIPanel;
    public GameObject[] TutorialPopUp;

    [SerializeField] GameObject[] firstEnemies;
    [SerializeField] GameObject[] secondEnemySet;
    [SerializeField] GameObject BossEnemy;

    

    int tutorialIndex;
    public bool doesTutHaveConditions;
    public bool isTutorialActive;
    public bool uiDisappeared = false;

    [Header("Tutorial Conditions")]
    bool bossDefeated;

    float noInputTimer = 1f;

    // Start is called before the first frame update
    void Start()
    {
        if(instance != null)
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
        CheckTutorialStatus();
    }

    void CheckTutorialStatus()
    {
        if (isTutorialActive)
        {
            if (!doesTutHaveConditions)
            {
                if (noInputTimer <= 0)
                {
                    if (Input.anyKey)
                    {
                        Invoke("CloseTutorialPanel", 3f);
                    }
                }
                else
                {
                    noInputTimer -= GamePause.deltaTime;
                }
            }
            else
            {
                if (noInputTimer <= 0)
                {
                    if (Input.anyKey)
                    {
                        if (!uiDisappeared)
                        {
                            Invoke("CloseConditionPanel", 3f);
                        }
                    }
                }
                else
                {
                    noInputTimer -= GamePause.deltaTime;
                }

                bool conditionFulfilled = checkCondition();

                if (conditionFulfilled)
                {
                    
                    isTutorialActive = false;

                    if(tutorialIndex == 1 || tutorialIndex == 3 || tutorialIndex == 5)
                    {
                        
                        triggerTutorial(tutorialIndex + 1, false);
                    }
                }
            }
        }
    }

    private void CloseTutorialPanel()
    {
        TutorialPopUp[tutorialIndex].SetActive(false);
        isTutorialActive = false;
        tutorialUIPanel.SetActive(false);
    }

    void CloseConditionPanel()
    {
        TutorialPopUp[tutorialIndex].SetActive(false);
        tutorialUIPanel.SetActive(false);
        uiDisappeared = true;
    }

    public void triggerTutorial(int Index, bool hasCondition)
    {
        tutorialIndex = Index;
        isTutorialActive = true;
        
        
        doesTutHaveConditions = hasCondition;
        if (!GameManager.instance.tutorialTriggers[tutorialIndex])
        {
            uiDisappeared = false;
            tutorialUIPanel.SetActive(true);
            TutorialPopUp[tutorialIndex].SetActive(true);
            GameManager.instance.tutorialTriggers[tutorialIndex] = true;
        }
    }


    public void TutorialUpgradeBought()
    {

    }

    bool checkCondition()
    {
        if(tutorialIndex == 1)
        {
            return CheckEnemiesStatus(firstEnemies);
        } else if(tutorialIndex == 3)
        {
            return CheckEnemiesStatus(secondEnemySet);
        } else if(tutorialIndex == 5)
        {
            return CheckBossStatus();
        }
        return false;
    }

    bool CheckEnemiesStatus(GameObject[] enemiesToCheck)
    {
        bool enemiesDead = true;

        foreach(GameObject enemy in enemiesToCheck)
        {
            if(enemy != null)
            {
                enemiesDead = false;
            }
        }

        return enemiesDead;
    }

    bool CheckBossStatus()
    {
        bool bossDead = true;

        if(BossEnemy != null)
        {
            bossDead = false;
        }

        return bossDead;
    }
}
