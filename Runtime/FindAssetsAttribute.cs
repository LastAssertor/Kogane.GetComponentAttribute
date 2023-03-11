using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace Kogane
{
    /// <summary>
    /// AssetDatabase.FindAssets を実行する Attribute
    /// </summary>
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class FindAssetsAttribute
        : Attribute,
          IGetComponentAttribute
    {
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
            var fieldType = fieldInfo.FieldType;

            if (!serializedProperty.isArray)
            {
                var guid = AssetDatabase.FindAssets($"t:{fieldType.Name}").FirstOrDefault();

                if (string.IsNullOrEmpty(guid))
                {
                    return;
                }

                var assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath(assetPath, fieldType);

                serializedProperty.objectReferenceValue = asset;
                return;
            }

            var elementType = fieldType.GetElementType() ?? fieldType.GetGenericArguments().SingleOrDefault();

            var guids = AssetDatabase.FindAssets($"t:{elementType.Name}");

            var len = guids.Length;

            serializedProperty.arraySize = len;

            if (len == 0)
            {
                return;
            }

            var assets = guids.ToList().ConvertAll(x => AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(x), elementType));

            for (var i = 0; i < len; i++)
            {
                var element = serializedProperty.GetArrayElementAtIndex(i);

                element.objectReferenceValue = assets[i];
            }
        }
#endif
    }
}