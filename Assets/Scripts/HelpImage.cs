using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HelpImage : MonoBehaviour
{
    [SerializeField] private GameObject imageToHide;
    private Button thisButton;
    
    void Start()
    {
        thisButton = GetComponent<Button>();
        
        if (thisButton != null)
        {
            thisButton.onClick.AddListener(HideBoth);
        }
    }
    
    void HideBoth()
    {
        // Hide the image if assigned
        if (imageToHide != null)
        {
            imageToHide.SetActive(false);
        }
        
        // Hide the button itself
        thisButton.gameObject.SetActive(false);
        
        Debug.Log("Button and Image have been hidden");
    }
}