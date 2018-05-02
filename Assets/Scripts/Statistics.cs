using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour
{

    protected static Statistics m_Instance = null;
    public static Statistics Instance
    {
        get
        {
            return m_Instance;
        }
    }


    public static int towersDamageDealtTotal;
    public static int lifesLostTotal;
    public static int killsCountTotal;
    public static int groundUnitsKilledTotal;
    public static int airUnitsKilledTotal;
    public static int bossesKilledTotal;
    public static int towersBuiltTotal;
    public static int towersSoldTotal;
    public static int earnedCoinsTotal;
    public static int coinsSpentTotal;

    public int towersDamageDealt;
    public int lifesLost;
    public int killsCount;
    public int groundUnitsKilled;
    public int airUnitsKilled;
    public int bossesKilled;
    public int towersBuilt;
    public int towersSold;
    public int moneyFromSoldTowers;
    public int moneySpendForTowers;
    public int earnedCoins;
    public int coinsSpent;

    private void Awake()
    {
        m_Instance = this;
    }

    private void OnEnable()
    {
        Unit.OnDamageTaken += TowersDamageDealt;
        Unit.OnDieEvent += UnitKilled;
        GameController.OnAddCoins += CoinsEarned;
        GameController.OnSpentCoins += CoinsSpent;
        GameController.OnLifeLost += LifesLost;
        TowerController.OnBuyTower += TowerBought;
        TowerController.OnSellTower += TowerSold;
    }

    private void OnDisable()
    {
        Unit.OnDamageTaken -= TowersDamageDealt;
        Unit.OnDieEvent -= UnitKilled;
        GameController.OnAddCoins -= CoinsEarned;
        GameController.OnSpentCoins -= CoinsSpent;
        GameController.OnLifeLost -= LifesLost;
        TowerController.OnBuyTower -= TowerBought;
        TowerController.OnSellTower -= TowerSold;

    }

    private void TowersDamageDealt(params int[] args)
    {
        int x = args[0];
        towersDamageDealt += x;
        towersDamageDealtTotal += x;
    }

    private void LifesLost(int x)
    {
        lifesLost += x;
        lifesLostTotal += x;
    }

    private void UnitKilled(Unit unit)
    {
        switch (unit.unitType)
        {
            case Unit.UnitType.Air:
                airUnitsKilled++;
                airUnitsKilledTotal++;
                break;
            case Unit.UnitType.Ground:
                groundUnitsKilled++;
                groundUnitsKilledTotal++;
                break;
        }

        if (unit.isBoss)
        {
            bossesKilled++;
            bossesKilledTotal++;
        }

        killsCount++;
        killsCountTotal++;
    }

    private void TowerBought(int price)
    {
        moneySpendForTowers += price;
        towersBuilt++;
        towersBuiltTotal++;
    }

    private void TowerSold(int bid)
    {
        moneyFromSoldTowers += bid;
        towersSold++;
        towersSoldTotal++;
    }

    private void CoinsEarned(int x)
    {
        earnedCoins += x;
        earnedCoinsTotal += x;
    }

    private void CoinsSpent(int x)
    {
        coinsSpent += x;
        coinsSpentTotal += x;
    }
}
