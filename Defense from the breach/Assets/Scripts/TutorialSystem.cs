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

    [Header("Tutorial Conditions")]
    bool bossDefeated;


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
                if (Input.anyKey)
                {
                    Invoke("CloseTutorialPanel", 3f);
                }
            }
            else
            {
                bool conditionFulfilled = checkCondition();

                if (conditionFulfilled)
                {
                    TutorialPopUp[tutorialIndex].SetActive(false);
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

    public void triggerTutorial(int Index, bool hasCondition)
    {
        tutorialIndex = Index;
        isTutorialActive = true;
        doesTutHaveConditions = hasCondition;
        tutorialUIPanel.SetActive(true);
        TutorialPopUp[tutorialIndex].SetActive(true);
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
