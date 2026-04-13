using System.Collections;
using RPGDemo.Saving;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPGDemo.SceneManagement
{

    [RequireComponent(typeof(Saving.SavingSystem))]
    public class SavingWrapper : MonoBehaviour
    {

        [SerializeField] private Key saveKey = Key.U;
        // [SerializeField] private Key loadFileKey = Key.I;
        [SerializeField] private Key loadKey = Key.L;
        [SerializeField] private Key deleteKey = Key.K;

        const string defaultSaveFile = "RPGDemo";

        private void Awake()
        {
            //每次自动加载上次存档场景
            // StartCoroutine(LoadLastSceneAsync());
            //通过主菜单选择加载
        }

        private IEnumerator LoadLastSceneAsync()
        {
            yield return GetComponent<SavingSystem>().LoadLastSceneAsync(defaultSaveFile);
        }

        //TEST
        private void Update()
        {
            if (Keyboard.current[saveKey].wasPressedThisFrame)
            {

                Save();
            }
            // if (Keyboard.current[loadFileKey].wasPressedThisFrame)
            // {
            //
            //     LoadFile();
            // }

            if (Keyboard.current[loadKey].wasPressedThisFrame)
            {
                Debug.Log($"{loadKey} key was just pressed");
                // StartCoroutine(LoadLastSceneAsync("SaveableCube"));
                LoadScene();

            }

            if (Keyboard.current[deleteKey].wasPressedThisFrame)
            {
                DeleteFile();
            }
        }

        public void Save()
        {
            GetComponent<SavingSystem>().SaveFile(defaultSaveFile);
            BottomMessageBox.ShowCustom("游戏进度已保存");
        }

        /// <summary>
        /// 加载场景,并加载数据
        /// </summary>
        public void LoadScene()
        {
            StartCoroutine(LoadLastSceneAsync());
        }

        public bool HasFile()
        {
            return GetComponent<SavingSystem>().HasFile(defaultSaveFile);
        }

        /// <summary>
        /// 不加载场景,仅加载数据
        /// </summary>
        public void LoadFile()
        {
            GetComponent<SavingSystem>().LoadFile(defaultSaveFile);
        }

        public void DeleteFile()
        {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }



    }
}
