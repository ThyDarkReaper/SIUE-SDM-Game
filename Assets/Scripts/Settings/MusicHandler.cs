using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    private AudioSource audioSource;
    public GlobalVariables GV;
    public AudioClip song;
    private Camera currentCamera;
    
    void Start()
    {
        if (GV == null)
            GV = FindObjectOfType<GlobalVariables>();
        
        if (song == null)
        {
            Debug.LogError("Song not assigned!");
            return;
        }
        
        // Initial attach
        FindAndAttachToActiveCamera();
    }
    
    void Update()
    {
        // Check if current camera is still valid and active
        bool needNewCamera = false;
        
        if (currentCamera == null)
            needNewCamera = true;
        else if (!currentCamera.gameObject.activeInHierarchy)
            needNewCamera = true;
        else if (audioSource == null)
            needNewCamera = true;
        
        if (needNewCamera)
        {
            Debug.Log("Camera lost or disabled, searching for new active camera...");
            FindAndAttachToActiveCamera();
        }
        
        // Update volume
        if (audioSource != null && GV != null)
        {
            audioSource.volume = GV.getAudioVolume() / 10f;
        }
    }
    
    void FindAndAttachToActiveCamera()
    {
        Camera[] cameras = FindObjectsOfType<Camera>(true); // true includes inactive!
        
        foreach (Camera cam in cameras)
        {
            // Check if camera is active in hierarchy
            if (cam.gameObject.activeInHierarchy)
            {
                // Don't switch to the same camera
                if (currentCamera == cam)
                    return;
                
                currentCamera = cam;
                
                // Get or add AudioSource
                audioSource = cam.GetComponent<AudioSource>();
                if (audioSource == null)
                    audioSource = cam.gameObject.AddComponent<AudioSource>();
                
                // Configure
                audioSource.clip = song;
                audioSource.loop = true;
                audioSource.volume = GV != null ? GV.getAudioVolume() / 10f : 0.5f;
                audioSource.Play();
                
                Debug.Log($"Music now playing on: {cam.name}");
                return;
            }
        }
    }
}