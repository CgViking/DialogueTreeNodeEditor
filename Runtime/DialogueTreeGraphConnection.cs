using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Runtime
{
    [System.Serializable]
    public struct DialogueTreeGraphConnection
    {
        public DialogueTreeGraphConnectionPort inputPort;
        public DialogueTreeGraphConnectionPort outputPort;

        public DialogueTreeGraphConnection(DialogueTreeGraphConnectionPort input, DialogueTreeGraphConnectionPort output)
        {
            inputPort = input;
            outputPort = output;
        }

        public DialogueTreeGraphConnection(string inputPortId, int inputIndex, string outputPortId,  int outputIndex)
        {
            inputPort = new DialogueTreeGraphConnectionPort(inputPortId, inputIndex);
            outputPort = new DialogueTreeGraphConnectionPort(outputPortId, outputIndex);
        }
    }

    [System.Serializable]
    public struct DialogueTreeGraphConnectionPort
    {
        public string nodeId;
        public int portIndex;

        public DialogueTreeGraphConnectionPort(string id, int index)
        {
            this.nodeId = id;
            this.portIndex = index;
        }
    }
}