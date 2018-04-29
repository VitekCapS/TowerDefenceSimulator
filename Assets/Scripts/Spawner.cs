using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public void Spawn(GameObject unit)
    {
        unit.transform.position = transform.position;
        unit.SetActive(true);
    }

    public void Spawn(GameObject unit, float delay)
    {
        if (delay > 0)
        {
            StartCoroutine(SpawnCoroutine(unit, delay));
        }
    }

    IEnumerator SpawnCoroutine(GameObject unit, float delay)
    {
        yield return new WaitForSeconds(delay);
        Spawn(unit);
    }
}
