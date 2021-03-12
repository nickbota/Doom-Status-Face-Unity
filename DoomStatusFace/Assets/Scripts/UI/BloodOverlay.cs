using UniRx;
using UnityEngine;
using System.Collections;

public class BloodOverlay : MonoBehaviour
{
    private CompositeDisposable subscriptions = new CompositeDisposable();
    private IEnumerator flash;
    private CanvasGroup canvasGroup;
    private float oldHealth;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
    }
    #region Subscribe & unsubscribe from events
    private void OnEnable()
    {
        StartCoroutine(Subscribe());
    }
    private IEnumerator Subscribe()
    {
        yield return new WaitUntil(() => GameEvents.instance != null);
        GameEvents.instance.health
            .ObserveEveryValueChanged(x => x.Value)
            .Subscribe(value =>
            {
                if (oldHealth > value)
                {
                    if (flash != null)
                    {
                        StopCoroutine(flash);
                        flash = null;
                    }

                    flash = FlashRed();
                    StartCoroutine(flash);

                    oldHealth = value;
                }
            })
            .AddTo(subscriptions);
    }
    private void OnDisable()
    {
        subscriptions.Clear();
    }
    #endregion

    private void Start()
    {
        oldHealth = GameEvents.instance.health.Value;
    }

    private IEnumerator FlashRed()
    {
        for (int i = 0; i < 10; i++)
        {
            canvasGroup.alpha += 0.1f;
            yield return new WaitForSeconds(0.01f);
        }
        yield return new WaitForSeconds(0.1f);
        for (int i = 0; i < 10; i++)
        {
            canvasGroup.alpha -= 0.1f;
            yield return new WaitForSeconds(0.01f);
        }
        flash = null;
    }
}