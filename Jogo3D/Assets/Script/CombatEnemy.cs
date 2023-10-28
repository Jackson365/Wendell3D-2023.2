using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Security.Cryptography;
using TreeEditor;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

   
  


public class CombatEnemy : MonoBehaviour
{
    [Header("Atributtes")] 
    public float totalHealth = 100;
    public float attackDamage;
    public float movementSpeed;
    public float lookRadius;
    public float colliderRadius = 2;
    public float rotationSpeed;

    [Header("Components")] 
    //[SerializeField]
    private Animator anim;
    private CapsuleCollider capsule;
    private NavMeshAgent agent;

    [Header("Others")] 
    private Transform player;

    private bool walking;
    private bool attacking;
    private bool hiting;
    private bool waitFor;
    private bool playerIsDead;

    [Header("WayPoints")]
    public List<Transform> wayPoints = new List<Transform>();
    public int currentPathIndex;
    public float pathDistance;

    public AudioSource Pain;
    public AudioSource PainComplet;
    public AudioSource SpiderAttack;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        capsule = GetComponent<CapsuleCollider>();
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        if (totalHealth > 0)
        { 
            float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= lookRadius)
        {
            //Personagem está no raio de ação
            agent.isStopped = false;
            if (!attacking)
            {
                agent.SetDestination(player.position);
                anim.SetBool("Walk Forward", true);
                walking = true;
            }

            if (distance <= agent.stoppingDistance)
            {
                StartCoroutine("Attack");
                LookTarget();
                //agent.isStopped = true;
            }
            else
            {
                attacking = false;
            }
        }
        else
        {
            //Personagem está não no raio de ação
            anim.SetBool("Walk Forward", false);
            //agent.isStopped = true;
            walking = false;
            attacking = false;
            MoveToWayPoints();

        }
    }
}

    void MoveToWayPoints()
    {
        if(wayPoints.Count > 0)
        {
            float distance = Vector3.Distance(wayPoints[currentPathIndex].position, transform.position);
            agent.destination = wayPoints[currentPathIndex].position;

            if(distance <= pathDistance)
            {
                //vai para o proximo ponto 
                currentPathIndex = Random.Range(0, wayPoints.Count);
            }
            anim.SetBool("Walk Forward", true);
            walking = true;
        }
    }
    
    IEnumerator Attack()
    {
        if (!waitFor && !hiting && !playerIsDead)
        {
            waitFor = true;
            attacking = true;
            walking = false;
            anim.SetBool("Walk Forward", false);
            anim.SetBool("Claw Attack" , true);
            SpiderAttack.Play();
            yield return new WaitForSeconds(1.5f);
            GetPlayer();
            yield return new WaitForSeconds(1f);
            waitFor = false;
        }

        if (playerIsDead)
        {
            anim.SetBool("Walk Forward", false); 
            anim.SetBool("Claw Attack" , false);
            walking = false;
            attacking = false;
            agent.isStopped = true;
        }
    }

    void GetPlayer()
    {
        foreach (Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        {
            if (c.gameObject.CompareTag("Player"))
            {
                //VAI CAUSAR DANO NO PLAYER
                c.gameObject.GetComponent<Player>().GetHit(attackDamage);
                Pain.Play();
                if(playerIsDead = c.gameObject.GetComponent<Player>().isDead)
                {
                    PainComplet.Play();
                }
                //playerIsDead = c.gameObject.GetComponent<Player>().isDead;
            }
        }
    }

    public void GetHit(float damage)
    {
        totalHealth -= damage;
        if (totalHealth > 0)
        {
            //Esta Vivo
            StopCoroutine("Attack");
            anim.SetTrigger("Take Damage");
            hiting = true;
            StartCoroutine("RecoveryHit");
        }
        else
        {
            //Esta Morto
            anim.SetTrigger("Die");
        }
    }

    IEnumerator RecoveryHit()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("Walk Forward", false);
        anim.SetBool("Claw Attack", false);
        hiting = false;
        waitFor = false;
    }

    void LookTarget()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
}
