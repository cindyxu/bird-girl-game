using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(IsSortingLayerAttribute))]
public class IsSortingLayerPropertyDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        int selected = -1;

        var names = new string[SortingLayer.layers.Length];
        for (int i = 0; i < SortingLayer.layers.Length; i++) {
            names[i] = SortingLayer.layers[i].name;
            if (property.stringValue == names[i])
                selected = i;
        }

        EditorGUI.BeginProperty(position, label, property);
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        selected = EditorGUI.Popup(position, selected, names);
        EditorGUI.EndProperty();

        if (selected >= 0)
            property.stringValue = names[selected];
    }
}