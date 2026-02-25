using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardUnit : MonoBehaviour
{
    // variable declaration 
    public double health = 60; // current health points of the unit
    public double maxHealth = 60; // maximum health point the unit can have
    public double attack = 10; // the amount of damage the unit can deal
    public double speed = 2; // the rate at which this unit can perform actions
    public double attackRange = 3; // details the maximum distance at which a unit can deal damage to another unit
    public int team = -1; // intiger that tells what team a unit is on. It is used to tell whether a unit is an enemy or not
    public string name; // variable to tell the user the type of unit
    private HealthBar healthBar;

    public Color UnitColor;

    public WizardUnit(int xPos, int yPos, double maxHealth, double attack, int speed, int team)
    {
        this.maxHealth = maxHealth;
        this.health = maxHealth;
        this.attack = attack;
        this.team = team = -1;
        this.speed = speed;
        this.attackRange = 1;
        this.name = "Wizard";
    }

    public void ChangeTeam(int newTeam)
    {
        team = newTeam;
        AssignColor();
    }

    void Start()
    {
        Color playerColor = AssignColor();
        GetComponent<SpriteRenderer>().color = playerColor;
        healthBar = GetComponentInChildren<HealthBar>();
    }

    //allows the program to assign a colour based on the unit's team
    private Color AssignColor()
    {
        if (team < 0)
            return Color.green; // neutral colour
        else
            return (team == 0) ? Color.red : Color.blue; // team colours
    }

    // Update is called once per frame
    void Update()
    {
        float healthBarHealth = (float)(health / maxHealth);
        healthBar.SetHealth(healthBarHealth);
    }
}
