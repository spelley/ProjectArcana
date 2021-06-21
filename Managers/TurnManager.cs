using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TurnManager
{
    // variables
    List<ITurnTaker> turnTakers = new List<ITurnTaker>();
    public ITurnTaker curTurnTaker { get; private set; }

    // events
    public event Action<ITurnTaker> OnTurnStart;
    public event Action<ITurnTaker> OnTurnEnd;

    public TurnManager(List<ITurnTaker> toAdd)
    {
        AddTurnTakers(toAdd);
    }

    public void AddTurnTakers(List<ITurnTaker> toAdd)
    {
        foreach(ITurnTaker turnTaker in toAdd)
        {
            AddTurnTaker(turnTaker);
        }
    }

    public void AddTurnTaker(ITurnTaker turnTaker, int startingCT = 0)
    {
        turnTaker.ct = startingCT;
        turnTakers.Add(turnTaker);
    }

    public void StartNextTurn()
    {
        curTurnTaker = GetNextTurn();
        if(curTurnTaker.incapacitated)
        {
            EndTurn();
        }
        else
        {
            curTurnTaker.StartTurn();
            OnTurnStart?.Invoke(curTurnTaker);
        }

        BattleManager.Instance.IsEncounterResolved();
    }

    public void EndTurn()
    {
        BattleManager.Instance.FlowRiver(1);
        curTurnTaker.EndTurn();
        OnTurnEnd?.Invoke(curTurnTaker);

        if(!BattleManager.Instance.IsEncounterResolved())
        {
            StartNextTurn();
        }
    }

    ITurnTaker GetNextTurn()
    {
        ITurnTaker curTurnTaker = null;
        curTurnTaker = turnTakers.Where(tt => tt.ct >= 100).OrderByDescending(tt => tt.ct).ThenByDescending(tt => tt.speed).FirstOrDefault();

        // safety measure in case somehow everyone has 0 speed or we messed up somehow
        int loopCount = 0;
        int loopMax = 100;
        List<ITurnTaker> maxedList = new List<ITurnTaker>();
        while(curTurnTaker == null && loopCount < loopMax)
        {
            for(int i = 0; i < turnTakers.Count; i++)
            {
                turnTakers[i].ct += turnTakers[i].speed;
                if(turnTakers[i].ct >= 100)
                {
                    maxedList.Add(turnTakers[i]);
                }
            }
            if(maxedList.Count == 1)
            {
                curTurnTaker = maxedList[0];
            }
            else if(maxedList.Count > 1)
            {
                curTurnTaker = maxedList.OrderByDescending(tt => tt.ct).ThenByDescending(tt => tt.speed).FirstOrDefault();
            }
            loopCount++;
        }
        if(curTurnTaker == null)
        {
            Debug.Log("why on earth is everyone at 0 speed?");
            return turnTakers[0];
        }
        curTurnTaker.ct = 0;
        return curTurnTaker;
    }

    public Queue<ITurnTaker> GetPredictedTurns(int totalTurns)
    {
        Queue<ITurnTaker> predictedTurnTakers = new Queue<ITurnTaker>();
        Dictionary<ITurnTaker, int> turnDictionary = new Dictionary<ITurnTaker, int>();
        
        foreach(ITurnTaker turnTaker in turnTakers)
        {
            turnDictionary.Add(turnTaker, turnTaker.ct);
        }

        int maxLoop = 200;
        int curLoop = 0;
        while(predictedTurnTakers.Count < totalTurns && curLoop < maxLoop)
        {
            List<ITurnTaker> maxedList = new List<ITurnTaker>();
            foreach(ITurnTaker turnTaker in turnTakers)
            {
                if(turnDictionary[turnTaker] >= 100)
                {
                    maxedList.Add(turnTaker);
                }
            }
            if(maxedList.Count > 0)
            {
                List<ITurnTaker> sortedList = maxedList.OrderByDescending(tt => turnDictionary[tt]).ThenByDescending(tt => tt.speed).ToList();
                foreach(ITurnTaker turnTaker in sortedList)
                {
                    if(predictedTurnTakers.Count < totalTurns)
                    {
                        predictedTurnTakers.Enqueue(turnTaker);
                        turnDictionary[turnTaker] = 0;
                    }
                }
            }
            else {
                foreach(ITurnTaker turnTaker in turnTakers)
                {
                    turnDictionary[turnTaker] += turnTaker.speed;
                }
            }
            maxedList.Clear();
            curLoop++;
        }

        return predictedTurnTakers;
    }

    public void Clear()
    {
        turnTakers.Clear();
    }
}