using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotSpot : MonoBehaviour, IInteract
{
    public static LayerMask Default;
    public Transform pos;
    public GameObject flowerGameObject;
    public Requirements[] possibleActions;

    private FlowerObject flower = null;
    private bool watered;
    private Renderer material;

    public int StageOfGrowth;
    public Phase CurrentPhase;

    public Color dry;
    public Color wet;
    public Vector3 PlantOffset;



    private void Awake()
    {
        material = GetComponent<Renderer>();
    }

    private float time;

    private void FixedUpdate()
    {
        if (flower != null)
        {
            if (time > 3)
            {
                if ((StageOfGrowth + 1) % flower.aging.Length == 0)
                    StageOfGrowth = -1;
                AgePlant();
                time = 0;
            }
            time += Time.deltaTime;
        }
    }

    // Age plant
    public void AgePlant()
    {
        StageOfGrowth += 1;
        ShowFlower();
    }

    // plants plant in pot ; returns false if unable to
    public bool Plant(FlowerObject f)
    {
        //Debug.Log("planting");
        if (flower == null)
        {
            StageOfGrowth = 0;
            flower = f;
            ShowFlower();
            return true;
        }
        Debug.LogWarning("planting incomplete");
        return false;
    }

    internal void ShowFlower()
    {
        if (flower != null)
        {
            if (flowerGameObject == null || flowerGameObject.gameObject != flower.aging[StageOfGrowth].gameObject)
            {
                //Debug.Log("Updateing flower");
                if (flowerGameObject != null)
                    Destroy(flowerGameObject); // remove old model

                flowerGameObject = Instantiate(flower.aging[StageOfGrowth].gameObject, transform);
                flowerGameObject.transform.localScale = Common.DevideVector3Components(Vector3.one, flowerGameObject.transform.lossyScale);
                flowerGameObject.transform.localPosition = PlantOffset;
                flowerGameObject.layer = Default;
            }
        }
    }
    internal void HideFlower()
    {
        if (flowerGameObject != null)
        {
            flowerGameObject.SetActive(false);
        }
    }
    internal void RemoveFlower()
    {
        if (flowerGameObject != null)
        {
            Destroy(flowerGameObject);
        }
    }

    // IInteract
    public List<Requirements> Actions()
    {
        // start with all actions
        List<Requirements> actions = new List<Requirements>(possibleActions);

        if (flower != null)
        {
            foreach (Requirements req in actions)
            {
                if (req.a == ActionType.Place)
                {
                    actions.Remove(req);
                    break;
                }

            }
        }
        else
        {
            foreach (Requirements req in actions)
            {
                if (req.a == ActionType.Remove)
                {
                    actions.Remove(req);
                    break;
                }

            }
        }

        return actions;
    }

    public void Interact()
    {
        if (flower != null)
            Debug.Log("The flowers smell nice");
        else
            Debug.Log("This might look nice with some flowers");
    }

    public void Interact(ActionType action)
    {
        Interact();
    }

    public void Interact(ActionType action, object obj)
    {
        switch (action)
        {
            case ActionType.Place:
                Plant(obj as FlowerObject);
                break;
            case ActionType.Remove:

                break;
            case ActionType.Water:
                WaterPlant();
                break;
            default:
                Interact();
                Debug.Log("Default Action");
                break;
        }
    }

    private bool WaterPlant()
    {
        if (!watered)
        {
            material.material.color = wet;
            watered = true;
            return true;
        }
        return false;
    }
}
