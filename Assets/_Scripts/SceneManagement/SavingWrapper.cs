using System.Collections;
using QFSW.QC;
using RPGDemo.Saving;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

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


        private IEnumerator LoadLastSceneAsync()
        {
            Fader fader = FindFirstObjectByType<Fader>();
            yield return fader.FadeOut(0.2f);
            yield return GetComponent<SavingSystem>().LoadLastSceneAsync(defaultSaveFile);
            yield return fader.LoadingProgress(1f);
            yield return fader.FadeIn(0.2f);
        }

        //TEST
        private void Update()
        {
            if (GameStatus.Instance.IsUsingConsole) return;
            if (Keyboard.current[saveKey].wasPressedThisFrame)
            {

                Save();
            }


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
        [Command("load-scene")]
        public void LoadScene()
        {
            StartCoroutine(LoadLastSceneAsync());
        }
        /// <summary>
        /// 新游戏
        /// </summary>
        internal void LoadNewGame()
        {
            StartCoroutine(LoadNewGameAsync());
        }

        IEnumerator LoadNewGameAsync()
        {
            Fader fader = FindFirstObjectByType<Fader>();
            yield return fader.FadeOut(0.2f);
            yield return SceneManager.LoadSceneAsync(1);
            yield return fader.LoadingProgress(1f);
            yield return fader.FadeIn(0.2f);
        }

        /// <summary>
        /// 玩家死亡重生
        /// </summary>
        internal void Respawn()
        {
            StartCoroutine(RespawnAsync());
        }

        IEnumerator RespawnAsync()
        {
            Fader fader = FindFirstObjectByType<Fader>();

            yield return fader.FadeOut(0.2f);
            yield return SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
            LoadFile();
            //寻找角色出生点并更新角色位置
            Portal portal = GameObject.FindFirstObjectByType<Portal>();
            portal.UpdatePlayer(portal);
            yield return fader.LoadingProgress(1f);
            yield return fader.FadeIn(0.2f);

            Save();//重生后存档
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
