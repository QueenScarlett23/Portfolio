using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBuilding : MonoBehaviour
{
    public int team = 0;
    public double health = 150;
    public double maxHealth = 150;
    public int productionSpeed = 190;
    public int productionCounter = 0;
    private HealthBar healthBar;

    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        productionCounter = 0;
        AssignColor();
    }

    public void ChangeTeam(int newTeam)
    {
        team = newTeam;
        AssignColor();
    }

    //ASsigns colour depending on the team
    private void AssignColor()
    {
        if (team < 0)
            GetComponent<SpriteRenderer>().color = Color.green; // neutral colour
        else
            GetComponent<SpriteRenderer>().color = (team == 0) ? Color.red : Color.blue; // team colours
    }

    // Update is called once per frame
    void Update()
    {
        float healthBarHealth = (float)(health / maxHealth);
        healthBar.SetHealth(healthBarHealth);
    }
}
