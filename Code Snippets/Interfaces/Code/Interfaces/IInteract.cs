
using System;
using System.Collections.Generic;
using UnityEngine;

public enum ActionType
{
    Interact,
    Place,
    Remove,
    Water
}

[Serializable]
public struct Requirements
{
    public ActionType a;
    public Type t;
}

public interface IInteract
{
    public void Interact();
    public void Interact(ActionType action);
    public void Interact(ActionType action, object obj);
    public List<Requirements> Actions();
}