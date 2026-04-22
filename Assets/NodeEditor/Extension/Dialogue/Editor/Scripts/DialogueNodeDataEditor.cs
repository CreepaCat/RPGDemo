using UnityEditor;
using UnityEngine;

namespace MyNodeEditor.Extension.Dialogue
{
    [CustomEditor(typeof(DialogueNodeData), true)] // 指定目标脚本类型,true代表适用于子类预览
    public class DialogueNodeDataEditor : Editor
    {
        //绘制自定义检视面板
        public override void OnInspectorGUI()
        {
            // 获取序列化对象（支持子类）
            serializedObject.Update();

            //禁用脚本字段交互
            SerializedProperty scriptProperty = serializedObject.FindProperty("m_Script");

            EditorGUI.BeginDisabledGroup(true); //禁止交互字段区域
            if (scriptProperty != null)
            {

                EditorGUILayout.PropertyField(scriptProperty, new GUIContent("Script"));

            }

            //名字字段
            SerializedProperty speakerNameProperty = serializedObject.FindProperty("speakerName");
            if (speakerNameProperty != null)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(speakerNameProperty, new GUIContent("SpeakerName"));
                EditorGUI.EndDisabledGroup();
            }


            EditorGUILayout.BeginHorizontal();

            // 手动绘制 Sprite 字段
            SerializedProperty spriteProperty = serializedObject.FindProperty("avatar");
            EditorGUILayout.PropertyField(spriteProperty);

            // 如果 Sprite 已赋值，立即在字段后绘制预览
            if (spriteProperty.objectReferenceValue != null)
            {
                Sprite mySprite = (Sprite)spriteProperty.objectReferenceValue;

                Rect previewRect = GUILayoutUtility.GetRect(50f, 50f);

                // 绘制 Sprite 预览
                EditorGUI.DrawPreviewTexture(previewRect, mySprite.texture, null, ScaleMode.ScaleToFit);
            }
            else
            {
                //GUILayout.Label("No Sprite assigned.");
            }


            // 结束水平布局
            EditorGUILayout.EndHorizontal();

            EditorGUI.EndDisabledGroup();

            string[] dontDrawProperties = new[]
            {
                "m_Script", "speakerName", "avatar", "state", "started"
            };

            // 如果有其他字段，继续手动绘制它们（例如，子类字段）
            DrawPropertiesExcluding(serializedObject, dontDrawProperties);



            // 应用修改
            serializedObject.ApplyModifiedProperties();

        }
    }
}
