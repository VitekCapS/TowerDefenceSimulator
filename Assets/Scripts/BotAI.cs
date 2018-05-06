using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BotAI : MonoBehaviour
{

    public Unit bot;
    public State currentState = State.Move;
    public Waypoint[] waypoints;

    private Waypoint _currentWaypoint;

    public delegate void FinishReachedEventHandler(Unit unit);

    public static event FinishReachedEventHandler OnFinishEvent;

    public enum State
    {
        Move,
        Dead
    }


    protected virtual void Start()
    {
        waypoints = FindObjectsOfType<Waypoint>();
        _currentWaypoint = waypoints.First(x => x.isStart);
    }

    protected virtual void Update()
    {
        StateBehaviour();
    }

    protected virtual void StateBehaviour()
    {
        switch (currentState)
        {
            case State.Move:
                MoveToWaypoint(_currentWaypoint);
                break;
            case State.Dead:
                break;
        }
    }

    protected virtual void ChangeState(State state)
    {
        if (currentState == state)
            return;

        //конструкция для выхода из состояния A (в данном случае пустая)
        switch (currentState)
        {
            case State.Move:
                break;
            case State.Dead:
                break;
        }

        //конструкция для входа в состояние B (в данном случае тоже пустая)
        switch (state)
        {
            case State.Move:
                break;
            case State.Dead:
                break;
        }

        currentState = state;

    }

    private float _partOfStep = 1; //переменная доли шага юнита (при достаточно высокой скорости либо при малом fps призвана устранять рывки передвижения)

    public void MoveToWaypoint(Waypoint waypoint)
    {
        Vector3 dir = waypoint.transform.position - bot.transform.position;             //направление передвижения в точку
        float sqrSpeed = (Time.deltaTime * bot.Speed) * (Time.deltaTime * bot.Speed);  //скорость передвижения в квадрате

        //Перемещаем бота в нужном направлении если на этом шаге он не "перескочит" точку
        if (dir.sqrMagnitude >= sqrSpeed)
        {
            Vector3 newPos = Vector3.MoveTowards(transform.position, _currentWaypoint.transform.position, bot.Speed * Time.deltaTime);
            bot.transform.position = newPos;
            _partOfStep = 1;
        }
        else
        {
            _partOfStep = dir.sqrMagnitude / sqrSpeed;

            if (!waypoint.isFinish)
            {
                NextWaypoint();
            }
            else
            {
                if (OnFinishEvent != null)
                    OnFinishEvent.Invoke(bot);
            }
        }

    }

    public void NextWaypoint()
    {
        _currentWaypoint = _currentWaypoint.nextWaypoint;
    }

}
