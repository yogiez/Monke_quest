using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed;
    public LayerMask solidObjectsLayer;
    public LayerMask grassLayer;

    public event Action OnEncountered; //event potrzebny dla gamecontrollera, aby mogl wlaczyc odpowiedni controller freeroam/battle

    private bool isMoving; //zmienna do sprawdzania czy postac sie porusza
    private Vector2 input;

    public Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>(); //odniesienie dla animatora
    }

    public void HandleUpdate()
    {
        //jezeli postac sie nie porusza, sprawdzmy input, aby sprobowac go poruszyc 
        if(!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal"); //GetAxisRaw = input będzie zawsze -1 lub 1
            input.y = Input.GetAxisRaw("Vertical");

            //wylaczenie movementu przekatnego
            if (input.x != 0) input.y = 0;

            //jezeli input =/= 0 przesun postac w odpowiednia strone
            if(input != Vector2.zero)
            {
                animator.SetFloat("moveX", input.x); //jezeli input =/= 0 rozpocznij animacje zalezne od moveX, moveY
                animator.SetFloat("moveY", input.y);

                var targetPos = transform.position;
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (IsWalkable(targetPos))
                    StartCoroutine(Move(targetPos)); //zacznij ciag instrukcji Move
            }
        }

        animator.SetBool("isMoving", isMoving); //rozpocznij animacje chodzenia
    }   

    //funkcja sluzaca do przemieszczania z aktualnej pozycji do pozycji docelowej
    IEnumerator Move(Vector3 targetPos) //funkcja IEnumerator sluzy do robienia czegos przez pewien okres czasu
    {
        isMoving = true;
        //sprawdzenie roznicy pozycji docelowej a pozycji aktualnej przez małą wartość
        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            //jezeli faktycznie jest roznica pomiedzy pozycjami, postac przesunie sie o ta mala wartosc, a nastepnie powroci do ^39; funkcja bedzie sie powtarzac do momentu az te roznice beda do siebie bardzo zblizone 
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        //aktualna pozycja gracza = docelowa pozycja gracza
        transform.position = targetPos;

        isMoving = false;

        CheckForEncounters();
    }

    //funkcja sprawdzajaca czy pozycja docelowa jest mozliwa do chodzenia
    private bool IsWalkable(Vector3 targetPos)
    {
        if(Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer) != null) //sprawdzenie czy na pozycji docelowej jest objekt
        {
            return false;
        }

        return true;

    }


    //funkcja sprawdzająca czy gracz stoi na longgrass i wywolujaca ataki przeciwnikow
    private void CheckForEncounters()
    {
        if(Physics2D.OverlapCircle(transform.position, 0.2f, grassLayer) != null)
        {
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                animator.SetBool("isMoving", false);
                System.Threading.Thread.Sleep(1000);
                OnEncountered();
            }

        }
    }

}
