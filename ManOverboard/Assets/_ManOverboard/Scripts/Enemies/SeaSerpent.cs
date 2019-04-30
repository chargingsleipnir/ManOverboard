using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaSerpent : Enemy {

    private void Start() {
        health = 2000;
    }

    public override void TakeDamage(int damage) {
        health -= damage;

        if (health <= 0)
            lvlMngr.EnemyKilled(this);
    }    
}
