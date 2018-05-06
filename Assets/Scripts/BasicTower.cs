using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class BasicTower : MonoBehaviour, IPointerClickHandler
{
    public TypeAim typeAim = TypeAim.All;
    public int price;
    public int bid;
    public float damage;
    public float range;
    public float projectileSpeed;
    public float fireRate = 0.5f;
    public GameObject projectilePrefab;
    public GameObject selector;
    public Sprite image;

    protected bool _isAimed;
    protected bool _isReloading;
    protected float _reloadTimer;
    protected float _rangeSqr;
    protected Transform _currentTarget;
    protected LayerMask _layerMask;

    public delegate void ClickEventHandler(BasicTower obj);

    public static event ClickEventHandler OnClickTower;

    public enum TypeAim
    {
        Ground,
        Air,
        All
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickTower != null)
            OnClickTower.Invoke(this);
    }


    protected virtual void Start()
    {
        _rangeSqr = range * range;

        switch (typeAim)
        {
            case TypeAim.Ground:
                _layerMask = LayerMask.GetMask("GroundUnits");
                break;
            case TypeAim.Air:
                _layerMask = LayerMask.GetMask("AirUnits");
                break;
            case TypeAim.All:
                _layerMask = LayerMask.GetMask("GroundUnits", "AirUnits");
                break;
        }

        StartCoroutine(AimCoroutine());
    }

    protected virtual void Update()
    {
        if (_isReloading)
        {
            _reloadTimer -= Time.deltaTime;
            if (_reloadTimer <= 0)
            {
                _reloadTimer = fireRate;
                _isReloading = false;
            }
        }

        if (_isAimed && !_isReloading)
        {
            if ((_currentTarget.transform.position - transform.position).sqrMagnitude > _rangeSqr)
            {
                _isAimed = FindClosestTarget();

                if (!_isAimed)
                    StartCoroutine(AimCoroutine());
            }
            else if (_currentTarget.gameObject.activeInHierarchy)
            {
                Shoot();
                _isReloading = true;
            }
            else
            {
                _isAimed = FindClosestTarget();

                if (!_isAimed)
                    StartCoroutine(AimCoroutine());

            }
        }
    }

    protected virtual void Shoot()
    {
        GameObject go = PoolManager.Spawn(projectilePrefab, transform.position + Vector3.up * 2, Quaternion.identity);
        Projectile projectile = go.GetComponent<Projectile>();
        projectile.Owner = this;
        projectile.target = _currentTarget;
    }

    protected virtual bool FindClosestTarget()
    {
        Collider[] colliders = Physics.OverlapCapsule(transform.position + Vector3.up * 3, transform.position, range, _layerMask.value);
        float sqrDistance;
        for (int i = 0; i < colliders.Length; i++)
        {
            sqrDistance = (colliders[i].transform.position - transform.position).sqrMagnitude;
            if (sqrDistance < _rangeSqr)
            {
                _currentTarget = colliders[i].transform;
                return true;
            }

        }
        _currentTarget = null;
        return false;
    }

    IEnumerator AimCoroutine()
    {
        while (!_isAimed)
        {
            yield return new WaitForSeconds(0.5f);
            _isAimed = FindClosestTarget();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }

}
