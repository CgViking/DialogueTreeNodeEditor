using System;
using UnityEngine;
using DTNE.DialogueTreeNodeEditor.Runtime;
using UnityEngine.UIElements;

namespace DTNE
{
    public class DialogueExampleScript : MonoBehaviour
    {
        
        [SerializeField] private UIDocument uiDocument;
        public DialogueTreeGraphObject dialogueTreeGraphObject;
        private VisualElement _root;

        private void Awake()
        {
            _root = uiDocument.rootVisualElement;
        }

        private void OnEnable()
        {
            Button btn = _root.Q<Button>("button"); //Add your actual button here.
            btn.clicked += Test;
        }
        
        public void Test()
        {
            if (dialogueTreeGraphObject != null)
            {
                //dialogueTreeGraphObject.MoveToNextNode();
            }
        
        }

    }
}
