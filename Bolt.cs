
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

[UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
public class Bolt : UdonSharpBehaviour
{
    [SerializeField] private GameObject toolHead;
    [SerializeField] private float removalSpeed = 1.0f;
    [UdonSynced] private bool isLoose = false;
    [UdonSynced] private bool isActive = true;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == toolHead) //the object that has the wrench code
        {
            if (!Networking.IsOwner(gameObject))
            {
                Networking.SetOwner(Networking.LocalPlayer, gameObject);
            }
            if(!isLoose)
            {
                isLoose = true; //sets loose

                RequestSerialization();//non-owner players get bolt loosened
                SyncBolt(); //owner gets bolt loosened
            }
        }
    }

    private void SyncBolt() //syncs the status of a bolt, updates physical appearance, and locks tool if toolis present
    {

        gameObject.GetComponent<BoxCollider>().enabled = isActive;

        if (isActive)
        {
            gameObject.GetComponent<MeshRenderer>().material.SetFloat("_MatcapEmissionStrength", 1f);//disable matcap
            gameObject.GetComponent<MeshRenderer>().material.SetFloat("_MatcapReplace", 1f);
        }
        else
        {
            gameObject.GetComponent<MeshRenderer>().material.SetFloat("_MatcapEmissionStrength", 0f);//disable matcap
            gameObject.GetComponent<MeshRenderer>().material.SetFloat("_MatcapReplace", 0f);
        }

        if (isLoose)//check that it's loose
        {
            gameObject.GetComponent<MeshRenderer>().material.SetFloat("_MatcapEmissionStrength", 0f);//disable matcap
            gameObject.GetComponent<MeshRenderer>().material.SetFloat("_MatcapReplace", 0f);

            if(Networking.IsOwner(gameObject))//only owner should call check functions
            {
                toolHead.GetComponent<ToolHead>().LockTool();
                SendCustomEventDelayedSeconds("CallToolUnlock", removalSpeed, VRC.Udon.Common.Enums.EventTiming.Update);
            }
        }
    }

    public void CallToolUnlock()
    {
        if (Networking.IsOwner(gameObject))
        {
            toolHead.GetComponent<ToolHead>().UnlockTool(); //calls the toolhead component to unlock corresponding pickup
            gameObject.GetComponent<MeshRenderer>().enabled = false; //disables bolt mesh
            if (gameObject.GetComponentInParent<EnginePart>() != null)//makes sure there's actualy engine part installed (shouldn't fail once project is complete)
            {
                gameObject.GetComponentInParent<EnginePart>().CheckBolts(); //calls function to check that all bolts are loosened, increments int in EnginePart that keeps track of how many bolts are loosened
            }
        }
    }

    public void DeactivateBolt() //this disables removal of bolts, for layers that aren't active
    {
        isActive = false;

        RequestSerialization();
        SyncBolt();
    }

    public void ActivateBolt() //this enables removal of bolts for active layers
    {
        isActive = true;

        RequestSerialization();
        SyncBolt();
    }

    public override void OnDeserialization() //client sync
    {
        SyncBolt();
    }
}
