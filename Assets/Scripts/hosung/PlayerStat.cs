using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerStat : MonoBehaviourPunCallbacks, IPunObservable
{
    // 나중에 플레이어 actornumber로 플레이어 판별 등을 할 예정 
    [Header("OwnerPlayerInfo")]
    public int ownerPlayerActorNumber;

    public enum Status
    {
        idle = 1,
        walk,
        attack,
        damaged,
        die
    }
    public static Status status;

    [SerializeField] float hp;
    [SerializeField] float maxHp = 90f;
    [SerializeField] int money;

    public bool isCold = false;

    // Callback Methods
    void Awake()
    {
        hp = maxHp;
        photonView.RPC("AddPlayerStat", RpcTarget.AllBuffered);
    }

    [PunRPC]
    void AddPlayerStat()
    {
        PlayerList.Instance.playerStats.Add(this);
    }

    void Update()
    {

    }

    // Public Methods
    // ����
    public void AddHp(float _hp)
    {
        hp += _hp;
    }

    // ����
    public void AddMoney(int _money)
    {
        money += _money;
    }

    // 플레이어 자원 동기화
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(hp);
            stream.SendNext(maxHp);
            stream.SendNext(money);
        }
        else
        {
            hp = (float)stream.ReceiveNext();
            maxHp = (float)stream.ReceiveNext();
            money = (int)stream.ReceiveNext();
        }
    }
}
