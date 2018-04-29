using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    private static MenuController m_Instance;
    public static MenuController Instance
    {
        get { return m_Instance; }
    }

    public int minCoins = 25;
    public int maxUnits = 30;
    public int maxWaves = 10;
    [SerializeField]
    private int wavesCount = 5;
    [SerializeField]
    private int lifesCount = 10;
    [SerializeField]
    private int startCoins = 50;
    private int currentWaveId;
    public int groundUnitIndex;
    public int groundUnitsCount;
    public int airUnitIndex;
    public int airUnitsCount;
    public int bossIndex;
    public int bossCount;
    [Header("UI")]
    public Text waveCountText;
    public Text lifeCountText;
    public Text startCoinsText;
    [Space(5)]
    public Text currentWaveText;
    public Text groundUnitsCountText;
    public Text groundUnitsType;
    public Text airUnitsCountText;
    public Text airUnitsType;
    public Text bossCountText;
    public Text bossTypeText;
    [Space(4)]
    public GameObject popupUnitInfo;
    public List<GameObject> unitsPrefabs;
    public List<GameObject> bossesPrefabs;

    public List<WaveInfo> wavesInfo = new List<WaveInfo>();
    private List<Unit> m_unitsPrefabsInfo = new List<Unit>();
    private List<Unit> m_bossesPrefabsInfo = new List<Unit>();

    //public List<WaveSettings> wavesSettings = new List<WaveSettings>();
    //public struct WaveSettings
    //{
    //    public int groundUnitIndex;
    //    public int groundUnitsCount;
    //    public int airUnitIndex;
    //    public int airUnitCount;
    //    public int bossIndex;
    //    public int bossCount;
    //}

    public int WavesCount
    {
        get
        {
            return wavesCount;
        }

        set
        {
            if (value >= 1)
                wavesCount = value;
            if (value > maxWaves)
                wavesCount = maxWaves;
            if (wavesCount < currentWaveId + 1)
                CurrentWave = wavesCount;
            waveCountText.text = wavesCount.ToString();
        }
    }

    public int LifesCount
    {
        get
        {
            return lifesCount;
        }

        set
        {
            if (value >= 0)
                lifesCount = value;
            else
                lifesCount = 0;

            lifeCountText.text = lifesCount.ToString();
        }
    }

    public int StartCoins
    {
        get
        {
            return startCoins;
        }

        set
        {
            if (value < minCoins)
                startCoins = minCoins;
            else
                startCoins = value;
            startCoinsText.text = startCoins.ToString();
        }
    }

    public int CurrentWave
    {
        get
        {
            return currentWaveId;
        }

        set
        {
            if (value < 0)
                currentWaveId = wavesCount - 1;
            else
            if (value >= wavesCount)
                currentWaveId = 0;
            else
                currentWaveId = value;
            currentWaveText.text = (currentWaveId + 1).ToString();

            UpdateWaveSettingsUI(currentWaveId);
        }
    }

    private void Awake()
    {
        m_Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        CheckPrefabs();

        LoadPrefs();
        CurrentWave = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateWaveSettingsUI(int id)
    {
        airUnitsCount = 0;
        airUnitIndex = m_unitsPrefabsInfo.FindIndex(x => x.unitType == Unit.UnitType.Air);
        groundUnitsCount = 0;
        groundUnitIndex = m_unitsPrefabsInfo.FindIndex(x => x.unitType == Unit.UnitType.Ground);
        bossCount = 0;

        foreach (KeyValuePair<WaveInfo.UnitInfo, int> keyPair in wavesInfo[id].dictionaryUnits)
        {
            switch (keyPair.Key.unitType)
            {
                case Unit.UnitType.Ground:
                    groundUnitIndex = unitsPrefabs.IndexOf(keyPair.Key.unitPrefab);
                    groundUnitsCount = keyPair.Value;
                    break;
                case Unit.UnitType.Air:
                    airUnitIndex = unitsPrefabs.IndexOf(keyPair.Key.unitPrefab);
                    airUnitsCount = keyPair.Value;
                    break;
            }
        }

        foreach (KeyValuePair<WaveInfo.UnitInfo, int> keyPair in wavesInfo[id].dictionaryBosses)
        {
            bossIndex = bossesPrefabs.IndexOf(keyPair.Key.unitPrefab);
            bossCount = keyPair.Value;
        }

        groundUnitsCountText.text = groundUnitsCount.ToString();
        groundUnitsType.text = unitsPrefabs[groundUnitIndex].name;
        airUnitsCountText.text = airUnitsCount.ToString();
        airUnitsType.text = unitsPrefabs[airUnitIndex].name;
        bossCountText.text = bossCount.ToString();
        bossTypeText.text = bossesPrefabs[bossIndex].name;
    }

    public void GroundUnitDetailInfoClick()
    {

    }

    public void ChangeWaveCount(int x)
    {
        WavesCount += x;
    }

    public void ChangeStartCoins(int x)
    {
        StartCoins += x;
    }

    public void ChangeLifesCount(int x)
    {
        LifesCount += x;
    }

    public void ChangeCurrentWave(int x)
    {
        CurrentWave += x;
    }

    public void ChangeCountGroundUnits(int x)
    {
        groundUnitsCount += x;
        CheckMinMaxCountValue(ref groundUnitsCount);

        WaveInfo.UnitInfo unitInfo = new WaveInfo.UnitInfo(Unit.UnitType.Ground, unitsPrefabs[groundUnitIndex]);

        if (wavesInfo[currentWaveId].dictionaryUnits.ContainsKey(unitInfo))
        {
            wavesInfo[currentWaveId].dictionaryUnits[unitInfo] = groundUnitsCount;
        }
        else
        {
            wavesInfo[currentWaveId].dictionaryUnits.Add(unitInfo, groundUnitsCount);
        }

        groundUnitsCountText.text = groundUnitsCount.ToString();
    }

    public void ChangeCountAirUnits(int x)
    {
        airUnitsCount += x;
        CheckMinMaxCountValue(ref airUnitsCount);

        WaveInfo.UnitInfo unitInfo = new WaveInfo.UnitInfo(Unit.UnitType.Air, unitsPrefabs[airUnitIndex]);

        if (wavesInfo[currentWaveId].dictionaryUnits.ContainsKey(unitInfo))
        {
            wavesInfo[currentWaveId].dictionaryUnits[unitInfo] = airUnitsCount;
        }
        else
        {
            wavesInfo[currentWaveId].dictionaryUnits.Add(unitInfo, airUnitsCount);
        }

        airUnitsCountText.text = airUnitsCount.ToString();
    }

    public void ChangeCountBosses(int x)
    {
        bossCount += x;
        CheckMinMaxCountValue(ref bossCount);

        WaveInfo.UnitInfo unitInfo = new WaveInfo.UnitInfo(bossesPrefabs[bossIndex].GetComponent<Unit>().unitType, bossesPrefabs[bossIndex]);

        if (wavesInfo[currentWaveId].dictionaryBosses.ContainsKey(unitInfo))
        {
            wavesInfo[currentWaveId].dictionaryBosses[unitInfo] = bossCount;
        }
        else
        {
            wavesInfo[currentWaveId].dictionaryBosses.Add(unitInfo, bossCount);
        }

        bossCountText.text = bossCount.ToString();
    }

    /// <summary>
    /// Функция для изменения идентификатора наземных юнитов
    /// </summary>
    /// <param name="x">Число, указывающее в какую сторону будем менять значение</param>
    public void ChangeTypeGroundUnit(int x)
    {
        ChangeTypeIndexValue(ref groundUnitIndex, x, Unit.UnitType.Ground);

        WaveInfo.UnitInfo unitInfo = new WaveInfo.UnitInfo(Unit.UnitType.Ground, unitsPrefabs[groundUnitIndex]);
        groundUnitsCount = 0;
        if (!wavesInfo[currentWaveId].dictionaryUnits.TryGetValue(unitInfo, out groundUnitsCount))
        {
            wavesInfo[currentWaveId].dictionaryUnits.Add(unitInfo, groundUnitsCount);
        }

        groundUnitsType.text = unitsPrefabs[groundUnitIndex].name;
        groundUnitsCountText.text = groundUnitsCount.ToString();
    }

    /// <summary>
    /// Функция для изменения идентификатора воздушных юнитов
    /// </summary>
    /// <param name="x">Число, указывающее в какую сторону будем менять значение</param>
    public void ChangeTypeAirUnit(int x)
    {
        ChangeTypeIndexValue(ref airUnitIndex, x, Unit.UnitType.Air);

        WaveInfo.UnitInfo unitInfo = new WaveInfo.UnitInfo(Unit.UnitType.Air, unitsPrefabs[airUnitIndex]);
        airUnitsCount = 0;
        if (!wavesInfo[currentWaveId].dictionaryUnits.TryGetValue(unitInfo, out airUnitsCount))
        {
            wavesInfo[currentWaveId].dictionaryUnits.Add(unitInfo, airUnitsCount);
        }

        airUnitsType.text = unitsPrefabs[airUnitIndex].name;
        airUnitsCountText.text = airUnitsCount.ToString();
    }

    /// <summary>
    /// Изменение идентификатора босса
    /// </summary>
    /// <param name="x">Число, указывающее в какую сторону будем менять значение</param>
    public void ChangeTypeBossUnit(int x)
    {
        bossIndex += x;
        if (bossIndex < 0)
            bossIndex = bossesPrefabs.Count - 1;
        if (bossIndex >= bossesPrefabs.Count)
            bossIndex = 0;

        WaveInfo.UnitInfo unitInfo = new WaveInfo.UnitInfo(bossesPrefabs[bossIndex].GetComponent<Unit>().unitType, bossesPrefabs[bossIndex]);
        bossCount = 0;
        if (!wavesInfo[currentWaveId].dictionaryBosses.TryGetValue(unitInfo, out bossCount))
        {
            wavesInfo[currentWaveId].dictionaryBosses.Add(unitInfo, bossCount);
        }

        bossTypeText.text = bossesPrefabs[bossIndex].name;
        bossCountText.text = bossCount.ToString();
    }

    public void SavePrefs()
    {
        PlayerPrefs.SetInt("startCoins", startCoins);
        PlayerPrefs.SetInt("startLifes", lifesCount);
        PlayerPrefs.SetInt("wavesCount", wavesCount);

        for (int id = 0; id < wavesInfo.Count; id++)
        {
            foreach (KeyValuePair<WaveInfo.UnitInfo, int> keyPair in wavesInfo[id].dictionaryUnits)
            {
                if (keyPair.Value > 0)
                    PlayerPrefs.SetInt("wave" + id + keyPair.Key.unitType.ToString() + keyPair.Key.unitPrefab.name + "Count", keyPair.Value);
            }

            foreach (KeyValuePair<WaveInfo.UnitInfo, int> keyPair in wavesInfo[id].dictionaryBosses)
            {
                if (keyPair.Value > 0)
                    PlayerPrefs.SetInt("wave" + id + keyPair.Key.unitType.ToString() + keyPair.Key.unitPrefab.name + "Count", keyPair.Value);
            }
        }
    }

    public void StartGame()
    {
        SavePrefs();
        SceneManager.LoadScene("Game");
    }

    public void SelectLevel(int num)
    {
        PlayerPrefs.SetInt("Level", num);
    }

    private void CheckMinMaxCountValue(ref int count)
    {
        if (count < 0)
            count = 0;
        if (count > maxUnits)
            count = maxUnits;
    }

    private void ChangeTypeIndexValue(ref int index, int x, Unit.UnitType checkedType)
    {
        int startIndex = index + x;
        if (startIndex < 0)
            startIndex = unitsPrefabs.Count - 1;
        if (startIndex >= unitsPrefabs.Count)
            startIndex = 0;

        for (int i = startIndex; i < unitsPrefabs.Count; i += x)
        {
            if (i < 0)
            {
                index = unitsPrefabs.FindLastIndex(u => u.GetComponent<Unit>().unitType == checkedType);
                return;
            }

            Unit unit = unitsPrefabs[i].GetComponent<Unit>();
            if (unit.unitType == checkedType)
            {
                index = i;
                return;
            }
        }

        index = unitsPrefabs.FindIndex(u => u.GetComponent<Unit>().unitType == checkedType);
    }

    private void CheckPrefabs()
    {
        foreach (GameObject prefab in unitsPrefabs.ToArray())
        {
            Unit unitInfo = prefab.GetComponent<Unit>();
            if (unitInfo)
            {
                m_unitsPrefabsInfo.Add(unitInfo);
            }
            else
            {
                Debug.LogWarning("У префаба " + prefab.name + "отсутствует компонент Unit. Префаб удален.");
                unitsPrefabs.Remove(prefab);
            }
        }

        foreach (GameObject prefab in bossesPrefabs.ToArray())
        {
            Unit unitInfo = prefab.GetComponent<Unit>();
            if (unitInfo)
            {
                if (!unitInfo.isBoss)
                    Debug.LogWarning("Для префаба босса " + prefab.name + " не установлен флаг isBoss");
                m_bossesPrefabsInfo.Add(unitInfo);
            }
            else
            {
                Debug.LogWarning("У префаба " + prefab.name + "отсутствует компонент Unit. Префаб удален.");
                bossesPrefabs.Remove(prefab);
            }
        }
    }

    private void LoadPrefs()
    {
        StartCoins = PlayerPrefs.GetInt("startCoins", startCoins);
        LifesCount = PlayerPrefs.GetInt("startLifes", lifesCount);
        WavesCount = PlayerPrefs.GetInt("wavesCount", wavesCount);
        wavesInfo = new List<WaveInfo>();

        for (int i = 0; i < maxWaves; i++)
        {
            WaveInfo waveInfo = new WaveInfo();

            foreach (var value in Enum.GetValues(typeof(Unit.UnitType)))
            {
                for (int j = 0; j < unitsPrefabs.Count; j++)
                {
                    int unitsCount = PlayerPrefs.GetInt("wave" + i + value.ToString() + unitsPrefabs[j].name + "Count", 0);
                    if (unitsCount > 0)
                    {
                        waveInfo.dictionaryUnits.Add(new WaveInfo.UnitInfo((Unit.UnitType)value, unitsPrefabs[j]), unitsCount);
                    }
                }

                for (int k = 0; k < bossesPrefabs.Count; k++)
                {
                    int bossesCount = PlayerPrefs.GetInt("wave" + i + value.ToString() + bossesPrefabs[k].name + "Count", 0);
                    if (bossesCount > 0)
                    {
                        waveInfo.dictionaryBosses.Add(new WaveInfo.UnitInfo((Unit.UnitType)value, bossesPrefabs[k]), bossesCount);
                    }
                }
            }

            wavesInfo.Add(waveInfo);
        }
    }


}
