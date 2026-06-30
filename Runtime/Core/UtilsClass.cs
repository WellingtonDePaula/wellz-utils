using System;
using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Wellz.Utils.Core {
    /// <summary>
    /// General utility functions for Unity 6 projects, focusing on 2D systems, coordinate math, and input handling.
    /// </summary>
    public static class UtilsClass {
        // ─── Input & Mouse World Positions ──────────────────────────────────

        /// <summary>
        /// Gets the current mouse position in world space using the main camera.
        /// </summary>
        /// <returns>The world space position of the mouse as a <see cref="Vector3"/>.</returns>
        public static Vector3 GetMouseWorldPosition() {
            return GetMouseWorldPosition(GetMouseScreenPosition(), Camera.main);
        }

        /// <summary>
        /// Gets the mouse position in world space using a specific camera.
        /// </summary>
        /// <param name="camera">The camera to project the screen point through.</param>
        /// <returns>The world space position of the mouse as a <see cref="Vector3"/>.</returns>
        public static Vector3 GetMouseWorldPosition(Camera camera) {
            return GetMouseWorldPosition(GetMouseScreenPosition(), camera);
        }

        /// <summary>
        /// Converts a specific screen position to world space, auto-adjusting the Z depth based on the camera distance.
        /// </summary>
        /// <param name="screenPosition">The screen position in pixels.</param>
        /// <param name="camera">The camera used for conversion. If null, falls back to <see cref="Camera.main"/>.</param>
        /// <returns>The converted world space position, or <see cref="Vector3.zero"/> if no camera is available.</returns>
        /// <remarks>
        /// Automatically forces the Z axis to match the camera's absolute distance to ensure accurate projection in 2D environments.
        /// </remarks>
        public static Vector3 GetMouseWorldPosition(Vector3 screenPosition, Camera camera = null) {
            Camera cam = camera ?? Camera.main;
            if (cam == null) {
                Debug.LogWarning("UtilsClass: No camera found to convert screen position to world position.");
                return Vector3.zero;
            }

            screenPosition.z = Mathf.Abs(cam.transform.position.z);
            return cam.ScreenToWorldPoint(screenPosition);
        }

        /// <summary>
        /// Abstracts screen position fetching depending on which Input System is active in the project settings.
        /// </summary>
        /// <returns>The current screen position of the cursor.</returns>
        public static Vector3 GetMouseScreenPosition() {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null) {
                return Mouse.current.position.ReadValue();
            }
#endif
            return Input.mousePosition;
        }

        public static Vector2 GetMouseCanvasPosition(RectTransform canvasRectTransform, Camera camera = null) {
            if (canvasRectTransform == null) {
                Debug.LogWarning("UtilsClass: Canvas RectTransform is null.");
                return Vector2.zero;
            }

            Vector3 screenPosition = GetMouseScreenPosition();
            Vector2 localPoint;

            // Se o Canvas for Screen Space - Overlay, a câmera DEVE ser null.
            // Se for Screen Space - Camera ou World Space, passa a câmera informada ou a Camera.main.
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRectTransform,
                screenPosition,
                camera,
                out localPoint
            );

            return localPoint;
        }

        // ─── 2D Physics & UI ────────────────────────────────────────────────

        /// <summary>
        /// Performs a 2D raycast from the current mouse world position to detect physics colliders.
        /// </summary>
        /// <param name="layerMask">The layer mask used to filter which colliders should be checked.</param>
        /// <returns>The <see cref="Collider2D"/> hit by the raycast, or <see langword="null"/> if nothing was intercepted.</returns>
        public static Collider2D GetMouseOverObject2D(LayerMask layerMask) {
            Vector3 mouseWorldPos = GetMouseWorldPosition();
            return GetPointOverObject2D(mouseWorldPos, layerMask);
        }

        public static Collider2D GetPointOverObject2D(Vector3 pointPos, LayerMask layerMask) {
            RaycastHit2D hit = Physics2D.Raycast(pointPos, Vector2.zero, 0f, layerMask);
            return hit.collider;
        }

        /// <summary>
        /// Checks if the mouse pointer is currently hovering over an Event System UI element.
        /// </summary>
        /// <returns><see langword="true"/> if the pointer is over UI; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// Highly recommended to prevent player input (like clicking a grid cell) from triggering behind active UI windows or buttons.
        /// </remarks>
        public static bool IsPointerOverUI() {
            return UnityEngine.EventSystems.EventSystem.current != null &&
                   UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject();
        }

        // ─── Trigonometry & Vectors ─────────────────────────────────────────

        /// <summary>
        /// Calculates the rotation angle in degrees from a given direction vector.
        /// </summary>
        /// <param name="direction">The direction vector to evaluate.</param>
        /// <returns>The angle in degrees ranging from 0 to 360.</returns>
        /// <remarks>
        /// Standard setup maps 0 degrees to the right axis (positive X), rotating counter-clockwise. Ideal for 2D look-at systems.
        /// </remarks>
        public static float GetAngleFromVector(Vector3 direction) {
            direction = direction.normalized;
            float n = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            if (n < 0)
                n += 360;
            return n;
        }

        /// <summary>
        /// Converts a specific angle into a directional unit vector.
        /// </summary>
        /// <param name="angle">The angle in degrees.</param>
        /// <returns>A normalized <see cref="Vector3"/> direction vector.</returns>
        public static Vector3 GetVectorFromAngle(float angle) {
            float angleRad = angle * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
        }

        public static Camera ResolveEventCamera(Canvas canvas) {
            if (canvas == null) {
                return null;
            }
            return canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
        }

        // ─── Debug Utilities ────────────────────────────────────────────────

        /// <summary>
        /// Spawns a 3D text mesh object in world space, primarily used for debugging grid data or coordinates.
        /// </summary>
        /// <param name="text">The initial string content to display.</param>
        /// <param name="parent">The parent transform component to attach the text object to.</param>
        /// <param name="localPosition">The local offset position relative to the parent.</param>
        /// <param name="fontSize">The sizing format of the font characters.</param>
        /// <param name="color">The color tint applied to the text mesh mesh renderers (defaults to White if null).</param>
        /// <param name="textAnchor">The pivot anchoring preset position of the text bounding box.</param>
        /// <param name="textAlignment">The horizontal text alignment styling inside multi-line fields.</param>
        /// <param name="sortingOrder">The sorting layer sequence ID order for render execution hierarchy layers.</param>
        /// <returns>The generated <see cref="TextMesh"/> component instance reference.</returns>
        public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPosition = default,
                                               int fontSize = 20, Color? color = null,
                                               TextAnchor textAnchor = TextAnchor.MiddleCenter,
                                               TextAlignment textAlignment = TextAlignment.Center, int sortingOrder = 5000) {
            GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;

            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color ?? Color.white;
            textMesh.GetComponent<MeshRenderer>().sortingOrder = sortingOrder;

            return textMesh;
        }

        /// <summary>
        /// Generates a randomized fully opaque RGB color value structure.
        /// </summary>
        /// <returns>A random <see cref="Color"/> with an Alpha value of 1.0f.</returns>
        public static Color GetRandomColor() {
            return new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), 1f);
        }
    }
}