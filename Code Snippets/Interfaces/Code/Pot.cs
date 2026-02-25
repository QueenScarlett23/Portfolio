using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pot : MonoBehaviour, IInteract
{
    public PotSpot[] flowerSpots;

    public SO_Soil soil;
    [Range(0f, 1f)]
    public float waterLevel;


    public void ShowFlowers()
    {
        foreach (PotSpot pot in flowerSpots)
        {
            pot.ShowFlower();
        }
    }
    public void HideFlowers()
    {
        foreach (PotSpot pot in flowerSpots)
        {
            pot.HideFlower();
        }
    }
    public void RemoveFlowers()
    {
        foreach (PotSpot pot in flowerSpots)
        {
            pot.RemoveFlower();
        }
    }

    public void Interact()
    {
        throw new System.NotImplementedException();
    }

    public void Interact(ActionType action)
    {
        throw new System.NotImplementedException();
    }

    public void Interact(ActionType action, object obj)
    {
        throw new System.NotImplementedException();
    }

    public List<Requirements> Actions()
    {
        throw new System.NotImplementedException();
    }
}
