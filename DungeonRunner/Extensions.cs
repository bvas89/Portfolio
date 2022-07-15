using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    /// <summary>
    /// Checks for hostile targets in the surrounding area.
    /// </summary>
    /// <param name="sender">The object checking for targets.</param>
    /// <param name="fovLength">The length the object can see in front of it.</param>
    /// <param name="fovAngle">How width of its field of view</param>
    /// <param name="aggroRadius">The radius its immediate surroundings.</param>
    /// <param name="checkLOS">Must objects not be obstructed?</param>
    /// <param name="FOVisPriority">Is Field of View priority over immediate surroundings?</param>
    /// <param name="type">The type of object we are checking for.</param>
    /// <returns>The closest target of type, depending on priority.</returns>
    public static GameObject CheckForHostiles(NPCController sender, float fovLength, float fovAngle, float aggroRadius, bool checkLOS, bool FOVisPriority, System.Type type)
    {
        // Get all colliders in maximum sight range
        Collider[] array = Physics.OverlapSphere(sender.transform.position, fovLength);

        // Finds targets within a short range.
        GameObject CheckAggroRadius()
        {
            GameObject target = null;
            List<GameObject> aList = new List<GameObject>();
            foreach (var v in array)
            {
                // Exclude object casting Method
                if (v.gameObject != sender.gameObject)
                {
                    // If checking for IAlignment
                    if (type == typeof(IAlignment))
                    {
                        IAlignment a = v.GetComponent<IAlignment>() != null ?
                            v.GetComponent<IAlignment>() : null;

                        if (a != null)
                        {
                            // If this is Hostile..
                            if (a.alignment != sender.Alignment && a.alignment != Alignment.Neutral)
                            {
                                // Get distance from Sender
                                float distance = Vector3.Distance(sender.transform.position, v.transform.position);

                                if (distance < aggroRadius)
                                {
                                    // Raycast if checking Line of Sight
                                    if (checkLOS)
                                    {
                                        // If not obstructed
                                        if (!Physics.Linecast(sender.transform.position, v.transform.position))
                                        {
                                            aList.Add(v.gameObject);
                                        }
                                    }
                                    else //If not checking Line of Sight
                                    {
                                        aList.Add(v.gameObject);
                                    }
                                }
                            }
                        }
                    }
                    else // If looking for script NOT of type IDamageable
                    {
                        if (Vector3.Distance(sender.transform.position, v.transform.position) < aggroRadius)
                            aList.Add(v.gameObject);
                    }
                }
            }

            // Find closest target within radius.
            float dist = Mathf.Infinity;
            for (int i = 0; i < aList.Count; i++)
            {
                float nDist = Vector3.Distance(sender.transform.position, aList[i].transform.position);
                if (nDist < dist)
                {
                    dist = nDist;
                    target = aList[i].gameObject;
                }
            }
            // List to reference, if needed
            //aggroList = aList;

            // Returns closest target
            return target;
        }

        // Finds targets within a field of view.
        GameObject CheckFieldOfView()
        {
            GameObject target = null;
            List<GameObject> fList = new List<GameObject>();

            Collider[] col = Physics.OverlapSphere(sender.transform.position, fovLength);
            // For all colliders in array...
            for (int i = 0; i < col.Length; i++)
            {
                // ..Do they have the type we're looking for?
                var v = col[i].GetComponent(type) != null ? col[i].GetComponent(type) : null;

                // If they do.. (and are not sender)
                if (v != null && v.gameObject != sender.gameObject)
                {
                    // Where is the unit relative to Sender?
                    float distance = Vector3.Distance(sender.transform.position, col[i].transform.position);
                    Vector3 direction = col[i].transform.position - sender.transform.position;
                    float angle = Vector3.Angle(direction, sender.transform.forward);

                    // Are they within our Field of View?
                    if (angle < fovAngle / 2 && distance < fovLength)
                    {
                        // Are we checking for Line of Sight?
                        if (checkLOS)
                        {
                            if (!Physics.Linecast(sender.transform.position, v.transform.position))
                            {
                                fList.Add(v.gameObject);
                            }

                        }
                        else //Add when not checking LOS
                        {
                            fList.Add(v.gameObject);
                        }
                    }
                }
            }


            // Remove friendly/neutral targets from List
            for (int i = 0; i < fList.Count; i++)
            {
                // If we're looking for their alignment
                if (type == typeof(IAlignment))
                {
                    // Remove from list if friendly or neutral.
                    var a = fList[i].GetComponent<IAlignment>();
                    if (a.alignment == sender.alignment || a.alignment == Alignment.Neutral)
                    { fList.Remove(fList[i]); }
                }
            }

            // Find closest target within our Field of View
            float dist = Mathf.Infinity;
            for (int i = 0; i < fList.Count; i++)
            {
                // Find closest target
                float d = Vector3.Distance(sender.transform.position, fList[i].transform.position);
                if (d < dist)
                {
                    dist = d;
                    target = fList[i].gameObject;
                }
            }

            // List reference, if needed
            // fieldList = fList;

            // The closest target in our field of view
            return target;
        }

        if (FOVisPriority)
            return CheckFieldOfView() != null ? CheckFieldOfView() : CheckAggroRadius();
        else
            return CheckAggroRadius() != null ? CheckAggroRadius() : CheckFieldOfView();
    }

    //TODO: Restructure. Include position to check, check for Neutral bool, etc.
    /// <summary>
    /// Finds all damageable targets in an area.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="length"></param>
    /// <param name="radius"></param>
    /// <returns></returns>
    public static List<IDamageable> GetDamageables(NPCController owner, float length, float radius)
    {
        Collider[] ExtendedArray = Physics.OverlapSphere(owner.transform.position, radius);
        List<IDamageable> dList = new List<IDamageable>();

        foreach (var v in ExtendedArray)
        {
            v.TryGetComponent(out IDamageable dam); ; v.TryGetComponent(out IAlignment align);
            if (dam != null && align != null)
            {
                if (align.alignment != owner.alignment)
                {
                    Vector3 targetDir = v.transform.position - owner.transform.position;
                    float angle = Vector3.Angle(targetDir, owner.transform.forward);
                    float dist = Vector3.Distance(owner.transform.position, v.transform.position);

                    if (angle <= radius / 2 && dist <= length)
                    {
                        dList.Add(v.GetComponent<IDamageable>());
                    }
                }
            }
        }

        return dList;
    }

    /// <summary>
    /// Chooses which ATK to use, from a pool
    /// </summary>
    /// <param name="atkArray">The list of attacks to pool</param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static ATK WeightedRandom(List<ATK> atkArray, out int index)
    {
        int[] weights;
        Dictionary<ATK, int> priorities = new Dictionary<ATK, int>();

          foreach (var v in atkArray)
          if (!v.isCoolingDown)
           priorities.Add(v, v.prio);

        weights = new int[priorities.Count];
        priorities.Values.CopyTo(weights, 0);

        // Sum of Weights
        int SumOfWeights(int[] i)
        {
            int j = 0;
            foreach (int k in i)
                j += k;
            return j;
        }

        // Choose from pool
        int r = Random.Range(0, SumOfWeights(weights));
        int choice = r;
        for (int i = 0; i < weights.Length; i++)
        {
            if (r < weights[i])
            {
                index = i;
                return atkArray[i];
            }
            r -= weights[i];
        }

        index = choice;
        return atkArray[choice];
    }

    /// <summary>
    /// Allows the NPC to wander anywhere within a given radius.
    /// </summary>
    /// <param name="sender">The NPC to wander</param>
    /// <returns>The position to wander to.</returns>
    public static Vector3 WanderVision(NPCController sender)
    {
        Vector3 randDir = Random.insideUnitSphere * sender.WanderRadius;
        randDir += sender.StartPosition;
        Vector3 pos = new Vector3();
        UnityEngine.AI.NavMeshHit hit;

        if (UnityEngine.AI.NavMesh.SamplePosition(randDir, out hit, sender.WanderRadius, UnityEngine.AI.NavMesh.AllAreas))
        {            
            pos = hit.position;
        }
        return pos;
    }

    /// <summary>
    /// Determines where the Unit should move to to use its next ability
    /// </summary>
    /// <param name="sender">The Unit calling this function</param>
    /// <param name="ability">The ability to check distance for.</param>
    /// <returns>The position the Unit should move to.</returns>
    public static Vector3 AbilityPosition(NPCController sender, Ability ability)
    {
        Vector3 vPos = new Vector3();

        float distance = Vector3.Distance(sender.transform.position, sender.target.transform.position);

        // If Unit is within an acceptable distance
        if ( distance < ability.MaxLength && distance > ability.MinLength)
        {
            vPos = sender.transform.position;
        }
        // If too far away, move closer
        else if (distance > ability.MaxLength && distance > ability.MinLength)
        {
            vPos = sender.target.transform.position;
        }
        // If too close, move away
        else if (distance < ability.MinLength && distance < ability.MaxLength)
        {
            Vector3 direction = sender.target.transform.position - sender.transform.position;
            direction.Normalize();
            vPos = direction;
            // sender.agent.SetDestination(direction);
        }
        // If Min/Max are the deadzone
        else if (distance > ability.MaxLength && distance < ability.MinLength)
        {
            float dCloser = 0f; float dAway = 0f;
            dCloser = distance - ability.MaxLength;
            dAway = ability.MinLength - distance;

            //If the distance is even, randomize
            if(dCloser == dAway)
            {
                int i = Random.value < 0.5f ? 1 : -1;
                dCloser += i;
            }

            // If Closer is quicker
            if (dCloser < dAway)
            {
                vPos = sender.target.transform.position;
            }
            // If Away is quicker
            else if (dAway < dCloser)
            {
                Vector3 direction = sender.target.transform.position - sender.transform.position;
                direction.Normalize();
                vPos = direction;
            }
        }

        return vPos;
    }
}





















/* ----------------------------------------------------------------------------------
*/

#region Old References
/*
    /// <summary>
    /// Finds a target within the Unit's line of sight.
    /// </summary>
    /// <param name="sender">The object looking for targts</param>
    /// <param name="length">How far the Unit can see</param>
    /// <param name="radius">The "Field of View" angle</param>
    /// <param name="type">The type of object to look for (Collider, Script, Transform).</param>
    /// <returns>The closest target.</returns>
    public static GameObject CheckForClosestTarget(GameObject sender, float length, float radius, System.Type type)
    {
        GameObject returnTarget = null;
        Collider[] col = Physics.OverlapSphere(sender.transform.position, length);

        List<GameObject> pTargets = new List<GameObject>();

        for (int i = 0; i < col.Length; i++)
        {
            if (col[i].GetComponent(type) != null
                && col[i].gameObject != sender)
            {
                float distance = Vector3.Distance(sender.transform.position, col[i].transform.position);
                Vector3 direction = col[i].transform.position - sender.transform.position;
                float angle = Vector3.Angle(direction, sender.transform.forward);

                if (angle < radius / 2 && distance < length)
                {
                    pTargets.Add(col[i].gameObject);
                }
            }
        }

        float dist = Mathf.Infinity;

        if (pTargets.Count > 0)
        {
            for (int i = 0; i < pTargets.Count; i++)
            {
                float d = Vector3.Distance(sender.transform.position, pTargets[i].transform.position);

                if (d < dist)
                {
                    dist = d;
                    returnTarget = pTargets[i];
                }
            }
        }

        return returnTarget;
    }


    /// <summary>
    /// Does a melee-style attack to nearby Targets;
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="length"></param>
    /// <param name="radius"></param>
    /// <param name="owner"></param>
    public static void DoAttack(NPCController owner, float length, float radius)
    {
        Collider[] ExtendedArray = Physics.OverlapSphere(owner.transform.position, radius);
        List<IDamageable> newList = new List<IDamageable>();

        foreach (var v in ExtendedArray)
        {
            v.TryGetComponent(out IDamageable dam); ;v.TryGetComponent(out IAlignment align);
            if (dam != null  && align != null)
            {
                if(align.alignment != owner.alignment)
                {
                    Vector3 targetDir = v.transform.position - owner.transform.position;
                    float angle = Vector3.Angle(targetDir, owner.transform.forward);
                    float dist = Vector3.Distance(owner.transform.position, v.transform.position);

                    if (angle <= radius / 2 && dist <= length)
                    {
                        newList.Add(v.GetComponent<IDamageable>());
                    }
                }
            }
        }

        foreach (var v in newList)
        {
            owner.DealDamage(v);
            v.TakeDamage(1);
        }
    }

    

    /// <summary>
    /// Returns a list of Hostile units in the aggro radius.
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="radius"></param>
    /// <param name="checkLOS"></param>
    /// <param name="closestTarget"></param>
    /// <returns></returns>
    public static List<IDamageable> GetAggroRadius(NPCController owner, float radius, bool checkLOS, out GameObject closestTarget)
    {
        Collider[] ExtendedArray = Physics.OverlapSphere(owner.transform.position, radius);
        List<IDamageable> dList = new List<IDamageable>();

        foreach (var v in ExtendedArray)
        {
            v.TryGetComponent(out IDamageable dam); v.TryGetComponent(out IAlignment align);
            if (dam != null && align != null)
            {
                if (align.alignment != owner.alignment)
                {
                    float distance = Vector3.Distance(owner.transform.position, v.transform.position);

                    if(distance < radius)
                    {
                        if(checkLOS)
                        {
                            Vector3 dir = v.transform.position - owner.transform.position;
                            Ray ray = new Ray(owner.transform.position, dir);
                            RaycastHit hit;
                            if(Physics.Raycast(ray, out hit))
                            {
                                if (hit.transform.gameObject == v.gameObject)
                                {
                                    dList.Add(v.GetComponent<IDamageable>());
                                }
                            }
                        }
                        else
                        {
                            dList.Add(v.GetComponent<IDamageable>());
                        }
                    }
                }
            }
        }

        float dist = Mathf.Infinity;
        GameObject target = null;
        for (int i = 0; i < dList.Count; i++)
        {
            Transform tran = dList[i] as Transform;
            float nDist = Vector3.Distance(owner.transform.position, tran.position);
            if (nDist < dist)
            {
                dist = nDist;
                target = tran.gameObject;
                Debug.Log("Target: " + target.name);
            }
        }

        closestTarget = target;
        return dList;
    }

    public static void DrawCircle(this GameObject container, float radius, float lineWidth)
    {
        var segments = 360;
        var line = container.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments + 1;

        var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);
        }

        line.SetPositions(points);
    }

    
*/
#endregion