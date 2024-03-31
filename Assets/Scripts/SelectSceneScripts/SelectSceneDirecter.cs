using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class SelectSceneDirecter : MonoBehaviour
{
    //�`�P�b�g�̖���
    static int ticketAmount = 30;
    //�����`�P�b�g�̖���
    int comsumptionticket;
    //���₷���z��Ƃ��ċL�^
    public static int[] addUnitsAmount = new int[7];
    //��̃R�X�g���L��
    int[] unitscost = { 1, 7, 8, 3, 4, 5, 6 };
    //��𑝂₷�����炷����؂�ւ���
    bool AddorReduce = true;
    //�`�P�b�g�̎c�薇����\������e�L�X�g
    [SerializeField] Text TicketAmounttext;
    //�e�R�}�̑��₷����
    [SerializeField] Text amountaddHu;
    [SerializeField] Text amountaddKei;
    [SerializeField] Text amountaddKyo;
    [SerializeField] Text amountaddGin;
    [SerializeField] Text amountaddKin;
    [SerializeField] Text amountaddKaku;
    [SerializeField] Text amountaddHisha;
    //��������
    public static int gameTimeId = 0;
    //�������ԑI���{�^�����i�[���郊�X�g
    public List<GameObject> gameRuleButton = new List<GameObject>();
    //�I�������������Ԃ������p�l��
    public GameObject showSelectGameRulePanel;
    // Start is called before the first frame update
    void Start()
    {
        //�z��̏�����
        for(int i = 0; i< addUnitsAmount.Length; i++)
        {
            addUnitsAmount[i] = 0;
        }
        //���ꂼ��̋�̑���������\��
        for(int i = 0; i < 7; i++)
        {
            updateaddunitamount(i);
        }
        //����`�P�b�g�����̏�����
        displayTicketAmount(0, ticketAmount);
    }

    public void OnClickAddUnits(int unitID)
    {
        print("Clicked�I");
        //�`�P�b�g������Ȃ��Ƃ�
        if (ticketAmount - unitscost[unitID] < 0 && AddorReduce)
        {
            print("�`�P�b�g������܂���I");
            return;
        }

        addUnitsAmount[unitID] += 1;
        ticketAmount -= unitscost[unitID];
        comsumptionticket += unitscost[unitID];

        displayTicketAmount(comsumptionticket,ticketAmount);
        updateaddunitamount(unitID);
    }

    public void OnClickReduceUnits(int unitID)
    {
        if (addUnitsAmount[unitID] <= 0) return;
        addUnitsAmount[unitID] -= 1;
        ticketAmount += unitscost[unitID];
        comsumptionticket -= unitscost[unitID];

        displayTicketAmount(comsumptionticket, ticketAmount);
        updateaddunitamount(unitID);
    }

    public void OnClickStartButton()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OnClickReturnButton()
    {
        SceneManager.LoadScene("TitleScene");
    }

    void displayTicketAmount(int useticket,int reminingticket)
    {
        TicketAmounttext.text =useticket.ToString() +"�� / "+ reminingticket.ToString();
    }

    void updateaddunitamount(int unitID)
    {
        var kindofaddunitamount = "�~" + addUnitsAmount[unitID].ToString();
        switch (unitID)
        {
            case 0:
                amountaddHu.text = kindofaddunitamount;
                break;
            case 1:
                amountaddKaku.text = kindofaddunitamount;
                break;
            case 2:
                amountaddHisha.text = kindofaddunitamount;
                break;
            case 3:
                amountaddKyo.text = kindofaddunitamount;
                break;
            case 4:
                amountaddKei.text = kindofaddunitamount;
                break;
            case 5:
                amountaddGin.text = kindofaddunitamount;
                break;
            case 6:
                amountaddKin.text = kindofaddunitamount;
                break;
        }
    }

    public void OnClickTimeButton(int gamerule)
    {
        gameTimeId = gamerule;
        showSelectGameRulePanel.transform.position = gameRuleButton[gamerule].transform.position;
        print(gameTimeId);
    }
}
