using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyUI : MonoBehaviour
{
    EnemyAI attachedEnemy;
    [SerializeField]
    Image enemyHealthBar;
    void Start()
    {
        attachedEnemy = GetComponentInParent<EnemyAI>();
    }

    // Update is called once per frame
    void Update()
    {
        enemyHealthBar.fillAmount = attachedEnemy.enemyHealth / attachedEnemy.enemyMaxHealth;

        if(enemyHealthBar.fillAmount < .4f)
        {
            enemyHealthBar.color = Color.red;
        }
        else
        {
            enemyHealthBar.color = Color.green;
        }

        transform.LookAt(PlayerBase.instance.transform);
    }
}
