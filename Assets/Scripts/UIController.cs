using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    #region Singleton
    private static UIController m_Instance;
    public static UIController Instance
    {
        get
        {
            if (m_Instance == null)
            {
                m_Instance = FindObjectOfType<UIController>() ?? new UIController();
            }
            return m_Instance;
        }
    }
    #endregion

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

    private List<StatisticsField> _statisticsFields = new List<StatisticsField>();
    private List<StatisticsFieldInfo> _statisticsFieldInfos = new List<StatisticsFieldInfo>();

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
           field.text = value.ToString();
        }
    }

    public void UpdateUIWaves(int value)
    {
        currentWaveText.text = value.ToString();
    }

    public void UpdateUILifes(int value)
    {
        currentLifesText.text = value.ToString();
    }

    public void UpdateUIUnitCount(int value)
    {
        unitsCountText.text = value.ToString();
    }

    public void UpdateTowersUI()
    {
        for (int i = 0; i < priceTowerTextFields.Count; i++)
        {
            BasicTower tower = GameController.Instance.towerPrefabs[i].GetComponent<BasicTower>();
            priceTowerTextFields[i].text = tower.price.ToString();
            buyTowerImages[i].sprite = tower.image;
        }
    }

    public void ShowStatisticsWindow()
    {
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

            _statisticsFields.Add(new StatisticsField(newField, newFieldName, newFieldValue));
            _statisticsFieldInfos.Add(new StatisticsFieldInfo(fieldName, value));
        }
    }

    public void UpdateStatisticsWindow()
    {
        FieldInfo[] fields = Statistics.Instance.GetType().GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

        foreach (FieldInfo field in fields)
        {
            string fieldName = field.Name + ":";
            string value = field.GetValue(Statistics.Instance).ToString();

            int indexField = _statisticsFieldInfos.FindIndex(x => x.fieldName == fieldName);

            if (indexField < 0)
                Debug.Log("Что-то пошло не так");

            _statisticsFields[indexField].valueText.text = value;
        }

    }


    private void OnEnable()
    {
        GameController.OnCoinsValueChanged += UpdateUICoins;
        GameController.OnFinishWave += UpdateUIWaves;
        GameController.OnLifesValueChanged += UpdateUILifes;
        GameController.OnGameOver += ShowWinLoseWindow;
        GameController.OnCountUnitsChanged += UpdateUIUnitCount;
    }

    private void OnDisable()
    {
        GameController.OnCoinsValueChanged -= UpdateUICoins;
        GameController.OnFinishWave -= UpdateUIWaves;
        GameController.OnLifesValueChanged -= UpdateUILifes;
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
        UpdateTowersUI();
    }


}
