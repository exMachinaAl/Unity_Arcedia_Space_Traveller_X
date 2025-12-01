#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

// [CustomEditor(typeof(Game_BestBootStraps))]
[CustomEditor(typeof(Root_GameInitManager))]
public class Editor_BestBootStraps : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        // Tombol untuk mengaktifkan atau menonaktifkan debug mode
        if (GUILayout.Button("Toggle Debug Mode"))
        {
            Game_BestBootStraps.ToggleDebugMode();
        }
    }
}
#endif