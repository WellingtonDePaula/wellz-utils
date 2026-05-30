# Wellz Utils

A robust, highly optimized utility package for **Unity 6** designed to streamline 2D game development, grid-based architectures, and common mathematical or input operations.

[![Unity Version](https://img.shields.io/badge/Unity-6000.0.0f1%2B-blue.svg)](https://unity.com/)
[![Package Version](https://img.shields.io/badge/version-0.3.0-green.svg)](#)
[![Author](https://img.shields.io/badge/Author-Wellington%20Zeitz%20de%20Paula-orange.svg)](https://github.com/WellingtonDePaula)

---

## Table of Contents
1. [Installation](#installation)
2. [Core Features](#core-features)
   - [GenericGrid&lt;T&gt;](#genericgridt)
   - [UtilsClass](#utilsclass)
3. [Usage Examples](#usage-examples)
4. [Package Metadata](#package-metadata)
5. [License](#license)

---

## Installation

You can install this package directly into your Unity project via the **Unity Package Manager (UPM)** using the Git URL.

1. Open your Unity Project.
2. Navigate to `Window` -> `Package Manager`.
3. Click the `+` icon in the top-left corner and select **Add package from git URL...**
4. Paste the following URL:
   ```text
   https://github.com/WellingtonDePaula/wellz-utils.git#upm
   ```
5. Click **Add**.

---

## Core Features

### GenericGrid<T>
A data-oriented, general-purpose generic 2D grid array matrix. It separates data structure logic from visual representation, providing built-in validation, lookup safety, and seamless transformations between world coordinates and cell indexes.

* **Fully Generic:** Works with primitive types (`int`, `bool`), structs, or custom reference classes (`TileData`, `Node`).
* **Event-Driven updates:** Built-in `OnValueChanged` action allows visual systems to update instantly when data modifications occur without using expensive polling (`Update` checks).
* **Coordinate Translation:** High-accuracy calculations translating screen or world coordinates into local grid index space and vice versa.
* **Advanced Querying & Filtering:** Features optimized internal sequence search algorithms (`TryFindFirst`), tuple stream iteration (`GetAllCells`), and custom functional actions (`ForEach`).

### UtilsClass
A static utility class serving as a performance-minded ecosystem for Unity 6 workflows, dealing with cameras, mathematics, raycasts, and graphics.

* **Unity 6 & New Input System Hybridization:** Detects your project setup out-of-the-box. Handles inputs seamlessly using either the classic `Input` module or the **New Input System** (`UnityEngine.InputSystem`) depending on package configurations.
* **Zero-Allocation Raycast Projections:** Eliminates typical `Camera.main` allocation search penalties by utilizing structural safety parameters and cached parameter bypass fallbacks.
* **Smart Coordinate Projection:** Solves the classic 2D mouse projection issue by analyzing the absolute distance offsets from the targeted view camera.
* **World-Space Typography:** Spawns diagnostic text mesh layouts anywhere in 3D/2D space for rapid telemetry tracking and matrix mapping testing.

---

## Usage Examples

### 1. Working with `GenericGrid<T>`

Here is how you can instantiate a grid, bind events to update visuals, populate data, and translate coordinate points:

```csharp
using UnityEngine;
using Wellz.Utils.Core;

public class MapGenerator : MonoBehaviour
{
    private GenericGrid<int> levelGrid;

    private void Start()
    {
        int width = 10;
        int height = 10;
        float cellSize = 1.5f;
        Vector3 origin = new Vector3(-5f, -5f, 0f);

        // Initialize the 2D Grid with a default value of 0 for each cell
        levelGrid = new GenericGrid<int>(width, height, cellSize, origin, (grid, x, y) => 0);

        // Subscribe to value change events
        levelGrid.OnValueChanged += OnGridCellModified;

        // Modify a cell value directly
        levelGrid.SetValue(2, 3, 99);
    }

    private void Update()
    {
        // Detect clicking on the grid via mouse position
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseWorldPos = UtilsClass.GetMouseWorldPosition();
            Vector2Int gridPos = levelGrid.GetGridPosition(mouseWorldPos);

            if (levelGrid.IsValidPosition(gridPos))
            {
                int currentValue = levelGrid.GetValue(gridPos);
                levelGrid.SetValue(gridPos, currentValue + 1);
            }
        }
    }

    private void OnGridCellModified(int x, int y, int newValue)
    {
        Debug.Log($"Cell changed at [{x}, {y}]! New value is: {newValue}");
    }
}
```

### 2. Working with `UtilsClass`

The `UtilsClass` makes interacting with objects and processing math vectors in 2D extremely clean:

```csharp
using UnityEngine;
using Wellz.Utils.Core;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private LayerMask interactableLayer;

    private void Update()
    {
        // 1. Safe Mouse Positioning in World Space
        Vector3 mousePosition = UtilsClass.GetMouseWorldPosition();
        
        // 2. Look towards mouse pointer (Trigonometry)
        Vector3 lookDirection = mousePosition - transform.position;
        float angle = UtilsClass.GetAngleFromVector(lookDirection);
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // 3. Fast 2D Hover Selection Detection
        if (!UtilsClass.IsPointerOverUI()) // Ensure we aren't clicking through UI
        {
            Collider2D targetCollider = UtilsClass.GetMouseOverObject2D(interactableLayer);
            if (targetCollider != null && Input.GetMouseButtonDown(0))
            {
                Debug.Log($"Clicked on object: {targetCollider.name}");
            }
        }
    }

    [ContextMenu("Spawn Telemetry Text")]
    private void SpawnDebugLabel()
    {
        // 4. Create diagnostic labels anywhere in the world
        UtilsClass.CreateWorldText("Player Target Marker", transform, new Vector3(0, 1.2f, 0), 24, Color.green);
    }
}
```

---

## Package Metadata

As configured inside the `package.json` environment:

| Property | Value |
| :--- | :--- |
| **Package ID Name** | `utils` |
| **Display Name** | `Wellz Utils` |
| **Current Version** | `0.3.0` |
| **Minimum Unity Required** | `6000.0.0f1` (Unity 6) |
| **Organization Name** | `wellz` |
| **Author Namespace** | `Wellz.Utils.Core` |

---

## License & Author Info

* **Developer:** Wellington Zeitz de Paula
* **GitHub Portal:** [WellingtonDePaula](https://github.com/WellingtonDePaula)
* **Contact Email:** owellingtondepaula@gmail.com

Developed with care for professional Unity game architectures. Feel free to open issues or contribute via pull requests on the repository.
