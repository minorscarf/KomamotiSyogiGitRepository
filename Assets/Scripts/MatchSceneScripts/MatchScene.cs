using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MatchScene : MonoBehaviour
{
    //対局判定、対局が継続中かどうか
    bool isGame = true;

    //勝利、敗北UI
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;

    //盤の画像（先手から見た盤面と後手から見た盤面の画像を２種類）
    [SerializeField] private GameObject firstTurnBoard;
    [SerializeField] private GameObject secondTurnBoard;

    //駒の画像
    [SerializeField] private List<GameObject> units = new List<GameObject>();
    //移動範囲を表示
    [SerializeField] private GameObject cursor;
    //選択している駒に対して表示
    [SerializeField] private GameObject selectUnitCursor;
    private GameObject selectCursor;

    //駒を成るかどうか選択するUI
    [SerializeField] private GameObject evolutionPanel;
    private bool evolveOrNot = true;
    private bool decideEvolve = false;

    //持ち駒の位置
    [SerializeField] private List<GameObject> firstTurnPlayerTakeUnitPosition = new List<GameObject>();
    [SerializeField] private List<GameObject> secondTurnPlayerTakeUnitPosition = new List<GameObject>();

    //升目の配置
    [SerializeField] private GameObject unitPosition;
    private GameObject[,] positions = new GameObject[9,9];
    private Vector2 unitInterval = new Vector2(31.7f * 5f / (34.8f * 9f), 5f / 9f);
    private Vector2 unit11Position = new Vector2(34.8f * 2.5f / 31.7f, 2.5f);
    private Vector2 adjustmentVector = new Vector2(0.4671805f, 0);

    //駒の初期配置ベクトル、10行目11行目は先手と後手の持ち駒の情報
    private int[,] defoultUnitArrangement =
    {
        { 16,17,18,19,22,19,18,17,16 },
        { 0,20,0,0,0,0,0,21,0 },
        { 15,15,15,15,15,15,15,15,15 },
        {0,0,0,0,0,0,0,0,0 },
        {0,0,0,0,0,0,0,0,0 },
        {0,0,0,0,0,0,0,0,0 },
        {1,1,1,1,1,1,1,1,1 },
        {0,7,0,0,0,0,0,6,0 },
        {2,3,4,5,8,5,4,3,2 },
        {1,2,3,4,5,6,7,0,0 },
        {15,16,17,18,19,20,21,0,0 }
    };

    //先手の持ち駒のオブジェクトを格納する配列
    private GameObject[] firstPlayerPossesionUnit =
        {null,null,null,null,null,null,null};
    //先手の持ち駒のそれぞれの枚数を格納する配列
    private int[] firstPlayerPossesionUnitAmount =
        {0,0,0,0,0,0,0};
    //先手の持ち駒のそれぞれの枚数を表示するテキストを格納する配列
    [SerializeField] private Text[] firstPlayerPossesionUnitAmoutText =
        {};
    //後手の持ち駒のオブジェクトを格納する配列
    private GameObject[] secondPlayerPossesionUnit =
        {null, null, null, null, null, null, null};
    //後手の持ち駒のそれぞれの枚数を格納する配列
    private int[] secondPlayerPossesionUnitAmount =
        {0,0,0,0,0,0,0};
    //後手の持ち駒のそれぞれの枚数を表示するテキストを格納する配列
    [SerializeField]private Text[] secondPlayerPossesionUnitAmoutText =
        {};

    //駒が動ける位置を一時的に格納するリスト
    private List<Vector2Int> range = new List<Vector2Int>();

    //現在の駒の配置ベクトル
    private int[,] UnitIDArrangement;

    //駒を配列に格納して管理、呼び出しが行えるようにする
    private GameObject[,] UnitArrangement =
    {
        {null,null,null,null,null,null,null,null,null },
        {null,null,null,null,null,null,null,null,null },
        {null,null,null,null,null,null,null,null,null },
        {null,null,null,null,null,null,null,null,null },
        {null,null,null,null,null,null,null,null,null },
        {null,null,null,null,null,null,null,null,null },
        {null,null,null,null,null,null,null,null,null },
        {null,null,null,null,null,null,null,null,null },
        {null,null,null,null,null,null,null,null,null },
        {null,null,null,null,null,null,null,null,null },
        {null,null,null,null,null,null,null,null,null },
    };

    //選択した駒のUnitBehaviorを一時的に格納する
    private UnitBehavior selectUnitUnitBehavior;
    //選択した駒が進める升目のリスト
    private List<Vector2Int> movablePositionList = new List<Vector2Int>();
    //表示した升目を一時的に格納するリスト
    private List<GameObject> movablePositionCursors = new List<GameObject>();
    //選択中の升目を一時的に格納する変数
    private Vector2Int selectedPosition = new Vector2Int(1,1);
    //駒が選択されているかどうか判定する
    private bool isSelected = false;
    //駒が選択された時にちょっと上に動く量
    private Vector3 selectedUnitLittleFloat = new Vector3(-0.1f, 0.11f, 0);

    //プレイヤーID（先手は0、後手なら１）
    private int playerID = 0;

    //自分の手番かどうか
    private bool turn;
    // Start is called before the first frame update
    void Start()
    {
        Vector2 unitPositionTransform;
        UnitIDArrangement = defoultUnitArrangement;
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                positions[i, j] = Instantiate(unitPosition);
                unitPositionTransform = (((unit11Position - new Vector2(i, j) * unitInterval)
                    + (unit11Position - new Vector2(i + 1, j + 1) * unitInterval)) / 2)
                    - adjustmentVector;
                positions[i, j].transform.position = unitPositionTransform;
                positions[i,j].name = new Vector2(i, j).ToString();

                UnitArrangement[j, i] = Instantiate(units[defoultUnitArrangement[j, i]],
                    unitPositionTransform, Quaternion.identity);
                UnitBehavior unitBehavior = UnitArrangement[j, i].AddComponent<UnitBehavior>();
                unitBehavior.RegisterUnitInfo(defoultUnitArrangement[j, i]);
            }
        }

        //駒台の初期化、駒台に全種類の駒を置いて非表示にしておく
        for (int i = 0; i < firstTurnPlayerTakeUnitPosition.Count; i++)
        {
            var takeUnitPosition = Instantiate(unitPosition
                , firstTurnPlayerTakeUnitPosition[i].transform.position
                , Quaternion.identity) as GameObject;
            takeUnitPosition.name = new Vector2(i,9).ToString();

            firstPlayerPossesionUnit[i] = Instantiate(units[i + 1], takeUnitPosition.transform.position,
                Quaternion.identity) as GameObject;

            var possesionUnitBehavior = firstPlayerPossesionUnit[i].AddComponent<UnitBehavior>();
            possesionUnitBehavior.RegisterUnitInfo(i+1);
            possesionUnitBehavior.onBoard = false;
            possesionUnitBehavior.active = false;
            firstPlayerPossesionUnit[i].SetActive(false);

            UnitArrangement[9,i] = firstPlayerPossesionUnit[i];
        }
        for (int i = 0; i < secondTurnPlayerTakeUnitPosition.Count; i++)
        {
            var takeUnitPosition = Instantiate(unitPosition
                , secondTurnPlayerTakeUnitPosition[i].transform.position
                , Quaternion.identity) as GameObject;
            takeUnitPosition.name = new Vector2(i,10).ToString();

            secondPlayerPossesionUnit[i] = Instantiate(units[21-i], takeUnitPosition.transform.position,
                Quaternion.identity) as GameObject;

            var possesionUnitBehavior = secondPlayerPossesionUnit[i].AddComponent<UnitBehavior>();
            possesionUnitBehavior.RegisterUnitInfo(i + 15);
            possesionUnitBehavior.onBoard = false;
            possesionUnitBehavior.active = false;
            secondPlayerPossesionUnit[i].SetActive(false);

            UnitArrangement[10,i] = secondPlayerPossesionUnit[i];
        }

        Camera.main.transform.rotation = Quaternion.Euler(0, 0, 180*playerID);
        evolutionPanel.SetActive(false);
        winUI.SetActive(false);
        loseUI.SetActive(false);

        if(playerID == 0)
        {
            turn = true;
            secondTurnBoard.SetActive(false);
            firstTurnBoard.SetActive(true);
        }
        else
        {
            turn = false;
            secondTurnBoard.SetActive(true);
            firstTurnBoard.SetActive(false);
        }
    }

    private void Update()
    {
        if(turn && Input.GetMouseButtonDown(0) && isGame)
        {
            JudgeSelectUnit();
        }
        if(!turn && isGame)
        {
            BotMoveUnit();
        }
    }

    //クリックされた場所にある駒を判定して処理を行う
    void JudgeSelectUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && !evolutionPanel.activeSelf)
        {
            // ヒットしたオブジェクトの情報を取得
            GameObject hitObject = hit.collider.gameObject;

            string objectName = hitObject.name;
            string[] coordinates = objectName.Split(',');
            float x = float.Parse(coordinates[0].Trim('(', ' '));
            float y = float.Parse(coordinates[1].Trim(')', ' '));
            Vector2 vector2 = new Vector2(x+1, y+1);
            Vector2Int vector2Int = new Vector2Int(Mathf.RoundToInt(vector2.x),
                Mathf.RoundToInt(vector2.y));

            selectUnitUnitBehavior = GetUnitObject(vector2Int).GetComponent<UnitBehavior>();

            //選択、選択中の駒が移動できるマスに対して処理を実行
            if (selectUnitUnitBehavior.owingPlayerID == playerID +1 
                || movablePositionList.Contains(vector2Int))
            {
                VisualizeSelectUnit(vector2Int);

                if (selectUnitUnitBehavior.onBoard)
                {
                    GetRangeOfMotionArea(vector2Int);
                    DisplayCursor(range, vector2Int);

                    if(GetUnitObject(selectedPosition)
                        .GetComponent<UnitBehavior>().onBoard)
                    {
                        MoveUnit(vector2Int);
                    }
                    else
                    {
                        SetUnitOnBoard(vector2Int);
                    }
                }
                else if(!selectUnitUnitBehavior.onBoard)
                {
                    GetPointEmptyMass(selectUnitUnitBehavior);
                }

                selectedPosition= vector2Int;   
            }

            if(selectUnitUnitBehavior.owingPlayerID == 0
                && !movablePositionList.Contains(vector2Int)
                && isSelected)
            {
                GetUnitObject(selectedPosition).transform.position -=
                    Mathf.Pow(-1, (playerID)) * selectedUnitLittleFloat;
                isSelected = false;
            }
        }
    }

    //ランダムに手を選択するCPUの作成
    void BotMoveUnit()
    {
        List<Vector2Int> botSelectedUnitVector = new List<Vector2Int>();
        List<Vector2Int> botMovablePositionList = new List<Vector2Int>();
        for (int i = 1; i < 10; i++)
        {
            for (int j = 1; j < 10; j++)
            {
                Vector2Int vec2 = new Vector2Int(i, j);
                int id = GetUnitObject(vec2).GetComponent<UnitBehavior>().owingPlayerID;
                if (id != 0
                    && id - 1 != playerID)
                {
                    GetRangeOfMotionArea(vec2);
                    foreach (var item in range)
                    {
                        print(GetUnitObject(vec2) +" , "+ (item+vec2));
                        botSelectedUnitVector.Add(vec2);
                        botMovablePositionList.Add(item + vec2);
                    }
                }
            }
        }
        int decidedMove = Random.RandomRange(0, botMovablePositionList.Count);
        print(botMovablePositionList.Count+" , " + botMovablePositionList[decidedMove]
            +" , "+ GetUnitObject(botSelectedUnitVector[decidedMove]));

        botSelectedUnitVector.Clear();
        botMovablePositionList.Clear();
        turn = true;
    }

    //選択中の駒を視認できるようにする
    void VisualizeSelectUnit(Vector2Int vector2Int)
    {
        //同じ駒を選択した時（駒を持ち上げたいとき）
        if (selectedPosition == vector2Int && !isSelected)
        {
            GetUnitObject(vector2Int).transform.position += 
                Mathf.Pow(-1, (playerID)) * selectedUnitLittleFloat;

            isSelected = true;
        }
        //同じ駒を選択した時（駒を置きたいとき）
        else if (selectedPosition == vector2Int && isSelected)
        {
            GetUnitObject(vector2Int).transform.position -= 
                Mathf.Pow(-1, (playerID)) * selectedUnitLittleFloat;
            movablePositionList.Clear();
            isSelected = false;
        }
        //異なる駒が選択された時
        else
        {
            GetUnitObject(vector2Int).transform.position += 
                Mathf.Pow(-1, (playerID)) * selectedUnitLittleFloat;

            //前に表示したカーソルを削除して選択した駒を示すカーソルを表示

            if (selectedPosition != Vector2Int.zero && isSelected == true)
            {
                GetUnitObject(selectedPosition).transform.position -= 
                    Mathf.Pow(-1, (playerID)) * selectedUnitLittleFloat;
            }
            isSelected = true;
        }
    }

    //指定のマスの駒の動ける範囲を検索する
   　void GetRangeOfMotionArea(Vector2Int vector2Int)
    {
        GameObject selectedUnit = GetUnitObject(vector2Int);
        UnitBehavior unitBehavior = selectedUnit.GetComponent<UnitBehavior>();

        range.Clear();

        if (!movablePositionList.Contains(vector2Int))
        {
            movablePositionList.Clear();
        }

        //以前に表示したカーソルを削除
        foreach (var item in movablePositionCursors)
        {
            Destroy(item);
        }
        movablePositionCursors.Clear();

        //動きが未登録なら登録
        if (!unitBehavior.registered)
        {
            unitBehavior.RegisteUnitBehavior(selectedUnit.name);
            unitBehavior.registered = true;
        }
        //選択した駒が飛び道具ではない（香、飛車、角、龍、馬以外の時の動ける場所の登録方法）
        if (!unitBehavior.projectile)
        {
            //登録されたベクトルをもとに動ける場所を仮決定
            foreach (var item in unitBehavior.unitMovableDirections)
            {
                range.Add(item);
            }

            //盤外判定
            JudgeBoardOutSide(range, vector2Int);
        }
        //選択した駒が先手の香車の時の登録方法
        else if (unitBehavior.thisUnitName == "FirstTurnKyo(Clone)")
        {
            if (unitBehavior.evolutionable)
            {
                JudgeForward(vector2Int, unitBehavior, range);
            }
            else
            {
                foreach (var item in unitBehavior.unitMovableDirections)
                {
                    range.Add(item);
                }
                JudgeBoardOutSide(range, vector2Int);
            }
        }

        //選択した駒が後手の香車の時の登録方法
        else if (unitBehavior.thisUnitName == "SecondTurnKyo(Clone)")
        {
            if (unitBehavior.evolutionable)
            {
                JudgeBack(vector2Int, unitBehavior, range);
            }
            else
            {
                foreach (var item in unitBehavior.unitMovableDirections)
                {
                    range.Add(item);
                }
                JudgeBoardOutSide(range, vector2Int);
            }
        }

        //選択した駒が飛車の時
        else if (unitBehavior.thisUnitName.Contains("Hisha"))
        {
            JudgeForward(vector2Int, unitBehavior, range);
            JudgeBack(vector2Int, unitBehavior, range);
            JudgeRight(vector2Int, unitBehavior, range);
            JudgeLeft(vector2Int, unitBehavior, range);
            if (!unitBehavior.evolutionable)
            {
                foreach (var item in unitBehavior.unitMovableDirections)
                {
                    range.Add(item);
                }
                JudgeBoardOutSide(range, vector2Int);
            }
        }

        else if (unitBehavior.thisUnitName.Contains("Kaku"))
        {
            JudgeLeftDown(vector2Int, unitBehavior, range);
            JudgeRightDown(vector2Int, unitBehavior, range);
            JudgeRightUp(vector2Int, unitBehavior, range);
            JudgeLeftUp(vector2Int, unitBehavior, range);
            if (!unitBehavior.evolutionable)
            {
                foreach (var item in unitBehavior.unitMovableDirections)
                {
                    range.Add(item);
                }
                JudgeBoardOutSide(range, vector2Int);
            }
        }
    }

    //可動範囲を盤上に表示
    void DisplayCursor(List<Vector2Int> range, Vector2Int vector2Int)
    {
        var unitID = GetUnitObject(vector2Int).GetComponent<UnitBehavior>().owingPlayerID;
        //最後にカーソルを表示、駒を置く場合にはしない
        if (isSelected)
        {
            if(unitID != 0 && unitID-1 != playerID)
            {
                range.Clear();
            }
            foreach (var rangePosition in range)
            {
                movablePositionCursors.Add(Instantiate(cursor,
                    positions[vector2Int[0] - 1 + rangePosition[0]
                    , vector2Int[1] - 1 - rangePosition[1]].transform.position
                    , Quaternion.identity));
                movablePositionList.Add(new Vector2Int(vector2Int[0] + rangePosition[0]
                    , vector2Int[1] - rangePosition[1]));
            }
        }
    }
    //movablePositionListをもとに駒を動かす
    void MoveUnit(Vector2Int vector2Int)
    {
        if(isSelected && movablePositionList.Contains(vector2Int))
        {
            GetUnitObject(selectedPosition).transform.position 
                = positions[vector2Int.x-1,vector2Int.y-1].transform.position;

            var unitObj = UnitArrangement[vector2Int.y - 1, vector2Int.x - 1];
            var unitInt = UnitIDArrangement[vector2Int.y - 1, vector2Int.x - 1];

            UnitArrangement[vector2Int.y - 1, vector2Int.x - 1]
                = UnitArrangement[selectedPosition.y - 1, selectedPosition.x - 1];
            UnitIDArrangement[vector2Int.y - 1, vector2Int.x - 1]
                = UnitIDArrangement[selectedPosition.y - 1, selectedPosition.x - 1];

            if(unitObj.name != "Empty(Clone)")
            {
                UnitArrangement[selectedPosition.y - 1, selectedPosition.x - 1]
                    = units[0];
                UnitIDArrangement[selectedPosition.y - 1, selectedPosition.x - 1]
                    = 0;
            }
            else
            {
                UnitArrangement[selectedPosition.y - 1, selectedPosition.x - 1]
                    = unitObj;
                UnitIDArrangement[selectedPosition.y - 1, selectedPosition.x - 1]
                    = unitInt;
            }

            if (unitObj.name != "Empty(Clone)")
            {
                UnitArrangement[selectedPosition.y - 1, selectedPosition.x - 1]
                    = unitObj;
                TakeUnit(unitObj, unitInt);
            }

            Destroy(selectCursor);
            //動かした駒を示すカーソルを表示
            selectCursor = Instantiate(selectUnitCursor, positions[vector2Int[0] - 1
                    , vector2Int[1] - 1].transform.position
                    , Quaternion.identity) as GameObject;

            movablePositionList.Clear();
            StartCoroutine(EvolutionUnit(vector2Int));
            isSelected = false;
            turn = false;
        }
    }

    //駒を成る
    IEnumerator EvolutionUnit(Vector2Int vector2Int)
    {
        var unitBehavior = GetUnitObject(vector2Int).GetComponent<UnitBehavior>();
        //先手の駒が成る時
        if (playerID== 0)
        {
            if ((!(vector2Int.y <= 3)&& !(selectedPosition.y <= 3))
                || !unitBehavior.evolutionable) yield break;

            if((unitBehavior.hukyo || unitBehavior.kei) && vector2Int.y == 1)
            {
                EvolutionUnitOperate(vector2Int, unitBehavior);
                yield break;
            }
            if(unitBehavior.kei && vector2Int.y == 2)
            {
                EvolutionUnitOperate(vector2Int, unitBehavior);
                yield break;
            }

            evolutionPanel.SetActive(true);
            Vector2 screenPoint = Camera.main.WorldToScreenPoint
                (new Vector2(unitBehavior.transform.position.x
                , unitBehavior.transform.position.y));
            evolutionPanel.transform.position = screenPoint;

            yield return new WaitUntil(() => decideEvolve);
            decideEvolve = false;

            if (!evolveOrNot) yield break;
            EvolutionUnitOperate(vector2Int, unitBehavior);
        }
        //後手の駒が成る時
        if (playerID == 1)
        {
            if ((!(vector2Int.y >= 7) && !(selectedPosition.y >= 7))
                || !unitBehavior.evolutionable) yield break;

            if ((unitBehavior.hukyo || unitBehavior.kei) && vector2Int.y == 9)
            {
                EvolutionUnitOperate(vector2Int, unitBehavior);
                yield break;
            }
            if (unitBehavior.kei && vector2Int.y == 8)
            {
                EvolutionUnitOperate(vector2Int, unitBehavior);
                yield break;
            }

            evolutionPanel.SetActive(true);

            yield return new WaitUntil(() => decideEvolve);
            decideEvolve = false;

            if (!evolveOrNot) yield break;
            EvolutionUnitOperate(vector2Int, unitBehavior);
        }
    }

    void EvolutionUnitOperate(Vector2Int vector2Int,UnitBehavior unitBehavior)
    {
        unitBehavior.ChangeMoveblePositionByEvolution(playerID);
        var unitID = GetUnitID(vector2Int);
        //見た目の変更
        if (!unitBehavior.thisUnitName.Contains("Hisha") && !unitBehavior.thisUnitName.Contains("Kaku"))
        {
            GetUnitObject(vector2Int).GetComponent<SpriteRenderer>().sprite
            = units[unitID + 8].GetComponent<SpriteRenderer>().sprite;
        }
        else if (unitBehavior.thisUnitName.Contains("Hisha") || unitBehavior.thisUnitName.Contains("Kaku"))
        {
            GetUnitObject(vector2Int).GetComponent<SpriteRenderer>().sprite
            = units[unitID + 7].GetComponent<SpriteRenderer>().sprite;
        }
    }

    //駒を取る
    void TakeUnit(GameObject unitObj, int unitInt)
    {
        print(unitObj);
        UnitArrangement[selectedPosition.y-1,selectedPosition.x - 1]
            = Instantiate(units[0]
            , unitObj.transform.position, Quaternion.identity) as GameObject;

        UnitArrangement[selectedPosition.y - 1, selectedPosition.x - 1].AddComponent<UnitBehavior>();
        Destroy(unitObj);

        int newUnitID = unitInt + Mathf.FloorToInt(Mathf.Pow(-1, playerID + 1) * 14);

        //成駒を取ったときの処理、‐8することで元の駒が取得できる
        //馬・龍の時はこれにさらに1を加えて調整（金が成れないためインデックスがずれている）
        if (units[newUnitID].name.Contains("Nari"))
        {
            newUnitID -= 8;
            if (units[newUnitID].name.Contains("Kaku")
            || units[newUnitID].name.Contains("Kin"))
            {
                newUnitID += 1;
            }
        }

        //王様を取ったときの処理
        if (units[newUnitID].name.Contains("Ou"))
        {
            isGame = false;
            if(unitObj.GetComponent<UnitBehavior>().owingPlayerID - 1 != playerID)
            {
                WinGame();
            }
            else if(unitObj.GetComponent<UnitBehavior>().owingPlayerID - 1 == playerID)
            {
                LoseGame();
            }
            StartCoroutine(finishGame());
            return;
        }


        if(playerID == 0)
        {
            if (firstPlayerPossesionUnitAmount[newUnitID-1] == 0)
            {
                firstPlayerPossesionUnit[newUnitID - 1].SetActive(true);
                firstPlayerPossesionUnit[newUnitID - 1].GetComponent<UnitBehavior>().active = true;
            }
            firstPlayerPossesionUnitAmount[newUnitID - 1] += 1;
            var text = firstPlayerPossesionUnitAmount[newUnitID - 1];
            firstPlayerPossesionUnitAmoutText[newUnitID-1].text = text.ToString();
            if (firstPlayerPossesionUnitAmount[newUnitID-1] == 1)
            {
                firstPlayerPossesionUnitAmoutText[newUnitID-1].text = "";
            }
        }
        if(playerID == 1)
        {
            if (secondPlayerPossesionUnitAmount[6-(newUnitID - 15)] == 0)
            {
                secondPlayerPossesionUnit[6 - (newUnitID - 15)].SetActive(true);
                secondPlayerPossesionUnit[6 - (newUnitID - 15)].GetComponent<UnitBehavior>().active = true;
            }
            secondPlayerPossesionUnitAmount[6 - (newUnitID - 15)] += 1;
            var text = secondPlayerPossesionUnitAmount[6-(newUnitID - 15)];
            secondPlayerPossesionUnitAmoutText[6 - (newUnitID - 15)].text = text.ToString();
            if (secondPlayerPossesionUnitAmount[6 - (newUnitID - 15)] == 1)
            {
                secondPlayerPossesionUnitAmoutText[6 - (newUnitID - 15)].text = "";
            }
        }
    }

    //持ち駒を打てるマスの取得、表示
    void GetPointEmptyMass(UnitBehavior selectUnitUnitBehavior)
    {
        if (selectUnitUnitBehavior.active)
        {
            foreach (var item in movablePositionCursors)
            {
                Destroy(item);
            }

            movablePositionList.Clear();

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (UnitArrangement[j, i].name == "Empty(Clone)")
                    {
                        movablePositionList.Add(new Vector2Int(i, j) + Vector2Int.one);
                    }
                }
            }
            if (isSelected)
            {
                foreach (var item in movablePositionList)
                {
                    movablePositionCursors.Add(Instantiate(cursor,
                            positions[item.x-1, item.y-1].transform.position
                            , Quaternion.identity) as GameObject);
                }
            }
        }
    }

    //持ち駒を打つ
    void SetUnitOnBoard(Vector2Int vector2Int)
    {
        if (isSelected && movablePositionList.Contains(vector2Int))
        {
            var setUnit = Instantiate(units[GetUnitID(selectedPosition)]
                , positions[vector2Int.x-1, vector2Int.y-1].transform.position
                , Quaternion.identity) as GameObject;
            var addUnitBehavior = setUnit.AddComponent<UnitBehavior>();
            addUnitBehavior.RegisterUnitInfo(GetUnitID(selectedPosition));

            //駒を打った後の盤面を一時的に作って王手判定、王手が掛かっていたら駒は打てない
            UnitArrangement[vector2Int.y - 1, vector2Int.x - 1]
             = setUnit;
            UnitIDArrangement[vector2Int.y - 1, vector2Int.x - 1]
                = GetUnitID(selectedPosition);
            UnitArrangement[vector2Int.y - 1, vector2Int.x - 1]
             = units[0];
            UnitIDArrangement[vector2Int.y - 1, vector2Int.x - 1]
                = 0;

            if (playerID == 0)
            {
                firstPlayerPossesionUnitAmount[GetUnitID(selectedPosition) - 1] -= 1;
                var text = firstPlayerPossesionUnitAmount[GetUnitID(selectedPosition) - 1];
                firstPlayerPossesionUnitAmoutText[GetUnitID(selectedPosition) - 1].text = text.ToString();
                if (firstPlayerPossesionUnitAmount[GetUnitID(selectedPosition) - 1] == 0)
                {
                    firstPlayerPossesionUnit[GetUnitID(selectedPosition) - 1].GetComponent<UnitBehavior>().active = false;
                    firstPlayerPossesionUnit[GetUnitID(selectedPosition) - 1].SetActive(false);
                    firstPlayerPossesionUnitAmoutText[GetUnitID(selectedPosition) - 1].text = "";
                }
                if (firstPlayerPossesionUnitAmount[GetUnitID(selectedPosition) - 1] == 1)
                {
                    firstPlayerPossesionUnitAmoutText[GetUnitID(selectedPosition) - 1].text = "";
                }
            }
            else
            {
                secondPlayerPossesionUnitAmount[GetUnitID(selectedPosition) - 15] -= 1;
                if (secondPlayerPossesionUnitAmount[GetUnitID(selectedPosition) - 15] == 0)
                {
                    secondPlayerPossesionUnit[GetUnitID(selectedPosition) - 15].GetComponent<UnitBehavior>().active = false;
                    secondPlayerPossesionUnit[GetUnitID(selectedPosition) - 15].SetActive(false);
                    secondPlayerPossesionUnitAmoutText[GetUnitID(selectedPosition) - 15].text = "";
                }
                if (secondPlayerPossesionUnitAmount[GetUnitID(selectedPosition) - 15] == 1)
                {
                    secondPlayerPossesionUnitAmoutText[GetUnitID(selectedPosition) - 15].text = "";
                }
            }


            UnitArrangement[vector2Int.y -1, vector2Int.x -1]
             = setUnit;
            UnitIDArrangement[vector2Int.y -1, vector2Int.x - 1]
                = GetUnitID(selectedPosition);

            Destroy(selectCursor);
            selectCursor = Instantiate(selectUnitCursor, positions[vector2Int[0] - 1
                    , vector2Int[1] - 1].transform.position
                    , Quaternion.identity) as GameObject;

            movablePositionList.Clear();
            isSelected= false;
        }
    }

    void WinGame()
    {
        winUI.SetActive(true);
    }
    void LoseGame()
    {
        loseUI.SetActive(true);
    }

    IEnumerator finishGame()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("TitleScene");
    }

    //盤外判定
    void JudgeBoardOutSide(List<Vector2Int> range, Vector2Int vector2Int)
    {
        //動ける場所から盤の外の部分を削除
        for (int i = range.Count - 1; i >= 0; i--)
        {
            //判定するマス
            var item = vector2Int - Vector2Int.one + new Vector2Int(1, -1) * range[i];

            //盤の外を判定
            if (item[0] > 8 || item[1] > 8 ||
                item[0] < 0 || item[1] < 0)
            {
                range.RemoveAt(i);
                continue;
            }
            if (GetUnitObject(item + Vector2Int.one).GetComponent<UnitBehavior>().owingPlayerID
                == selectUnitUnitBehavior.owingPlayerID)
            {
                range.RemoveAt(i);
                continue;
            }

            if (GetUnitObject(item + Vector2Int.one).GetComponent<UnitBehavior>().owingPlayerID != 0)
            {
                if (GetUnitObject(item + Vector2Int.one).GetComponent<UnitBehavior>().owingPlayerID
                != selectUnitUnitBehavior.owingPlayerID) continue;
            }
        }
    }

    //香車や飛車などの前方方向の判定（後手の場合は後進方向）
    void JudgeForward(Vector2Int vector2Int,UnitBehavior unitBehavior, List<Vector2Int> range)
    {
        var hit = false;
        for (int i = 1; i < 9; i++)
        {
            var movableVector = vector2Int + new Vector2Int(0, -i);
            if (movableVector[0] > 9 || movableVector[1] > 9 ||
            movableVector[0] < 1 || movableVector[1] < 1)
            {
                foreach (var vector in unitBehavior.FirstTurnLanceBehavior(vector2Int, movableVector - Vector2Int.right))
                {
                    range.Add(vector);
                }
                hit = true;
                break;
            }
            if (GetUnitID(movableVector) != 0)
            {
                foreach (var vector in unitBehavior.FirstTurnLanceBehavior(vector2Int, movableVector))
                {
                    range.Add(vector);
                }
                AddEnemyUnitMass(movableVector,vector2Int, range);
                hit = true;
                break;
            }
        }

        if (!hit)
        {
            print("no hit");
            //途中で駒に当たらない場合
            foreach (var item in unitBehavior.unitMovableDirections)
            {
                range.Add(item);
            }

            JudgeBoardOutSide(range, vector2Int);
        }
    }

    //香車や飛車などの後方方向の判定（後手の場合は前方方向）
    void JudgeBack(Vector2Int vector2Int,UnitBehavior unitBehavior,List<Vector2Int> range)
    {
        var hit = false;
        for (int i = 1; i < 9; i++)
        {
            var movableVector = vector2Int + new Vector2Int(0, i);
            if (movableVector[0] > 9 || movableVector[1] > 9 ||
            movableVector[0] < 1 || movableVector[1] < 1)
            {
                foreach (var vector in unitBehavior.SecondTurnLanceBehavior(vector2Int, movableVector - Vector2Int.right))
                {
                    range.Add(vector);
                }
                hit = true;
                break;
            }
            if (GetUnitID(movableVector) != 0)
            {
                foreach (var vector in unitBehavior.SecondTurnLanceBehavior(vector2Int, movableVector))
                {
                    range.Add(vector);
                }
                AddEnemyUnitMass(movableVector, vector2Int, range);
                hit = true;
                break;
            }
        }
        if (!hit)
        {
            //途中で駒に当たらない場合
            foreach (var item in unitBehavior.unitMovableDirections)
            {
                range.Add(item);
            }
            JudgeBoardOutSide(range, vector2Int);
        }
    }

    void JudgeRight(Vector2Int vector2Int, UnitBehavior unitBehavior, List<Vector2Int> range)
    {
        var hit = false;
        for (int i = 1; i < 9; i++)
        {
            var movableVector = vector2Int + new Vector2Int(-i, 0);
            if (movableVector[0] > 9 || movableVector[1] > 9 ||
            movableVector[0] < 1 || movableVector[1] < 1)
            {
                foreach (var vector in unitBehavior.GetRightDirection(vector2Int, movableVector - Vector2Int.up))
                {
                    range.Add(vector);
                }
                hit = true;
                break;
            }
            if (GetUnitID(movableVector) != 0)
            {
                foreach (var vector in unitBehavior.GetRightDirection(vector2Int, movableVector))
                {
                    range.Add(vector);
                }
                AddEnemyUnitMass(movableVector, vector2Int, range);
                hit = true;
                break;
            }
        }
        if (!hit)
        {
            //途中で駒に当たらない場合
            foreach (var item in unitBehavior.unitMovableDirections)
            {
                range.Add(item);
            }
            JudgeBoardOutSide(range, vector2Int);
        }
    }

    void JudgeLeft(Vector2Int vector2Int, UnitBehavior unitBehavior, List<Vector2Int> range)
    {
        var hit = false;
        for (int i = 1; i < 9; i++)
        {
            var movableVector = vector2Int + new Vector2Int(i, 0);
            if (movableVector[0] > 9 || movableVector[1] > 9 ||
            movableVector[0] < 1 || movableVector[1] < 1)
            {
                foreach (var vector in unitBehavior.GetLeftDirection(vector2Int, movableVector - Vector2Int.up))
                {
                    range.Add(vector);
                }
                hit = true;
                break;
            }
            if (GetUnitID(movableVector) != 0)
            {
                foreach (var vector in unitBehavior.GetLeftDirection(vector2Int, movableVector))
                {
                    range.Add(vector);
                }
                AddEnemyUnitMass(movableVector, vector2Int, range);
                hit = true;
                break;
            }
        }
        if (!hit)
        {
            //途中で駒に当たらない場合
            foreach (var item in unitBehavior.unitMovableDirections)
            {
                range.Add(item);
            }
            JudgeBoardOutSide(range, vector2Int);
        }
    }

    void JudgeLeftDown(Vector2Int vector2Int, UnitBehavior unitBehavior, List<Vector2Int> range)
    {
        var hit = false;
        for (int i = 1; i < 9; i++)
        {
            var movableVector = vector2Int + new Vector2Int(i, i);
            if (movableVector[0] > 9 || movableVector[1] > 9 ||
            movableVector[0] < 1 || movableVector[1] < 1)
            {
                foreach (var vector in unitBehavior.GetLeftDownDirection(vector2Int, movableVector - Vector2Int.one))
                {
                    range.Add(vector);
                }
                hit = true;
                break;
            }
            if (GetUnitID(movableVector) != 0)
            {
                foreach (var vector in unitBehavior.GetLeftDownDirection(vector2Int, movableVector))
                {
                    range.Add(vector);
                }
                AddEnemyUnitMass(movableVector, vector2Int, range);
                hit = true;
                break;
            }
        }
        if (!hit)
        {
            //途中で駒に当たらない場合
            foreach (var item in unitBehavior.unitMovableDirections)
            {
                range.Add(item);
            }
            JudgeBoardOutSide(range, vector2Int);
        }
    }

    void JudgeRightDown(Vector2Int vector2Int, UnitBehavior unitBehavior, List<Vector2Int> range)
    {
        var hit = false;
        for (int i = 1; i < 9; i++)
        {
            var movableVector = vector2Int + new Vector2Int(-i, i);
            if (movableVector[0] > 9 || movableVector[1] > 9 ||
            movableVector[0] < 1 || movableVector[1] < 1)
            {
                foreach (var vector in unitBehavior.GetRightDownDirection(vector2Int, movableVector))
                {
                    range.Add(vector);
                }
                hit = true;
                break;
            }
            if (GetUnitID(movableVector) != 0)
            {
                foreach (var vector in unitBehavior.GetRightDownDirection(vector2Int, movableVector))
                {
                    range.Add(vector);
                }
                AddEnemyUnitMass(movableVector, vector2Int, range);
                hit = true;
                break;
            }
        }
        if (!hit)
        {
            //途中で駒に当たらない場合
            foreach (var item in unitBehavior.unitMovableDirections)
            {
                range.Add(item);
            }
            JudgeBoardOutSide(range, vector2Int);
        }
    }

    void JudgeRightUp(Vector2Int vector2Int, UnitBehavior unitBehavior, List<Vector2Int> range)
    {
        var hit = false;
        for (int i = 1; i < 9; i++)
        {
            var movableVector = vector2Int + new Vector2Int(-i, -i);
            if (movableVector[0] > 9 || movableVector[1] > 9 ||
            movableVector[0] < 1 || movableVector[1] < 1)
            {
                foreach (var vector in unitBehavior.GetRightUpDirection(vector2Int, movableVector))
                {
                    range.Add(vector);
                }
                hit = true;
                break;
            }
            if (GetUnitID(movableVector) != 0)
            {
                foreach (var vector in unitBehavior.GetRightUpDirection(vector2Int, movableVector))
                {
                    range.Add(vector);
                }
                AddEnemyUnitMass(movableVector, vector2Int,range);
                hit = true;
                break;
            }
        }
        if (!hit)
        {
            //途中で駒に当たらない場合
            foreach (var item in unitBehavior.unitMovableDirections)
            {
                range.Add(item);
            }
            JudgeBoardOutSide(range, vector2Int);
        }
    }

    void JudgeLeftUp(Vector2Int vector2Int, UnitBehavior unitBehavior, List<Vector2Int> range)
    {
        var hit = false;
        for (int i = 1; i < 9; i++)
        {
            var movableVector = vector2Int + new Vector2Int(i, -i);
            if (movableVector[0] > 9 || movableVector[1] > 9 ||
            movableVector[0] < 1 || movableVector[1] < 1)
            {
                foreach (var vector in unitBehavior.GetLeftUpDirection(vector2Int, movableVector))
                {
                    range.Add(vector);
                }
                hit = true;
                break;
            }
            if (GetUnitID(movableVector) != 0)
            {
                foreach (var vector in unitBehavior.GetLeftUpDirection(vector2Int, movableVector))
                {
                    range.Add(vector);
                }
                AddEnemyUnitMass(movableVector, vector2Int,range);
                hit = true;
                break;
            }
        }
        if (!hit)
        {
            //途中で駒に当たらない場合
            foreach (var item in unitBehavior.unitMovableDirections)
            {
                range.Add(item);
            }
            JudgeBoardOutSide(range, vector2Int);
        }
    }

    //駒の利きに他の駒があり、その駒が相手の駒だった時（そこに動かしたら駒が取れるマス）
    void AddEnemyUnitMass(Vector2Int movableVector,Vector2Int vector2Int,List<Vector2Int> range)
    {
        if (GetUnitID(movableVector)/ 14 != playerID)
        {
            range.Add(new Vector2Int(1, -1) * (movableVector - vector2Int));
        }
    }

    GameObject GetUnitObject(Vector2Int boardPositionID)
    {
        int positionx = boardPositionID.x - 1;
        int positiony = boardPositionID.y - 1;

        return UnitArrangement[positiony, positionx];
    }

    //指定した升目に入っている駒のIDを返す
    int GetUnitID(Vector2Int boardPositionID)
    {
        int positionx = boardPositionID.x - 1;
        int positiony = boardPositionID.y - 1;

        return UnitIDArrangement[positiony, positionx];
    }

    public void DecideEvolveOrNot(bool agreeEvolve)
    {
        evolveOrNot = agreeEvolve;
        evolutionPanel.SetActive(false);
        decideEvolve = true;
    }
}
