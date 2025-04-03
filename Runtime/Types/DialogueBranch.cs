using System.Collections.Generic;
using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;
using DTNE.DialogueTreeNodeEditor.Runtime.ScriptableObjects;
using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Runtime.Types
{
    [NodeInfo("Dialogue Branch (Experimental)", "Dialogue/Branch")] //TODO: Find a way to add or remove inputs/outputs.
    public class DialogueBranch : DialogueGraphNode
    {
        [SerializeField] private List<string> _choices = new List<string>(2);
        public List<string> Choices => _choices;
        [ExposedProperty, Tooltip("The start dialogue\nCan be left blank")]
        public string StartDialogue;
        
        private bool _awaitingChoice;

        public override int FlowOutputCount => Choices.Count;
        public override int GetOutputPortCount()
        {
            return FlowOutputCount;
        }
        public override string OnProcess(DialogueTreeAsset currentGraph, int choice = 0)
        {
            if (!_awaitingChoice)
            {
                RaiseDialogueUpdated(new DialogueEventArgs(this.StartDialogue, this.Actor, this._choices));
                _awaitingChoice = true;
                return this.id;
            }

            // Reset state for future interactions
            _awaitingChoice = false;

            // Get the next node's ID based on the choice
            string nextNodeId = currentGraph.GetNodeFromOutput(id, choice)?.id;

            // Immediately trigger processing of the next node
            if (!string.IsNullOrEmpty(nextNodeId))
            {
                DialogueGraphNode nextNode = currentGraph.GetNodeFromOutput(id,choice);
                if (nextNode != null)
                {
                    // Process the next node
                    return nextNode.OnProcess(currentGraph);
                }
            }

            return base.OnProcess(currentGraph, choice); // Fallback;
        }
    }
}