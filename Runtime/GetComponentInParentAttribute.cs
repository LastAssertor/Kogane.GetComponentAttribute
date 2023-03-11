using System;
using System.Reflection;
using UnityEngine;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Kogane
{
    /// <summary>
    /// GetComponentInParent を実行する Attribute
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class GetComponentInParentAttribute
        : Attribute,
          IGetComponentAttribute
    {
        private readonly bool m_includeInactive;

        public GetComponentInParentAttribute() : this(true)
        {
        }

        public GetComponentInParentAttribute(bool includeInactive)
        {
            m_includeInactive = includeInactive;
        }

        //================================================================================
        // 関数
        //================================================================================
#if UNITY_EDITOR
        /// <summary>
        /// 指定されたパラメータに参照を割り当てます
        /// </summary>
        public void Inject
        (
            MonoBehaviour monoBehaviour,
            FieldInfo fieldInfo,
            SerializedProperty serializedProperty
        )
        {
            if (serializedProperty.isArray)
            {
                return;
            }

            serializedProperty.objectReferenceValue =
                monoBehaviour.GetComponentInParent(fieldInfo.FieldType, m_includeInactive);
        }
#endif
    }
}