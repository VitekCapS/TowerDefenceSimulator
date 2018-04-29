using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    private static UIController m_Instance;
    public static UIController Instance
    {
        get { return m_Instance; }
    }

    public GameObject winloseWindow;
    public List<Image> buyTowerImages;
    public List<Text> priceTowerTextFields;
    public List<Text> moneyCountTextFields;
    public GameObject sellTowerButton;
    public Text bidTowerText;
    public Text currentWaveText;
    public Text currentLifesText;
    public Text unitsCountText;

    [Header("STATISTICS")]
    public GameObject statisticsWindow;
    public GameObject statisticsContent;
    public GameObject statisticsFieldPrefab;

    private List<StatisticsField> statisticsFields = new List<StatisticsField>();
    private struct StatisticsField
    {
        public GameObject gameObject;
        public Text fieldNameText;
        public Text valueText;

        public StatisticsField(GameObject obj, Text name, Text val)
        {
            gameObject = obj;
            fieldNameText = name;
            valueText = val;
        }
    }

    private List<StatisticsFieldInfo> statisticsFieldInfos = new List<StatisticsFieldInfo>();
    private struct StatisticsFieldInfo
    {
        public string fieldName;
        public string value;

        public StatisticsFieldInfo(string name, string val)
        {
            fieldName = name;
            value = val;
        }
    }
    #region Были полями статистики
    public Text towersDamageDealtTotal;
    public Text lifesLostTotal;
    public Text killsCountTotal;
    public Text groundUnitsKilledTotal;
    public Text airUnitsKilledTotal;
    public Text bossesKilledTotal;
    public Text towersBuiltTotal;
    public Text towersSoldTotal;
    public Text earnedCoinsTotal;
    public Text coinsSpentTotal;
    [Space(2)]
    public Text towersDamageDealt;
    public Text lifesLost;
    public Text killsCount;
    public Text groundUnitsKilled;
    public Text airUnitsKilled;
    public Text bossesKilled;
    public Text towersBuilt;
    public Text towersSold;
    public Text moneyFromSoldTowers;
    public Text moneySpendForTowers;
    public Text earnedCoins;
    public Text coinsSpent;
    #endregion

    private void OnEnable()
    {
        GameController.OnCoinsValueChanged += UpdateUICoins;
        GameController.OnFinishWave += UpdateUIWaves;
        GameController.OnLifeLost += UpdateUILifes;
        GameController.OnGameOver += ShowWinLoseWindow;
        GameController.OnCountUnitsChanged += UpdateUIUnitCount;
    }

    private void OnDisable()
    {
        GameController.OnCoinsValueChanged -= UpdateUICoins;
        GameController.OnFinishWave -= UpdateUIWaves;
        GameController.OnLifeLost -= UpdateUILifes;
        GameController.OnGameOver -= ShowWinLoseWindow;
        GameController.OnCountUnitsChanged -= UpdateUIUnitCount;
    }

    private void Awake()
    {
        m_Instance = this;
        FillStatisticsWindow();
    }

    private void Start()
    {
        UpdateUI();
    }

    public void EnableSellTower(BasicTower tower)
    {
        bidTowerText.text = tower.bid.ToString();
        sellTowerButton.SetActive(true);
    }

    public void DisableSellTower()
    {
        sellTowerButton.SetActive(false);
    }

    public void UpdateUICoins(int value)
    {
        foreach (Text field in moneyCountTextFields)
        {
           field.text = GameController.Instance.CurrentCoins.ToString();
        }
    }

    public void UpdateUIWaves(int value)
    {
        currentWaveText.text = value.ToString();
    }

    public void UpdateUILifes(int value)
    {
        currentLifesText.text = GameController.Instance.CurrentLifes.ToString();
    }

    public void UpdateUIUnitCount(int value)
    {
        unitsCountText.text = value.ToString();
    }

    public void UpdateUI()
    {
        for (int i = 0; i < priceTowerTextFields.Count; i++)
        {
            BasicTower tower = GameController.Instance.towerPrefabs[i].GetComponent<BasicTower>();
            priceTowerTextFields[i].text = tower.price.ToString();
            buyTowerImages[i].sprite = tower.image;
        }
        
    }

    //public void UpdateUIStatistics()
    //{
    //    towersDamageDealtTotal.text = Statistics.towersDamageDealtTotal.ToString();
    //    lifesLostTotal.text = Statistics.lifesLostTotal.ToString();
    //    killsCountTotal.text = Statistics.killsCountTotal.ToString();
    //    groundUnitsKilledTotal.text = Statistics.groundUnitsKilledTotal.ToString();
    //    airUnitsKilledTotal.text = Statistics.airUnitsKilledTotal.ToString();
    //    bossesKilledTotal.text = Statistics.bossesKilledTotal.ToString();
    //    towersBuiltTotal.text = Statistics.towersBuiltTotal.ToString();
    //    towersSoldTotal.text = Statistics.towersSoldTotal.ToString();
    //    earnedCoinsTotal.text = Statistics.earnedCoinsTotal.ToString();
    //    coinsSpentTotal.text = Statistics.coinsSpentTotal.ToString();

    //    towersDamageDealt.text = Statistics.Instance.towersDamageDealt.ToString();
    //    lifesLost.text = Statistics.Instance.lifesLost.ToString();
    //    killsCount.text = Statistics.Instance.killsCount.ToString();
    //    groundUnitsKilled.text = Statistics.Instance.groundUnitsKilled.ToString();
    //    airUnitsKilled.text = Statistics.Instance.airUnitsKilled.ToString();
    //    bossesKilled.text = Statistics.Instance.bossesKilled.ToString();
    //    towersBuilt.text = Statistics.Instance.towersBuilt.ToString();
    //    towersSold.text = Statistics.Instance.towersSold.ToString();
    //    moneyFromSoldTowers.text = Statistics.Instance.moneyFromSoldTowers.ToString();
    //    moneySpendForTowers.text = Statistics.Instance.moneySpendForTowers.ToString();
    //    earnedCoins.text = Statistics.Instance.earnedCoins.ToString();
    //    coinsSpent.text = Statistics.Instance.coinsSpent.ToString();
    //}

    public void ShowStatisticsWindow()
    {
        //UpdateUIStatistics();
        UpdateStatisticsWindow();
        statisticsWindow.SetActive(true);
    }

    public void ShowWinLoseWindow()
    {
        winloseWindow.SetActive(true);

        ShowStatisticsWindow();
    }

    public void FillStatisticsWindow()
    {
        FieldInfo[] fields = Statistics.Instance.GetType().GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            string fieldName = field.Name + ":";
            string value = field.GetValue(Statistics.Instance).ToString();

            GameObject newField = Instantiate(statisticsFieldPrefab, statisticsContent.transform);
            Text newFieldName = newField.transform.Find("FieldName").GetComponent<Text>();
            Text newFieldValue = newField.transform.Find("Value").GetComponent<Text>();
            newFieldName.text = fieldName;
            newFieldValue.text = value;

            statisticsFields.Add(new StatisticsField(newField, newFieldName, newFieldValue));
            statisticsFieldInfos.Add(new StatisticsFieldInfo(fieldName, value));
        }
    }

    public void UpdateStatisticsWindow()
    {
        FieldInfo[] fields = Statistics.Instance.GetType().GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            string fieldName = field.Name + ":";
            string value = field.GetValue(Statistics.Instance).ToString();

            int indexField = statisticsFieldInfos.FindIndex(x => x.fieldName == fieldName);

            if (indexField < 0)
                Debug.Log("Что-то пошло не так");

            statisticsFields[indexField].valueText.text = value;
        }

    }
}
