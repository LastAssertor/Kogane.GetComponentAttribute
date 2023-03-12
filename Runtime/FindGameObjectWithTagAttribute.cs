using System;
using System.Reflection;
using UnityEngine;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif
using System.Linq;
namespace Kogane
{
    /// <summary>
    /// GameObject.FindWithTag を実行する Attribute
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FindGameObjectWithTagAttribute
        : Attribute,
          IGetComponentAttribute
    {
        //================================================================================
        // 変数(readonly)
        //================================================================================
        private readonly string m_tag;

        //================================================================================
        // 関数
        //================================================================================
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FindGameObjectWithTagAttribute(string tag)
        {
            m_tag = tag;
        }

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

            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                return;
            }

            GameObject result = null;

            foreach (var go in monoBehaviour.gameObject.scene.GetRootGameObjects())
            {
                var transforms = go.GetComponentsInChildren<Transform>(true);

                var t = transforms.FirstOrDefault(x => x.CompareTag(m_tag));

                if (t != null)
                {
                    result = t.gameObject;
                    break;
                }
            }

            serializedProperty.objectReferenceValue = result;
        }
#endif
    }
}