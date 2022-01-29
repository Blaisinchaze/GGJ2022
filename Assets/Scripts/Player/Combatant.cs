using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combatant : Device
{
   // Components
   public Renderer myRenderer { get; private set; }
   public Transform myTransform { get; private set; }
   
   // Combat stats
   
   public bool isAlive;
   public int health;
   public int maxHealth;
   
   /// <summary>
   /// Damage dealt to other combatants 
   /// </summary>
   public int outDmg; 
   
   // For testing and messing around
   
   [Header("Debug Toggles")] 
   public bool kill;
   public bool invulnerable;

   protected void Awake()
   {
      myRenderer = GetComponent<Renderer>();
      myTransform = GetComponent<Transform>();
      if (health > maxHealth) maxHealth = health;

   }
   
   internal override void Update()
   {
      base.Update();
      CheckToggles();
      CheckHealth();
   }

   public void Reset()
   {
      if (maxHealth > 0)
         health = maxHealth;
      else
      {
         maxHealth = 5;
         health = 5;
      }

      isAlive = true;
   }
   
   internal void CheckToggles()
   {
      if (kill) Die();
      if (invulnerable) health = maxHealth;
   }

   internal void CheckHealth()
   {
      if (health <= 0 && !invulnerable)
         Die();
   }

   /// <summary>
   /// Modify this to kill the unit. By default, destroys gameObject
   /// </summary>
   public virtual void Die()
   {
      Destroy(gameObject);
   }
   
   /// <summary>
   /// What happens when I'm hit by a projectile or melee
   /// </summary>
   public virtual void GetHit(int damageValue)
   {
      if (invulnerable) return;
      health -= damageValue;
   }

   public void GetHit(Combatant attacker)
   {
      if (invulnerable) return;
      health -= attacker.outDmg;
   }

}