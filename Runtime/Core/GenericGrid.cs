using System;
using System.Collections.Generic;
using UnityEngine;

namespace Wellz.Utils.Core {
    /// <summary>
    /// General-purpose generic 2D grid.
    /// Supports iteration, searching, change events, and coordinate conversion.
    /// </summary>
    /// <typeparam name="T">The type of data stored in each grid cell.</typeparam>
    [Serializable]
    public class GenericGrid<T> {
        private readonly int width;
        private readonly int height;
        private readonly T[,] cells;
        private readonly float cellSize;
        private readonly Vector3 originPosition;

        // ─── Events ──────────────────────────────────────────────────────────

        /// <summary>
        /// Triggered whenever a cell's value is changed. 
        /// Passes the X coordinate, Y coordinate, and the new value.
        /// </summary>
        public event Action<int, int, T> OnValueChanged;

        // ─── Properties ─────────────────────────────────────────────────────

        /// <summary>Gets the width (number of columns) of the grid.</summary>
        public int Width => width;

        /// <summary>Gets the height (number of rows) of the grid.</summary>
        public int Height => height;

        /// <summary>Gets the size of each individual cell in world units.</summary>
        public float CellSize => cellSize;

        /// <summary>Gets the world space origin position of the grid (bottom-left corner).</summary>
        public Vector3 OriginPosition => originPosition;

        /// <summary>Gets the total number of cells in the grid (Width * Height).</summary>
        public int TotalCells => width * height;

        /// <summary>
        /// Initializes a new instance of the <see cref="Grid{T}"/> class.
        /// </summary>
        /// <param name="width">The number of columns in the grid.</param>
        /// <param name="height">The number of rows in the grid.</param>
        /// <param name="cellSize">The size of each cell side in world units.</param>
        /// <param name="originPosition">The starting world position of the grid's bottom-left corner.</param>
        /// <param name="initValue">An optional initialization function invoked for each cell, passing the grid instance, X, and Y coordinates.</param>
        public GenericGrid(int width, int height, float cellSize = 1f,
                    Vector3 originPosition = default,
                    Func<GenericGrid<T>, int, int, T> initValue = null) {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.originPosition = originPosition;
            cells = new T[width, height];

            if (initValue != null) {
                for (int x = 0; x < width; x++)
                    for (int y = 0; y < height; y++)
                        cells[x, y] = initValue(this, x, y);
            }
        }

        /// <summary>
        /// Gets the value at the specified grid coordinates.
        /// </summary>
        /// <param name="x">The column index.</param>
        /// <param name="y">The row index.</param>
        /// <returns>The value at the position, or the default value of <typeparamref name="T"/> if the position is invalid.</returns>
        public T GetValue(int x, int y) {
            if (!IsValidPosition(x, y))
                return default;
            return cells[x, y];
        }

        /// <summary>
        /// Gets the value at the specified grid position.
        /// </summary>
        /// <param name="pos">The coordinates as a Vector2Int (X as column, Y as row).</param>
        /// <returns>The value at the position, or the default value of <typeparamref name="T"/> if the position is invalid.</returns>
        public T GetValue(Vector2Int pos) => GetValue(pos.x, pos.y);

        /// <summary>
        /// Attempts to get the value at the specified grid coordinates.
        /// </summary>
        /// <param name="x">The column index.</param>
        /// <param name="y">The row index.</param>
        /// <param name="value">When this method returns, contains the value at the specified position if valid; otherwise, the default value of <typeparamref name="T"/>.</param>
        /// <returns><see langword="true"/> if the position is valid and the value was successfully retrieved; otherwise, <see langword="false"/>.</returns>
        public bool TryGetValue(int x, int y, out T value) {
            if (!IsValidPosition(x, y)) { value = default; return false; }
            value = cells[x, y];
            return true;
        }

        /// <summary>
        /// Sets the value at the specified grid coordinates and triggers the <see cref="OnValueChanged"/> event.
        /// </summary>
        /// <param name="x">The column index.</param>
        /// <param name="y">The row index.</param>
        /// <param name="value">The new value to assign to the cell.</param>
        public void SetValue(int x, int y, T value) {
            if (!IsValidPosition(x, y))
                return;
            cells[x, y] = value;
            OnValueChanged?.Invoke(x, y, value);
        }

        /// <summary>
        /// Sets the value at the specified grid position and triggers the <see cref="OnValueChanged"/> event.
        /// </summary>
        /// <param name="pos">The coordinates as a Vector2Int (X as column, Y as row).</param>
        /// <param name="value">The new value to assign to the cell.</param>
        public void SetValue(Vector2Int pos, T value) => SetValue(pos.x, pos.y, value);

        // ─── Coordinate Conversion ─────────────────────────────────────────

        /// <summary>
        /// Calculates the world space position of the bottom-left corner of the specified cell.
        /// </summary>
        /// <param name="x">The column index.</param>
        /// <param name="y">The row index.</param>
        /// <returns>The world space <see cref="Vector3"/> position of the cell.</returns>
        public Vector3 GetWorldPosition(int x, int y) =>
            new Vector3(x, y) * cellSize + originPosition;

        /// <summary>
        /// Converts a world space position into grid coordinates.
        /// </summary>
        /// <param name="worldPosition">The position in world space.</param>
        /// <returns>A <see cref="Vector2Int"/> representing the corresponding X and Y grid coordinates.</returns>
        public Vector2Int GetGridPosition(Vector3 worldPosition) {
            int x = Mathf.FloorToInt((worldPosition - originPosition).x / cellSize);
            int y = Mathf.FloorToInt((worldPosition - originPosition).y / cellSize);
            return new Vector2Int(x, y);
        }

        // ─── Validation ────────────────────────────────────────────────────────

        /// <summary>
        /// Checks if the specified grid coordinates are within the grid bounds.
        /// </summary>
        /// <param name="x">The column index.</param>
        /// <param name="y">The row index.</param>
        /// <returns><see langword="true"/> if the coordinates are within bounds; otherwise, <see langword="false"/>.</returns>
        public bool IsValidPosition(int x, int y) =>
            x >= 0 && y >= 0 && x < width && y < height;

        /// <summary>
        /// Checks if the specified grid position is within the grid bounds.
        /// </summary>
        /// <param name="pos">The coordinates as a Vector2Int (X as column, Y as row).</param>
        /// <returns><see langword="true"/> if the position is within bounds; otherwise, <see langword="false"/>.</returns>
        public bool IsValidPosition(Vector2Int pos) => IsValidPosition(pos.x, pos.y);

        // ─── Iteration and Search ─────────────────────────────────────────────────

        /// <summary>
        /// Enumerates through all cell values sequentially.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> containing all values in the grid.</returns>
        public IEnumerable<T> GetAllValues() {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    yield return cells[x, y];
        }

        /// <summary>
        /// Enumerates through all cells, returning their coordinates along with their values.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of tuples containing the X coordinate, Y coordinate, and cell value.</returns>
        public IEnumerable<(int x, int y, T value)> GetAllCells() {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    yield return (x, y, cells[x, y]);
        }

        /// <summary>
        /// Searches the grid for the first cell that matches the conditions defined by the specified predicate.
        /// </summary>
        /// <param name="predicate">The delegate that defines the conditions of the element to search for.</param>
        /// <param name="x">When this method returns, contains the column index of the first matching cell if found; otherwise, -1.</param>
        /// <param name="y">When this method returns, contains the row index of the first matching cell if found; otherwise, -1.</param>
        /// <returns><see langword="true"/> if a matching cell is found; otherwise, <see langword="false"/>.</returns>
        public bool TryFindFirst(Predicate<T> predicate, out int x, out int y) {
            for (int xi = 0; xi < width; xi++) {
                for (int yi = 0; yi < height; yi++) {
                    if (predicate(cells[xi, yi])) {
                        x = xi;
                        y = yi;
                        return true;
                    }
                }
            }
            x = -1;
            y = -1;
            return false;
        }

        /// <summary>
        /// Executes the specified action on each cell in the grid.
        /// </summary>
        /// <param name="action">The delegate to execute on each cell, passing X, Y, and the cell value.</param>
        public void ForEach(Action<int, int, T> action) {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    action(x, y, cells[x, y]);
        }

        /// <summary>
        /// Resets all cells in the grid to a default value.
        /// </summary>
        /// <param name="defaultValue">The value to assign to every cell. Defaults to <see langword="default"/>.</param>
        public void Clear(T defaultValue = default) {
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    SetValue(x, y, defaultValue);
        }
    }
}