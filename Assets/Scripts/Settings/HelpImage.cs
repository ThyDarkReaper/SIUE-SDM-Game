using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class HelpImage : MonoBehaviour
{
    [SerializeField] private Button targetButton;
    [SerializeField] private Image targetImage;
    
    private void OnEnable()
    {
        // Add listener when object becomes active
        if (targetButton != null && targetImage != null)
        {
            targetButton.onClick.AddListener(HideImage);
        }
    }
    
    private void OnDisable()
    {
        // Remove listener to prevent memory leaks (good practice)
        if (targetButton != null && targetImage != null)
        {
            targetButton.onClick.RemoveListener(HideImage);
        }
    }
    
    private void HideImage()
    {
        if (targetImage != null)
        {
            targetImage.gameObject.SetActive(false);
        }
    }
}