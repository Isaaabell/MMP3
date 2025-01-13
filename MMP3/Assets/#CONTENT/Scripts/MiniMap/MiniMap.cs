using System.Collections;
using UnityEngine;

public class MiniMap : MonoBehaviour
{
    private Transform player;

    [Header("Canvases")]
    [SerializeField] private GameObject miniMapCanvas;
    [SerializeField] private GameObject fullMapCanvas;

    private bool isMiniMapActive = true;

    void Start()
    {
        StartCoroutine(FindPlayer());

        // Ensure only the minimap is active at the start
        if (miniMapCanvas != null) miniMapCanvas.SetActive(true);
        if (fullMapCanvas != null) fullMapCanvas.SetActive(false);
    }

    void Update()
    {
        // Toggle between the minimap and full map when pressing "M"
        if (Input.GetKeyDown(KeyCode.M))
        {
            isMiniMapActive = !isMiniMapActive;

            if (miniMapCanvas != null) miniMapCanvas.SetActive(isMiniMapActive);
            if (fullMapCanvas != null) fullMapCanvas.SetActive(!isMiniMapActive);
        }
    }

    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 newPosition = player.position;
            newPosition.y = transform.position.y;
            transform.position = newPosition;
        }
    }

    private IEnumerator FindPlayer()
    {
        // Wait until the player is spawned
        while (player == null)
        {
            GameObject foundPlayer = GameObject.FindWithTag("Player");
            if (foundPlayer != null)
            {
                player = foundPlayer.transform;
            }
            yield return null; // Check again in the next frame
        }
    }
}
