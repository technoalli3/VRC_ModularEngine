
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
public class EnginePart : UdonSharpBehaviour
{
    [SerializeField] private GameObject[] bolts;
    [SerializeField] private MasterEngine master;
    [UdonSynced] private int boltCount = 0;
    [UdonSynced, HideInInspector] public bool isFree = false;
    void Start()
    {
        
    }

    //called every time a bolt is loosened... not very optimized but I tried my best
    public void CheckBolts() //checks that all bolts are loosened and unlocks the part accordingly
    {
        if(!Networking.IsOwner(gameObject))
        {
            Networking.SetOwner(Networking.LocalPlayer, gameObject);
        }

        if (Networking.IsOwner(gameObject))
        {
            boltCount++;
            if (boltCount == bolts.Length) //if the amount of times this has been called matches how many bolts are registered, unlock the part
            {
                isFree = true;
                ChangePartState();
            }
        }
    }

    private void ChangePartState() //makes the part pickupable if it is free
    {
        gameObject.GetComponent<VRCPickup>().pickupable = isFree;
        master.CheckLayer();
    }

    public override void OnDeserialization()
    {
        ChangePartState();
    }

    public void DisableBolts() //disables all bolts for this part according to active layer
    {
        for(int i = 0; i < bolts.Length; i++)
        {
            bolts[i].GetComponent<Bolt>().DeactivateBolt();
        }
    }

    public void EnableBolts() //enables all bolts for this part according to active layer
    {
        for (int i = 0; i < bolts.Length; i++)
        {
            bolts[i].GetComponent<Bolt>().ActivateBolt();
        }
    }
}
