using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HelpButton : MonoBehaviour
{
    [SerializeField] private GameObject imageToToggle;
    [SerializeField] private GameObject textToToggle;
    
    private Button thisButton;
    private bool isVisible = false;
    
    void Start()
    {
        thisButton = GetComponent<Button>();
        
        if (thisButton != null)
        {
            thisButton.onClick.AddListener(ToggleBoth);
        }
        
        // Start with both hidden
        SetVisibility(false);
    }
    
    void ToggleBoth()
    {
        isVisible = !isVisible;
        SetVisibility(isVisible);
    }
    
    void SetVisibility(bool visible)
    {
        if (imageToToggle != null)
            imageToToggle.SetActive(visible);
            
        if (textToToggle != null)
            textToToggle.SetActive(visible);
            
        Debug.Log("Image and Text visibility: " + visible);
    }
}