using System;
using UnityEngine;

namespace Wellz.Utils.Core {
    public class GenericGrid<TGridType> {
        private int width;
        private int height;
        private float cellSize;
        private Vector3 originPos;

        public TGridType[,] gridArray;
        public GenericGrid(int width, int height, float cellSize, Vector3 originPos, Func<GenericGrid<TGridType>, int, int, TGridType> createGridObject) {
            this.width = width;
            this.height = height;
            this.cellSize = cellSize;
            this.originPos = originPos;

            gridArray = new TGridType[width, height];

            for (int x = 0; x < width; x++) {
                for (int y = 0; y < height; y++) {
                    gridArray[x, y] = createGridObject(this, x, y);
                }
            }
        }

        private Vector3 GetWorldPosition(int x, int y) {
            return new Vector3(x, y) * cellSize + originPos;
        }
        private void GetXY(Vector3 worldPosition, out int x, out int y) {
            x = Mathf.FloorToInt((worldPosition.x - originPos.x) / cellSize);
            y = Mathf.FloorToInt((worldPosition.y - originPos.y) / cellSize);
        }

        public void SetValue(int x, int y, TGridType value) {
            if (x >= 0 && x < width && y >= 0 && y < height) {
                gridArray[x, y] = value;
            } else {
                throw new IndexOutOfRangeException("Grid coordinates out of bounds");
            }
        }
        public void SetValue(Vector3 worldPosition, TGridType value) {
            int x, y;
            GetXY(worldPosition, out x, out y);
            SetValue(x, y, value);
        }
        public TGridType GetValue(int x, int y) {
            if (x >= 0 && x < width && y >= 0 && y < height) {
                return gridArray[x, y];
            } else {
                throw new IndexOutOfRangeException("Grid coordinates out of bounds");
            }
        }
        public TGridType GetValue(Vector3 worldPosition) {
            int x, y;
            GetXY(worldPosition, out x, out y);
            return GetValue(x, y);
        }

        public int GetWidth() { return width; }
        public int GetHeight() { return height; }
        public float GetCellSize() { return cellSize; }

        //public float GetCellSize() {
        //    return cellSize;
        //}
    }
}
