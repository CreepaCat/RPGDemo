using UnityEngine;

[RequireComponent(typeof(Light))]
public class FlameShadowLight : MonoBehaviour
{
    [Header("火焰影子参数")]
    public float positionShake = 0.08f;     // 位置抖动幅度
    public float intensityShake = 0.6f;     // 强度抖动幅度
    public float speed = 8f;                // 抖动速度

    private Light pointLight;
    private Vector3 originalPos;
    private float originalIntensity;
    private float timeOffset;

    void Start()
    {
        pointLight = GetComponent<Light>();
        originalPos = transform.position;
        originalIntensity = pointLight.intensity;

        // 给每个光源一个随机偏移，避免所有火焰同步抖动
        timeOffset = Random.Range(0f, 100f);
    }

    void Update()
    {
        float t = Time.time * speed + timeOffset;

        // 位置轻微抖动（主要在 X 和 Y 轴）
        Vector3 shakePos = new Vector3(
            Mathf.Sin(t * 1.7f) * positionShake,
            Mathf.Cos(t * 2.3f) * positionShake * 0.7f,
            Mathf.Sin(t * 1.1f) * positionShake * 0.5f
        );

        transform.position = originalPos + shakePos;

        // 强度抖动（模拟火焰明暗变化）
        float intensityNoise = Mathf.PerlinNoise(t * 0.8f, t * 1.3f);
        pointLight.intensity = originalIntensity + intensityNoise * intensityShake - intensityShake * 0.5f;
    }
}
