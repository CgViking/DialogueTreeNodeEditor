using DTNE.DialogueTreeNodeEditor.Runtime;
using UnityEditor;
using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Editor
{
    [CustomEditor(typeof(DialogueTreeAsset))]
    public class DialogueTreeAssetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Open"))
            {
                DialogueTreeEditorWindow.Open((DialogueTreeAsset)target);
            }
        }
        
    }
}