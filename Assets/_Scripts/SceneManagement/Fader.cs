using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace RPGDemo.SceneManagement
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Fader : MonoBehaviour
    {
        CanvasGroup _coverCanvasGroup;

        [SerializeField] float _fadeInTime = 0.2f, _fadeOutTime = 0.2f, _waitTime = 0.2f;
        [SerializeField] Image _img_loadingBarFill;
        [SerializeField] Image _img_rotationIcon;
        [SerializeField] float rotationSpeed = 180f;





        //TEST
        [SerializeField] bool startFading = false;
        bool canFading = true;

        private void Awake()
        {
            _coverCanvasGroup = GetComponent<CanvasGroup>();
            _coverCanvasGroup.alpha = 0f;
            _coverCanvasGroup.blocksRaycasts = false;
            _img_loadingBarFill.fillAmount = 0f;

        }

        private void Update()
        {
            if (startFading && canFading)
            {
                StartFadeOutIn();
                startFading = false;
            }

            if (_img_loadingBarFill.fillAmount > 0f && _img_loadingBarFill.fillAmount < 1f)
                _img_rotationIcon.rectTransform.rotation =
                  Quaternion.Euler(new Vector3(0, 0,
                  _img_rotationIcon.rectTransform.rotation.eulerAngles.z + rotationSpeed * Time.unscaledDeltaTime));
        }

        public void FadeOutImmediate()
        {
            _coverCanvasGroup.alpha = 1f;
            _coverCanvasGroup.blocksRaycasts = true;
            _img_loadingBarFill.fillAmount = 0f;
        }

        public void StartFadeOutIn()
        {
            StartCoroutine(FadeOutIn(_fadeInTime, _fadeOutTime, _waitTime));
        }
        public void StartFadeOutIn(float fadeInTime, float fadeOutTime, float waitTime)
        {
            StartCoroutine(FadeOutIn(fadeInTime, fadeOutTime, waitTime));
        }


        public IEnumerator FadeOutIn(float fadeInTime, float fadeOutTime, float waitTime)
        {
            canFading = false;
            _img_loadingBarFill.fillAmount = 0f;
            // Debug.Log("Fading out");
            yield return FadeOut(fadeOutTime);
            yield return LoadingProgress(waitTime);

            // yield return new WaitForSeconds(waitTime);
            //  Debug.Log("Fading in");
            yield return FadeIn(fadeInTime);
            canFading = true;
        }

        public IEnumerator LoadingProgress(float waitTime)
        {
            float timer = 0f;
            while (timer < waitTime)
            {
                _img_loadingBarFill.fillAmount += Time.unscaledDeltaTime / waitTime * 0.2f;

                timer += Time.unscaledDeltaTime;
                yield return null;
            }
            _img_loadingBarFill.fillAmount = 1f;
        }

        public IEnumerator FadeOut(float fadeOutTime)
        {
            _coverCanvasGroup.blocksRaycasts = true;
            _img_loadingBarFill.fillAmount = 0f;
            while (_coverCanvasGroup.alpha < 1f)
            {
                yield return null;
                _img_loadingBarFill.fillAmount += Time.unscaledDeltaTime / fadeOutTime * 0.8f;
                // _img_rotationIcon.rectTransform.rotation =
                //  Quaternion.Euler(new Vector3(0, 0,
                //  _img_rotationIcon.rectTransform.rotation.eulerAngles.z + rotationSpeed * Time.unscaledDeltaTime));

                _coverCanvasGroup.alpha += Time.unscaledDeltaTime / fadeOutTime;

            }
            _coverCanvasGroup.alpha = 1f;
        }

        public IEnumerator FadeIn(float fadeInTime)
        {
            _coverCanvasGroup.blocksRaycasts = false;
            while (_coverCanvasGroup.alpha > 0)
            {
                yield return null;
                _coverCanvasGroup.alpha -= Time.unscaledDeltaTime / fadeInTime;
            }
            _coverCanvasGroup.alpha = 0f;

        }

    }
}
