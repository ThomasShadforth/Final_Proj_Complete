using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UpgradePointIndicator : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI UIText;
    public float destroyTime;


    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(PlayerBase.instance.transform);
        transform.position += new Vector3(0f, 1f * GamePause.deltaTime, 0f);
    }

    public void SetUI(float pointVal)
    {
        UIText.text = "" + pointVal;
        Invoke("DestroyObject", destroyTime);
    }

    public void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
