using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed;
    private CharacterController controller;

    private Transform cam;
    private Vector3 moveDirection;
    public float gravity;
    public float colliderRadius;

    private Animator anim;

    public float smoothRotTime;
    private float turnSmoothVelocity;

    public List<Transform> enemyList = new List<Transform>();
    
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
    }
    
    // Update is called once per frame
    void Update()
    {
        Move();
        GetMouseInput();
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

                    //move o personagem
                    //controller.Move(moveDirection * speed * Time.deltaTime);   
                }
                else
                {
                    moveDirection = Vector3.zero;
                    anim.SetBool("Walking", false);
                }
            }
            else
            {
                //anim.SetInteger("Transition", 0);
                anim.SetBool("Walking", false);
                moveDirection = Vector3.zero;
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
        anim.SetInteger("Transition",2);
        yield return new WaitForSeconds(1f);
        GetEnemiesList();
        foreach (Transform e in enemyList)
        {
            Debug.Log(e.name);
        }

        yield return new WaitForSeconds(1f);
        anim.SetInteger("Transition", 0);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward, colliderRadius);
    }
}
