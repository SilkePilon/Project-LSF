using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayersStats : MonoBehaviour
{
    // Start is called before the first frame update

    public int health = 100;

    public GameObject healthBar1;
    public GameObject healthBar2;
    public GameObject healthBar3;
    public GameObject healthBar4;
    

    public void doDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            healthBar4.SetActive(false);
            Destroy(gameObject);
        }
        if (health == 75)
        {
            healthBar1.SetActive(false);
        }
        if (health == 50)
        {
            healthBar2.SetActive(false);
        }
        if (health == 25)
        {
            healthBar3.SetActive(false);
        }
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
