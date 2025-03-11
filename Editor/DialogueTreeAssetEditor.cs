using DTNE.DialogueTreeNodeEditor.Runtime;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Editor
{
    [CustomEditor(typeof(DialogueTreeAsset))]
    public class DialogueTreeAssetEditor : UnityEditor.Editor
    {
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceId, int index)
        {
            Object asset = EditorUtility.InstanceIDToObject(instanceId);
            if (asset.GetType() == typeof(DialogueTreeAsset))
            {
                DialogueTreeEditorWindow.Open((DialogueTreeAsset)asset);
                return true;
            }
            return false;
        }
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open"))
            {
                DialogueTreeEditorWindow.Open((DialogueTreeAsset)target);
            }
        }
        
    }
}