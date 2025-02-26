using UnityEngine;
using UnityEditor;

public class RoomGenerator : EditorWindow
{
    public GameObject FloorPrefab;
    public GameObject WallPrefab;
    // public GameObject WallWithDoorPrefab;
    // public GameObject WallWithWindowPrefab;
    public string CustomParentName = "Room Parent"; // New field for custom parent name

    private float width = 5;
    private float length = 5;
    private float height = 3f;

    [MenuItem("Tools/Room Generator")]
    public static void ShowWindow()
    {
        GetWindow<RoomGenerator>("Room Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("Room Settings", EditorStyles.boldLabel);
        width = EditorGUILayout.FloatField("Width", width);
        length = EditorGUILayout.FloatField("Length", length);
        height = EditorGUILayout.FloatField("Height", height);

        FloorPrefab = (GameObject)EditorGUILayout.ObjectField("Floor Prefab", FloorPrefab, typeof(GameObject), false);
        WallPrefab = (GameObject)EditorGUILayout.ObjectField("Wall Prefab", WallPrefab, typeof(GameObject), false);

        // Add a field for setting the custom name of the parent GameObject
        CustomParentName = EditorGUILayout.TextField("Custom Parent Name", CustomParentName);

        if (GUILayout.Button("Generate Room"))
        {
            GenerateRoom();
        }
    }

    void GenerateRoom()
    {
        // Create the custom parent GameObject with the specified name
        GameObject parent = new GameObject(CustomParentName);

        // Step 1: Create the floor at position (0, 0, 0)
        if (FloorPrefab != null)
        {
            Vector3 floorPosition = Vector3.zero; // Position at 0, 0, 0
            GameObject floor = Instantiate(FloorPrefab, floorPosition, Quaternion.identity);
            floor.transform.SetParent(parent.transform); // Set custom parent
            floor.transform.localScale = new Vector3(width, 1, length); // Scale with customizable width (X) and length (Z)
        }

        // Step 2: Create the walls along the edges of the floor

        // Wall 1: Bottom wall (along Z-axis)
        if (WallPrefab != null)
        {
            Vector3 wall1Position = new Vector3(0, 0, -length); // Positioned at center of bottom edge
            GameObject wall1 = Instantiate(WallPrefab, wall1Position, Quaternion.identity);
            wall1.transform.SetParent(parent.transform); // Set custom parent
            wall1.transform.localScale = new Vector3(width, 1, 1); // Scale: width, height (Y), depth (Z)
        }

        // Wall 2: Top wall (along Z-axis)
        if (WallPrefab != null)
        {
            Vector3 wall2Position = new Vector3(0, 0, length); // Positioned at center of top edge
            GameObject wall2 = Instantiate(WallPrefab, wall2Position, Quaternion.identity);
            wall2.transform.SetParent(parent.transform); // Set custom parent
            wall2.transform.localScale = new Vector3(width, 1, 1); // Scale: width, height (Y), depth (Z)
        }

        // Wall 3: Left wall (along X-axis)
        if (WallPrefab != null)
        {
            Vector3 wall3Position = new Vector3(-width, 0, 0); // Positioned at center of left edge
            GameObject wall3 = Instantiate(WallPrefab, wall3Position, Quaternion.Euler(0, 90, 0)); // Rotate 90 degrees for vertical wall
            wall3.transform.SetParent(parent.transform); // Set custom parent
            wall3.transform.localScale = new Vector3(length, 1, 1); // Scale: length (Z), height (Y), depth (X)
        }

        // Wall 4: Right wall (along X-axis)
        if (WallPrefab != null)
        {
            Vector3 wall4Position = new Vector3(width, 0, 0); // Positioned at center of right edge
            GameObject wall4 = Instantiate(WallPrefab, wall4Position, Quaternion.Euler(0, 90, 0)); // Rotate 90 degrees for vertical wall
            wall4.transform.SetParent(parent.transform); // Set custom parent
            wall4.transform.localScale = new Vector3(length, 1, 1); // Scale: length (Z), height (Y), depth (X)
        }
    }
}
