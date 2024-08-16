# gmtk-2024

[GMTK Game Jam 2024](https://itch.io/jam/gmtk-2024) entry made in unity in under 96 hours.

### Theme
 \-

### Working Title
 \-

### Concept statement
 \-

### Made by:

- [kastrbl4nik](https://github.com/kastrbl4nik) - Developer
- [kilativ-dotcom](https://github.com/kilativ-dotcom) - Developer
- [tab4she](https://github.com/tab4she) - Sound designer / Artist
- \_bykeksik\_ - Artist
- izonia - Artist

### Folder structure

```
Assets
    +---Art # Anything art related
    |  +---Materials
    |  +---Models
    |  +---Textures
    +---Audio
    |  +---Music
    |  \---Sound
    +---Code
    |  +---Scripts  # C# scripts
    |  \---Shaders  # Shader files and shader graphs
    +---Docs  # Wiki, concept art, etc.
    +---Level  # Anything related to game design in Unity
    |  +---Prefabs
    |  +---Scenes
    |  \---UI
```

### Naming Conventions

| Type                                 | Naming                       | Examples                          |
| ------------------------------------ | ---------------------------- | --------------------------------- |
| Images / Sprites / Textures / Sounds | kebab-case                   | female-child.png / main-theme.wav |
| Classes / Scripts / Folders          | PascalCase                   | AudioManager.cs / AudioManager    |
| Interfaces                           | PascalCase prefixed with `I` | IWalkable.cs / IWalkable          |
| Game Objects / Prefabs               | Capitalized first letters    | Main Camera / Audio Manager       |

> Use .editorconfig suggestions for everything else

### Team agreements

- (!) Inject dependencies via `Awake()` method when the dependency shouldn't be configured to avoid setting up the dependencies in the inspector:

    ```csharp
        // good
        private RigidBody2D rb;

        private void Awake() {
            rb = GetComponent<RigidBody2d>();
        }
    ```

    ```csharp
        // bad
        [SerializeField] private RigidBody2D rb;
    ```

- (!) Avoid working on the same scene simultaneously to avoid merge conficts in meta files. If something needs to be tested on the scene create a demo scene and make changes there.

- Remove any `Debug.Logs` before merging

- Remember about `Single Responsibility` principle and `Strategy` patterns.
    ```csharp
    public class PathFinding {
        [SerializeField] private pathfindingStrategy; // any pathfinding algorithm can be put here
        private List<Node> grid;

        public List<Node> GetPath() {
            return pathfindingStrategy.GetPath(grid)
        }
    }
    ```

- (!) Don't overuse the prefabs, instead make use of the Unity `Scriptable Objects` and `Factory` pattern. 
    ```csharp
        public class Shop() {
            [SerializeField] private ItemData itemToSell;

            public ItemData Sell() {
                return itemToSell;
            }
        }
    ```

- (!) Merge pull requests yourself, if you're confident about the changes, to save up time.

- Use [conventional commit](https://www.conventionalcommits.org/en/v1.0.0/) messages.
