using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField, Range(0, 1), Tooltip("Процент снижения урона при попадании")]
    protected float defence = 0.01f;
    [SerializeField]
    protected int healthMax = 1;
    [SerializeField]
    protected int reward = 1;
    [SerializeField]
    protected int damageToLifes = 1;
    [SerializeField]
    protected float speed = 0.1f;
    protected float speedMultiplier = 1;

    protected int health;

    public enum UnitType
    {
        Ground,
        Air
    }

    public UnitType unitType = UnitType.Ground;
    public bool isBoss = false;

    public delegate void UnitValueHasChanged(params int[] args);
    public delegate void UnitDieEventHandler(Unit unit);

    /// <summary>
    /// Сколько урона получено юнитом
    /// </summary>
    public static event UnitValueHasChanged OnDamageTaken;
    public static event UnitDieEventHandler OnDieEvent;
    public static event Action OnDisableEvent;

    /// <summary>
    /// Событие изменения значения здоровья (передаем текущее и максимальное кол-во)
    /// </summary>
    public event UnitValueHasChanged OnHealthValueChanged;

    private List<Effect> effects = new List<Effect>(); //бафы, дебафы и прочее

    public float Defence
    {
        get { return defence; }
        set { defence = value; }
    }

    public int Health
    {
        get { return health; }
        set
        {
            health = value;

            if (health <= 0)
            {
                health = 0;
                Die();
            }
            if (OnHealthValueChanged != null)
                OnHealthValueChanged.Invoke(value >= 0 ? value : 0, healthMax);
        }
    }

    public int Reward
    {
        get { return reward; }
        set { reward = value; }
    }

    public int DamageToLifes
    {
        get { return damageToLifes; }
        set { damageToLifes = value; }
    }

    public float Speed
    {
        get { return speed * speedMultiplier; }
        set { speed = value; }
    }

    public float SpeedMultiplier
    {
        get { return speedMultiplier; }
        set { speedMultiplier = Mathf.Clamp01(value); }
    }

    // Use this for initialization
    void Start()
    {
        health = healthMax;
    }

    // Update is called once per frame
    void Update()
    {
        ApplyEffects();
    }

    public virtual void Damage(float damage)
    {
        int damageTaken = (int)damage - (int)(damage * defence);
        Health -= damageTaken;

        if (OnDamageTaken != null)
            OnDamageTaken.Invoke(damageTaken);

    }

    public virtual void TrueDamage(float damage)
    {
        Health -= (int) damage;

        if (OnDamageTaken != null)
            OnDamageTaken.Invoke((int) damage);
    }

    public virtual void ApplyEffects()
    {
        SpeedMultiplier = 1;
        if (effects.Count > 0)
        {
            foreach (Effect effect in effects)
            {
                effect.ApplyEffect(this);
                effect.UpdateCycle();
            }

            effects.RemoveAll(x => x.Duration == 0);
        }
    }

    public virtual void AddEffect(Effect effect)
    {
        Effect.EffectType checkingType = effect.effectType;

        if (effect.isStackable)
        {
            if (effect.maxStacksCount > effects.Count(x => x.effectType == checkingType))
            {
                goto Adding; //перепрыгиваем к добавлению
            }
            else
            {
                return;
            }
        }

        Effect similarTypeEffect = effects.Find(x => x.effectType == checkingType);
        if (similarTypeEffect)
        {
            if (similarTypeEffect.strength > effect.strength)
            {
                return;
            }
            else if (!effect.isRepeatable)
            {
                effects.Remove(similarTypeEffect);
            }
            else
            {
                similarTypeEffect.Duration = effect.Duration;
                return;
            }
        }

        Adding: //добавление эффекта в список и упорядочивание по приоритету
        effects.Add(effect.Clone());
        effects.OrderBy(ef => ef.priority);
    }

    protected virtual void Die()
    {
        Disable();

        if (OnDieEvent != null)
            OnDieEvent.Invoke(this);
    }

    public virtual void Disable()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (OnDisableEvent != null)
            OnDisableEvent.Invoke();
    }

}
