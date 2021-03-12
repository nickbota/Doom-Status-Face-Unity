using UniRx;
using UnityEngine;
using System.Linq;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    private CompositeDisposable subscriptions = new CompositeDisposable();

    [Header("Attack Stats")]
    [SerializeField] private float range;
    [SerializeField] private float damage;
    [SerializeField] private float attackCooldown;

    [Header("Player Layer")]
    [SerializeField] private LayerMask playerLayer;

    [Header("SFX")]
    [SerializeField] private AudioClip attackSound;

    private float cooldownTimer = Mathf.Infinity;
    private Transform player;
    private AudioSource source;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    #region Subscribe & unsubscribe from events
    private void OnEnable()
    {
        StartCoroutine(Subscribe());
    }
    private IEnumerator Subscribe()
    {
        yield return new WaitUntil(() => GameEvents.instance != null);
        GameEvents.instance.playerDead
            .ObserveEveryValueChanged(x => x.Value)
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
        Vector3 targetPostition = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(targetPostition);

        if (PlayerInSight() != null)
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer > attackCooldown)
            {
                PlayerInSight().TakeDamage(damage);
                cooldownTimer = 0;
                AttackSound();
            }
        }
    }
    private PlayerHealth PlayerInSight()
    {
        Collider[] collisionData = Physics.OverlapSphere(transform.position, range, playerLayer);

        if (collisionData.Count() > 0)
            return collisionData[0].GetComponentInParent<PlayerHealth>();
        else
            return null;
    }
    private void AttackSound()
    {
        source.PlayOneShot(attackSound);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
}