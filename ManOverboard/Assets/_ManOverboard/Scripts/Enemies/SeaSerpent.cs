using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaSerpent : Enemy {

    private void Start() {
        health = 1000;
    }

    public override void TakeDamage(int damage) {
        health -= damage;

        if (health <= 0)
            lvlMngr.EnemyKilled(this);
    }    
}
