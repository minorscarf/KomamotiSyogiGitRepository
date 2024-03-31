using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MatchScene : MonoBehaviour
{
    //�΋ǔ���A�΋ǂ��p�������ǂ���
    bool isGame = true;

    //�����A�s�kUI
    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;

    //�Ղ̉摜�i��肩�猩���Ֆʂƌ�肩�猩���Ֆʂ̉摜���Q��ށj
    [SerializeField] private GameObject firstTurnBoard;
    [SerializeField] private GameObject secondTurnBoard;

    //��̉摜
    [SerializeField] private List<GameObject> units = new List<GameObject>();
    //�ړ��͈͂�\��
    [SerializeField] private GameObject cursor;
    //�I�����Ă����ɑ΂��ĕ\��
    [SerializeField] private GameObject selectUnitCursor;
    private GameObject selectCursor;

    //��𐬂邩�ǂ����I������UI
    [SerializeField] private GameObject evolutionPanel;
    private bool evolveOrNot = true;
    private bool decideEvolve = false;

    //������̈ʒu
    [SerializeField] private List<GameObject> firstTurnPlayerTakeUnitPosition = new List<GameObject>();
    [SerializeField] private List<GameObject> secondTurnPlayerTakeUnitPosition = new List<GameObject>();

    //���ڂ̔z�u
    [SerializeField] private GameObject unitPosition;
    private GameObject[,] positions = new GameObject[9,9];
    private Vector2 unitInterval = new Vector2(31.7f * 5f / (34.8f * 9f), 5f / 9f);
    private Vector2 unit11Position = new Vector2(34.8f * 2.5f / 31.7f, 2.5f);
    private Vector2 adjustmentVector = new Vector2(0.4671805f, 0);

    //��̏����z�u�x�N�g���A10�s��11�s�ڂ͐��ƌ��̎�����̏��
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

    //���̎�����̃I�u�W�F�N�g���i�[����z��
    private GameObject[] firstPlayerPossesionUnit =
        {null,null,null,null,null,null,null};
    //���̎�����̂��ꂼ��̖������i�[����z��
    private int[] firstPlayerPossesionUnitAmount =
        {0,0,0,0,0,0,0};
    //���̎�����̂��ꂼ��̖�����\������e�L�X�g���i�[����z��
    [SerializeField] private Text[] firstPlayerPossesionUnitAmoutText =
        {};
    //���̎�����̃I�u�W�F�N�g���i�[����z��
    private GameObject[] secondPlayerPossesionUnit =
        {null, null, null, null, null, null, null};
    //���̎�����̂��ꂼ��̖������i�[����z��
    private int[] secondPlayerPossesionUnitAmount =
        {0,0,0,0,0,0,0};
    //���̎�����̂��ꂼ��̖�����\������e�L�X�g���i�[����z��
    [SerializeField]private Text[] secondPlayerPossesionUnitAmoutText =
        {};

    //�������ʒu���ꎞ�I�Ɋi�[���郊�X�g
    private List<Vector2Int> range = new List<Vector2Int>();

    //���݂̋�̔z�u�x�N�g��
    private int[,] UnitIDArrangement;

    //���z��Ɋi�[���ĊǗ��A�Ăяo�����s����悤�ɂ���
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

    //�I���������UnitBehavior���ꎞ�I�Ɋi�[����
    private UnitBehavior selectUnitUnitBehavior;
    //�I��������i�߂鏡�ڂ̃��X�g
    private List<Vector2Int> movablePositionList = new List<Vector2Int>();
    //�\���������ڂ��ꎞ�I�Ɋi�[���郊�X�g
    private List<GameObject> movablePositionCursors = new List<GameObject>();
    //�I�𒆂̏��ڂ��ꎞ�I�Ɋi�[����ϐ�
    private Vector2Int selectedPosition = new Vector2Int(1,1);
    //��I������Ă��邩�ǂ������肷��
    private bool isSelected = false;
    //��I�����ꂽ���ɂ�����Ə�ɓ�����
    private Vector3 selectedUnitLittleFloat = new Vector3(-0.1f, 0.11f, 0);

    //�v���C���[ID�i����0�A���Ȃ�P�j
    private int playerID = 0;

    //�����̎�Ԃ��ǂ���
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

        //���̏������A���ɑS��ނ̋��u���Ĕ�\���ɂ��Ă���
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

    //�N���b�N���ꂽ�ꏊ�ɂ����𔻒肵�ď������s��
    void JudgeSelectUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && !evolutionPanel.activeSelf)
        {
            // �q�b�g�����I�u�W�F�N�g�̏����擾
            GameObject hitObject = hit.collider.gameObject;

            string objectName = hitObject.name;
            string[] coordinates = objectName.Split(',');
            float x = float.Parse(coordinates[0].Trim('(', ' '));
            float y = float.Parse(coordinates[1].Trim(')', ' '));
            Vector2 vector2 = new Vector2(x+1, y+1);
            Vector2Int vector2Int = new Vector2Int(Mathf.RoundToInt(vector2.x),
                Mathf.RoundToInt(vector2.y));

            selectUnitUnitBehavior = GetUnitObject(vector2Int).GetComponent<UnitBehavior>();

            //�I���A�I�𒆂̋�ړ��ł���}�X�ɑ΂��ď��������s
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

    //�����_���Ɏ��I������CPU�̍쐬
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

    //�I�𒆂̋�����F�ł���悤�ɂ���
    void VisualizeSelectUnit(Vector2Int vector2Int)
    {
        //�������I���������i��������グ�����Ƃ��j
        if (selectedPosition == vector2Int && !isSelected)
        {
            GetUnitObject(vector2Int).transform.position += 
                Mathf.Pow(-1, (playerID)) * selectedUnitLittleFloat;

            isSelected = true;
        }
        //�������I���������i���u�������Ƃ��j
        else if (selectedPosition == vector2Int && isSelected)
        {
            GetUnitObject(vector2Int).transform.position -= 
                Mathf.Pow(-1, (playerID)) * selectedUnitLittleFloat;
            movablePositionList.Clear();
            isSelected = false;
        }
        //�قȂ��I�����ꂽ��
        else
        {
            GetUnitObject(vector2Int).transform.position += 
                Mathf.Pow(-1, (playerID)) * selectedUnitLittleFloat;

            //�O�ɕ\�������J�[�\�����폜���đI��������������J�[�\����\��

            if (selectedPosition != Vector2Int.zero && isSelected == true)
            {
                GetUnitObject(selectedPosition).transform.position -= 
                    Mathf.Pow(-1, (playerID)) * selectedUnitLittleFloat;
            }
            isSelected = true;
        }
    }

    //�w��̃}�X�̋�̓�����͈͂���������
   �@void GetRangeOfMotionArea(Vector2Int vector2Int)
    {
        GameObject selectedUnit = GetUnitObject(vector2Int);
        UnitBehavior unitBehavior = selectedUnit.GetComponent<UnitBehavior>();

        range.Clear();

        if (!movablePositionList.Contains(vector2Int))
        {
            movablePositionList.Clear();
        }

        //�ȑO�ɕ\�������J�[�\�����폜
        foreach (var item in movablePositionCursors)
        {
            Destroy(item);
        }
        movablePositionCursors.Clear();

        //���������o�^�Ȃ�o�^
        if (!unitBehavior.registered)
        {
            unitBehavior.RegisteUnitBehavior(selectedUnit.name);
            unitBehavior.registered = true;
        }
        //�I���������ѓ���ł͂Ȃ��i���A��ԁA�p�A���A�n�ȊO�̎��̓�����ꏊ�̓o�^���@�j
        if (!unitBehavior.projectile)
        {
            //�o�^���ꂽ�x�N�g�������Ƃɓ�����ꏊ��������
            foreach (var item in unitBehavior.unitMovableDirections)
            {
                range.Add(item);
            }

            //�ՊO����
            JudgeBoardOutSide(range, vector2Int);
        }
        //�I����������̍��Ԃ̎��̓o�^���@
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

        //�I����������̍��Ԃ̎��̓o�^���@
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

        //�I���������Ԃ̎�
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

    //���͈͂�Տ�ɕ\��
    void DisplayCursor(List<Vector2Int> range, Vector2Int vector2Int)
    {
        var unitID = GetUnitObject(vector2Int).GetComponent<UnitBehavior>().owingPlayerID;
        //�Ō�ɃJ�[�\����\���A���u���ꍇ�ɂ͂��Ȃ�
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
    //movablePositionList�����Ƃɋ�𓮂���
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
            //����������������J�[�\����\��
            selectCursor = Instantiate(selectUnitCursor, positions[vector2Int[0] - 1
                    , vector2Int[1] - 1].transform.position
                    , Quaternion.identity) as GameObject;

            movablePositionList.Clear();
            StartCoroutine(EvolutionUnit(vector2Int));
            isSelected = false;
            turn = false;
        }
    }

    //��𐬂�
    IEnumerator EvolutionUnit(Vector2Int vector2Int)
    {
        var unitBehavior = GetUnitObject(vector2Int).GetComponent<UnitBehavior>();
        //���̋���鎞
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
        //���̋���鎞
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
        //�����ڂ̕ύX
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

    //������
    void TakeUnit(GameObject unitObj, int unitInt)
    {
        print(unitObj);
        UnitArrangement[selectedPosition.y-1,selectedPosition.x - 1]
            = Instantiate(units[0]
            , unitObj.transform.position, Quaternion.identity) as GameObject;

        UnitArrangement[selectedPosition.y - 1, selectedPosition.x - 1].AddComponent<UnitBehavior>();
        Destroy(unitObj);

        int newUnitID = unitInt + Mathf.FloorToInt(Mathf.Pow(-1, playerID + 1) * 14);

        //�����������Ƃ��̏����A�]8���邱�ƂŌ��̋�擾�ł���
        //�n�E���̎��͂���ɂ����1�������Ē����i��������Ȃ����߃C���f�b�N�X������Ă���j
        if (units[newUnitID].name.Contains("Nari"))
        {
            newUnitID -= 8;
            if (units[newUnitID].name.Contains("Kaku")
            || units[newUnitID].name.Contains("Kin"))
            {
                newUnitID += 1;
            }
        }

        //���l��������Ƃ��̏���
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

    //�������łĂ�}�X�̎擾�A�\��
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

    //�������ł�
    void SetUnitOnBoard(Vector2Int vector2Int)
    {
        if (isSelected && movablePositionList.Contains(vector2Int))
        {
            var setUnit = Instantiate(units[GetUnitID(selectedPosition)]
                , positions[vector2Int.x-1, vector2Int.y-1].transform.position
                , Quaternion.identity) as GameObject;
            var addUnitBehavior = setUnit.AddComponent<UnitBehavior>();
            addUnitBehavior.RegisterUnitInfo(GetUnitID(selectedPosition));

            //���ł�����̔Ֆʂ��ꎞ�I�ɍ���ĉ��蔻��A���肪�|�����Ă������͑łĂȂ�
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

    //�ՊO����
    void JudgeBoardOutSide(List<Vector2Int> range, Vector2Int vector2Int)
    {
        //������ꏊ����Ղ̊O�̕������폜
        for (int i = range.Count - 1; i >= 0; i--)
        {
            //���肷��}�X
            var item = vector2Int - Vector2Int.one + new Vector2Int(1, -1) * range[i];

            //�Ղ̊O�𔻒�
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

    //���Ԃ��ԂȂǂ̑O�������̔���i���̏ꍇ�͌�i�����j
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
            //�r���ŋ�ɓ�����Ȃ��ꍇ
            foreach (var item in unitBehavior.unitMovableDirections)
            {
                range.Add(item);
            }

            JudgeBoardOutSide(range, vector2Int);
        }
    }

    //���Ԃ��ԂȂǂ̌�������̔���i���̏ꍇ�͑O�������j
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
            //�r���ŋ�ɓ�����Ȃ��ꍇ
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
            //�r���ŋ�ɓ�����Ȃ��ꍇ
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
            //�r���ŋ�ɓ�����Ȃ��ꍇ
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
            //�r���ŋ�ɓ�����Ȃ��ꍇ
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
            //�r���ŋ�ɓ�����Ȃ��ꍇ
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
            //�r���ŋ�ɓ�����Ȃ��ꍇ
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
            //�r���ŋ�ɓ�����Ȃ��ꍇ
            foreach (var item in unitBehavior.unitMovableDirections)
            {
                range.Add(item);
            }
            JudgeBoardOutSide(range, vector2Int);
        }
    }

    //��̗����ɑ��̋����A���̋����̋�������i�����ɓ�������������}�X�j
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

    //�w�肵�����ڂɓ����Ă�����ID��Ԃ�
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
