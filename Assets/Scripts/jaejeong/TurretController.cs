using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TurretController : Turret
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform turretTransform;
    public GroundTrigger trigger;

    [SerializeField] private float range = 30f;

    private string playerTag = "Player";

    private Transform target;
    private Enemy targetEnemy;

    private string turretOwner = "";


    protected override void Start()
    {
        base.Start();
        turretOwner = photonView.Owner.NickName;

        photonView.RPC("Init", RpcTarget.All);
    }

    [PunRPC]
    public void Init()
    {
        trigger = GameObject.Find(photonView.Owner.NickName + "HomeArea").GetComponent<GroundTrigger>();
        trigger.myTurret = this;

        //InvokeRepeating("UpdateTarget", 0f, 0.5f);
        if (photonView.Owner == PhotonNetwork.LocalPlayer)
            photonView.RPC("RepeatInvoke", RpcTarget.All);
    }

    [PunRPC]
    private void RepeatInvoke()
    {
        /*if (photonView.Owner == PhotonNetwork.LocalPlayer)
            */InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    private void UpdateTarget() //Turret????
    {
        if (firePoint == null)
            return;
        GameObject[] players = GameObject.FindGameObjectsWithTag(playerTag);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestPlayer = null;
        foreach (GameObject player in players)
        {
            if (photonView.Owner.NickName == player.name||player.GetComponent<PlayerStat>().status==PlayerStat.Status.die)
                continue;
            float distanceToPlayer = Vector3.Distance(turretTransform.transform.position, player.transform.position);
            if (distanceToPlayer < shortestDistance)
            {
                shortestDistance = distanceToPlayer;
                nearestPlayer = player;
            }
        }
        if (shortestDistance > range)
            trigger.inOtherHome = false;
        if (nearestPlayer != null && shortestDistance <= range && !nearestPlayer.GetComponent<Enemy>().isChanged) //???? ????x, ??ž ???Ÿ? ??, ???? ???? x
        {
            photonView.RPC("ChangeTarget", RpcTarget.All, nearestPlayer.GetComponent<PhotonView>().Owner.ActorNumber);
            Enemy.originalColor = nearestPlayer.GetComponentInChildren<SkinnedMeshRenderer>().material.color;
        }
        else
            target = null;
    }

    private void Update()
    {
        if (target == null||firePoint==null)
            return;

        attack = target.GetComponent<PlayerController>().IsAiming;

        if (!trigger.inOtherHome && !attack)
            return;

        if (photonView.IsMine)
            photonView.RPC("LockOnTarget", RpcTarget.All, target.position.x, rotatePart.position.y,target.position.z);

        if (fireTimeLimit <= 0f && trigger.inOtherHome || fireTimeLimit <= 0f&&attack&&!trigger.inOtherHome)
        {
            if (photonView.IsMine)
            {
                var firedBullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation * Quaternion.Euler(new Vector3(0, 90, 0)));
                firedBullet.GetComponent<PhotonView>().RPC("Shoot", RpcTarget.All, firePoint.right.x, firePoint.right.y, firePoint.right.z);
            }
            fireTimeLimit = 1f / fireRate;
        }
        fireTimeLimit -= Time.deltaTime;
    }

    [PunRPC]
    void LockOnTarget(float x, float y, float z)
    {
        StopCoroutine(RotateTurret());
        Vector3 position = new Vector3(x, y, z);
        rotatePart.transform.LookAt(position);
        rotatePart.transform.Rotate(0, -90, 0);
    }

    [PunRPC]
    public void ChangeTarget(int actorNumber)
    {
        target = PlayerList.Instance.playersWithActorNumber[actorNumber].transform;
        targetEnemy = target.GetComponent<Enemy>();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
//??ž ???Ÿ? ?? && ???? ?? ?????? ???? ???? ???? ???? ????
//?? ?? ?? && ??ž ???Ÿ? ?? && ???? ???? ???? ????
//Turret ???? ?ȿ??? ?Ѿȵ??? ????x