using UniRx;
using UnityEngine;
using System.Collections;

public class PlayerMover : MonoBehaviour
{
    private CompositeDisposable subscriptions = new CompositeDisposable();
    [SerializeField] private float speed;
    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

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
                if (value) enabled = false;
            })
            .AddTo(subscriptions);
    }
    private void OnDisable()
    {
        subscriptions.Clear();
    }
    #endregion

    private void Update()
    {
        body.velocity = new Vector3(Input.GetAxis("Horizontal") * speed, 0, Input.GetAxis("Vertical") * speed);
    }
}