using UniRx;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatusFace : MonoBehaviour
{
    private CompositeDisposable subscriptions = new CompositeDisposable();
    [Header("UI Elements")]
    [SerializeField] private Text health;
    [SerializeField] private Image playerFace;

    [Header("Face Sprites")]
    [SerializeField] private Sprite[] faceSprites;

    private string[] directions = new string[] { "Forward", "Left", "Right" };
    private float faceTimer;
    private float oldHealth;

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
                HealthChange(value);
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
    private void Update()
    {
        //If 1 second goes by revert to default face of looking around
        if (faceTimer > 0)
            faceTimer -= Time.deltaTime;
        else
            SetFace("Look" + directions[Random.Range(0, directions.Length)] + GetDamageStatus(oldHealth));
    }

    private void HealthChange(float _health)
    {
        if (_health <= 0) //Check if dead or not
            SetFace("Dead");
        else
        {
            float damage = oldHealth - _health;
            if (damage < 20) SetFace("Damage" + GetDamageStatus(_health)); //If took more than 20 damage show the shockface
            else SetFace("Shock" + GetDamageStatus(_health));
        }

        oldHealth = _health;
        health.text = _health.ToString("F0");
    }
    private string GetDamageStatus(float _health)
    {
        //Sets the suffix for looking up the correct sprite in the spritesheet
        string suffix = "";
        if (_health >= 80) suffix = "100";
        else if (_health < 80 && _health >= 60) suffix = "75";
        else if (_health < 60 && _health >= 40) suffix = "50";
        else if (_health < 40 && _health >= 20) suffix = "25";
        else if (_health < 20 && _health > 0) suffix = "0";
        return "_" + suffix;
    }
    private void SetFace(string _face)
    {
        //Find sprite with relevant name in the spritesheet and let it be shown for 1 second
        for (int i = 0; i < faceSprites.Length; i++)
        {
            if (faceSprites[i].name == _face)
                playerFace.sprite = faceSprites[i];
        }
        faceTimer = 1f;
    }
}