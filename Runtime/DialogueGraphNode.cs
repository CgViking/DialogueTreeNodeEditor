using System;
using System.Collections.Generic;
using DTNE.DialogueTreeNodeEditor.Runtime.ScriptableObjects;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Runtime
{
    [System.Serializable]
    public class DialogueGraphNode 
    {
        [SerializeField]
        private string _guid;
        [SerializeField]
        private Rect position;

        public virtual bool HasFlowInput { get; } = true;
        public virtual bool HasFlowOutput { get; } =  true;
        public virtual int FlowInputCount { get; } = 1;
        public virtual int FlowOutputCount { get; set; } = 1;

        [SerializeField] private Actor _actor;
        public Actor Actor => _actor;

        /// <summary>
        /// Returns the dialogue of the current node.
        /// </summary>
        public static Action<string> DisplayDialogue;
        
        public string typeName;
        public string id => _guid;
        public Rect Position => position;
        public DialogueGraphNode()
        {
            NewGUID();
        }

        private void NewGUID()
        {
            _guid = Guid.NewGuid().ToString();
        }

        public void SetPosition(Rect newPosition)
        {
            position = newPosition;
        }

        // The Flow
        public virtual string OnProcess(DialogueTreeAsset currentGraph, int choice = 0)
        {
            DialogueGraphNode nextNodeInFlow = currentGraph.GetNodeFromOutput(_guid, choice);
            if (nextNodeInFlow != null)
            {
                return nextNodeInFlow.id;
            }
            
            return string.Empty;
        }

        /// <summary>
        /// Returns the number of output ports, if null it returns 0.
        /// </summary>
        /// <returns></returns>
        public virtual int GetOutputPortCount()
        {
            return this.FlowOutputCount;
        }

        public string GetOutputPortName(int index)
        {
            return "Output " + index;
        }
    }
}