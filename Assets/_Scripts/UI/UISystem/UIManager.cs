using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
namespace RPGDemo.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }


        [Header("面板配置")]
        public List<PanelPrefab> panelPrefabs = new List<PanelPrefab>();

        // 当前打开的面板栈
        private Stack<BasePanel> panelStack = new Stack<BasePanel>();

        // 所有面板的缓存
        private Dictionary<string, BasePanel> panelCache = new Dictionary<string, BasePanel>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            //无需永久化
            // DontDestroyOnLoad(gameObject);

            Initialize();
        }


        private void Initialize()
        {
            foreach (var panelPrefab in panelPrefabs)
            {
                string panelName = panelPrefab.prefab.gameObject.name;
                if (panelCache.ContainsKey(panelName))
                {
                    Debug.Log($"字典已有面板{panelName}");
                    continue;
                }
                panelCache[panelName] = panelPrefab.prefab;
            }
        }

        private void Update()
        {   //主菜单UI不通过Esc关闭
            if (SceneManager.GetActiveScene().buildIndex == 0)
                return;

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                if (panelStack.Count > 0)
                {

                    ClosePanel();

                }
                else
                {

                    OpenPanel<PauseMenu>();
                }
            }
        }

        private void SetCursorState(CursorLockMode cursorLockMode)
        {
            Cursor.lockState = cursorLockMode;
        }

        /// <summary>
        /// 打开面板
        /// </summary>
        public T OpenPanel<T>() where T : BasePanel
        {


            string panelName = typeof(T).Name;

            // 如果已经在栈顶，直接返回
            if (panelStack.Count > 0 && panelStack.Peek().GetType() == typeof(T))
            {

                return panelStack.Peek() as T;
            }

            // 查找面板
            BasePanel panel = GetOrCreatePanel(panelName);
            if (panel == null)
            {
                Debug.Log($"找不到面板{panelName}，是否未加入初始化？");
                return null;
            }

            // 关闭当前栈顶面板（不销毁）
            if (panelStack.Count > 0)
            {
                panelStack.Peek().OnHide();
            }

            // 打开新面板

            panel.OnShow();

            panelStack.Push(panel);

            if (panelStack.Count > 0)
            {
                //有打开的ui时禁止角色操控输入
                Player.GetInstance()?.DisablePlayerControl();
                SetCursorState(CursorLockMode.None);
            }

            return panel as T;
        }

        /// <summary>
        /// 关闭当前最上层面板（返回上一个）
        /// </summary>
        public void ClosePanel()
        {
            if (panelStack.Count == 0) return;

            BasePanel current = panelStack.Pop();
            current.OnHide();
            // current.gameObject.SetActive(false);

            // 显示上一个面板
            if (panelStack.Count > 0)
            {
                BasePanel previous = panelStack.Peek();
                //previous.gameObject.SetActive(true);
                previous.OnShow();
            }
            if (panelStack.Count == 0)
            {
                //若没有新的UIpanel恢复角色操控输入
                Player.GetInstance()?.EnablePlayerControl();
                SetCursorState(CursorLockMode.Locked);
            }
        }

        /// <summary>
        /// 强制关闭指定面板（不走栈逻辑）
        /// </summary>
        public void ClosePanel<T>() where T : BasePanel
        {
            string panelName = typeof(T).Name;
            if (panelCache.TryGetValue(panelName, out BasePanel panel))
            {
                if (panelStack.Contains(panel))
                {
                    // 从栈中移除
                    Stack<BasePanel> temp = new Stack<BasePanel>();
                    while (panelStack.Count > 0)
                    {
                        BasePanel p = panelStack.Pop();
                        if (p != panel) temp.Push(p);
                    }
                    while (temp.Count > 0)
                    {
                        panelStack.Push(temp.Pop());
                    }
                }

                panel.OnHide();


                // panel.gameObject.SetActive(false);
            }
            if (panelStack.Count == 0)
            {
                //若没有新的UIpanel恢复角色操控输入
                Player.GetInstance()?.EnablePlayerControl();
                SetCursorState(CursorLockMode.Locked);
            }
        }

        private BasePanel GetOrCreatePanel(string panelName)
        {
            // 缓存中已有
            if (panelCache.TryGetValue(panelName, out BasePanel cachedPanel))
            {
                return cachedPanel;
            }
            return null;
        }

    }

    // 用于 Inspector 中方便拖入 Prefab
    [System.Serializable]
    public class PanelPrefab
    {
        public BasePanel prefab;
        public Canvas canvas;        // 面板所属Canvas
    }
}
