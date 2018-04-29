using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector]
    public float damage;
    [HideInInspector]
    public float speed;
    public string damageMethodName = "Damage";
    public Effect effect;
    public Transform target;

    protected BasicTower owner;
    public BasicTower Owner
    {
        get { return owner; }
        set
        {
            if (owner != value)
                OwnerChanged(value);
        }
    }

    private Vector3 targetLastPosition;

    private void OwnerChanged(BasicTower tower)
    {
        owner = tower;
        damage = owner.damage;
        speed = owner.projectileSpeed;
    }

    void Update()
    {
        if (target)
        {
            LifeCycle();
        }
        else
        {
            MoveTo(targetLastPosition);
        }
    }

    public virtual void LifeCycle()
    {
        MoveToTarget();
    }

    public virtual void MoveToTarget()
    {
        MoveTo(target.transform.position);
    }

    public virtual void MoveTo(Vector3 position)
    {
        Vector3 dir = position - transform.position;
        float _speed = speed * Time.deltaTime;
        if (dir.sqrMagnitude >= _speed * _speed)
        {
            transform.Translate(dir.normalized * _speed);
            targetLastPosition = position;
        }
        else
        {
            EndLife();
        }
    }

    /// <summary>
    /// Конец жизненного цикла объекта. При вызове сработает метод ReturnToPool на gameObjec'e
    /// </summary>
    public virtual void EndLife()
    {
        PoolManager.Despawn(gameObject); //пытаемся вызвать возвращение в пул

        if (target)
        {
            target.gameObject.SendMessageUpwards(damageMethodName, damage, SendMessageOptions.DontRequireReceiver);
            if (effect) //если есть эффект, то он будет применен к боту
                target.gameObject.SendMessageUpwards("AddEffect", effect, SendMessageOptions.DontRequireReceiver);
        }

        if (gameObject.activeSelf) //если не активен / не вернулся в пул => удаляем
            Destroy(this);
    }

}
