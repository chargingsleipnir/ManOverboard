using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour {

    protected int health;
    private EnemySet enemies;

    public abstract void TakeDamage(int damage);

    protected LevelManager lvlMngr;
    public LevelManager LvlMngr { set { lvlMngr = value; } }

    private void Awake() {
        enemies = Resources.Load<EnemySet>("ScriptableObjects/SpriteSets/EnemySet");
        enemies.Add(this);
    }
}
