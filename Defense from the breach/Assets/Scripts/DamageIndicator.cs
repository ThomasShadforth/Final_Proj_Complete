using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageIndicator : MonoBehaviour
{
    public float destroyTime;
    [SerializeField] TextMeshProUGUI damageText;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyObject", destroyTime);
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y + 2 * GamePause.deltaTime, transform.position.z);
        
    }

    public void setDamage(float damage)
    {
        damageText.text = "" + damage;
    }

    void DestroyObject()
    {
        Destroy(gameObject);
    }
}
