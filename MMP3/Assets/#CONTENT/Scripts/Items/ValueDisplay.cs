using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor.VersionControl;
using TMPro.EditorUtilities;

public class ValueDisplay : MonoBehaviour
{
    [Header("Display Settings")]
    [HideInInspector] public int itemValue;
    public string currencySymbol = "$";
    public float minDisplayDistance = 2f;
    public float maxDisplayDistance = 10f;
    public float minTextSize = 0.5f;
    public float maxTextSize = 1.5f;
    
    [Header("Text Settings")]
    public float fontSize = 4f;
    public float heightOffset = 1f;

    // References
    private TextMeshPro valueText;
    private GameObject textObject;
    
    // Cache all cameras in the scene
    private List<Camera> allCameras = new List<Camera>();

    private void Awake()
    {
        // Try to load the TMP font before creating the text
    }

    private void Start()
    {
        FindAllCameras();
        CreateValueDisplay();
    }

    private void OnEnable()
    {
        if (textObject != null)
            textObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (textObject != null)
            textObject.SetActive(false);
    }

    private void CreateValueDisplay()
    {
        // Create a new game object for the value text
        textObject = new GameObject("ValueText");
        textObject.transform.SetParent(transform);
        textObject.transform.localPosition = new Vector3(0, heightOffset, 0); // Position above object

        // Add TextMeshPro component
        valueText = textObject.AddComponent<TextMeshPro>();
        valueText.text = currencySymbol + itemValue.ToString();
        valueText.fontSize = fontSize;
        valueText.alignment = TextAlignmentOptions.Center;
        
        // Set color based on value
        SetTextColor();
        
        // Make text visible from both sides - will use the default material
        valueText.outlineWidth = 0.2f;
        valueText.outlineColor = new Color(0, 0, 0, 0.5f);
    }

    private void SetTextColor()
    {
        if (valueText == null) return;
        
        // Set color based on value range
        if (itemValue >= 500)
        {
            valueText.color = new Color(1f, 0.84f, 0f); // Gold for high value
        }
        else if (itemValue >= 200)
        {
            valueText.color = new Color(0.75f, 0.75f, 0.75f); // Silver for medium value
        }
        else
        {
            valueText.color = new Color(0.72f, 0.45f, 0.2f); // Bronze for low value
        }
    }

    private void FindAllCameras()
    {
        // Clear the list and find all cameras
        allCameras.Clear();

        // Find all active cameras in the scene
        Camera[] cameras = Camera.allCameras;
        foreach (Camera camera in cameras)
        {
            allCameras.Add(camera);
        }

        // If no cameras were found, try again in the next frame
        if (allCameras.Count == 0)
        {
            Invoke("FindAllCameras", 0.5f);
        }
    }

    private void LateUpdate()
    {
        if (valueText == null || textObject == null) return;

        // Periodically check for new cameras (e.g., if a new player joins)
        if (Time.frameCount % 60 == 0) // Check every 60 frames
        {
            FindAllCameras();
        }

        // Find the closest camera
        Camera closestCamera = FindClosestCamera();
        if (closestCamera == null) return;

        // Calculate distance to closest camera
        float distanceToCamera = Vector3.Distance(transform.position, closestCamera.transform.position);

        // Update text visibility and size based on distance
        UpdateTextAppearance(distanceToCamera);

        // Make the text face the camera
        textObject.transform.LookAt(2 * textObject.transform.position - closestCamera.transform.position);
    }

    private Camera FindClosestCamera()
    {
        Camera closest = null;
        float minDistance = float.MaxValue;

        foreach (Camera camera in allCameras)
        {
            if (camera == null) continue;

            float distance = Vector3.Distance(transform.position, camera.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = camera;
            }
        }

        return closest;
    }

    private void UpdateTextAppearance(float distance)
    {
        if (distance > maxDisplayDistance || distance < minDisplayDistance)
        {
            // Hide text if too far or too close
            valueText.enabled = false;
        }
        else
        {
            // Show text when in the right distance range
            valueText.enabled = true;

            // Calculate normalized distance (0-1) within our display range
            float normalizedDistance = (distance - minDisplayDistance) / (maxDisplayDistance - minDisplayDistance);

            // Invert for size calculation (closer = bigger)
            float sizeRatio = 1 - normalizedDistance;

            // Calculate text size (larger when closer)
            float textSize = Mathf.Lerp(minTextSize, maxTextSize, sizeRatio);
            textObject.transform.localScale = Vector3.one * textSize;

            // Apply a fade effect at the min/max edges
            float alpha = 1.0f;
            if (normalizedDistance < 0.1f)
            {
                alpha = normalizedDistance / 0.1f; // Fade in at min distance
            }
            else if (normalizedDistance > 0.9f)
            {
                alpha = (1 - normalizedDistance) / 0.1f; // Fade out at max distance
            }

            // Apply alpha to text
            Color textColor = valueText.color;
            textColor.a = alpha;
            valueText.color = textColor;
        }
    }
}
