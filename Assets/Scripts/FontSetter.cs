using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FontSetter : MonoBehaviour
{
    public Font font;
    public void SetFont()
    {
        Text[] texts = GetComponentsInChildren<Text>();
        for (int i = 0; i < texts.Length; i++)
            texts[i].font = font;
    }
    public void SetNativeSize()
    {
        Image[] images = GetComponentsInChildren<Image>();
        for (int i = 0; i < images.Length; i++)
            images[i].SetNativeSize();
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(FontSetter))]
[CanEditMultipleObjects]
class FontSetterIspector : Editor
{
    FontSetter scr;
    private void OnEnable()
    {
        scr = (FontSetter)target;
    }
    public override void OnInspectorGUI()
    {
        GUILayout.BeginHorizontal();
        scr.font = (Font)EditorGUILayout.ObjectField(scr.font,typeof(Font),false,GUILayout.Width(200));
        if (GUILayout.Button("Set Font"))
            scr.SetFont();
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Set Native Size"))
            scr.SetNativeSize();
    }
}
#endif
