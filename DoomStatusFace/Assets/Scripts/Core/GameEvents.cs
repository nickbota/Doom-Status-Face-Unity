using UniRx;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents instance { get; private set; }
    private void Awake()
    {
        if (instance != null && instance != this) Destroy(gameObject);
        instance = this;
    }

    //Reactive properties
    public FloatReactiveProperty health { get; private set; } = new FloatReactiveProperty(100);
    public BoolReactiveProperty playerDead { get; private set; } = new BoolReactiveProperty(false);
    public BoolReactiveProperty weaponPickup { get; private set; } = new BoolReactiveProperty(false);
}