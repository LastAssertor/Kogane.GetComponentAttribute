using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Linq;
using System.Diagnostics;
using System.Reflection;

namespace Kogane
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public class FindChildrenWithTagAttribute : Attribute, IGetComponentAttribute
    {
        private readonly string m_tag;

        public FindChildrenWithTagAttribute(string tag)
        {
            m_tag = tag;
        }

#if UNITY_EDITOR
        public void Inject(MonoBehaviour monoBehaviour, FieldInfo fieldInfo, SerializedProperty serializedProperty)
        {
            if (!serializedProperty.isArray)
            {
                return;
            }

            var fieldType = fieldInfo.FieldType;
            var elementType = fieldType.GetElementType() ?? fieldType.GetGenericArguments().SingleOrDefault();

            var components = monoBehaviour.GetComponentsInChildren(elementType, true)
            .Where(x => x.CompareTag(m_tag)).ToArray();

            var componentCount = components.Length;

            serializedProperty.arraySize = componentCount;

            for (var i = 0; i < componentCount; i++)
            {
                var element = serializedProperty.GetArrayElementAtIndex(i);
                var component = components[i];

                element.objectReferenceValue = component;
            }
        }
#endif
    }
}