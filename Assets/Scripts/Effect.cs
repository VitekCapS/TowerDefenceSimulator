using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Effect : ScriptableObject
{
#if UNITY_EDITOR
    [MenuItem("Assets/Create/Tower_Defence/Effect")]
    public static void CreateMyAsset()
    {
        Effect asset = ScriptableObject.CreateInstance<Effect>();

        AssetDatabase.CreateAsset(asset, "Assets/Prefabs/Effects/NewEffect.asset");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();

        Selection.activeObject = asset;
    }
#endif

    public EffectType effectType;
    public bool isRepeatable;
    public bool isSpeedEffect;
    public bool isStackable;
    public int maxStacksCount;
    public int priority;
    public int strength;

    [SerializeField]
    protected float _duration;
    [SerializeField]
    protected float _value;
    [SerializeField]
    protected float _interval;

    private bool _isReadyToCast = false;
    private bool _isEnded = false;
    private float _castTime = 0;

    public Effect(float time, EffectType type)
    {
        _duration = time;
        effectType = type;
    }

    public enum EffectType
    {
        Poison,
        Slow,
        Stun
    }

    public float Duration
    {
        get { return _duration; }
        set { _duration = value; }
    }

    /// <summary>
    /// Создает *неполный клон (только нестатические поля)
    /// </summary>
    public Effect Clone()
    {
        return (Effect)this.MemberwiseClone();
    }

    public void UpdateCycle()
    {
        if (_isEnded)
            return;

        if (isRepeatable)
        {
            _castTime += Time.deltaTime;

            if (_castTime > _interval)
            {
                _isReadyToCast = true;
                _castTime -= _interval;
            }
        }

        if (isSpeedEffect)
        {
            _isReadyToCast = true;
        }

        _duration -= Time.deltaTime;
        if (_duration <= 0)
        {
            _duration = 0;
            EndEffect();
        }

    }

    public virtual void ApplyEffect(Unit bot)
    {
        if (!_isReadyToCast)
            return;

        switch (effectType)
        {
            case EffectType.Poison:
                bot.TrueDamage(_value);
                _isReadyToCast = false;
                break;
            case EffectType.Slow:
                bot.SpeedMultiplier -= _value / 100;
                break;
            case EffectType.Stun:
                bot.SpeedMultiplier = 0;
                break;
        }

    }

    public virtual void EndEffect()
    {
        _isEnded = true;
    }
}
