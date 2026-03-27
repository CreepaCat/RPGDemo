using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Core.ObjectPool
{

    /// <summary>
    /// 以预制体作为key的对象池管理器，GetPool()获取对象池，Get()获取对象，Release()释放对象
    ///Get()之后先Setup() 然后SetActive(true);
    /// </summary>
    public class ObjectPool : MonoBehaviour
    {

        [SerializeField] private Transform poolRoot;

        private Dictionary<GameObject, IObjectPool<GameObject>> pools = null;

        /// <summary>
        /// 获取对象池，若没有则创建
        /// </summary>
        public IObjectPool<GameObject> GetPool(GameObject originPrefab)
        {
            if (pools == null) pools = new();

            if (!pools.ContainsKey(originPrefab))
            {
                CreateNewPool(originPrefab);
            }
            return pools[originPrefab];

        }

        private void CreateNewPool(GameObject originPrefab, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 1000)
        {
            if (pools.ContainsKey(originPrefab)) return;

            ObjectPool<GameObject> newPool = new ObjectPool<GameObject>(
               //直接匿名函数传参
               () =>
               {
                   GameObject obj = Instantiate(originPrefab, poolRoot);
                   obj.gameObject.SetActive(false);
                   return obj;
               },
               OnGet,
               OnRelease,
               OnDestroyObject,
               collectionCheck,
               defaultCapacity,
               maxSize);

            pools[originPrefab] = newPool;
        }

        public void DelayRelease(float delayTime, GameObject objToRelease, GameObject prefab)
        {
            if (!pools.ContainsKey(prefab)) return;
            if (objToRelease.activeSelf == false) return;
            StartCoroutine(DelayReleaseCR(delayTime, objToRelease, pools[prefab]));

        }
        private IEnumerator DelayReleaseCR(float delayTime, GameObject objToRelease, IObjectPool<GameObject> pool)
        {
            yield return new WaitForSeconds(delayTime);
            if (objToRelease.activeSelf == false) yield break;
            pool.Release(objToRelease);

        }


        #region 对象池回调方法
        //^创建对象时，直接匿名函数传参CreateObject()

        private void OnGet(GameObject obj)
        {
            obj.transform.parent = transform;
        }

        private void OnRelease(GameObject obj)
        {
            obj.gameObject.SetActive(false);
            obj.transform.parent = poolRoot;

        }
        private void OnDestroyObject(GameObject obj)
        {
            Destroy(obj);
        }
        #endregion


    }
}
