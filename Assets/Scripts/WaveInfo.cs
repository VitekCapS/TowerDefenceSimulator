using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveInfo
{
    public Dictionary<UnitInfo, int> dictionaryUnits = new Dictionary<UnitInfo, int>();
    public Dictionary<UnitInfo, int> dictionaryBosses = new Dictionary<UnitInfo, int>();

    //=================================// для редактирования в Unity
    public Dict[] dictUnits;
    public Dict[] dictBosses;
    //=================================
    [System.Serializable]
    public struct Dict
    {
        public UnitInfo unitInfo;
        public int count;
    }

    [System.Serializable]
    public struct UnitInfo
    {
        public Unit.UnitType unitType;
        public GameObject unitPrefab;

        public UnitInfo(Unit.UnitType type, GameObject prefab)
        {
            unitType = type;
            unitPrefab = prefab;
            unitPrefab.SetActive(false);
        }
    }
}
