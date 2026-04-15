# Dialogue-Tree Node Editor

A Unity plugin for authoring branching dialogue as visual node graphs. Build conversations in a GraphView editor, attach them to a `MonoBehaviour`, and play them back at runtime through a simple event API.

![Editor screenshot](Documentation~/img_1.png)

## Features

- **Node-based editor** built on Unity's GraphView — pan, zoom, multi-select, undo/redo.
- **Built-in node types**: Start, Dialogue (line of speech), Dialogue Branch (player choices), Event Trigger (fire a named event for game logic).
- **Actor ScriptableObjects** for speaker name, color, and portrait — drop one onto a node and it shows up on the playback side automatically.
- **Event-driven playback** — subscribe to `DialogueGraphNode.DialogueUpdated` to drive any UI you like (UI Toolkit, uGUI, IMGUI).
- **Extensible** — add your own node types by subclassing `DialogueGraphNode` and tagging with `[NodeInfo]`. Expose inspector fields with `[ExposedProperty]`.

## Requirements

- Unity **2022.3** or newer.

## Installation

**Via Unity Package Manager (Git URL):**

1. Open `Window > Package Manager`.
2. Click `+` → `Add package from git URL...`
3. Paste: `https://github.com/CgViking/DialogueTreeNodeEditor.git`

**Or copy into your project:**

Drop the `DialogueTreeNodeEditor` folder into `Assets/Plugins/`.

## Quick start

1. **Create a dialogue asset** — `Assets > Create > Dialogue > Dialogue Tree`.
2. **Open it** — double-click the asset to launch the graph editor. A Start node is created for you.
3. **Add nodes** — right-click the canvas → pick a node type → connect ports.
4. **Add a runtime object** — on any GameObject, add a `DialogueTreeGraphObject` component and assign your dialogue asset.
5. **Drive the UI** — subscribe to the dialogue event and call `MoveToNextNode` from your "continue" button.

```csharp
using DTNE.DialogueTreeNodeEditor.Runtime;

public class MyDialogueUI : MonoBehaviour
{
    public DialogueTreeGraphObject dialogue;

    void OnEnable()
    {
        DialogueGraphNode.DialogueUpdated += (_, e) =>
        {
            myLabel.text = e.Actor != null ? $"{e.Actor.actorName}: {e.Dialogue}" : e.Dialogue;
            // e.Choices is non-null for branch nodes — render one button per choice
            // and pass its index to dialogue.MoveToNextNode(index).
        };
    }

    public void OnContinueClicked() => dialogue.MoveToNextNode(0);
}
```

A complete working sample ships with the package — import it from `Window > Package Manager > Dialogue-Tree Node Editor > Samples > Dialogue Samples`. See [Documentation~/Tutorial.md](Documentation~/Tutorial.md) for a step-by-step walkthrough.

## Node reference

| Node | Purpose |
|---|---|
| **Start** | Entry point of every dialogue. Auto-created and undeletable. |
| **Dialogue** | Single line of speech. Set the speaking Actor and the text. |
| **Dialogue Branch** *(experimental)* | Player choices. Add one output per choice, connect each to a follow-up node. |
| **Event Trigger** | Fires a named string event on the runtime object — hook game logic to it via `DialogueTreeGraphObject.OnEventTriggered`. |

## Authoring custom nodes

```csharp
using DTNE.DialogueTreeNodeEditor.Runtime;
using DTNE.DialogueTreeNodeEditor.Runtime.Attributes;
using DTNE.DialogueTreeNodeEditor.Runtime.ScriptableObjects;
using UnityEngine;

[System.Serializable]
[NodeInfo("Wait", "Actions/Wait")]
public class WaitNode : DialogueGraphNode
{
    [ExposedProperty, SerializeField] public float seconds;

    public override string OnProcess(DialogueTreeAsset graph, int choice = 0)
    {
        // ... your behavior here ...
        return base.OnProcess(graph, choice);
    }
}
```

The new node will appear under `Actions/Wait` in the right-click create menu the next time you open a graph.

## Status & known limitations

This plugin is in **active development (0.1.x)**. Expect API changes.

- `DialogueBranch` and `IfConditionNode` are still marked experimental.
- Output ports are single-capacity — one output can connect to one input.
- No conditional/variable system yet.
- No automated test suite yet.

## Links

- **Repository:** https://github.com/CgViking/DialogueTreeNodeEditor
- **Issues:** https://github.com/CgViking/DialogueTreeNodeEditor/issues
- **Tutorial:** [Documentation~/Tutorial.md](Documentation~/Tutorial.md)
- **Changelog:** [CHANGELOG.md](CHANGELOG.md)

## License

ISC — see `package.json`.