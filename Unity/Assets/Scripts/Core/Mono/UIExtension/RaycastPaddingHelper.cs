using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif
/// <summary>
/// I also needed this resource for a while, so I decided to make a MacGyverism.
/// it is only a partial and visual remedy in response to the topic:
/// https://forum.unity.com/threads/ui-image-component-raycast-padding-needs-a-gizmo.1019260/
/// how to config: https://imgur.com/a/NVpHgzu 
/// I hope it helps.
/// @AllanSamurai
/// </summary>
[RequireComponent(typeof(Image), typeof(RectTransform))]
public class RaycastPaddingHelper : UIBehaviour
{
#if UNITY_EDITOR

    #region https://github.com/Unity-Technologies/UnityCsReference/blob/61f92bd79ae862c4465d35270f9d1d57befd1761/Editor/Mono/Inspector/RectTransformEditor.cs#L24-L26
    private static Vector2 kShadowOffset = new Vector2(1, -1);
    private static Color kShadowColor = new Color(0, 0, 0, 0.5f);
    private const float kDottedLineSize = 5f;
    #endregion

    void OnDrawGizmos()
    {
        Image image = GetComponent<Image>();
        RectTransform gui = GetComponent<RectTransform>();

        #region https://github.com/Unity-Technologies/UnityCsReference/blob/61f92bd79ae862c4465d35270f9d1d57befd1761/Editor/Mono/Inspector/RectTransformEditor.cs#L646-L660
        Rect rectInOwnSpace = gui.rect;
        // Rect rectInUserSpace = rectInOwnSpace;
        Rect rectInParentSpace = rectInOwnSpace;
        Transform ownSpace = gui.transform;
        // Transform userSpace = ownSpace;
        Transform parentSpace = ownSpace;
        RectTransform guiParent = null;
        if (ownSpace.parent != null)
        {
            parentSpace = ownSpace.parent;
            rectInParentSpace.x += ownSpace.localPosition.x;
            rectInParentSpace.y += ownSpace.localPosition.y;

            guiParent = parentSpace.GetComponent<RectTransform>();
        }
        #endregion

        // patSilva's post: https://forum.unity.com/threads/ui-image-component-raycast-padding-needs-a-gizmo.1019260/#post-6828020
        // The image.raycastPadding order of the Vector4 is:
        // X = Left
        // Y = Bottom
        // Z = Right
        // W = Top

        Rect paddingRect = new Rect(rectInParentSpace);
        paddingRect.xMin += image.raycastPadding.x;
        paddingRect.xMax -= image.raycastPadding.z;
        paddingRect.yMin += image.raycastPadding.y;
        paddingRect.yMax -= image.raycastPadding.w;

        // uncomment below line to show only when rect tool is active
        // if (Tools.current == Tool.Rect)
        {
            //change the color of the handles as you wish
            Handles.color = Color.green;
            DrawRect(paddingRect, parentSpace, true);
        }
    }

    #region https://github.com/Unity-Technologies/UnityCsReference/blob/61f92bd79ae862c4465d35270f9d1d57befd1761/Editor/Mono/Inspector/RectTransformEditor.cs#L618-L638
    void DrawRect(Rect rect, Transform space, bool dotted)
    {
        Vector3 p0 = space.TransformPoint(new Vector2(rect.x, rect.y));
        Vector3 p1 = space.TransformPoint(new Vector2(rect.x, rect.yMax));
        Vector3 p2 = space.TransformPoint(new Vector2(rect.xMax, rect.yMax));
        Vector3 p3 = space.TransformPoint(new Vector2(rect.xMax, rect.y));
        if (!dotted)
        {
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p1, p2);
            Handles.DrawLine(p2, p3);
            Handles.DrawLine(p3, p0);
        }
        else
        {
            DrawDottedLineWithShadow(kShadowColor, kShadowOffset, p0, p1, kDottedLineSize);
            DrawDottedLineWithShadow(kShadowColor, kShadowOffset, p1, p2, kDottedLineSize);
            DrawDottedLineWithShadow(kShadowColor, kShadowOffset, p2, p3, kDottedLineSize);
            DrawDottedLineWithShadow(kShadowColor, kShadowOffset, p3, p0, kDottedLineSize);
        }
    }
    #endregion

    #region https://github.com/Unity-Technologies/UnityCsReference/blob/61f92bd79ae862c4465d35270f9d1d57befd1761/Editor/Mono/Inspector/RectHandles.cs#L278-L296
    public static void DrawDottedLineWithShadow(Color shadowColor, Vector2 screenOffset, Vector3 p1, Vector3 p2, float screenSpaceSize)
    {
        Camera cam = Camera.current;
        if (!cam || Event.current.type != EventType.Repaint)
            return;

        Color oldColor = Handles.color;

        // shadow
        shadowColor.a = shadowColor.a * oldColor.a;
        Handles.color = shadowColor;
        Handles.DrawDottedLine(
            cam.ScreenToWorldPoint(cam.WorldToScreenPoint(p1) + (Vector3)screenOffset),
            cam.ScreenToWorldPoint(cam.WorldToScreenPoint(p2) + (Vector3)screenOffset), screenSpaceSize);

        // line itself
        Handles.color = oldColor;
        Handles.DrawDottedLine(p1, p2, screenSpaceSize);
    }
    #endregion

#endif
}
