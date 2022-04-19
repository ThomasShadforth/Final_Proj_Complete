using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdvancedEnemyUI : MonoBehaviour
{
    AdvancedEnemyAI attachedEnemy;
    [SerializeField] Image enemyHealthBar;
    [SerializeField] Image enemyArmourBar;
    // Start is called before the first frame update
    void Start()
    {
        attachedEnemy = GetComponentInParent<AdvancedEnemyAI>();

        if (!attachedEnemy.hasArmour)
        {
            enemyArmourBar.gameObject.SetActive(false);
        }
        else
        {
            enemyArmourBar.gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        enemyHealthBar.fillAmount = attachedEnemy.enemyHealth / attachedEnemy.enemyMaxHealth;

        if (enemyArmourBar.gameObject.activeInHierarchy)
        {
            enemyArmourBar.fillAmount = attachedEnemy.ArmourDurability / attachedEnemy.defaultArmDurability;
        }

        if (Vector3.Distance(attachedEnemy.transform.position, PlayerBase.instance.transform.position) < attachedEnemy.detectRadius)
        {

            transform.LookAt(PlayerBase.instance.transform);
        }
    }
}
