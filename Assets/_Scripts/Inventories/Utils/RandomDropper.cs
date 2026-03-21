using UnityEngine;
using UnityEngine.AI;

namespace RPGDemo.Inventories.Utils
{


    /// <summary>
    /// 获取角色附近的随机掉落位置
    /// </summary>
    public class RandomDropper : ItemDropper
    {
        [SerializeField] private float dropDistance = 1f;
        private const int ATTEMPS = 30;//尝试30次获取掉落位置


        /// <summary>
        /// 重写获取物品掉落位置方法
        /// </summary>
        /// <returns></returns>
        protected override Vector3 GetDropLocation()
        {

            for (int i = 0; i < ATTEMPS; i++)
            {
                Vector2 randomCircle = UnityEngine.Random.insideUnitCircle;
                Vector3 dropLocation = transform.position + new Vector3(randomCircle.x, 0, randomCircle.y) * dropDistance;

                //判断随机点是否在地面
                NavMeshHit hit;
                if (NavMesh.SamplePosition(dropLocation, out hit, 0.2f, NavMesh.AllAreas))
                {
                    // Debug.Log("随机位置：" +hit.position);
                    return hit.position;
                }

            }
            Debug.Log("没有合法位置，就掉落在脚下");
            //如果没有合法位置，就掉落在脚下
            return transform.position;
        }
    }
}
