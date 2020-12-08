using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class Player_SyncRotation : MonoBehaviourPunCallbacks
{
    PhotonView PV;

    [SerializeField]
    private Transform myTransform;
    [SerializeField]
    private Transform camTransform;
    [SerializeField]
    private float lerpRate = 15;
    [SerializeField]
    private float threshold = 0.15f;

    private float syncPlayerRot;
    private float syncCamRot;

    void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    void FixedUpdate()
    {
        TransmitRotation();
        LerpRotation();
    }

    void LerpRotation()
    {
        if (!PV.IsMine && camTransform)
        {
            Vector3 newPlayerRot = new Vector3(0, syncPlayerRot, 0);
            if (Quaternion.Angle(myTransform.rotation, Quaternion.Euler(newPlayerRot)) > threshold)
                myTransform.rotation = Quaternion.Lerp(myTransform.rotation, Quaternion.Euler(newPlayerRot), Time.deltaTime * lerpRate);

            Vector3 newCamRot = new Vector3(syncCamRot, syncPlayerRot, 0);
            if (Quaternion.Angle(camTransform.rotation, Quaternion.Euler(newCamRot)) > threshold)
                camTransform.rotation = Quaternion.Lerp(camTransform.rotation, Quaternion.Euler(newCamRot), Time.deltaTime * lerpRate);
        }
    }

    void TransmitRotation()
    {
        if (PV.IsMine)
        {
            Vector3 playerRot = new Vector3(0, syncPlayerRot, 0);
            Vector3 camRot = new Vector3(syncCamRot, syncPlayerRot, 0);
            if (Quaternion.Angle(myTransform.rotation, Quaternion.Euler(playerRot)) > threshold ||
                Quaternion.Angle(camTransform.rotation, Quaternion.Euler(camRot)) > threshold)
            {
                Hashtable hash = new Hashtable();
                hash.Add("playerRot", myTransform.localEulerAngles.y);
                hash.Add("camRot", camTransform.localEulerAngles.x);
                PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
            }
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps)
    {
        if (!PV.IsMine && targetPlayer == PV.Owner)
        {
            if (changedProps.ContainsKey("playerRot") && changedProps.ContainsKey("camRot"))
            {
                syncPlayerRot = (float)changedProps["playerRot"];
                syncCamRot = (float)changedProps["camRot"];
            }
        }
    }
}
