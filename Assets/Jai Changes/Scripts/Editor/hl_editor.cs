using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class hl_editor : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }
    #region Editor Usage

    [MenuItem("hl_menu/instance obj")]
    static void CreateHLObject()
    {
        GameObject hl_wall = new("hl_wall");
        hl_wall.transform.position = Camera.current.transform.position + Camera.current.transform.forward * 5;
        hl_wall.AddComponent<MeshRenderer>();
        hl_wall.AddComponent<MeshFilter>().mesh = Resources.GetBuiltinResource<Mesh>("Cube.fbx");
    }

    [UnityEditor.Callbacks.DidReloadScripts]
    private static void ScriptsHasBeenReloaded()
    {
        SceneView.duringSceneGui += DuringSceneGui;
    }

    private static void DuringSceneGui(SceneView sceneView)
    {
        Event e = Event.current;

        if (e.type == EventType.KeyUp)
        {
            //Do Something
        }

        if (e.type == EventType.KeyDown)
        {
            //Do Something
            if (e.keyCode == KeyCode.C)
            {
                GameObject clone = Instantiate(Selection.activeTransform.gameObject, Selection.activeTransform.position, Selection.activeTransform.rotation);
                clone.name = Selection.activeTransform.gameObject.name;
                clone.tag = Selection.activeTransform.tag;

                clone.transform.parent = Selection.activeTransform.parent;
                Selection.activeTransform = clone.transform;
            }

            if (e.keyCode == KeyCode.Escape)
            {
                Selection.activeTransform = null;
            }
        }

        //Right mouse button
        if (e.type == EventType.MouseDown && Event.current.button == 0)
        {
            //Do Something
        }
    }

    [MenuItem("hl_menu/clean obj")]
    public static void RemoveDuplicateBoxColliders()
    {
        // Get all GameObjects in the scene.
        GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();

        // Iterate through each GameObject.
        foreach (GameObject gameObject in allGameObjects)
        {
            // Get all Box Colliders on the current GameObject.
            BoxCollider[] boxColliders = gameObject.GetComponents<BoxCollider>();

            // If there are more than one Box Collider, remove duplicates.
            if (boxColliders.Length > 1)
            {
                // Create a list to store unique Box Colliders.
                //List<BoxCollider> uniqueColliders = new List<BoxCollider>();

                // Iterate through each Box Collider on the GameObject.
                foreach (BoxCollider collider in boxColliders)
                {
                    // Destroy the duplicate collider.
                    DestroyImmediate(collider);
                }
            }
        }
    }
    #endregion
}
