using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FactoryMeeleeBuilding : MonoBehaviour
{
    public int team = 0;
    public double health = 150;
    public double maxHealth = 150;
    public int productionSpeed = 20;
    public int productionCounter = 0;
    private HealthBar healthBar;
    public GameObject meeleeUnit;
    // Start is called before the first frame update
    void Start()
    {
        healthBar = GetComponentInChildren<HealthBar>();
        productionCounter = 0;
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

    public void ChangeTeam(int newTeam)
    {
        team = newTeam;
        AssignColor();
    }

    //called once per frame
    void Update()
    {
        float healthBarHealth = (float) (health / maxHealth);
        healthBar.SetHealth(healthBarHealth);

        productionCounter++;

        if (productionCounter >= productionSpeed)
        {
            productionCounter = 0;
            SpawnUnit();
        }
    }

    void SpawnUnit()
    {
        GameObject u = Instantiate(meeleeUnit, new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 1.3f, gameObject.transform.position.z), Quaternion.identity) as GameObject;
        u.GetComponent<MeeleeUnit>().ChangeTeam(team);
    }
}
