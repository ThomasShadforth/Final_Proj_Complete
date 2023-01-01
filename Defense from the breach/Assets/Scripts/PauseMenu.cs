using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject MainPauseWindow;
    public GameObject[] menuWindows;
    public GameObject[] classWindows;
    public Text upgradePoints;
    public static PauseMenu instance;

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
            DontDestroyOnLoad(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (!MainPauseWindow.activeInHierarchy)
            {
                //Open The Pause Menu, Pause the game
                MainPauseWindow.SetActive(true);
                menuWindows[0].SetActive(true);
                GamePause.paused = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                //Close the pause menu, un-pause the game

                //Include loop that closes all pre-existing windows before backing out of the menu
                for(int i = 0; i < menuWindows.Length; i++)
                {
                    menuWindows[i].SetActive(false);
                }

                MainPauseWindow.SetActive(false);
                GamePause.paused = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        upgradePoints.text = "Upgrade Points: " + GameManager.instance.upgradePoints;
    }

    public void OpenWindow(int windowIndex)
    {
        for (int i = 0; i < menuWindows.Length; i++)
        {
            if (i == windowIndex)
            {
                menuWindows[i].SetActive(true);
                if(windowIndex == 1)
                {
                    if(PlayerBase.instance.selectedClass == classType.Simple)
                    {
                        classWindows[0].SetActive(true);
                        classWindows[1].SetActive(false);
                    }
                    else
                    {
                        classWindows[0].SetActive(false);
                        classWindows[1].SetActive(true);
                    }
                }
            }
            else
            {
                menuWindows[i].SetActive(false);
            }
        }
    }

    public void closeMenu()
    {
        for (int i = 0; i < menuWindows.Length; i++)
        {
            menuWindows[i].SetActive(false);
        }

        MainPauseWindow.SetActive(false);
        GamePause.paused = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void selectClass(int classIndex)
    {
        classType[] classes = (classType[])System.Enum.GetValues(typeof(classType));
        
        
            
        if (classes[classIndex] != PlayerBase.instance.selectedClass)
        {
            PlayerBase.instance.selectedClass = classes[classIndex];
            PlayerBase.instance.activateClass(classes[classIndex]);

            for(int i = 0; i < classes.Length; i++)
            {
                if(i == classIndex)
                {
                    classWindows[classIndex].SetActive(true);
                }
                else
                {
                    classWindows[i].SetActive(false);
                }
            }
            
        }
        
        
    }
}
