using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleSceneDirector : MonoBehaviour
{

    //マイページ
    [SerializeField] GameObject MypagePanel;
    [SerializeField] Text NumberOfWinInfo;
    static int amountofticket = 20;
    static List<int> usetickets = new List<int>();

    // Start is called before the first frame update
    void Start()
    {
        MypagePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickPvP()
    {
        SceneManager.LoadScene("SelectScene");
    }

    public void OnClickPvE()
    {
        SceneManager.LoadScene("SelectScene");
    }

    public void OnClickEvE()
    {
        SceneManager.LoadScene("SelectScene");
    }

    public void OnClickMypage()
    {
        MypagePanel.SetActive(true);
        NumberOfWinInfo.text = ResultCounter.numberofwin + "勝" + ResultCounter.numberoflose + "敗" + ResultCounter.numberofdraw + "分";
    }

    public void OnClickMaypageClose()
    {
        MypagePanel.SetActive(false);
        SceneManager.LoadScene("TitleScene");
    }
}
