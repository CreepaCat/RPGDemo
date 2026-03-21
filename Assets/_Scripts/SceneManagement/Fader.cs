using System.Collections;
using UnityEngine;

namespace RPGDemo.SceneManagement
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Fader : MonoBehaviour
    {
        CanvasGroup _coverCanvasGroup;
        
        [SerializeField] float _fadeInTime = 0.2f, _fadeOutTime = 0.2f,_waitTime = 0.2f;
        
    
 
        
        
        //TEST
        [SerializeField] bool startFading = false;
         bool canFading = true;

        private void Awake()
        {
            _coverCanvasGroup = GetComponent<CanvasGroup>();
        }

        private void Update()
        {
            if (startFading && canFading)
            {
                StartFadeOutIn();
                startFading = false;
            }
        }

        public void FadeOutImmediate()
        {
            _coverCanvasGroup.alpha = 1f;
            _coverCanvasGroup.blocksRaycasts = true;
        }

        public void StartFadeOutIn()
        {
            StartCoroutine(FadeOutIn(_fadeInTime,_fadeOutTime,_waitTime));
        }

        public IEnumerator FadeOutIn(float fadeInTime, float fadeOutTime, float waitTime)
        {
            canFading = false;
            Debug.Log("Fading out");
            yield return FadeOut(fadeOutTime);
            yield return new WaitForSeconds(waitTime);
            Debug.Log("Fading in");
            yield return FadeIn(fadeInTime);
            canFading = true;
        }

        public IEnumerator FadeOut(float fadeOutTime)
        {
            _coverCanvasGroup.blocksRaycasts = true;
            while (_coverCanvasGroup.alpha < 1f)
            {
                yield return null;
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
