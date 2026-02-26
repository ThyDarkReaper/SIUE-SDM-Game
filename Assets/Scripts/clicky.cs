using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class clicky : MonoBehaviour
{
    public Button medVial;
    // Start is called before the first frame update
    void Start()
    {
        medVial.onClick.AddListener(OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClick()
    {
        Debug.Log("Button Pressed");
    }
}
