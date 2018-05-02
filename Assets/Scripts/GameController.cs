using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private static GameController m_Instance;
    public static GameController Instance
    {
        get { return m_Instance; }
    }

    public int startCoins = 250;
    public int startLifes = 10;
    public int wavesCount = 1;
    public float delayBetweenSpawnUnits = 0.6f;
    public float delayBetweenWaves = 5.0f;
    public bool dontLoadPrefs = false;
    public List<WaveInfo> wavesInfo;

    [Space(10), Header("TOWER PREFABS"), Space(2)]
    public List<GameObject> towerPrefabs;
    [Header("tower graphics object for placing")]
    public List<GameObject> towerTemps;
    [Space(3), Header("UNIT PREFABS"), Space(2)]
    public List<GameObject> unitsPrefabs;
    public List<GameObject> bossesPrefabs;
    [Header("LEVELS")]
    public int currentLevel;
    public List<GameObject> levelPrefabs;
    [Space(10)]
    public int currentWaveNum = 1;

    private int countUnits;
    private int currentCoins;
    private int currentLifes;
    private List<Wave> wavesList = new List<Wave>();
    private bool isGameOver = false;
    private Spawner spawner;

    public delegate void GameValueChangedEventHandler(int x);
    public static event GameValueChangedEventHandler
        OnCountUnitsChanged, OnCoinsValueChanged, OnLifesValueChanged, OnAddCoins, OnSpentCoins, OnLifeLost, OnStartWave, OnFinishWave;
    public static event Action OnGameOver;

    #region Properties

    public int CurrentCoins
    {
        get
        {
            return currentCoins;
        }

        set
        {
            currentCoins = value;
            if (OnCoinsValueChanged != null)
                OnCoinsValueChanged.Invoke(value);
        }
    }

    public int CurrentLifes
    {
        get
        {
            return currentLifes;
        }

        set
        {
            currentLifes = value;
            if (OnLifesValueChanged != null)
                OnLifesValueChanged.Invoke(value);
        }
    }

    public int CountUnits
    {
        get
        {
            return countUnits;
        }

        set
        {
            countUnits = value;
            if (OnCountUnitsChanged != null)
                OnCountUnitsChanged.Invoke(value);
        }
    }

    #endregion

    [System.Serializable]
    public struct Wave
    {
        public List<GameObject> units;
    }

    private void OnEnable()
    {
        BotAI.OnFinishEvent += DecreaseLifes;
        Unit.OnDieEvent += GetRewardForKill;
        Unit.OnDisableEvent += DecreaseUnitsCount;
    }

    private void OnDisable()
    {
        BotAI.OnFinishEvent -= DecreaseLifes;
        Unit.OnDieEvent -= GetRewardForKill;
        Unit.OnDisableEvent -= DecreaseUnitsCount;
    }

    void Awake()
    {
        m_Instance = this;
        currentLevel = PlayerPrefs.GetInt("Level", 0);
        Instantiate(levelPrefabs[currentLevel]);
    }

    // Use this for initialization
    void Start()
    {
        if (!dontLoadPrefs)
        {
            LoadPrefs();
        }
        else
        {
            InitWithoutLoadPrefs();
        }

        InitializeGame();
    }

    public void AddMoney(int amount)
    {
        CurrentCoins += amount;

        if (OnAddCoins != null)
            OnAddCoins.Invoke(amount);
    }

    public void SpendMoney(int amount)
    {
        CurrentCoins -= amount;

        if (OnSpentCoins != null)
            OnSpentCoins.Invoke(amount);
    }

    public void DecreaseLifes(Unit sender)
    {
        if (CurrentLifes > sender.DamageToLifes)
        {
            CurrentLifes -= sender.DamageToLifes;

            if (OnLifeLost != null)
                OnLifeLost.Invoke(sender.DamageToLifes);

        }
        else
        {
            if (OnLifeLost != null)
                OnLifeLost.Invoke(CurrentLifes);

            GameOver();
        }

        sender.Disable();
    }

    //---------------------GAME OVER -------------------------\\
    public void GameOver()
    {
        if (isGameOver)
            return;

        isGameOver = true;

        if (OnGameOver != null)
            OnGameOver.Invoke();

    }

    /// <summary>
    /// Проверка возможности покупки вернет true если покупка возможна
    /// </summary>
    /// <param name="id">Id башни в спске префабов</param>
    /// <returns></returns>
    public bool AcceptBuyTower(int id)
    {
        BasicTower tower = towerPrefabs[id].GetComponent<BasicTower>();

        if (tower.price <= CurrentCoins)
            return true;
        else
            return false;
    }

    public void RestartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitToMainMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    /// <summary>
    /// Получить награду за убийство юнита
    /// </summary>
    /// <param name="sender"></param>
    private void GetRewardForKill(Unit sender)
    {
        AddMoney(sender.Reward);
    }

    private void DecreaseUnitsCount()
    {
        CountUnits--;
    }

    /// <summary>
    /// Загрузка значений, выставленных в настройках
    /// </summary>
    private void LoadPrefs()
    {
        CurrentCoins = PlayerPrefs.GetInt("startCoins", startCoins);
        CurrentLifes = PlayerPrefs.GetInt("startLifes", startLifes);
        wavesCount = PlayerPrefs.GetInt("wavesCount", wavesCount);
        wavesInfo = new List<WaveInfo>();

        for (int i = 0; i < wavesCount; i++)
        {
            WaveInfo waveInfo = new WaveInfo();

            foreach (var value in Enum.GetValues(typeof(Unit.UnitType)))
            {
                for (int j = 0; j < unitsPrefabs.Count; j++)
                {
                    int unitsCount = PlayerPrefs.GetInt("wave" + i + value.ToString() + unitsPrefabs[j].name + "Count", 0);
                    if (unitsCount > 0)
                        waveInfo.dictionaryUnits.Add(new WaveInfo.UnitInfo((Unit.UnitType)value, unitsPrefabs[j]), unitsCount);
                }

                for (int k = 0; k < bossesPrefabs.Count; k++)
                {
                    int bossesCount = PlayerPrefs.GetInt("wave" + i + value.ToString() + bossesPrefabs[k].name + "Count", 0);
                    if (bossesCount > 0)
                        waveInfo.dictionaryBosses.Add(new WaveInfo.UnitInfo((Unit.UnitType)value, bossesPrefabs[k]), bossesCount);
                }
            }

            wavesInfo.Add(waveInfo);
        }

    }

    private void InitWithoutLoadPrefs()
    {
        CurrentCoins = startCoins;
        CurrentLifes = startLifes;

        if (wavesInfo.Count == 0)
        {
            Debug.LogWarning("Список волн пуст. Для начала игры необходима минимум одна не пустая волна.");
        }
        else
        {

        }

        wavesCount = wavesInfo.Count;
    }

    private void RemoveEmptyWaves()
    {
        int errorWave = wavesInfo.FindIndex(x =>
        {
            if (x.dictionaryUnits.Values.All(v => v == 0) && x.dictionaryBosses.Values.All(v => v == 0))
                return true;
            else return false;
        });

        if (errorWave > -1)
        {
            wavesInfo.RemoveAt(errorWave);
            wavesCount--;
            Debug.LogWarning("У волны " + errorWave + " не выставлено кол-во юнитов, волна была удалена!");
            RemoveEmptyWaves();
        }
    }

    /// <summary>
    /// Главная функция, инициализирующая игру
    /// </summary>
    private void InitializeGame()
    {
        spawner = FindObjectOfType<Spawner>();
        if (spawner == null)
        {
            Debug.Log("На локации отстутствует спаунер, игра не может быть начата!");
            return;
        }

        StartCoroutine(LifeCycleGame());
    }

    private void PrepareWaves()
    {
        for (int i = 0; i < wavesCount; i++)
        {
            PrepareWave(i, wavesInfo[i]);
        }
    }

    private void PrepareWave(int id, WaveInfo waveInfo)
    {
        Wave wave = new Wave();
        wave.units = new List<GameObject>();

        if (dontLoadPrefs)
        {
            foreach (WaveInfo.Dict dict in waveInfo.dictUnits)
            {
                for (int i = 0; i < dict.count; i++)
                {
                    wave.units.Add(Instantiate(dict.unitInfo.unitPrefab));
                }
            }

            foreach (WaveInfo.Dict dict in waveInfo.dictBosses)
            {
                for (int i = 0; i < dict.count; i++)
                {
                    wave.units.Add(Instantiate(dict.unitInfo.unitPrefab));
                }
            }

            goto Adding;
        }

        foreach (KeyValuePair<WaveInfo.UnitInfo, int> keyPair in waveInfo.dictionaryUnits)
        {
            for (int i = 0; i < keyPair.Value; i++)
            {
                GameObject newUnit = Instantiate(keyPair.Key.unitPrefab);
                newUnit.SetActive(false);
                wave.units.Add(newUnit);
            }
        }

        foreach (KeyValuePair<WaveInfo.UnitInfo, int> keyPair in waveInfo.dictionaryBosses)
        {
            for (int i = 0; i < keyPair.Value; i++)
            {
                GameObject newUnit = Instantiate(keyPair.Key.unitPrefab);
                newUnit.SetActive(false);
                wave.units.Add(newUnit);
            }
        }

        Adding:
        wavesList.Add(wave);

    }

    IEnumerator LifeCycleGame()
    {
        PrepareWaves(); //подготавливаем волны. Будут созданы все юниты для данной игры.

        while (!isGameOver)
        {
            yield return new WaitForSeconds(delayBetweenWaves);

            yield return StartCoroutine(LifeCycleWave());
        }
    }

    IEnumerator LifeCycleWave()
    {
        Wave currentWave = wavesList[currentWaveNum - 1];

        CountUnits = currentWave.units.Count;

        if (OnStartWave != null)
            OnStartWave.Invoke(currentWaveNum);

        yield return new WaitForSeconds(1);

        //Поочереди спавним юнитов с неким интервалом
        for (int i = 0; i < currentWave.units.Count; i++)
        {
            spawner.Spawn(currentWave.units[i]);
            yield return new WaitForSeconds(delayBetweenSpawnUnits);
        }
        //После спавна всех юнитов запускам цикл проверки
        //где каждую секунду будем проверять остались ли еще активные юниты на поле
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (currentWave.units.All(x => x.activeSelf == false))
            {
                if (OnFinishWave != null)
                    OnFinishWave.Invoke(currentWaveNum);

                if (currentWaveNum < wavesCount)
                    currentWaveNum++;
                else
                    GameOver(); //завершаем игру, если это была последняя волна

                //если выживших не осталось переключаемся на следующую волну и завершаем корутину
                yield break;
            }
        }
    }


}
