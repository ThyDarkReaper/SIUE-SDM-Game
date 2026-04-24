using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MovementHelp : MonoBehaviour
{
    [SerializeField] private GameObject imageToHide;
    [SerializeField] private Button hideButton;
    
    private const string IMAGE_STATE_KEY = "MovementHelp_ImageHidden";
    private bool isImageHidden = false;
    
    void Awake()
    {
        // Make this object persist across scenes
        DontDestroyOnLoad(gameObject);
        
        // Subscribe to scene load event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    void Start()
    {
        // Load saved state
        LoadState();
        
        // Apply the state to current objects
        ApplyState();
        
        // Add button listener
        if (hideButton != null)
        {
            hideButton.onClick.AddListener(HideImageAndButton);
        }
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Re-find references in the new scene
        FindReferences();
        
        // Apply the saved state to new scene objects
        ApplyState();
    }
    
    void FindReferences()
    {
        // Try to find image by tag first, then by name
        if (imageToHide == null)
        {
            imageToHide = GameObject.FindGameObjectWithTag("TargetImage");
            
            if (imageToHide == null)
            {
                imageToHide = GameObject.Find("TargetImage");
            }
        }
        
        // Try to find button
        if (hideButton == null)
        {
            hideButton = GetComponent<Button>();
            
            if (hideButton == null)
            {
                hideButton = FindObjectOfType<Button>();
            }
        }
    }
    
    void HideImageAndButton()
    {
        isImageHidden = true;
        SaveState();
        ApplyState();
    }
    
    void LoadState()
    {
        // Load saved state (default = false = visible)
        isImageHidden = PlayerPrefs.GetInt(IMAGE_STATE_KEY, 0) == 1;
    }
    
    void SaveState()
    {
        PlayerPrefs.SetInt(IMAGE_STATE_KEY, isImageHidden ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    void ApplyState()
    {
        // Hide or show the image
        if (imageToHide != null)
        {
            imageToHide.SetActive(!isImageHidden);
        }
        
        // Hide or show the button
        if (hideButton != null)
        {
            hideButton.gameObject.SetActive(!isImageHidden);
        }
    }
    
    void OnDestroy()
    {
        // Unsubscribe from events - just use -= without null check
        SceneManager.sceneLoaded -= OnSceneLoaded;
        
        // Remove button listener
        if (hideButton != null)
        {
            hideButton.onClick.RemoveListener(HideImageAndButton);
        }
    }
    
    // Public method to reset (call from a reset button if needed)
    public void ResetState()
    {
        isImageHidden = false;
        PlayerPrefs.DeleteKey(IMAGE_STATE_KEY);
        ApplyState();
    }
}