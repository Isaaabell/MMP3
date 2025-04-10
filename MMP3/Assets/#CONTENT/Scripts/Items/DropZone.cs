using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropZone : MonoBehaviour
{
    [Header("Visual Settings")]
    [Tooltip("The color of the drop zone when inactive")]
    public Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 0.3f);
    [Tooltip("The color of the drop zone when an item is hovering over it")]
    public Color activeColor = new Color(0.0f, 1.0f, 0.0f, 0.5f);
    [Tooltip("The message to display when an item is dropped")]
    public string dropMessage = "Dropped!";
    [Tooltip("Duration to show the message in seconds")]
    public float messageDuration = 2f;
    
    [Header("Particle Effects")]
    [Tooltip("Particle effect to play when an item is dropped")]
    public GameObject dropEffectPrefab;
    
    [Header("Audio")]
    [Tooltip("Sound to play when an item is collected")]
    public AudioClip dropSound;
    [Range(0f, 1f)]
    public float volume = 0.5f;
    
    // Private references
    private Renderer zoneRenderer;
    private TextMeshPro messageText;
    private AudioSource audioSource;
    private List<ObjectGrabbable> itemsInZone = new List<ObjectGrabbable>();
    
    private void Awake()
    {
        // Get or add required components
        zoneRenderer = GetComponent<Renderer>();
        if (zoneRenderer != null)
        {
            // Set initial color
            SetZoneColor(inactiveColor);
        }
        
        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null && dropSound != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 1.0f; // Make it 3D sound
            audioSource.volume = volume;
            audioSource.pitch = Random.Range(0.9f, 1.1f); // Slight pitch variation
        }
        
        // Create floating text for messages
        CreateMessageText();
    }
    
    private void CreateMessageText()
    {
        // Create a GameObject for the message text
        GameObject textObj = new GameObject("DropZone_Message");
        textObj.transform.SetParent(transform);
        textObj.transform.localPosition = new Vector3(0, 1.5f, 0); // Position above drop zone
        
        // Add TextMeshPro component
        messageText = textObj.AddComponent<TextMeshPro>();
        messageText.alignment = TextAlignmentOptions.Center;
        messageText.fontSize = 10;
        messageText.text = "";
        
        // Make it face the camera
        textObj.AddComponent<LookAtCamera>(); // Assuming you have this helper script
        
        // Initially hide it
        messageText.enabled = false;
    }
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering object is a grabbable item
        ObjectGrabbable item = other.GetComponent<ObjectGrabbable>();
        if (item != null && !IsSmallItem(item))
        {
            // Add to tracked items and update visuals
            if (!itemsInZone.Contains(item))
            {
                itemsInZone.Add(item);
                SetZoneColor(activeColor);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        // Check if the exiting object is a tracked item
        ObjectGrabbable item = other.GetComponent<ObjectGrabbable>();
        if (item != null && itemsInZone.Contains(item))
        {
            // Remove from tracked items
            itemsInZone.Remove(item);
            
            // If no more items in zone, reset color
            if (itemsInZone.Count == 0)
            {
                SetZoneColor(inactiveColor);
            }
        }
    }
    
    private void Update()
    {
        // Check each item in the zone to see if it's been released
        for (int i = itemsInZone.Count - 1; i >= 0; i--)
        {
            ObjectGrabbable item = itemsInZone[i];
            
            // If the item is null (destroyed) remove it from the list
            if (item == null)
            {
                itemsInZone.RemoveAt(i);
                continue;
            }
            
            // If the item is in the zone and not being held, collect it
            if (!IsBeingHeld(item))
            {
                CollectItem(item);
                itemsInZone.RemoveAt(i);
            }
        }
        
        // If no more items in zone, reset color
        if (itemsInZone.Count == 0)
        {
            SetZoneColor(inactiveColor);
        }
    }
    
    private bool IsBeingHeld(ObjectGrabbable item)
    {
        // An item is being held if it has a non-null grab point transform
        // This assumes your ObjectGrabbable class has a way to check if it's being held
        // You might need to add a public property to ObjectGrabbable if this isn't accessible
        
        // For now, we'll check if gravity is disabled which happens when grabbed
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null)
        {
            return !rb.useGravity;
        }
        return false;
    }
    
    private bool IsSmallItem(ObjectGrabbable item)
    {
        // Check if this is a small item (which should be collected directly)
        return item.itemType == ItemType.SmallItem;
    }
    
    private void CollectItem(ObjectGrabbable item)
    {
        // Get the value of the item
        int itemValue = item.monetaryValue;
        
        // Add the value to the money manager
        MoneyManager moneyManager = FindObjectOfType<MoneyManager>();
        if (moneyManager != null)
        {
            moneyManager.AddMoney(itemValue);
        }
        
        // Show success message with the value
        ShowMessage("+" + moneyManager.currencySymbol + itemValue);
        
        // Play sound effect
        PlayDropSound();
        
        // Spawn particle effect if available
        SpawnDropEffect(item.transform.position);
        
        // Destroy the item
        Destroy(item.gameObject);
    }
    
    private void ShowMessage(string message)
    {
        if (messageText != null)
        {
            // Set the message and make it visible
            messageText.text = message;
            messageText.enabled = true;
            
            // Animate it
            StartCoroutine(AnimateMessage());
        }
    }
    
    private IEnumerator AnimateMessage()
    {
        float startTime = Time.time;
        Vector3 startPos = messageText.transform.localPosition;
        Vector3 endPos = startPos + Vector3.up * 0.5f;
        
        // Fade in and rise up
        while (Time.time < startTime + messageDuration)
        {
            float elapsed = Time.time - startTime;
            float t = elapsed / messageDuration;
            
            // Move up
            messageText.transform.localPosition = Vector3.Lerp(startPos, endPos, t);
            
            // Fade out near the end
            if (t > 0.5f)
            {
                Color textColor = messageText.color;
                textColor.a = 1.0f - (t - 0.5f) * 2.0f;
                messageText.color = textColor;
            }
            
            yield return null;
        }
        
        // Hide the text and reset
        messageText.enabled = false;
        messageText.transform.localPosition = startPos;
        
        // Reset text color alpha
        Color resetColor = messageText.color;
        resetColor.a = 1.0f;
        messageText.color = resetColor;
    }
    
    private void PlayDropSound()
    {
        if (audioSource != null && dropSound != null)
        {
            audioSource.clip = dropSound;
            audioSource.pitch = Random.Range(0.9f, 1.1f); // Add slight variation
            audioSource.Play();
        }
    }
    
    private void SpawnDropEffect(Vector3 position)
    {
        if (dropEffectPrefab != null)
        {
            // Spawn at the item's position but elevated slightly
            Vector3 spawnPos = new Vector3(position.x, transform.position.y + 0.1f, position.z);
            GameObject effect = Instantiate(dropEffectPrefab, spawnPos, Quaternion.identity);
            
            // Automatically destroy after a few seconds
            Destroy(effect, 3f);
        }
    }
    
    private void SetZoneColor(Color color)
    {
        if (zoneRenderer != null)
        {
            // Update material color
            MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
            zoneRenderer.GetPropertyBlock(propBlock);
            propBlock.SetColor("_Color", color);
            zoneRenderer.SetPropertyBlock(propBlock);
        }
    }

    // Add a simple helper class for making text face camera if you don't already have one
    public class LookAtCamera : MonoBehaviour
    {
        private List<Camera> cameras = new List<Camera>();
        
        void Start()
        {
            FindAllCameras();
        }
        
        void FindAllCameras()
        {
            cameras.Clear();
            foreach (Camera cam in Camera.allCameras)
            {
                cameras.Add(cam);
            }
        }
        
        void LateUpdate()
        {
            // Periodically refresh camera list
            if (Time.frameCount % 60 == 0)
            {
                FindAllCameras();
            }
            
            if (cameras.Count > 0)
            {
                // Find closest camera
                Camera closest = null;
                float closestDistance = float.MaxValue;
                
                foreach (Camera cam in cameras)
                {
                    if (cam != null)
                    {
                        float dist = Vector3.Distance(transform.position, cam.transform.position);
                        if (dist < closestDistance)
                        {
                            closestDistance = dist;
                            closest = cam;
                        }
                    }
                }
                
                if (closest != null)
                {
                    transform.rotation = closest.transform.rotation;
                }
            }
        }
    }
}
