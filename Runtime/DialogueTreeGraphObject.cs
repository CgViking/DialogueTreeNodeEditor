using System;
using DTNE.DialogueTreeNodeEditor.Runtime.ScriptableObjects;
using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Runtime
{
    public class DialogueTreeGraphObject : MonoBehaviour
    {
        [SerializeField] private DialogueTreeAsset dialogueTreeAsset;
        private DialogueTreeAsset _dialogueTreeAssetInstance;
        
        public event Action<string> OnEventTriggered; 
    
        // Track the current node in the dialogue
        private DialogueGraphNode _currentNode;
        // Cache the next node id after processing the current node
        private string _nextNodeId;

        private void OnEnable()
        {
            if (dialogueTreeAsset != null)
            {
                _dialogueTreeAssetInstance = Instantiate(dialogueTreeAsset);
                _dialogueTreeAssetInstance.Init(this.gameObject);

                if (_dialogueTreeAssetInstance != null) return;
                
                // Get and process the starting node
                _currentNode = _dialogueTreeAssetInstance.GetStartNode();
                ProcessCurrentNode();
            }
            else
            {
                Debug.LogWarning("DialogueTreeAsset is null");
            }
        
        }

        private void OnDestroy()
        {
            Destroy(_dialogueTreeAssetInstance);
        }
        
        /// <summary>
        /// Used by EventTriggerNode to trigger events.
        /// </summary>
        /// <param name="eventName"></param>
        public void TriggerEvent(string eventName)
        {
            if (string.IsNullOrEmpty(eventName)) return;
            Debug.Log($"Triggering event: {eventName}");
            OnEventTriggered?.Invoke(eventName);
        }

        // Process the current node (display dialogue, etc.)
        private void ProcessCurrentNode(int choice = 0)
        {
            if (_currentNode == null)
            {
                Debug.Log("Dialogue ended.");
                return;
            }
        
            // Process the node and determine the next node id.
            // This method could update UI elements, display text, etc.
            _nextNodeId = _currentNode.OnProcess(_dialogueTreeAssetInstance, choice);
            // Instead of immediately moving to the next node,
            // we now wait for the user to trigger MoveToNextNode()
        }
        
        /// <summary>Call this method (e.g. via a UI button) to manually move to the next node.</summary>
        /// <param name="choiceIndex">Choice pick which output to continue from. Starts at index 0.
        /// A choice of (-1) would mean an early exit/end conversation.</param>
        public bool MoveToNextNode(int choiceIndex)
        {
            //TODO: Make choices.
            if (!string.IsNullOrEmpty(_nextNodeId))
            {
                _currentNode = _dialogueTreeAssetInstance.GetNode(_nextNodeId);
                ProcessCurrentNode(choiceIndex);
                return false;
            }
            else
            {
                Debug.Log("No further nodes. Dialogue ended.");
                return true;
            }
        }

        /// <summary>
        /// Function to start a new dialogue.
        /// </summary>
        /// <param name="newDialogue"></param>
        public void NewDialogue(DialogueTreeAsset newDialogue)
        {
            if (dialogueTreeAsset != null)
                Destroy(_dialogueTreeAssetInstance); //Destroy the old instance.
            
            this.dialogueTreeAsset = newDialogue;
            _dialogueTreeAssetInstance = Instantiate(dialogueTreeAsset);
            _dialogueTreeAssetInstance.Init(this.gameObject);
            _currentNode = _dialogueTreeAssetInstance.GetStartNode();
            ProcessCurrentNode();
        }
    }
}