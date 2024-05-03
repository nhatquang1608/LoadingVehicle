using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour
{
    [SerializeField] private bool received;
    public bool canGo;
    [SerializeField] private Transform box;
    [SerializeField] private Transform robot;
    [SerializeField] private Transform boxPosition;
    [SerializeField] private Rigidbody2D truckRB;
    [SerializeField] private Rigidbody2D frontTireRB;
    [SerializeField] private Rigidbody2D backTireRB;
    [SerializeField] private GameManager gameManager;
    private float moveSpeed = 3000;

    private void Awake()
    {
        StartCoroutine(Stay());
    }

    private void Update()
    {
        if(received && box && !canGo)
        {
            Vector2 point1 = new Vector2(boxPosition.position.x, 0);
            Vector2 point2 = new Vector2(robot.position.x, 0);
            if(Vector2.Distance(point1, point2) >= 4.5 && point1.x > point2.x)
            {
                canGo = true;
                truckRB.constraints = RigidbodyConstraints2D.None;
                box.GetComponent<BoxCollider2D>().isTrigger = true;
                box.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
                box.SetParent(transform);
                StartCoroutine(DelayDestroy());
            }
        }
    }

    private void FixedUpdate()
    {
        if(canGo)
        {
            frontTireRB.AddTorque(-moveSpeed  * Time.fixedDeltaTime);
            backTireRB.AddTorque(-moveSpeed * Time.fixedDeltaTime);
        }
    }

    private IEnumerator DelayDestroy()
    {
        gameManager.GameOver(true);
        yield return new WaitForSeconds(3);
        Destroy(gameObject);
    }

    private IEnumerator Stay()
    {
        while(!canGo)
        {
            frontTireRB.angularVelocity = 0;
            backTireRB.angularVelocity = 0;
            truckRB.constraints = RigidbodyConstraints2D.FreezePosition;
            yield return null;
        }
    }

    private void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.transform.tag == "Box")
        {
            if(Vector2.Distance(collider.transform.position, boxPosition.position) < 0.1)
            {
                received = true;
                box = collider.transform;
            }
            else
            {
                received = false;
                canGo = false;
            }
        }
        else if(collider.transform.tag == "Player")
        {
            canGo = false;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if(collider.transform.tag == "Box")
        {
            received = false;
            canGo = false;
        }
    }
}
