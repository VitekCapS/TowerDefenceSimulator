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

    protected BasicTower _owner;

    private Vector3 _targetLastPosition;

    public BasicTower Owner
    {
        get { return _owner; }
        set
        {
            if (_owner != value)
                OwnerChanged(value);
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
            _targetLastPosition = position;
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

    private void OwnerChanged(BasicTower tower)
    {
        _owner = tower;
        damage = _owner.damage;
        speed = _owner.projectileSpeed;
    }

    private void Update()
    {
        if (target)
        {
            LifeCycle();
        }
        else
        {
            MoveTo(_targetLastPosition);
        }
    }

}
