using UnityEngine;

namespace Feathersoft.XRI.Hands
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsBasedHands : MonoBehaviour
    {
        public Transform handTarget;
        public float velocityMultiplier = 500;

        private Transform _cachedTransform;
        private Rigidbody _handModelRigidbody;

        private void Awake()
        {
            _handModelRigidbody = GetComponent<Rigidbody>();
            _cachedTransform = transform;
            _handModelRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            _handModelRigidbody.useGravity = false;
        }

        private void FixedUpdate()
        {
            float deltaTime = Time.deltaTime;
            Vector3 directionToTarget = handTarget.position - _cachedTransform.position;
            _handModelRigidbody.velocity = directionToTarget * velocityMultiplier * deltaTime;
            _cachedTransform.rotation = handTarget.rotation;
        }
    }
}