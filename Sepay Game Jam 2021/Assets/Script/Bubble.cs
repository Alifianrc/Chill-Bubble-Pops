using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    private GameManager manager;
    [SerializeField] private int ID;

    private Animator myAnimator;
    private bool isPopped;

    private Vector2 minPosCamera;
    private Vector2 maxPosCamera;

    private float duration = 1000f;
    private Vector3 targetPos;
    private float distaceLimit = 0.8f;

    private float rotateSpeed;
    private int rotationDir;

    private bool canGenerate = false;

    private float checkPosCoolDownTime = 5;
    private float checkPosCoolDown = 5;
    private Transform firstPos;

    private float bombLifeTime;

    private void Start()
    {
        // Find game manager
        manager = FindObjectOfType<GameManager>();

        // Find screen area
        minPosCamera = manager.GetMinPosCamera();
        maxPosCamera = manager.GetMaxPosCamera();

        // Make random target point
        targetPos = new Vector3(Random.Range(minPosCamera.x, maxPosCamera.x), Random.Range(minPosCamera.y, maxPosCamera.y), 0);

        // Change size
        if (ID != 2)
        {
            float randS = Random.Range(0.5f, 0.8f);
            Vector3 newScale = new Vector3(randS, randS, 1);
            transform.localScale = newScale;
        }

        // Rotation
        rotateSpeed = Random.Range(150, 300);
        if(Random.Range(0,2) == 0)
        {
            rotationDir = 1;
        }
        else
        {
            rotationDir = -1;
        }

        // Animation
        myAnimator = GetComponent<Animator>();
        isPopped = false;

        // Bomb life time
        bombLifeTime = Random.Range(5, 16);

        firstPos = transform;
    }


    private void Update()
    {
        if (!MenuManager.MenuIsActive && !isPopped)
        {
            // If not close to the target yet ...
            if (Mathf.Abs(transform.position.x - targetPos.x) > distaceLimit && Mathf.Abs(transform.position.y - targetPos.y) > distaceLimit)
            {
                // ... go to that target point
                float t = Random.Range(0.9f, 1.7f) / duration;
                transform.position = new Vector3(Mathf.Lerp(this.transform.position.x, targetPos.x, t), Mathf.Lerp(this.transform.position.y, targetPos.y, t), 0);

                canGenerate = true;
            }
            else if (canGenerate)
            {
                // ... generate new target
                StartCoroutine(GenerateNewTargetPos());
            }

            // Rotation
            transform.Rotate(0, 0, (rotateSpeed * rotationDir) * Time.deltaTime);

            // Self destroy bomb
            if (ID == 1)
            {
                if (bombLifeTime <= 0)
                {
                    FindObjectOfType<AudioManager>().Play("BubblePop");
                    myAnimator.SetTrigger("IsDead");
                }
                else
                {
                    bombLifeTime -= Time.deltaTime;
                }
            }
            

            // Check pos
            CheckPos();
        }
    }

    // If clicked by player
    private void OnMouseDown()
    {
        if (!isPopped && !MenuManager.MenuIsActive)
        {
            isPopped = true;

            // Tell manager
            manager.BubblePopped(ID);

            // Animation
            myAnimator.SetTrigger("IsPop");

            // Animation for Bomb
            if (ID == 1)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            // Animation for heart
            else if(ID == 2)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                GetComponent<Rigidbody2D>().AddForce(transform.up * 20);
                GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }

    private IEnumerator GenerateNewTargetPos()
    {
        canGenerate = false;
        // Random cooldown
        float t = Random.Range(1, 3);
        // Change rotation speed
        rotateSpeed = Random.Range(150, 450);
        // Start cooldown
        yield return new WaitForSeconds(t);
        // Generate new target pos
        targetPos = new Vector3(Random.Range(minPosCamera.x, maxPosCamera.x), Random.RandomRange(minPosCamera.y, maxPosCamera.y), 0);
    }

    private void CheckPos()
    {
        if (checkPosCoolDown <= 0)
        {
            checkPosCoolDown = checkPosCoolDownTime;

            if(firstPos.position == transform.position)
            {
                StartCoroutine(GenerateNewTargetPos());
            }
            else
            {
                firstPos = transform;
            }
        }
        else
        {
            checkPosCoolDown -= Time.deltaTime;
        }
    }
}
