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
    public enum EffectType
    {
        Poison,
        Slow,
        Stun
    }

    public EffectType effectType;
    public bool isRepeatable;
    public bool isSpeedEffect;
    public bool isStackable;
    public int maxStacksCount;
    public int priority;
    public int strength;

    [SerializeField]
    protected float duration;
    [SerializeField]
    protected float value;
    [SerializeField]
    protected float interval;

    private bool isAlreadyApplied = false;
    private bool isEnded = false;
    private float castTime = 0;

    public float Duration
    {
        get { return duration; }
        set { duration = value; }
    }

    public Effect(float time, EffectType type)
    {
        duration = time;
        effectType = type;
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
        if (isEnded)
            return;

        if (isRepeatable)
        {
            castTime += Time.deltaTime;

            if (castTime > interval)
            {
                isAlreadyApplied = false;
                castTime -= interval;
            }
        }

        duration -= Time.deltaTime;
        if (duration <= 0)
        {
            duration = 0;
            EndEffect();
        }

    }

    public virtual void ApplyEffect(Unit bot)
    {
        if (isAlreadyApplied && !isRepeatable && !isSpeedEffect)
            return;
        else
            isAlreadyApplied = true;

        switch (effectType)
        {
            case EffectType.Poison:
                bot.Damage(value);
                break;
            case EffectType.Slow:
                bot.SpeedMultiplier -= 1 - (value / 100);
                break;
            case EffectType.Stun:
                bot.SpeedMultiplier = 0;
                break;
        }

    }

    public virtual void EndEffect()
    {
        isEnded = true;
    }
}
