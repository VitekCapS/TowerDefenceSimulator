using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour
{

    public bool isStart;
    public bool isFinish;
    public Waypoint nextWaypoint;
    [SerializeField]
    private Color gizmosColor;

    private void Start()
    {
        if (isFinish && nextWaypoint != null)
        {
            nextWaypoint = null;
            Debug.LogWarning(gameObject.name + " вейпоинт помечен как финишный, ссылка на следующий вейпоинт была удалена");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmosColor;
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        if (nextWaypoint != null)
            Gizmos.DrawLine(transform.position, nextWaypoint.transform.position);
        Gizmos.color = Color.white;
    }
}
