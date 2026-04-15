# Changelog
## [0.2.0]
### Changed
- Dropped npm packaging. The plugin is now distributed exclusively as a Unity Package Manager package via git URL.
- Renamed package from `@CgViking/com.bearlygames.dialogue-tree-editor` to `com.bearlygames.dialogue-tree-editor` to match UPM naming conventions.

### Removed
- `index.js`, `package-lock.json`, and the `release-package.yml` GitHub Action that published to GitHub's npm registry.
- npm-only fields (`type`, `main`, `publishConfig`, `scripts`) from `package.json`.
- Node-template noise from `.gitignore`.

## [0.1.9]
### Fixed
- Actor portrait now persists when reopening a dialogue asset (was being orphaned on rebind).
- Search menu no longer reuses one shared C# instance per node type — picking the same node twice now creates two distinct nodes.
- `DrawConnections` no longer throws when a stale connection is pruned mid-iteration.
- Stale port indices on connections are bounds-checked instead of throwing `IndexOutOfRangeException`.
- `DialogueTreeAsset.Init` rebuilds the node dictionary so the constructor's ghost `StartNode` is dropped.

### Changed
- Search provider caches assembly/type scan and invalidates on assembly reload.
- Search provider tolerates `ReflectionTypeLoadException` from misbehaving assemblies.
- `IfConditionNode` hidden from the create menu until it has a backing variable system.

### Docs
- Rewrote README with installation, quick start, node reference, and custom-node example.
- Expanded Tutorial with end-to-end walkthrough including UI hookup and event handling.

## [0.1.8]
### Fixed 
- Fixed missing Serializable attributes on nodes.

## [0.1.4 -> 0.1.7]
- Changelog missing.
- fixes

## [0.1.3]
### Added
- Simple event interface to get current Dialogue and who is speaking it.
- Actor profile changes on adding

### Fixed
- Branch no longer adding a choice on each open of asset
- Can no longer delete list items in branch
- Undo actually works

## [0.1.2]
### Added
- Branch node is working.
- Actor Scriptable Object.

## [0.1.1]
### Added
- Branch node
- EventTrigger node
- Event that returns dialogue string
  - `public static Action<string> DisplayDialogue;`
    - `DialogueGraphNode.DisplayDialogue += UpdateText;`
### Changed
- DialogueTreeAssets start with a Start node
- Start node can't be deleted.
- Tagged unfinished nodes as `(Experimental)`

## [0.1.0] - 2025-03-18
### Added
- Initial beta release