using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public BoxCollider2D attackCollider;

    private Animator animator;

    public string otherPlayer;

    // Start is called before the first frame update
    void Start()
    {
         animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // if(do Input space)
        // {
        //     StartNormalAttack();
        // }

    }

    public void StartAttack()
    {
        Debug.Log("Attack");
        animator.SetTrigger("swing");
    }

    public void StartNormalAttack()
    {
        attackCollider.enabled = true;
    }

    public void EndNormalAttack()
    {
        attackCollider.enabled = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == otherPlayer)
        {
            Debug.Log("Hit");
            other.gameObject.GetComponent<PlayersStats>().doDamage(5);
        }
    }
}
