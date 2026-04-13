using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPGDemo.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        //CONFIG
        private const string SavingJsonExtension = ".sav";

        private const string LastSceneBuildIndexString = "lastSceneInBuildIndex";


        // PUBLIC
        /// <summary>
        /// 存档
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveFile(string fileName)
        {

            //存档前先尝试读取同名文件，创建或读取同名文件为JObject对象
            JObject state = LoadOrCreateJObject(fileName);

            //然后将数据存进JObject对象并存储为Json文件
            using (var streamWriter = File.CreateText(GetPathFromSaveFile(fileName)))
            {
                using (var writer = new JsonTextWriter(streamWriter))
                {
                    CaptureState(state);

                    writer.Formatting = Formatting.Indented; //json格式是否缩进
                    state.WriteTo(writer);
                }
            }

            Debug.Log("Save to: " + GetPathFromSaveFile(fileName));


        }

        public bool HasFile(string fileName)
        {
            return HasPath(fileName);
        }

        /// <summary>
        /// 不重新加载场景的读档
        /// </summary>
        /// <param name="fileName"></param>
        public void LoadFile(string fileName)
        {
            JObject state = LoadOrCreateJObject(fileName);
            if (state == null)
            {
                Debug.Log("没有找到存档文件" + GetPathFromSaveFile(fileName));
                return;
            }

            RestoreState(state);
            Debug.Log("Load from: " + GetPathFromSaveFile(fileName));
        }

        /// <summary>
        /// 重新加载场景并读取存档数据
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public IEnumerator LoadLastSceneAsync(string fileName)
        {
            if (!HasPath(fileName))
            {
                Debug.Log("找不到存档" + GetPathFromSaveFile(fileName));
                Debug.Log("使用默认场景 ");
                yield break;
            }

            JObject state = LoadOrCreateJObject(fileName);
            IDictionary<string, JToken> stateDict = state;

            //默认使用当前场景的索引，若存档中有上次保存的场景索引，则使用存档的索引
            int buildIndex = SceneManager.GetActiveScene().buildIndex;

            if (stateDict.ContainsKey(LastSceneBuildIndexString))
            {
                buildIndex = stateDict[LastSceneBuildIndexString].Value<int>();
            }
            yield return SceneManager.LoadSceneAsync(buildIndex);

            //保证场景加载完毕再读取存档数据
            RestoreState(state);
            Debug.Log("Load scene and file from: " + GetPathFromSaveFile(fileName));

        }

        /// <summary>
        /// 删除存档
        /// </summary>
        /// <param name="fileName"></param>
        public void Delete(string fileName)
        {
            File.Delete(GetPathFromSaveFile(fileName));
            Debug.Log("Delete file: " + GetPathFromSaveFile(fileName));
        }


        //PRIVATE

        /// <summary>
        /// 读取存档数据
        /// </summary>
        /// <param name="state"></param>
        private void RestoreState(IDictionary<string, JToken> state)
        {
            //IDictionary<string, JToken> state = state;
            foreach (var saveableEntity in FindObjectsByType<SaveableEntity>(FindObjectsSortMode.None))
            {
                string id = saveableEntity.GetUniqueIdentifier();
                if (state.ContainsKey(id))
                {
                    saveableEntity.RestoreState(state[id]);
                }
            }
        }

        /// <summary>
        /// 存储存档数据
        /// </summary>
        /// <param name="state"></param>
        private void CaptureState(IDictionary<string, JToken> state)
        {
            // IDictionary<string, JToken> state = state;
            foreach (var saveableEntity in FindObjectsByType<SaveableEntity>(FindObjectsSortMode.None))
            {
                state[saveableEntity.GetUniqueIdentifier()] = saveableEntity.CapatureState();
            }
            //记录当前场景index
            state[LastSceneBuildIndexString] = SceneManager.GetActiveScene().buildIndex;
        }

        private JObject LoadOrCreateJObject(string fileName)
        {
            if (!HasPath(fileName))
            {
                return new JObject();
            }

            using (var streamReader = File.OpenText(GetPathFromSaveFile(fileName)))
            {
                // Debug.Log(" File.OpenText :" + GetPathFromSaveFile(fileName));
                using (var reader = new JsonTextReader(streamReader))
                {
                    //定义如何解析浮点数
                    reader.FloatParseHandling = FloatParseHandling.Double;
                    return JObject.Load(reader);
                }
            }

        }

        private bool HasPath(string saveFileName)
        {
            string path = GetPathFromSaveFile(saveFileName);
            if (!File.Exists(path))
            {
                return false;
            }
            return true;
        }


        private string GetPathFromSaveFile(string saveFileName)
        {
            string fullPath = Path.Combine(Application.persistentDataPath, saveFileName + SavingJsonExtension);
            fullPath = fullPath.Replace('\\', '/');
            return fullPath;
        }

    }
}
