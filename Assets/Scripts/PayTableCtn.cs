using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PayTableCtn : MonoBehaviour
{
    [SerializeField]
    private Transform btn;
    bool flag;

    private void Start()
    {
        GameObject.Find("PayTableCtn/Canvas/Image").transform.Find("PayLinesImage").gameObject.SetActive(false);
        flag = false;
    }

    private void OnMouseDown()
    {
        //빌드이후에 페이테이블이 나오지 않고 있음
        if (flag == false)
        {
            GameObject.Find("PayTableCtn/Canvas/Image").transform.Find("A").gameObject.SetActive(false);
            GameObject.Find("PayTableCtn/Canvas/Image").transform.Find("B").gameObject.SetActive(true);
            GameObject.Find("PayTableCtn/Canvas/Image").transform.Find("PayLinesImage").gameObject.SetActive(true);
            flag = true;
        }
        else
        {
            GameObject.Find("PayTableCtn/Canvas/Image").transform.Find("A").gameObject.SetActive(true);
            GameObject.Find("PayTableCtn/Canvas/Image").transform.Find("B").gameObject.SetActive(false);
            GameObject.Find("PayTableCtn/Canvas/Image").transform.Find("PayLinesImage").gameObject.SetActive(false);
            flag = false;
        }
    }
}
