using UnityEditor;
using UnityEngine;

public class AlignAnchors : EditorWindow
{
    [MenuItem("Tools/Print Selected Object %&a")] // Ctrl + Alt + A
    private static void Align()
    {
        if (Selection.activeGameObject == null)
        {
            Debug.Log("No object selected in the scene.");
            return;
        }
        RectTransform rectTransform = Selection.activeGameObject.GetComponent<RectTransform>();
        RectTransform parentRect = rectTransform.parent.GetComponent<RectTransform>();

        Vector3 worldPosition = rectTransform.position;
        Vector2 imageSize = rectTransform.rect.size;
        Vector2 parentSize = parentRect.rect.size;

        Vector2 anchorMin = new Vector2(
            (rectTransform.localPosition.x - imageSize.x * rectTransform.pivot.x) / parentSize.x + 0.5f,
            (rectTransform.localPosition.y - imageSize.y * rectTransform.pivot.y) / parentSize.y + 0.5f
        );

        Vector2 anchorMax = new Vector2(
            anchorMin.x + imageSize.x / parentSize.x,
            anchorMin.y + imageSize.y / parentSize.y
        );

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
        rectTransform.position = worldPosition;
    }
}
