/*  AntController.css
 * 
 * Controls the basic functions of the Ant
 * 1. Movement
 * 2. Biting
 * 3. Death
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.U2D.Animation;

namespace Ant
{ 
    [RequireComponent(typeof(NavMeshAgent))]
    public class AntController : MonoBehaviour
    {
        [Tooltip("The scriptable object that houses the stats")]
        public Ant ant;

        [Tooltip("The ant's stats.")]
        [SerializeField] private Stats stats;

        [Tooltip("Toasty! Populates on Start.")]
        [SerializeField] private GameObject _toasty;

        [Tooltip("The Ant's current target.")]
        public GameObject target;

        [Tooltip("The spawn location gameObject. Deprecated")]
        [SerializeField] private GameObject antHills;

        [Tooltip("The anthill to spawn when returning a crumb.")]
        public GameObject antHill;

        [Tooltip("Is the ant currently holding a crumb?")]
        public bool hasCrumb = false;

        [Tooltip("The Pheramone gameObject to spawn when leaving a trail.")]
        public GameObject pheramone;

        //Can the ant move?
        private bool canMove = true;

        [Tooltip("The distance from Ant to Target. Mostly used for testing.")]
        public float _distance;

        //Components
        private NavMeshAgent _agent;
        private SpriteRenderer _sr;
        private Animator _animator;
        private bool _hasAnimator;

        //Animation IDs
        private string _anim_SpeedMultipler = "SpeedMultiplier";
        private string _anim_isBiting = "isBiting";

        //Strings
        private string s_Toasty = "Toasty";
        private string s_Pheramone = "Pheramone";

        /// <summary>
        /// Sends a signal when the ant is Squashed.
        /// </summary>
        public delegate void AntDiedEvent();
        public static event AntDiedEvent OnAntDeath;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _toasty = GameObject.FindGameObjectWithTag("Toasty");
            _sr = GetComponentInChildren<SpriteRenderer>();
            _hasAnimator =
                transform.GetChild(0).TryGetComponent<Animator>(out _animator);
        }

        void Start()
        {
            //Sets the NavMesh variables to the Ant Stats.
            InitializeAgent();

            gameObject.name = ant.stats.name;
        }
        void Update()
        {
            _distance = Vector3.Distance(transform.position, _agent.destination);
        }
        private void FixedUpdate()
        {
            Move();  
        }

        private void Move()
        {
            if (_agent.enabled == true)
                if (canMove)
                    if (hasCrumb)
                        MoveOutFromToasty(); //Finds location away from Toasty.
                    else
                        if (Target() != null)
                            _agent.SetDestination(Target().transform.position); //Move toward Target.
                        else
                            StartCoroutine(MoveRandom()); //Wander around.

            //Set animator to velocity percentage of top speed.
            float percentage = _agent.velocity.magnitude / _agent.speed;
            _animator.SetFloat(_anim_SpeedMultipler, percentage);
        }

        /// <summary>
        /// Moves away from Toasty, in the opposite direction it was facing.
        /// </summary>
        void MoveOutFromToasty()
        {
            canMove = false;
            Vector3 vec = -transform.forward * 15f;

            GameObject hill = Instantiate(antHill, vec, Quaternion.identity);
            Destroy(hill, 10f);

            _agent.SetDestination(hill.transform.position);
        }

        /// <summary>
        /// Casts a sphere to get surrounding objects; determines the Target.
        /// Checks for Toasty first, then checks for Pheramones.
        /// </summary>
        /// <returns></returns>
        GameObject Target()
        {
            Collider[] col = Physics.OverlapSphere(transform.position, ant.movement.searchRadius);
            float distance = Mathf.Infinity;
            GameObject g = null;

            foreach (Collider c in col)
            {
                //Sets Toasty as target, if detected.
                if (c.gameObject.CompareTag(s_Toasty))
                {
                    g = c.gameObject;
                }
                else
                {
                    //Otherwise, get closest Pheramone
                    if (c.gameObject.CompareTag(s_Pheramone))
                    {
                        float d = Vector3.Distance(c.transform.position, _toasty.transform.position);

                        if (d < distance)
                        {
                            distance = d;
                            float dist = Vector3.Distance(transform.position, _toasty.transform.position);
                            if(distance < dist)
                                g = c.gameObject;
                        }
                    }
                }    
            }

            return g;
        }

        /// <summary>
        /// If the Ant has no target, wanders around.
        /// </summary>
        /// <returns></returns>
        IEnumerator MoveRandom()
        {
            canMove = false;
            _agent.SetDestination(RandomNavMeshLocation());
            yield return new WaitUntil(() => IsCloseEnough());
            yield return new WaitForSeconds(ant.movement.WaitTime());
            canMove = true;
        }

        /// <summary>
        /// Deprecated. Finds the closes preset AntHill to move to.
        /// </summary>
        /// <returns></returns>
        GameObject ClosestAntHill()
        {
            Transform[] obj = antHills.GetComponentsInChildren<Transform>();
            float distance = Mathf.Infinity;
            GameObject o = null;
            foreach (Transform g in obj)
            {
                float d = Vector3.Distance(transform.position, g.transform.position);
                if (d < distance)
                {
                    distance = d;
                    o = g.gameObject;
                }
            }
            return o;
        }

        /// <summary>
        /// Releases a pheramone every X seconds while the Ant has a crumb.
        /// </summary>
        /// <returns></returns>
        IEnumerator ReleasePheramone()
        {
            while (hasCrumb)
            {
                GameObject p = Instantiate(pheramone, transform.position, Quaternion.identity, null);
                p.GetComponent<PheramoneHandler>().duration = ant.movement.pheramoneDuration;
                yield return new WaitForSeconds(ant.movement.pheramoneSpawnTime);
            }
        }

        /// <summary>
        /// If the NavMesh agent is close enough to its location. Determined by me.
        /// </summary>
        /// <returns></returns>
        bool IsCloseEnough()
        {
            return _distance < 1.0f;
        }

        //Finds a random location on the NavMesh within a radius.
        private Vector3 RandomNavMeshLocation()
        {
            float radius = ant.movement.movementRadius;
            Vector3 randDir = Random.insideUnitSphere * radius;
            randDir += transform.position;
            NavMeshHit hit;
            Vector3 finalPos = Vector3.zero;

            if (NavMesh.SamplePosition(randDir, out hit, radius, _agent.areaMask))
            {
                finalPos =
                    Vector3.Lerp(hit.position, _toasty.transform.position,
                    ant.movement.gravity);
            }
            return finalPos;
        }

        //Set the NavMeshAgent variables to Ant stats
        void InitializeAgent()
        {
            _agent.speed = ant.movement.speed;
            _agent.angularSpeed = ant.movement.rotationSpeed;
            _agent.acceleration = ant.movement.acceleration;

            stats.hp = ant.stats.hp;

            GetComponentInChildren<SpriteLibrary>().spriteLibraryAsset =
                ant.visuals.spriteLibraryAsset;
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Toasty"))
            {
                StartCoroutine(Bite());
            }
            if(other.CompareTag("Ant Hill"))
            {
                DropCrumb();
            }
        }

        /// <summary>
        /// Drops the crumb and stops the release of Pheramones.
        /// </summary>
        void DropCrumb()
        {
            hasCrumb = false;
            StopCoroutine(ReleasePheramone());
            canMove = true;
        }

        /// <summary>
        /// Bites Toasty and starts the crumb returnal process.
        /// </summary>
        /// <returns></returns>
        IEnumerator Bite()
        {
            _agent.enabled = false;
            _animator.SetBool(_anim_isBiting, true);
            yield return new WaitForSeconds(ant.combat.BiteTime());
            hasCrumb = true;
            StartCoroutine(ReleasePheramone());
            _animator.SetBool(_anim_isBiting, false);
            _agent.enabled = true;
        }

        /// <summary>
        /// Starts the TakeDamage Coroutine.
        /// </summary>
        /// <param name="damage">How much damage to be dealt to the ant.</param>
        public void TakeDamage(float damage)
        {
            StartCoroutine(TakeDamageHandler(damage));
        }

        /// <summary>
        /// Handles the taking of damage.
        /// </summary>
        /// <param name="damage">The damage to receive.</param>
        /// <returns></returns>
        IEnumerator TakeDamageHandler(float damage)
        {
            stats.hp -= damage;
            _sr.color = Color.red;
            if (stats.hp <= 0)
                StartCoroutine(OnDeath());
            yield return new WaitForSeconds(.15f);
            _sr.color = Color.white;
        }

        /// <summary>
        /// Announces the Ant's death and deactivates it.
        /// </summary>
        /// <returns></returns>
        IEnumerator OnDeath()
        {
            if (OnAntDeath != null)
                OnAntDeath();

            gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
            //Destroy(gameObject);
        }

        private void OnDrawGizmosSelected()
        {
            //Shows the ant's target search radius.
            Gizmos.DrawWireSphere(transform.position, ant.movement.searchRadius);
        }
    }

    /// <summary>
    /// The individual ant's stats.
    /// </summary>
    [System.Serializable]
    public class Stats
    {
        public float hp;
    }
}
