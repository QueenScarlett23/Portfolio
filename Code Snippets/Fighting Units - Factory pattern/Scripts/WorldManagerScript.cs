using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManagerScript : MonoBehaviour
{
    public GameObject FactoryMeelee;
    public GameObject FactoryRanged;
    public GameObject ResourceBuilding;
    public GameObject wizardsUnit;
    public int noOfBuildings = 10;
    bool spawned = false;

    public int ResourcesTeam1 = 0;
    public int resourcesTeam2 = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void SpawnMap()
    {
        int team = 0;
        

        //spawns buildings
        for (int i = 0; i < noOfBuildings; i++) {

            float building = Random.Range(0f, 1f);

            float x = Random.Range(-15f, 15f);
            float y = Random.Range(-15f, 15f);
            float z = Random.Range(-15f, 15f);

            if (building > 0.5f)
            {
                GameObject u = Instantiate(FactoryMeelee, new Vector3(x, y, z), Quaternion.identity);
                u.GetComponent<FactoryMeeleeBuilding>().ChangeTeam(team);
            } else
            {
                GameObject u = Instantiate(FactoryRanged, new Vector3(x, y, z), Quaternion.identity) as GameObject;
                u.GetComponent<FactoryRangedBuilding>().ChangeTeam(team);
            }

            if (team == 0)
                team = 1;
            else
                team = 0;

        }

        // spawns resource buildings
        for (int i = 0; i < 6; i++)
        {
            float x = Random.Range(-15f, 15f);
            float y = Random.Range(-15f, 15f);
            float z = Random.Range(-15f, 15f);

            if (team == 0)
                team = 1;
            else
                team = 0;

            GameObject u = Instantiate(ResourceBuilding, new Vector3(x, y, z), Quaternion.identity) as GameObject;
            u.GetComponent<ResourceBuilding>().ChangeTeam(team);

        }


        // spawns wizzrds
        for (int i = 0; i < noOfBuildings; i++)
        {
            float x = Random.Range(-15f, 15f);
            float y = Random.Range(-15f, 15f);
            float z = Random.Range(-15f, 15f);

            GameObject u = Instantiate(wizardsUnit, new Vector3(x, y, z), Quaternion.identity) as GameObject;
            u.GetComponent<WizardUnit>().ChangeTeam(-1); // assigns wizzards to neutral team
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (spawned == false)
        {
            spawned = true;
            SpawnMap();
        }
    }
}
