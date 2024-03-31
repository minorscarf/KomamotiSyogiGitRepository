using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    public PhotonManager instance;
    //�΋Ǒ҂��̎��ɕ\������p�l��
    public GameObject waitPanel;
    //�΋Ǒ҂����ɕ\������e�L�X�g
    public Text waitText;
    //�`�P�b�g�I��UI 
    public GameObject selectTicketPart;
    //�������ԑI���{�^��
    public GameObject selectGameRule;
    //�΋ǊJ�n�{�^��
    public GameObject startGameButton;
    //�^�C�g����ʂɖ߂�{�^��
    public GameObject returnTitleSceneButton;
    //��I�����[�h�ɖ߂�{�^��
    public GameObject returnSelectModeButton;
    //�҂�����
    private float waitTime = 5f;
    //���[���}�X�^�[�Ƃ��ĕ������쐬���郂�[�h���ǂ���
    private bool isRoomMaster = false;
    //�����ɃA�N�Z�X���邽�߂̃L�[�̃��X�g
    private int[] playerKeys;
    //�v���C���[�̃����N
    private int playerRank;

    private void Awake()
    {
        instance = this;
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        ChangeSelectModeOrWait(true); 
    }

    //�΋ǊJ�n�������ƃ}�b�`���O�J�n
    public void ConnectNetwork()
    {
        playerRank = Setting.CalculateRank(PlayerPrefs.GetInt("PlayerRating", 1500));
        int keyWidth = PlayerPrefs.GetInt("KeyWidth", 1);
        playerKeys = Setting.CalculateKeys(playerRank, keyWidth);

        PhotonNetwork.ConnectUsingSettings();

        ChangeSelectModeOrWait(false);
    }

    public override void OnConnectedToMaster()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Connected to Master Server");

            PhotonNetwork.JoinLobby();
        }
    }

    public override void OnJoinedLobby()
    {
        print("Joined lobby!");
        PhotonNetwork.JoinRandomRoom();
        //JoinRandomRoomWithRating();
    }

    //�߂�{�^���������ƃl�b�g���[�N����ؒf�����
    public void DisConnectNetwork()
    {
        PhotonNetwork.Disconnect();

        ChangeSelectModeOrWait(true);
    }

    //�ҋ@��ԂƑI����Ԃ̐؂�ւ�
    //SelectMode��true�Ȃ�UI��\�����ċ�I�����[�h
    //false�Ȃ�ҋ@���
    void ChangeSelectModeOrWait(bool SelectMode)
    {
        selectTicketPart.SetActive(SelectMode);

        selectGameRule.SetActive(SelectMode);

        startGameButton.SetActive(SelectMode);

        returnTitleSceneButton.SetActive(SelectMode);

        waitPanel.SetActive(!SelectMode);

        returnSelectModeButton.SetActive(!SelectMode);
    }

    //���[���������N�}�P�͈̔͂Ō���
    void JoinRandomRoomWithRating()
    {
        if (PhotonNetwork.InLobby)
        {
            foreach (int key in playerKeys)
            {
                ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable();
                expectedCustomRoomProperties.Add("RoomKey", key);

                PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, 2);

                print(key);
            }

            //�K�؂ȕ�����������Ȃ������ꍇ�V�����������쐬����
          //  CreateRoomWithKey(playerRank);
        }
        else
        {
            Debug.Log("Not in lobby yet. Waiting for OnJoinedLobby or OnConnectedToMaster callback.");
        }
        
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to join a random room. Creating a new room.");
        CreateRoomWithKey(playerRank);
    }

    public override void OnCreatedRoom()
    {
        // ����������ɍ쐬���ꂽ�Ƃ��̏���
        Debug.Log("Room created successfully.");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // �V�����v���C���[�����[���ɓ������Ƃ��̏���
        Debug.Log("Player entered the room: " + newPlayer.NickName);
        if (PhotonNetwork.IsMasterClient)
        {
            if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
            {
                PhotonNetwork.CurrentRoom.IsOpen = false;
                StartMatch();
            }
        }
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined Room!");
    }

    void CreateRoomWithKey(int roomKey)
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 2,
            IsVisible = true,
            IsOpen = true,
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable() { { "RoomKey", roomKey } },
            CustomRoomPropertiesForLobby = new string[] { "RoomKey" } // ���r�[�ŕ\������J�X�^���v���p�e�B
        };

        PhotonNetwork.CreateRoom(null, roomOptions);
    }

    //�΋ǃV�[���ɑJ��
    public  void StartMatch()
    {
        PhotonNetwork.LoadLevel("MatchScene");
    }
}
