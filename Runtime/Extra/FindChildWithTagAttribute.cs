using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Linq;
using System.Diagnostics;

namespace Kogane
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public class FindChildWithTagAttribute : Attribute, IGetComponentAttribute
    {
        private readonly string m_tag;

        public FindChildWithTagAttribute(string tag)
        {
            m_tag = tag;
        }

#if UNITY_EDITOR
        public void Inject(MonoBehaviour monoBehaviour, FieldInfo fieldInfo, SerializedProperty serializedProperty)
        {
            if (serializedProperty.isArray)
            {
                return;
            }

            var fieldType = fieldInfo.FieldType;

            var components = monoBehaviour.GetComponentsInChildren(fieldType, true);

            serializedProperty.objectReferenceValue = components.FirstOrDefault(x => x.CompareTag(m_tag));
        }
#endif
    }
}