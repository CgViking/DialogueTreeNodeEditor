using UnityEngine;
using DTNE.DialogueTreeNodeEditor.Runtime;
using DTNE.DialogueTreeNodeEditor.Runtime.ScriptableObjects;
using UnityEngine.UIElements;

namespace DTNE
{
    public class DialogueExampleScript : MonoBehaviour
    {
        
        [SerializeField] private UIDocument uiDocument;
        public DialogueTreeGraphObject dialogueTreeGraphObject;
        private VisualElement _root;
        private Label _textBox;

        private void Awake()
        {
            _root = uiDocument.rootVisualElement;
            _textBox = _root.Q<Label>("textBox"); //Display the text in any UI type.
        }

        private void OnEnable()
        {
            Button btn = _root.Q<Button>("continueButton"); //Add a button to activate dialogue.
            btn.clicked += Continue;
            
            VisualElement choicesContainer = _root.Q<VisualElement>("choicesContainer");
            
            //Subscribe to this event to display dialogue.
            DialogueGraphNode.DialogueUpdated += (sender, e) =>
            {
                string dialogueText = string.IsNullOrEmpty(e.Dialogue) ? "No Dialogue" : e.Dialogue;
                Actor actor = e.Actor;
                if (actor != null)
                {
                    _textBox.text = $"{actor.name}: {dialogueText}";
                }
                else
                {
                    _textBox.text = dialogueText;
                }

                if (e.Choices == null) return;
                
                //bind choices to buttons, either way you chose. And Move to next node with that index.
                if (e.Choices.Count > 0)
                {
                    btn.style.display = DisplayStyle.None; // Hide continue button.
                    for (int i = 0; i < e.Choices.Count; i++)
                    {
                        int currentChoice = i;
                        var newButton = new Button();
                        newButton.text = e.Choices[i];
                        newButton.clicked += () =>
                        {
                            btn.style.display = DisplayStyle.Flex; // Show it again.
                            bool ended = dialogueTreeGraphObject.MoveToNextNode(currentChoice);
                            choicesContainer.Clear();
                            if (ended) EndDialogue(); 
                        };
                        choicesContainer.Add(newButton);
                    }
                }
            };
        }

        private void Continue()
        {
            if (dialogueTreeGraphObject != null)
            {
                bool ended = dialogueTreeGraphObject.MoveToNextNode(0);
                if (ended) EndDialogue();
            }
        }

        private void EndDialogue()
        {
            _root.style.display = DisplayStyle.None;
        }
    }
}
