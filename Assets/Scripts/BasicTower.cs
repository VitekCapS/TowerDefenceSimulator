using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class BasicTower : MonoBehaviour, IPointerClickHandler
{

    public enum TypeAim
    {
        Ground,
        Air,
        All
    }

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

    protected bool isAimed;
    protected bool isReloading;
    protected float reloadTimer;
    protected float rangeSqr;
    protected Transform currentTarget;
    protected LayerMask layerMask;

    public delegate void ClickEventHandler(BasicTower obj);
    public static event ClickEventHandler OnClickTower;

    // Use this for initialization
    void Start()
    {
        rangeSqr = range * range;

        switch (typeAim)
        {
            case TypeAim.Ground:
                layerMask = LayerMask.GetMask("GroundUnits");
                break;
            case TypeAim.Air:
                layerMask = LayerMask.GetMask("AirUnits");
                break;
            case TypeAim.All:
                layerMask = LayerMask.GetMask("GroundUnits", "AirUnits");
                break;
        }

        StartCoroutine(AimCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        if (isReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0)
            {
                reloadTimer = fireRate;
                isReloading = false;
            }
        }

        if (isAimed && !isReloading)
        {
            if ((currentTarget.transform.position - transform.position).sqrMagnitude > rangeSqr)
            {
                isAimed = FindClosestTarget();

                if (!isAimed)
                    StartCoroutine(AimCoroutine());
            }
            else if (currentTarget.gameObject.activeInHierarchy)
            {
                Shoot();
                isReloading = true;
            }
            else
            {
                isAimed = FindClosestTarget();

                if (!isAimed)
                    StartCoroutine(AimCoroutine());

            }
        }
    }

    protected virtual void Shoot()
    {
        GameObject go = PoolManager.Spawn(projectilePrefab, transform.position + Vector3.up * 2, Quaternion.identity);
        Projectile projectile = go.GetComponent<Projectile>();
        projectile.Owner = this;
        projectile.target = currentTarget;
    }

    protected virtual bool FindClosestTarget()
    {
        Collider[] colliders = Physics.OverlapCapsule(transform.position + Vector3.up * 3, transform.position, range, layerMask.value);
        float sqrDistance;
        for (int i = 0; i < colliders.Length; i++)
        {
            sqrDistance = (colliders[i].transform.position - transform.position).sqrMagnitude;
            if (sqrDistance < rangeSqr)
            {
                currentTarget = colliders[i].transform;
                return true;
            }

        }
        currentTarget = null;
        return false;
    }

    IEnumerator AimCoroutine()
    {
        while (!isAimed)
        {
            yield return new WaitForSeconds(0.5f);
            isAimed = FindClosestTarget();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickTower != null)
            OnClickTower.Invoke(this);
    }
}
