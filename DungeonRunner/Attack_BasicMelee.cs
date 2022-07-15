/* Attack_BasicMelee
 * A basic melee attack.
 * 
 * Attacks/Abilities are prefabs instantiated in world space and are then destroyed.
 * This basic attack gets nearby damageable targets and deals damage.
 * 
 * By instantiating abilities, this allows me to more easily work with projectiles
 * or spells that have their own logic.
 * -For instance, a ball of fire that rises in the air slowly and shoots its own fireballs.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_BasicMelee : Ability
{
    // List of objects to be damaged.
    List<IDamageable> _dList;

    private void Start()
    {
        _dList = Extensions.GetDamageables(Owner, MaxLength, Radius);

        foreach(var v in _dList)
        {
            v.TakeDamage(DamageToSend());
        }

        Destroy(gameObject, Cooldown);
    }

    //TODO: Make more robust. For now, send just owner base attack.
    public float DamageToSend()
    {
        float x = Owner.BaseAttack;
        return x;
    }
}
