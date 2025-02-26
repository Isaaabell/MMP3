using UnityEngine;
using UnityEditor;

public class RoomColorGenerator : EditorWindow
{
    // public GameObject FloorPrefab;
    // public GameObject WallPrefab;
    // public Color FloorColor = Color.white;
    // public Color WallColor = Color.white;

    public GameObject RoomParent;
    public Material FloorMaterial;
    public Material WallMaterial;

    [MenuItem("Tools/Room Color Generator")]
    public static void ShowWindow()
    {
        GetWindow<RoomColorGenerator>("Room Color Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Room Generator", EditorStyles.boldLabel);

        RoomParent = (GameObject)EditorGUILayout.ObjectField("Room Parent", RoomParent, typeof(GameObject), true);
        FloorMaterial = (Material)EditorGUILayout.ObjectField("Floor Material", FloorMaterial, typeof(Material), false);
        WallMaterial = (Material)EditorGUILayout.ObjectField("Wall Material", WallMaterial, typeof(Material), false);


        if (GUILayout.Button("Generate Room"))
        {
            CahngeMaterial();
        }
    }

    private void CahngeMaterial()
    {
        if (RoomParent == null)
        {
            Debug.LogError("Assign Room Prefab before generating the room!");
            return;
        }

        if (FloorMaterial == null || WallMaterial == null)
        {
            Debug.LogError("Assign Floor and Wall Material before generating the room!");
            return;
        }

        foreach (Transform child in RoomParent.transform)
        {
            Renderer renderer = child.GetComponent<Renderer>();
            if (child.name.StartsWith("Floor"))
            {

                if (renderer != null)
                {
                    renderer.sharedMaterial = FloorMaterial;
                }
            }
            else if (child.name.StartsWith("Wall") || child.name.StartsWith("Door") || child.name.StartsWith("Window"))
            {

                // Ensure the object has multiple materials before modifying
                Material[] materials = renderer.sharedMaterials;
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i] = WallMaterial;
                }
                renderer.sharedMaterials = materials;
            }
        }


        Debug.Log("Wall and Floor Material changed Successfully!");
    }
}
