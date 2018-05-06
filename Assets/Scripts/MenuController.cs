using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    #region Singleton
    private static MenuController _instance;
    public static MenuController Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MenuController>() ?? new MenuController();
            }
            return _instance;
        }
    }
    #endregion

    [SerializeField]
    private int _minCoins = 25;
    [SerializeField]
    private int _maxUnits = 30;
    [SerializeField]
    private int _maxWaves = 10;
    [SerializeField]
    private int _wavesCount = 5;
    [SerializeField]
    private int _lifesCount = 10;
    [SerializeField]
    private int _startCoins = 50;

    [Header("UI")]
    [SerializeField] private Text waveCountText;
    [SerializeField] private Text lifeCountText;
    [SerializeField] private Text startCoinsText;
    [Space(5)]
    [SerializeField] private Text currentWaveText;
    [SerializeField] private Text groundUnitsCountText;
    [SerializeField] private Text groundUnitsType;
    [SerializeField] private Text airUnitsCountText;
    [SerializeField] private Text airUnitsType;
    [SerializeField] private Text bossCountText;
    [SerializeField] private Text bossTypeText;
    [Space(4)]
    [SerializeField] private GameObject popupUnitInfo;
    [SerializeField] private List<GameObject> unitsPrefabs;
    [SerializeField] private List<GameObject> bossesPrefabs;

    [SerializeField] private List<WaveInfo> wavesInfo = new List<WaveInfo>();

    private List<Unit> _unitsPrefabsInfo = new List<Unit>();
    private List<Unit> _bossesPrefabsInfo = new List<Unit>();

    private int _currentWaveId;
    private int _groundUnitIndex;
    private int _groundUnitsCount;
    private int _airUnitIndex;
    private int _airUnitsCount;
    private int _bossIndex;
    private int _bossCount;

    #region Properties

    public int WavesCount
    {
        get
        {
            return _wavesCount;
        }

        set
        {
            if (value >= 1)
                _wavesCount = value;
            if (value > _maxWaves)
                _wavesCount = _maxWaves;
            if (_wavesCount < _currentWaveId + 1)
                CurrentWave = _wavesCount;
            waveCountText.text = _wavesCount.ToString();
        }
    }

    public int LifesCount
    {
        get
        {
            return _lifesCount;
        }

        set
        {
            if (value >= 0)
                _lifesCount = value;
            else
                _lifesCount = 0;

            lifeCountText.text = _lifesCount.ToString();
        }
    }

    public int StartCoins
    {
        get
        {
            return _startCoins;
        }

        set
        {
            if (value < _minCoins)
                _startCoins = _minCoins;
            else
                _startCoins = value;
            startCoinsText.text = _startCoins.ToString();
        }
    }

    public int CurrentWave
    {
        get
        {
            return _currentWaveId;
        }

        set
        {
            if (value < 0)
                _currentWaveId = _wavesCount - 1;
            else
            if (value >= _wavesCount)
                _currentWaveId = 0;
            else
                _currentWaveId = value;
            currentWaveText.text = (_currentWaveId + 1).ToString();

            UpdateWaveSettingsUI(_currentWaveId);
        }
    }

    #endregion

    public void UpdateWaveSettingsUI(int id)
    {
        _airUnitsCount = 0;
        _airUnitIndex = _unitsPrefabsInfo.FindIndex(x => x.unitType == Unit.UnitType.Air);
        _groundUnitsCount = 0;
        _groundUnitIndex = _unitsPrefabsInfo.FindIndex(x => x.unitType == Unit.UnitType.Ground);
        _bossCount = 0;

        foreach (KeyValuePair<WaveInfo.UnitInfo, int> keyPair in wavesInfo[id].dictionaryUnits)
        {
            switch (keyPair.Key.unitType)
            {
                case Unit.UnitType.Ground:
                    _groundUnitIndex = unitsPrefabs.IndexOf(keyPair.Key.unitPrefab);
                    _groundUnitsCount = keyPair.Value;
                    break;
                case Unit.UnitType.Air:
                    _airUnitIndex = unitsPrefabs.IndexOf(keyPair.Key.unitPrefab);
                    _airUnitsCount = keyPair.Value;
                    break;
            }
        }

        foreach (KeyValuePair<WaveInfo.UnitInfo, int> keyPair in wavesInfo[id].dictionaryBosses)
        {
            _bossIndex = bossesPrefabs.IndexOf(keyPair.Key.unitPrefab);
            _bossCount = keyPair.Value;
        }

        groundUnitsCountText.text = _groundUnitsCount.ToString();
        groundUnitsType.text = unitsPrefabs[_groundUnitIndex].name;
        airUnitsCountText.text = _airUnitsCount.ToString();
        airUnitsType.text = unitsPrefabs[_airUnitIndex].name;
        bossCountText.text = _bossCount.ToString();
        bossTypeText.text = bossesPrefabs[_bossIndex].name;
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
        _groundUnitsCount += x;
        CheckMinMaxCountValue(ref _groundUnitsCount);

        WaveInfo.UnitInfo unitInfo = new WaveInfo.UnitInfo(Unit.UnitType.Ground, unitsPrefabs[_groundUnitIndex]);

        if (wavesInfo[_currentWaveId].dictionaryUnits.ContainsKey(unitInfo))
        {
            wavesInfo[_currentWaveId].dictionaryUnits[unitInfo] = _groundUnitsCount;
        }
        else
        {
            wavesInfo[_currentWaveId].dictionaryUnits.Add(unitInfo, _groundUnitsCount);
        }

        groundUnitsCountText.text = _groundUnitsCount.ToString();
    }

    public void ChangeCountAirUnits(int x)
    {
        _airUnitsCount += x;
        CheckMinMaxCountValue(ref _airUnitsCount);

        WaveInfo.UnitInfo unitInfo = new WaveInfo.UnitInfo(Unit.UnitType.Air, unitsPrefabs[_airUnitIndex]);

        if (wavesInfo[_currentWaveId].dictionaryUnits.ContainsKey(unitInfo))
        {
            wavesInfo[_currentWaveId].dictionaryUnits[unitInfo] = _airUnitsCount;
        }
        else
        {
            wavesInfo[_currentWaveId].dictionaryUnits.Add(unitInfo, _airUnitsCount);
        }

        airUnitsCountText.text = _airUnitsCount.ToString();
    }

    public void ChangeCountBosses(int x)
    {
        _bossCount += x;
        CheckMinMaxCountValue(ref _bossCount);

        WaveInfo.UnitInfo unitInfo = new WaveInfo.UnitInfo(bossesPrefabs[_bossIndex].GetComponent<Unit>().unitType, bossesPrefabs[_bossIndex]);

        if (wavesInfo[_currentWaveId].dictionaryBosses.ContainsKey(unitInfo))
        {
            wavesInfo[_currentWaveId].dictionaryBosses[unitInfo] = _bossCount;
        }
        else
        {
            wavesInfo[_currentWaveId].dictionaryBosses.Add(unitInfo, _bossCount);
        }

        bossCountText.text = _bossCount.ToString();
    }

    /// <summary>
    /// Функция для изменения идентификатора наземных юнитов
    /// </summary>
    /// <param name="x">Число, указывающее в какую сторону будем менять значение</param>
    public void ChangeTypeGroundUnit(int x)
    {
        ChangeTypeIndexValue(ref _groundUnitIndex, x, Unit.UnitType.Ground);

        WaveInfo.UnitInfo unitInfo = new WaveInfo.UnitInfo(Unit.UnitType.Ground, unitsPrefabs[_groundUnitIndex]);
        _groundUnitsCount = 0;
        if (!wavesInfo[_currentWaveId].dictionaryUnits.TryGetValue(unitInfo, out _groundUnitsCount))
        {
            wavesInfo[_currentWaveId].dictionaryUnits.Add(unitInfo, _groundUnitsCount);
        }

        groundUnitsType.text = unitsPrefabs[_groundUnitIndex].name;
        groundUnitsCountText.text = _groundUnitsCount.ToString();
    }

    /// <summary>
    /// Функция для изменения идентификатора воздушных юнитов
    /// </summary>
    /// <param name="x">Число, указывающее в какую сторону будем менять значение</param>
    public void ChangeTypeAirUnit(int x)
    {
        ChangeTypeIndexValue(ref _airUnitIndex, x, Unit.UnitType.Air);

        WaveInfo.UnitInfo unitInfo = new WaveInfo.UnitInfo(Unit.UnitType.Air, unitsPrefabs[_airUnitIndex]);
        _airUnitsCount = 0;
        if (!wavesInfo[_currentWaveId].dictionaryUnits.TryGetValue(unitInfo, out _airUnitsCount))
        {
            wavesInfo[_currentWaveId].dictionaryUnits.Add(unitInfo, _airUnitsCount);
        }

        airUnitsType.text = unitsPrefabs[_airUnitIndex].name;
        airUnitsCountText.text = _airUnitsCount.ToString();
    }

    /// <summary>
    /// Изменение идентификатора босса
    /// </summary>
    /// <param name="x">Число, указывающее в какую сторону будем менять значение</param>
    public void ChangeTypeBossUnit(int x)
    {
        _bossIndex += x;
        if (_bossIndex < 0)
            _bossIndex = bossesPrefabs.Count - 1;
        if (_bossIndex >= bossesPrefabs.Count)
            _bossIndex = 0;

        WaveInfo.UnitInfo unitInfo = new WaveInfo.UnitInfo(bossesPrefabs[_bossIndex].GetComponent<Unit>().unitType, bossesPrefabs[_bossIndex]);
        _bossCount = 0;
        if (!wavesInfo[_currentWaveId].dictionaryBosses.TryGetValue(unitInfo, out _bossCount))
        {
            wavesInfo[_currentWaveId].dictionaryBosses.Add(unitInfo, _bossCount);
        }

        bossTypeText.text = bossesPrefabs[_bossIndex].name;
        bossCountText.text = _bossCount.ToString();
    }

    public void SavePrefs()
    {
        PlayerPrefs.SetInt("startCoins", _startCoins);
        PlayerPrefs.SetInt("startLifes", _lifesCount);
        PlayerPrefs.SetInt("wavesCount", _wavesCount);

        for (int id = 0; id < wavesInfo.Count; id++)
        {
            foreach (KeyValuePair<WaveInfo.UnitInfo, int> keyPair in wavesInfo[id].dictionaryUnits)
            {
                PlayerPrefs.SetInt("wave" + id + keyPair.Key.unitType.ToString() + keyPair.Key.unitPrefab.name + "Count", keyPair.Value);
            }

            foreach (KeyValuePair<WaveInfo.UnitInfo, int> keyPair in wavesInfo[id].dictionaryBosses)
            {
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
        if (count > _maxUnits)
            count = _maxUnits;
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
                _unitsPrefabsInfo.Add(unitInfo);
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
                _bossesPrefabsInfo.Add(unitInfo);
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
        StartCoins = PlayerPrefs.GetInt("startCoins", _startCoins);
        LifesCount = PlayerPrefs.GetInt("startLifes", _lifesCount);
        WavesCount = PlayerPrefs.GetInt("wavesCount", _wavesCount);
        wavesInfo = new List<WaveInfo>();

        for (int i = 0; i < _maxWaves; i++)
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

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        CheckPrefabs();

        LoadPrefs();
        CurrentWave = 0;
    }

}
