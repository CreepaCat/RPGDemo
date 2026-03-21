using System.Linq;
using UnityEditor;
using UnityEngine;
using RPGDemo.Core;

[CustomEditor(typeof(ConditionSO))]
public class ConditionSOEditor : Editor
{
    private bool? _lastTestResult = null;
    private string _lastMessage = "";

    public override void OnInspectorGUI()
    {
        // 绘制默认字段（包含 predicateType 枚举）
        DrawDefaultInspector();

        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("=== 条件测试工具 ===", EditorStyles.boldLabel);

        GUI.enabled = Application.isPlaying;
        if (GUILayout.Button("🚀 立即测试条件", GUILayout.Height(45)))
        {
            TestCondition();
        }
        GUI.enabled = true;

        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("⚠️ 请先进入 Play Mode 再测试", MessageType.Warning);
        }

        if (_lastTestResult.HasValue)
        {
            string resultText = _lastTestResult.Value ? "✅ 通过（True）" : "❌ 不通过（False）";
            MessageType type = _lastTestResult.Value ? MessageType.Info : MessageType.Error;
            EditorGUILayout.HelpBox($"测试结果：{resultText}\n\n{_lastMessage}", type);
        }

        EditorGUILayout.Space(15);

        // ────────────────────────────────────────────────
        // 逻辑等价预览（根据枚举显示提示）
        // ────────────────────────────────────────────────
        EditorGUILayout.LabelField("=== 当前谓词类型说明 ===", EditorStyles.boldLabel);

        var condition = (ConditionSO)target;

        string previewText = GetPredicatePreview(condition.GetPredicateType(), condition.GetIsNegative());

        MessageType previewType = condition.GetIsNegative() ? MessageType.Warning : MessageType.Info;
        EditorGUILayout.HelpBox(previewText, previewType);

        EditorGUILayout.HelpBox(
            "提示：\n" +
            "• 勾选「是否整体取反」后，系统会自动取反结果。\n" +
            "• 枚举类型仅用于标识，不影响参数填写方式。",
            MessageType.Info);
    }

    private string GetPredicatePreview(Predicate predicate, bool isNegative)
    {
        string baseDesc = predicate switch
        {
            Predicate.None       => "无条件（始终通过）",
            Predicate.HasItem    => "拥有指定物品",
            Predicate.Kill       => "击杀指定敌人/达到击杀数",
            Predicate.Collection => "收集指定物品/达到收集数",
            _                    => "未知谓词类型"
        };

        if (!isNegative)
        {
            return $"当前谓词：{baseDesc}";
        }

        string negatedDesc = predicate switch
        {
            Predicate.None       => "无条件（始终通过）——取反无意义",
            Predicate.HasItem    => "不拥有指定物品（或数量不足）",
            Predicate.Kill       => "未达到击杀数量",
            Predicate.Collection => "未达到收集数量",
            _                    => "未知谓词类型"
        };

        return $"当前谓词：{baseDesc}\n" +
               $"（已取反）等价于：{negatedDesc}";
    }

    private void TestCondition()
    {
        var condition = (ConditionSO)target;

        if (ConditionHandler.GetInstance() == null)
        {
            _lastTestResult = null;
            _lastMessage = "❌ ConditionHandler.Instance 为 null！";
            Debug.LogError(_lastMessage);
            return;
        }

        bool result = condition.Check();

        _lastTestResult = result;
        _lastMessage = $"谓词类型：{condition.GetPredicateType()} | " +
                       $"取反：{(condition.GetIsNegative() ? "是" : "否")} | " +
                       $"参数数量：{condition.GetParameters()?.ToList().Count ?? 0}";

        Debug.Log($"[Condition 测试] {condition.name} → {result}   |   {_lastMessage}");
    }
}