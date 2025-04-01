using System.Collections.Generic;
using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;
using DTNE.DialogueTreeNodeEditor.Runtime.ScriptableObjects;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace DTNE.DialogueTreeNodeEditor.Runtime.Types
{
    [NodeInfo("Dialogue Branch (Experimental)", "Dialogue/Branch")] //TODO: Find a way to add or remove inputs/outputs.
    public class DialogueBranch : DialogueGraphNode
    {
        [SerializeField] private List<string> _choices = new List<string>();
        public List<string> Choices => _choices;
        [ExposedProperty, Tooltip("The start dialogue\nCan be left blank")]
        public string StartDialogue;

        public override int FlowOutputCount => Choices.Count;
        public override int GetOutputPortCount()
        {
            return FlowOutputCount;
        }
        public override string OnProcess(DialogueTreeAsset currentGraph, int choice)
        {
            return currentGraph.GetNodeFromOutput(id, choice)?.id;
        }
    }
}