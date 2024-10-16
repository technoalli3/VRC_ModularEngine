
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class ToolHead : UdonSharpBehaviour
{
    [SerializeField] private GameObject toolPickup;
    [SerializeField] private GameObject ratchetSound;
    [UdonSynced] private bool isLocked = false;
    void Start()
    {
        
    }

    public void LockTool()
    {
        if(!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
        if (Networking.IsOwner(gameObject))
        {
            toolPickup.GetComponent<VRCPickup>().Drop(); //drops tool
            toolPickup.GetComponent<VRCPickup>().pickupable = false; //locks tool
            ratchetSound.SetActive(true); //enable sound
            isLocked = true; //sets synced var
            RequestSerialization();
        }
    }

    public void UnlockTool()
    {
        if (!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }
        if (Networking.IsOwner(gameObject))
        {
            toolPickup.GetComponent<VRCPickup>().pickupable = true; //unlocks tool
            ratchetSound.SetActive(false); //disable sound
            isLocked = false; //sets synced var
            RequestSerialization();
        }
    }

    public override void OnDeserialization()
    {
        if(!Networking.IsOwner(gameObject)) //client sync
        {
            toolPickup.GetComponent<VRCPickup>().pickupable = !isLocked;
            ratchetSound.SetActive(isLocked);
        }
    }
}
