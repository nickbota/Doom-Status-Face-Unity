using UniRx;
using UnityEngine;
using System.Collections;

public class PlayerSounds : MonoBehaviour
{
    private CompositeDisposable subscriptions = new CompositeDisposable();
    private AudioSource source;
    [Header("Sounds")]
    [SerializeField] private AudioClip playerHurt;
    [SerializeField] private AudioClip playerDie;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    #region Subscribe & unsubscribe from events
    private void OnEnable()
    {
        StartCoroutine(Subscribe());
    }
    private IEnumerator Subscribe()
    {
        yield return new WaitUntil(() => GameEvents.instance != null);
        GameEvents.instance.health.ObserveEveryValueChanged(x => x.Value)
            .Subscribe(value =>
            {
                if (value <= 0) source.PlayOneShot(playerDie);
                else            source.PlayOneShot(playerHurt);
            })
            .AddTo(subscriptions);
    }
    private void OnDisable()
    {
        subscriptions.Clear();
    }
    #endregion
}