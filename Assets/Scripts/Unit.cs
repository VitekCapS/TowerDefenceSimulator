using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public UnitType unitType = UnitType.Ground;
    public bool isBoss = false;

    [SerializeField, Range(0, 1), Tooltip("Процент снижения урона при попадании")]
    protected float _defence = 0.01f;
    [SerializeField]
    protected int _healthMax = 1;
    [SerializeField]
    protected int _reward = 1;
    [SerializeField]
    protected int _damageToLifes = 1;
    [SerializeField]
    protected float _speed = 0.1f;
    protected float _speedMultiplier = 1;

    protected int _currentHealth;

    private List<Effect> effects = new List<Effect>(); //бафы, дебафы и прочее


    public delegate void UnitValueHasChanged(params int[] args);
    public delegate void UnitDieEventHandler(Unit unit);

    public static event UnitValueHasChanged OnDamageTaken;
    public static event UnitDieEventHandler OnDieEvent;
    public static event Action OnDisableEvent;

    /// <summary>
    /// Событие изменения значения здоровья (передаем текущее и максимальное кол-во)
    /// </summary>
    public event UnitValueHasChanged OnHealthValueChanged;

    public enum UnitType
    {
        Ground,
        Air
    }

    public float Defence
    {
        get { return _defence; }
        set { _defence = value; }
    }

    public int Health
    {
        get { return _currentHealth; }
        set
        {
            _currentHealth = value;

            if (_currentHealth <= 0)
            {
                _currentHealth = 0;
                Die();
            }
            if (OnHealthValueChanged != null)
                OnHealthValueChanged.Invoke(value >= 0 ? value : 0, _healthMax);
        }
    }

    public int Reward
    {
        get { return _reward; }
        set { _reward = value; }
    }

    public int DamageToLifes
    {
        get { return _damageToLifes; }
        set { _damageToLifes = value; }
    }

    public float Speed
    {
        get { return _speed * _speedMultiplier; }
        set { _speed = value; }
    }

    public float SpeedMultiplier
    {
        get { return _speedMultiplier; }
        set { _speedMultiplier = Mathf.Clamp01(value); }
    }


    public virtual void Damage(float damage)
    {
        int damageTaken = (int)damage - (int)(damage * _defence);
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

    public virtual void Disable()
    {
        gameObject.SetActive(false);
    }

    protected virtual void Die()
    {
        Disable();

        if (OnDieEvent != null)
            OnDieEvent.Invoke(this);
    }

    private void OnDisable()
    {
        if (OnDisableEvent != null)
            OnDisableEvent.Invoke();
    }

    private void Start()
    {
        _currentHealth = _healthMax;
    }

    private void Update()
    {
        ApplyEffects();
    }

}
