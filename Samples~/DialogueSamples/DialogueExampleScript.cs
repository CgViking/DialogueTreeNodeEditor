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
        private Label _textBox;
        
        public int currentChoiceIndex = -1;

        private void Awake()
        {
            _root = uiDocument.rootVisualElement;
            _textBox = _root.Q<Label>("textBox"); //Display the text in any UI type.
        }

        private void OnEnable()
        {
            Button btn = _root.Q<Button>("button"); //Add a button to activate dialogue.
            btn.clicked += Test;
            
            //Subscribe to this event to display dialogue.
            DialogueGraphNode.DisplayDialogue += UpdateText;
        }

        //Do whatever you want with the string.
        private void UpdateText(string test)
        {
            _textBox.text = test;
        }
        
        public void Test()
        {
            if (dialogueTreeGraphObject != null)
            {
                dialogueTreeGraphObject.MoveToNextNode(currentChoiceIndex);
            }
        
        }

    }
}
