using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class Player_SyncPosition : MonoBehaviourPunCallbacks
{
    PhotonView PV;

    [SerializeField]
    private Transform myTransform;
    [SerializeField]
    private float lerpRate = 15;
    [SerializeField]
    private float threshold = 0.15f;

    private Vector3 syncPos;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void FixedUpdate()
    {
        TransmitPostion();
        LerpPosition();
    }

    void LerpPosition()
    {
        if (!PV.IsMine)
        {
            if (Vector3.Distance(myTransform.position, syncPos) > threshold)
                myTransform.position = Vector3.Lerp(myTransform.position, syncPos, Time.deltaTime * lerpRate);
        }
    }

    void TransmitPostion()
    {
        if (PV.IsMine)
        {
            if (Vector3.Distance(myTransform.position, syncPos) > threshold)
            {
                Hashtable hash = new Hashtable();
                hash.Add("position", myTransform.position);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine && targetPlayer == PV.Owner)
        {
            if (changedProps.ContainsKey("position"))
                syncPos = (Vector3)changedProps["position"];
        }
    }
}
