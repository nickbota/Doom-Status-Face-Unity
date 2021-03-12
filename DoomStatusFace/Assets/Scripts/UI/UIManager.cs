using UniRx;
using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
    private CompositeDisposable subscriptions = new CompositeDisposable();
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject gameOverUI;

    #region Subscribe & unsubscribe from events
    private void OnEnable()
    {
        StartCoroutine(Subscribe());
    }
    private IEnumerator Subscribe()
    {
        yield return new WaitUntil(() => GameEvents.instance != null);
        GameEvents.instance.playerDead.ObserveEveryValueChanged(x => x.Value)
            .Subscribe(value =>
            {
                gameOverUI.SetActive(value);
            })
            .AddTo(subscriptions);
    }
    private void OnDisable()
    {
        subscriptions.Clear();
    }
    #endregion
}