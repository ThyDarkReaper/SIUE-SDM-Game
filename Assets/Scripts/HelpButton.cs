using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HelpButton : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    
    private Button button;
    private bool isVisible = false;
    
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(ToggleBoth);
        
        // Start with both hidden
        if (targetImage != null)
            targetImage.gameObject.SetActive(false);

    }
    
    void ToggleBoth()
    {
        isVisible = !isVisible;
        
        if (targetImage != null)
            targetImage.gameObject.SetActive(isVisible);

    }
}