using System;
using System.Reflection;
using UnityEngine;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;

#endif

namespace Kogane
{
    /// <summary>
    /// Object.FindObjectsOfType を実行する Attribute
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FindObjectsOfTypeAttribute
        : Attribute,
          IGetComponentAttribute
    {
#if UNITY_2020_1_OR_NEWER
        //================================================================================
        // 変数(readonly)
        //================================================================================
        private readonly bool m_includeInactive;

        //================================================================================
        // 関数
        //================================================================================
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FindObjectsOfTypeAttribute() : this(true)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FindObjectsOfTypeAttribute(bool includeInactive)
        {
            m_includeInactive = includeInactive;
        }
#endif

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
            if (!serializedProperty.isArray)
            {
                return;
            }

            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                return;
            }

            var fieldType = fieldInfo.FieldType;
            var elementType = fieldType.GetElementType() ?? fieldType.GetGenericArguments().SingleOrDefault();

            List<Object> components = new();

            foreach (var go in monoBehaviour.gameObject.scene.GetRootGameObjects())
            {

                components.AddRange(go.GetComponentsInChildren(elementType, m_includeInactive));
            }

            var componentCount = components.Count;

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