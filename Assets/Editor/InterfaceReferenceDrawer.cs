using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(InterfaceReference<>))]
public class InterfaceReferenceDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty targetProp = property.FindPropertyRelative("target");

        EditorGUI.BeginProperty(position, label, property);

        targetProp.objectReferenceValue = EditorGUI.ObjectField(
            position,
            label,
            targetProp.objectReferenceValue,
            typeof(MonoBehaviour),
            true
        );

        property.serializedObject.ApplyModifiedProperties();

        var interfaceReference = fieldInfo.GetValue(property.serializedObject.targetObject);
        var isValidMethod = interfaceReference.GetType().GetProperty("IsValid").GetGetMethod();
        bool isValid = (bool)isValidMethod.Invoke(interfaceReference, null);

        if (targetProp.objectReferenceValue != null && !isValid)
        {
            targetProp.objectReferenceValue = null;
            Debug.LogWarning("Assigned object does not implement the required interface!");
        }

        EditorGUI.EndProperty();
    }
}