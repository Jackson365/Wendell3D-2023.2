using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public float speed;
    private CharacterController controller;
    public float damage = 20;
    public float totalHealth = 140;
    
    private Transform cam;
    private Vector3 moveDirection;
    public float gravity;
    public float colliderRadius;

    private Animator anim;

    public float smoothRotTime;
    private float turnSmoothVelocity;

    public List<Transform> enemyList = new List<Transform>();

    private bool isWalking;
    private bool waitFor;
    private bool isHitting;

    public bool isDead;
    public AudioSource AttackingSword;
    public AudioSource ColectHealth;
    public AudioSource ColectCoin;
    
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform; 

        GameController.instance.UpdateLives(totalHealth);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            Move();
            GetMouseInput(); 
        }
    }

    private void Move()
    {
        if(controller.isGrounded)
        {
        //pega a entrada na horizontal (Tecla direita e esquerda)
        float horizontal = Input.GetAxisRaw("Horizontal");
        
        //pega a entrada na vetical (Tecla cima e baixo)
        float vertical = Input.GetAxisRaw("Vertical");
        
        //varialvel local que armazena o valor do eixo horizontal e vertical
        Vector3 direction = new Vector3(horizontal, 0f, vertical);
        

        //verifica se o personagem está se movimentando (se for > 0)
            if (direction.magnitude > 0)
            {
                if (!anim.GetBool("Attacking"))
                {
                    //Armazena a rotação e o angulo da camera
                    float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;

                    //Armazena a rotação mais suave
                    float smoothAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, angle, ref turnSmoothVelocity, smoothRotTime);

                    //rotaciona o personagem
                    transform.rotation = Quaternion.Euler(0f,smoothAngle,0f);
            
                    //armazena a direção
                    moveDirection = Quaternion.Euler(0f, angle,0f) * Vector3.forward * speed;

                    anim.SetInteger("Transition",1);
                    isWalking = true; 
                }
                else
                {
                    moveDirection = Vector3.zero;
                    anim.SetBool("Walking", false);
                }
            }
            else if(isWalking)
            {
                anim.SetInteger("Transition", 0);
                anim.SetBool("Walking", false);
                moveDirection = Vector3.zero;
                isWalking = false;
                //anim.SetInteger("Transition",0);
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }
    void GetMouseInput()
    {
        if(controller.isGrounded)
        {
            if(Input.GetMouseButtonDown(0))
            {
                if (anim.GetBool("Walking"))
                {
                    anim.SetBool("Walking", false);
                    anim.SetInteger("Transition", 0);
                }

                if (!anim.GetBool("Walking"))
                {
                    StartCoroutine(Attack());
                }
            }
        }
    }

    IEnumerator Attack()
    {
        if (!waitFor && !isHitting)
        {
            waitFor = true;
            anim.SetBool("Attacking", true);

            anim.SetInteger("Transition", 2);
            AttackingSword.Play();
            yield return new WaitForSeconds(1f);
            GetEnemiesList();
            foreach (Transform e in enemyList)
            {
                CombatEnemy enemy = e.GetComponent<CombatEnemy>();

                if (enemy != null)
                {
                    enemy.GetHit(damage);
                }
                //Debug.Log(e.name);
            }

            yield return new WaitForSeconds(0.5f);
            anim.SetInteger("Transition", 0);
            anim.SetBool("Attacking", false);
            waitFor = false;
        }
    }

    void GetEnemiesList()
    {
        enemyList.Clear();
        foreach (Collider c in Physics.OverlapSphere((transform.position + transform.forward * colliderRadius), colliderRadius))
        {
            if (c.gameObject.CompareTag("Enemy"))
            {
                enemyList.Add(c.transform);
            }
        }
    }
    
    public void GetHit(float damage)
    {
        totalHealth -= damage;
        GameController.instance.UpdateLives(totalHealth);

        if (totalHealth > 0)
        {
            //Esta Vivo
            StopCoroutine("Attack");
            anim.SetInteger("Transition", 3);
            isHitting = true;
            StartCoroutine("RecoveryHit");
        }
        else
        {
            //Esta Morto
            isDead = true;
            anim.SetTrigger("die");
        }
    }

    IEnumerator RecoveryHit()
    {
        yield return new WaitForSeconds(1f);
        anim.SetInteger("Transition", 0);
        isHitting = false;
        anim.SetBool("Attacking", false);
    }

    public void IncreaseHealth(float value)
    {
        totalHealth += value;
        GameController.instance.UpdateLives(totalHealth);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, colliderRadius);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.gameObject.tag == "IntemHealth")
        {
            ColectHealth.Play();
        }

        if(collider.gameObject.tag == "IntemCoin")
        {
            ColectCoin.Play();
        }
    }
}
