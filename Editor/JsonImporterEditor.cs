using System;
using System.Collections;
using System.IO;
using System.Reflection;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using Object = System.Object;

namespace Editor
{
    public class JsonImporterEditor : UnityEditor.Editor
    {
        private JObject _obj;
        private bool _isJson;
        private UnityEditor.Editor _editor;

        private void OnEnable()
        {
            var txt = ((TextAsset) target).text;
            _obj = JObject.Parse(txt);
            _isJson = Path.GetExtension(AssetDatabase.GetAssetPath(target)) == ".json";
            _editor = _isJson ? null : CreateEditor(target, typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.TextAssetInspector"));
        }

        private void OnDisable()
        {
            DestroyImmediate(_editor);
        }

        public override void OnInspectorGUI()
        {
            if (!_isJson)
            {
                _editor.OnInspectorGUI();
            }
            else
            {
                GUI.enabled = true;
                Draw(_obj);
                GUI.enabled = false;
            }
        }

        private static void Draw(JObject obj)
        {
            foreach (var keyValuePair in obj)
            {
                var (key, value) = (keyValuePair.Key, keyValuePair.Value);
                using (new EditorGUI.IndentLevelScope())
                {
                    if (value != null && value.Type == JTokenType.Array)
                    {
                        if (Foldout(value, key))
                        {
                            Draw(value.Value<JArray>());
                        }

                        continue;
                    }

                    if (value != null && value.Type == JTokenType.Object)
                    {
                        if (Foldout(value, key))
                        {
                            Draw(value.Value<JObject>());
                        }

                        continue;
                    }
                }

                EditorGUILayout.LabelField($"{key}: {value}");
            }
        }

        private static void Draw(JArray obj)
        {
            for (var index = 0; index < obj.Count; index++)
            {
                var token = obj[index];
                if (Foldout(token, $"element {index}"))
                    using (new EditorGUI.IndentLevelScope())
                    {
                        switch (token.Type)
                        {
                            case JTokenType.Object:
                                Draw(token.Value<JObject>());

                                break;
                            case JTokenType.Array:
                                using (new EditorGUI.IndentLevelScope())
                                    Draw(token.Value<JArray>());
                                break;
                            default:
                                var value = token.ToString(Formatting.Indented);
                                EditorGUILayout.LabelField(value);
                                break;
                        }
                    }

                
            }
        }

        private static bool Foldout(JToken token, string title)
        {
            var val = EditorGUILayout.Foldout(SessionState.GetBool(token.Path, true), title);
            SessionState.SetBool(token.Path, val);
            return val;
        }
    }

    internal static class Injector
    {
        [InitializeOnLoadMethod]
        private static void Inject()
        {
            var customEditorAttributesType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.CustomEditorAttributes");
            var initFieldInfo = GetStaticNonPublicFieldInfo(customEditorAttributesType, "s_Initialized");
            if (!(bool) initFieldInfo.GetValue(null)) //If initialized is false, the dictionary has not been created yet, so we must do it
            {
                var rebuildMethodInfo = GetStaticPrivateMethodInfo(customEditorAttributesType, "Rebuild");
                rebuildMethodInfo.Invoke(null, null);
            }

            var dictFieldInfo = GetStaticNonPublicFieldInfo(customEditorAttributesType, "kSCustomEditors");
            var typeToEditorDictionary = (IDictionary) dictFieldInfo.GetValue(null);

            foreach (var entry in typeToEditorDictionary)
            {
                var (key, list) = GetKeyValuePairInfo(entry);
                if (key.Name != nameof(TextAsset))
                {
                    continue;
                }

                foreach (var obj in list)
                {
                    var objType = obj.GetType();
                    var inspectorTypeInfo = GetPublicFieldInfo(objType, "m_InspectorType");
                    inspectorTypeInfo.SetValue(obj, typeof(JsonImporterEditor));
                }
            }

            initFieldInfo.SetValue(null, true);
        }

        private static MethodInfo GetStaticPrivateMethodInfo(Type customEditorAttributes, string methodName)
        {
            return customEditorAttributes.GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
        }

        private static FieldInfo GetPublicFieldInfo(Type objType, string fieldName)
        {
            return objType.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance);
        }

        private static FieldInfo GetStaticNonPublicFieldInfo(Type type, string fieldName)
        {
            return type.GetField(fieldName, BindingFlags.Static | BindingFlags.NonPublic);
        }

        private static (Type keyInfo, IList valueInfo) GetKeyValuePairInfo(Object entry)
        {
            var type = entry.GetType();
            var keyPropInfo = type.GetProperty("Key", BindingFlags.Instance | BindingFlags.Public);
            var valuePropInfo = type.GetProperty("Value", BindingFlags.Instance | BindingFlags.Public);
            return ((Type) keyPropInfo?.GetValue(entry), (IList) valuePropInfo?.GetValue(entry));
        }
    }
}