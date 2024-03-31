using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UnitBehavior : MonoBehaviour
{
    public List<Vector2Int> unitMovableDirections = new List<Vector2Int>();
    public bool registered = false;
    public int owingPlayerID;
    public string thisUnitName;

    public bool projectile = false;
    public bool hukyo = false;
    public bool kei = false;

    public bool onBoard = true;
    public bool active = true;
    public bool evolutionable = true;

    public void RegisterUnitInfo(int unitID)
    {
        if (unitID == 0) return;
        owingPlayerID = (unitID  / 15)+1;
    }
    public void RegisteUnitBehavior(string unitName)
    {
        thisUnitName = unitName;

        if (unitName == "FirstTurnHu(Clone)")
        {
            unitMovableDirections.Add(new Vector2Int(0, 1));
            hukyo= true;
        }
        if (unitName == "FirstTurnKei(Clone)")
        {
            unitMovableDirections.Add(new Vector2Int(1, 2));
            unitMovableDirections.Add(new Vector2Int(-1, 2));
            kei = true;
        }
        if (unitName == "FirstTurnKyo(Clone)")
        {
            projectile = true;
            hukyo = true;
        }
        if (unitName == "FirstTurnGin(Clone)")
        {
            unitMovableDirections.Add(new Vector2Int(0, 1));
            unitMovableDirections.Add(new Vector2Int(1, 1));
            unitMovableDirections.Add(new Vector2Int(-1, 1));
            unitMovableDirections.Add(new Vector2Int(1, -1));
            unitMovableDirections.Add(new Vector2Int(-1, -1));
        }
        if (unitName == "FirstTurnKin(Clone)")
        {
            unitMovableDirections.Add(new Vector2Int(0, 1));
            unitMovableDirections.Add(new Vector2Int(1, 1));
            unitMovableDirections.Add(new Vector2Int(-1, 1));
            unitMovableDirections.Add(new Vector2Int(1, 0));
            unitMovableDirections.Add(new Vector2Int(-1, 0));
            unitMovableDirections.Add(new Vector2Int(0, -1));
            this.evolutionable = false;
        }
        if (unitName == "FirstTurnKaku(Clone)")
        {
            projectile = true;
        }
        if (unitName == "FirstTurnHisha(Clone)")
        {
            projectile = true;
        }
        if (unitName == "FirstTurnOu(Clone)" || unitName == "SecondTurnOu(Clone)")
        {
            unitMovableDirections.Add(new Vector2Int(0, 1));
            unitMovableDirections.Add(new Vector2Int(1, 0));
            unitMovableDirections.Add(new Vector2Int(0, -1));
            unitMovableDirections.Add(new Vector2Int(-1, 0));
            unitMovableDirections.Add(new Vector2Int(1, 1));
            unitMovableDirections.Add(new Vector2Int(-1, 1));
            unitMovableDirections.Add(new Vector2Int(1, -1));
            unitMovableDirections.Add(new Vector2Int(-1, -1));
            this.evolutionable = false;
        }

        if (unitName == "SecondTurnHu(Clone)")
        {
            unitMovableDirections.Add(new Vector2Int(0, -1));
            hukyo = true;
        }
        if (unitName == "SecondTurnKei(Clone)")
        {
            unitMovableDirections.Add(new Vector2Int(-1, -2));
            unitMovableDirections.Add(new Vector2Int(1, -2));
            kei = true;
        }
        if (unitName == "SecondTurnKyo(Clone)")
        {
            projectile = true;
            hukyo = true;
        }
        if (unitName == "SecondTurnGin(Clone)")
        {
            unitMovableDirections.Add(new Vector2Int(0, -1));
            unitMovableDirections.Add(new Vector2Int(-1, -1));
            unitMovableDirections.Add(new Vector2Int(1, -1));
            unitMovableDirections.Add(new Vector2Int(-1, 1));
            unitMovableDirections.Add(new Vector2Int(1, 1));
        }
        if (unitName == "SecondTurnKin(Clone)")
        {
            unitMovableDirections.Add(new Vector2Int(0, -1));
            unitMovableDirections.Add(new Vector2Int(-1, -1));
            unitMovableDirections.Add(new Vector2Int(1, -1));
            unitMovableDirections.Add(new Vector2Int(0,1));
            unitMovableDirections.Add(new Vector2Int(-1, 0));
            unitMovableDirections.Add(new Vector2Int(1, 0));
            this.evolutionable = false;
        }
        if (unitName == "SecondTurnKaku(Clone)")
        {
            projectile = true;

        }
        if (unitName == "SecondTurnHisha(Clone)")
        {
            projectile = true;
        }

    }

    public List<Vector2Int> FirstTurnLanceBehavior(Vector2Int startPosition ,Vector2Int endPosition)
    {
        var movablePositions = new List<Vector2Int>();

        for (int i = startPosition[1] - endPosition[1] -1; i >= 1; i--)
        {
            movablePositions.Add(new Vector2Int(0, i));
        }
        return movablePositions;
    }

    public List<Vector2Int> SecondTurnLanceBehavior(Vector2Int startPosition, Vector2Int endPosition)
    {
        var movablePositions = new List<Vector2Int>();

        for (int i = endPosition[1] - startPosition[1] -1; i >= 1; i--)
        {
            movablePositions.Add(new Vector2Int(0, -i));
        }
        return movablePositions;
    }

    public List<Vector2Int> GetLeftDirection(Vector2Int startPosition, Vector2Int endPosition)
    {
        var movablePositions = new List<Vector2Int>();

        for (int i = endPosition[0] - startPosition[0] - 1; i >= 1; i--)
        {
            movablePositions.Add(new Vector2Int(i,0));
        }
        return movablePositions;
    }

    public List<Vector2Int> GetRightDirection(Vector2Int startPosition, Vector2Int endPosition)
    {
        var movablePositions = new List<Vector2Int>();
        for (int i = startPosition[0] - endPosition[0] - 1; i >= 1; i--)
        {
            movablePositions.Add(new Vector2Int(-i, 0));
        }
        return movablePositions;
    }

    public List<Vector2Int> GetLeftDownDirection(Vector2Int startPosition, Vector2Int endPosition)
    {
        var movablePositions = new List<Vector2Int>();

        for (int i = endPosition[1] - startPosition[1] - 1; i >= 1; i--)
        {
            movablePositions.Add(new Vector2Int(i, -i));
        }
        return movablePositions;
    }

    public List<Vector2Int> GetRightDownDirection(Vector2Int startPosition, Vector2Int endPosition)
    {
        var movablePositions = new List<Vector2Int>();

        for (int i = endPosition[1] - startPosition[1] - 1; i >= 1; i--)
        {
            movablePositions.Add(new Vector2Int(-i, -i));
        }
        return movablePositions;
    }

    public List<Vector2Int> GetRightUpDirection(Vector2Int startPosition, Vector2Int endPosition)
    {
        var movablePositions = new List<Vector2Int>();

        for (int i = startPosition[1] - endPosition[1] - 1; i >= 1; i--)
        {
            movablePositions.Add(new Vector2Int(-i, i));
        }
        return movablePositions;
    }

    public List<Vector2Int> GetLeftUpDirection(Vector2Int startPosition, Vector2Int endPosition)
    {
        var movablePositions = new List<Vector2Int>();

        for (int i = startPosition[1] - endPosition[1] - 1; i >= 1; i--)
        {
            movablePositions.Add(new Vector2Int(i, i));
        }
        return movablePositions;
    }

    public void ChangeMoveblePositionByEvolution(int playerID)
    {
        this.evolutionable = false;
        unitMovableDirections.Clear();
        if(thisUnitName.Contains("Hisha"))
        {
            unitMovableDirections.Add(new Vector2Int(1, 1));
            unitMovableDirections.Add(new Vector2Int(-1, 1));
            unitMovableDirections.Add(new Vector2Int(1, -1));
            unitMovableDirections.Add(new Vector2Int(-1, -1));
        }
        else if (thisUnitName.Contains("Kaku"))
        {
            unitMovableDirections.Add(new Vector2Int(0, 1));
            unitMovableDirections.Add(new Vector2Int(0, -1));
            unitMovableDirections.Add(new Vector2Int(1, 0));
            unitMovableDirections.Add(new Vector2Int(-1, 0));
        }
        else if(!thisUnitName.Contains("Hisha") 
            && !thisUnitName.Contains("Kaku") 
            && playerID == 0)
        {
            unitMovableDirections.Add(new Vector2Int(0, 1));
            unitMovableDirections.Add(new Vector2Int(1, 1));
            unitMovableDirections.Add(new Vector2Int(-1, 1));
            unitMovableDirections.Add(new Vector2Int(1, 0));
            unitMovableDirections.Add(new Vector2Int(-1, 0));
            unitMovableDirections.Add(new Vector2Int(0, -1));
        }
        else if(!thisUnitName.Contains("Hisha")
            && !thisUnitName.Contains("Kaku")
            &&playerID == 1)
        {
            unitMovableDirections.Add(new Vector2Int(0, -1));
            unitMovableDirections.Add(new Vector2Int(-1, -1));
            unitMovableDirections.Add(new Vector2Int(1, -1));
            unitMovableDirections.Add(new Vector2Int(0, 1));
            unitMovableDirections.Add(new Vector2Int(-1, 0));
            unitMovableDirections.Add(new Vector2Int(1, 0));
        }
    }
}
