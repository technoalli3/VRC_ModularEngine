
using System.Linq;
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class MasterEngine : UdonSharpBehaviour
{
    [SerializeField] private GameObject[] layer1;
    [SerializeField] private GameObject[] layer2;
    [SerializeField] private GameObject[] layer3;

    [UdonSynced] private int activeLayer = 1;
    void Start()
    {
        activateLayers(1); //sets active layer to layer 1
    }

    public void CheckLayer()
    {
        bool progress = false;
        switch (activeLayer) //calls validation function with appropriate layer
        {
            case 1:
                progress = ValidateLayer(layer1);
                break;
            case 2:
                progress = ValidateLayer(layer2);
                break;
            case 3:
                progress = ValidateLayer(layer3);
                break;
            default:
                break;
        }

        if (progress) //if verification passes, increase active layer and activate it
        {
            activeLayer++;
            activateLayers(activeLayer);
        }
    }

    private bool ValidateLayer(GameObject[] layer)
    {
        int index = 0;

        while (index < layer.Length)
        {
            if (layer[index] != null) //checks through all parts to make sure they are all unlocked, returns false at first locked part
            {
                if (!layer[index].GetComponent<EnginePart>().isFree)
                {
                    Debug.Log("False");
                    return false;
                }
            }
            index++;
        }
        Debug.Log("all checks passed");
        return true;
    }

    private void activateLayers(int layer)
    {
        switch (layer) //activates appropriate layer and deactivates rest
        {
            case 1:
                for (int i = 0; i < layer1.Length; i++)
                {
                    if (layer1[i] != null)
                    {
                        layer1[i].GetComponent<EnginePart>().EnableBolts();
                    }
                }

                for (int i = 0; i < layer2.Length; i++)
                {
                    if (layer2[i] != null)
                    {
                        layer2[i].GetComponent<EnginePart>().DisableBolts();
                    }
                }
                for (int i = 0; i < layer3.Length; i++)
                {
                    if (layer2[i] != null)
                    {
                        layer3[i].GetComponent<EnginePart>().DisableBolts();
                    }
                }
                break;
            case 2:
                for (int i = 0; i < layer1.Length; i++)
                {
                    if (layer1[i] != null)
                    {
                        layer1[i].GetComponent<EnginePart>().DisableBolts();
                    }
                }

                for (int i = 0; i < layer2.Length; i++)
                {
                    if (layer2[i] != null)
                    {
                        layer2[i].GetComponent<EnginePart>().EnableBolts();
                    }
                }
                for (int i = 0; i < layer3.Length; i++)
                {
                    if (layer2[i] != null)
                    {
                        layer3[i].GetComponent<EnginePart>().DisableBolts();
                    }
                }
                break;
            default:
                break;
        }
    }
}
