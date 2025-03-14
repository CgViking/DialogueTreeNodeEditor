using System;
using UnityEngine;

namespace DTNE.DialogueTreeNodeEditor.Runtime
{
    public class DialogueTreeGraphObject : MonoBehaviour
    {
        [SerializeField] private DialogueTreeAsset dialogueTreeAsset;
        private DialogueTreeAsset _dialogueTreeAssetInstance;

        private void OnEnable()
        {
            _dialogueTreeAssetInstance = Instantiate(dialogueTreeAsset);
            ExecuteAsset();
        }

        private void ExecuteAsset()
        {
            _dialogueTreeAssetInstance.Init();
            
            DialogueGraphNode startNode = _dialogueTreeAssetInstance.GetStartNode();
            ProcessAndMoveToNextNode(startNode);
        }

        private void ProcessAndMoveToNextNode(DialogueGraphNode startNode)
        {
            string nextNodeId = startNode.OnProcess(_dialogueTreeAssetInstance);

            if (string.IsNullOrEmpty(nextNodeId))
            {
                DialogueGraphNode node = _dialogueTreeAssetInstance.GetNode(nextNodeId);
                ProcessAndMoveToNextNode(node);
            }
        }
    }
}