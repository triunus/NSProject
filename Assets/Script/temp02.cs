using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temp02 : MonoBehaviour
{
    private RectTransform my;

    private void Awake()
    {
        this.my = this.GetComponent<RectTransform>();
        Debug.Log(this.my.GetSiblingIndex());

        this.my.SetAsFirstSibling();
        Debug.Log(this.my.GetSiblingIndex());

        this.my.SetAsLastSibling();
        Debug.Log(this.my.GetSiblingIndex());

        this.my.SetSiblingIndex(3);
        Debug.Log(this.my.GetSiblingIndex());
    }
}
