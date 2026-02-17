using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [Header("Drag & Drop Target Here")]
    public GameObject objectToRotate; // Drag any GameObject here in Inspector
    
    [Header("Rotation Settings")]
    public float speed = 60f;
    public float targetAngle = 90f;
    
    private Coroutine rotationCoroutine;
    
    // Call this from any script to start rotation
    public void RotateAssignedObject()
    {
        if (objectToRotate == null)
        {
            Debug.LogError("No GameObject assigned to rotate!", this);
            return;
        }
        
        if (rotationCoroutine != null)
        {
            StopCoroutine(rotationCoroutine);
        }
        
        rotationCoroutine = StartCoroutine(RotateObjectCoroutine());
    }
    
    IEnumerator RotateObjectCoroutine()
    {
        Transform targetTransform = objectToRotate.transform;
        float currentY = targetTransform.eulerAngles.y;
        
        // Continue rotating until we reach the target angle
        while (currentY < targetAngle)
        {
            // Calculate rotation amount for this frame
            float rotationAmount = speed * Time.deltaTime;
            
            // Rotate the assigned GameObject
            targetTransform.Rotate(0f, rotationAmount, 0f, Space.World);
            
            // Update current rotation
            currentY = targetTransform.eulerAngles.y;
            
            yield return null; // Wait for next frame
        }
        
        // Ensure exact target angle
        targetTransform.eulerAngles = new Vector3(
            targetTransform.eulerAngles.x,
            targetAngle,
            targetTransform.eulerAngles.z
        );
    }
}
