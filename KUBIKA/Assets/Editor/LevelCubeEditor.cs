using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
[CanEditMultipleObjects]
public class LevelCubeEditor : Editor
{
    private static bool isAnchor;
    private static bool isOptional;
    private static LevelCube selectedNode;
        static LevelCubeEditor()
    {
        SceneView.duringSceneGui -= OnSceneGUI; // CA ME CASSE LES COUILLES
        SceneView.duringSceneGui += OnSceneGUI;
    }

    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(LevelCube levelNode, GizmoType gizmoType)
    {
        if ((gizmoType & GizmoType.Selected) != 0) Gizmos.color = Color.yellow;

        else Gizmos.color = Color.yellow * 0.5f;

        Gizmos.DrawSphere(levelNode.transform.position, .5f);

        //drawxw red ines to connect the nodes
        if (levelNode.nextLevel != null)
        {
            if (levelNode.nextLevel.isAnchorNode)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(levelNode.transform.position, levelNode.nextLevel.transform.position);
            }

            else
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(levelNode.transform.position, levelNode.nextLevel.transform.position);
            }
        }


        Gizmos.color = Color.green;

        if (levelNode.nextOptionalLevel != null) Gizmos.DrawLine(levelNode.transform.position, levelNode.nextOptionalLevel.transform.position);
    }


    static void OnSceneGUI(SceneView sceneView)
    {
        if (Event.current.button == 1 && Event.current.type == EventType.MouseDown)
        {
            if (Selection.activeGameObject != null)
            {
                selectedNode = Selection.activeGameObject.GetComponent<LevelCube>();

                if (selectedNode != null)
                {
                    isOptional = selectedNode.isOptionalLevel;
                    isAnchor = selectedNode.isAnchorNode;

                    GenericMenu myMenu = new GenericMenu();
                    myMenu.AddDisabledItem(new GUIContent("Set Node Type"));
                    myMenu.AddItem(new GUIContent("Level is Optional"), isOptional, NodeCallback);
                    myMenu.AddItem(new GUIContent("is Path Anchor"), isAnchor, NodeAnchorCallback);

                    myMenu.ShowAsContext();
                }
            }
        }
    }


    static void NodeCallback()
    {
        isOptional = !isOptional;

        if (isOptional)
        {
            selectedNode.previousLevel.nextOptionalLevel = selectedNode;
            selectedNode.previousLevel.nextLevel = selectedNode.nextLevel;
            if (selectedNode.nextLevel != null) selectedNode.nextLevel.prevOptionalLevel = selectedNode;
        }

        else
        {
            selectedNode.previousLevel.nextOptionalLevel = null;
            if (selectedNode.nextLevel != null) selectedNode.nextLevel.prevOptionalLevel = null;
        }
    }

    static void NodeAnchorCallback()
    {
        LevelCube selectedNode = Selection.activeGameObject.GetComponent<LevelCube>();
        isAnchor = !isAnchor;

        /*            if(isAnchor) selectedNode.isAnchorNode = true;
                    else selectedNode.isAnchorNode = false;*/
    }
}
