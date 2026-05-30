using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Wellz.Utils.Core {
    public static class UtilsClass {
        public static Vector3 GetMouseWorldPosition() {
            return GetMouseWorldPosition(Input.mousePosition, Camera.main);
        }
        public static Vector3 GetMouseWorldPosition(Vector3 mousePos) {
            mousePos.z = 0f;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            return worldPos;
        }
        public static Vector3 GetMouseWorldPosition(Vector3 mousePos, Camera camera) {
            mousePos.z = 0f;
            Vector3 worldPos = camera.ScreenToWorldPoint(mousePos);
            return worldPos;
        }
    }
}
