using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ActionState
{
    ATTACK,
    SEEK,
    IDLE,
    ALERT
}
public class Bees : MonoBehaviour, IAttack
{
    [SerializeField]
    private Transform target;
    [SerializeField]
    private ActionState state;
    [SerializeField]
    private Attack attack;
    [SerializeField]
    private float SightDistance;
    [SerializeField]
    private float breakPersueRange;


    public GameObject player;
    public LayerMask layerMask;

    public GameObject SightSphere;
    public GameObject AttackSphere;

    // Update is called once per frame

    private void Start()
    {
        if (GameManager.I.Debug)
        {
            SightSphere.transform.localScale = Vector3.one * SightDistance * 2;
            AttackSphere.transform.localScale = Vector3.one * attack.Range * 2;
        }
        else
        {
            SightSphere.SetActive(false);
            AttackSphere.SetActive(false);
        }
    }

    void Update()
    {
        switch (state)
        {
            case ActionState.ATTACK:
                AttackMode();
                break;
            case ActionState.SEEK:
                SeekMode();
                break;
            case ActionState.IDLE:
                IdleMode();
                break;
            case ActionState.ALERT:
                AlertMode();
                break;
            default:
                IdleMode();
                break;
        }
    }

    void AttackMode()
    {
        // if in range attack
        if (Vector3.Distance(transform.position, target.position) < attack.Range)
        {
            Attack(attack.damage[0].Damage);
        }
        else // go to target
        {
            if (Vector3.Distance(transform.position, target.position) < breakPersueRange)
            {
                GetComponent<Move>().target = player.transform;
                GetComponent<Move>().enabled = true;
            }
            else
            {
                GetComponent<Move>().enabled = false;
                GetComponent<Move>().target = null;
                state = ActionState.ALERT;
            }
        }
    }


    void SeekMode()
    {

    }

    void IdleMode()
    {

    }

    void AlertMode()
    {
        // look for target
        if (player == null)
            player = GameObject.FindWithTag("Player");

        // bees are able to s4ee player
        if (Vector3.Distance(transform.position, player.transform.position) < SightDistance)
        {
            // ray cast to see if target is visible to bees
            Ray ray = new Ray(transform.position, (player.transform.position - transform.position));
            if (Physics.Raycast(ray.origin, ray.direction, SightDistance, layerMask))
            {
                // something in way
                target = null;
            }
            else
            {
                // nothing in way
                target = player.transform;
                GetComponent<Move>().target = target;
                Debug.Log("target set");
                state = ActionState.ATTACK;
            }
        }
    }

    public void Attack(float damage)
    {
        throw new System.NotImplementedException();
    }
}
