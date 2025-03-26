# Changelog
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