/* OlinHandController.cs
 * 
 * WORK IN PROGRESS
 * 
 * This script is a assigned to each hand and handles the functionality of
 * holding weapons or PickMeUp objects. Eventually for buttons as well.
 * 
 */
using UnityEngine;

namespace Olin.Input
{
    public class OlinHandController : MonoBehaviour
    {
        //Components
        private OlinInput oInput;

        [Header("Hand")]
        [Tooltip("Which hand is this?")]
        [SerializeField] private HandSide _handSide;

        [Tooltip("The layer(s) the hand is allowed to interact with.")]
        [SerializeField] private LayerMask _layerMask;

        [Tooltip("The amount of time before the button can be checked again.")]
        [SerializeField] private const float m_Cooldown = 0.5f;
        private float _cooldownDelta;
        private bool _isOffCooldown;

        [Tooltip("The radius to check when the button is used.")]
        [SerializeField] private float _radius =  1.0f;

        [Tooltip("The item being held.")]
        public GameObject heldItem;

        [Tooltip("The other hand this one is paired with.")]
        public OlinHandController otherHand;

        //Strings
        private const string s_PickMeUp = "PickMeUp";
        private const string s_Weapon = "Weapon";

        private void Awake()
        {
            oInput = GetComponentInParent<OlinInput>();
        }
        private void Start()
        {

        }
        private void FixedUpdate()
        {
            CheckForInput();
            HoldingItem();
        }

        void HoldingItem()
        {
            if (heldItem != false)
            {
                heldItem.transform.SetParent(transform);
                heldItem.transform.position = transform.position;
            }
        }

        void UseEmptyHand()
        {
            //Check for objects within a sphere
            Collider[] col =
                Physics.OverlapSphere(transform.position, _radius, _layerMask);

            float distance = Mathf.Infinity;
            for (int i = 0; i < col.Length; i++)
            {
                if (col[i] != otherHand.heldItem)
                {
                    float d = Vector3.Distance(transform.position, col[i].transform.position);
                    if (d < distance)
                    {
                        heldItem = col[i].gameObject;
                    }
                }
            }
        }

        //Handles the hand Behavior
        void HandleUseOfHand()
        {
            //Resets the cooldown timer
            _cooldownDelta = m_Cooldown;

            if (heldItem != null)
            {
                if (heldItem.CompareTag(s_PickMeUp))
                {

                    heldItem.gameObject.transform.SetParent(null) ;
                    heldItem = null;
                }
            }
            else
                UseEmptyHand();

            //Turns off boolean
            if (_handSide == HandSide.Left)
                oInput.useLeft = false;
            if (_handSide == HandSide.Right)
                oInput.useRight = false;
        }

        //Determines which Input to check for
        void CheckForInput()
        {
            //Is the ability off cooldown?
            _isOffCooldown = _cooldownDelta <= 0.0f ? true : false;

            //Prepares to use the ability
            if (_isOffCooldown)
            {
                //Coordinates Input and the ability
                if (_handSide == HandSide.Left)
                    if (oInput.useLeft)
                        HandleUseOfHand();

                if (_handSide == HandSide.Right)
                    if (oInput.useRight)
                        HandleUseOfHand();
            }

            //Countdown if still on cooldown.
            if (!_isOffCooldown)
                _cooldownDelta -= Time.deltaTime;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
    
    //Which side of the body is the hand on
    public enum HandSide { Left, Right }
}
