using Feathersoft.Tools;
using UnityEngine;

namespace Feathersoft.XRI.Hands
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicsBasedHands : MonoBehaviour
    {
        public Transform handTarget;
        public float velocityMultiplier = 100;

        [SerializeField]
        private bool usePhysicsRotation;

        private Transform _cachedTransform;
        private Rigidbody _handModelRigidbody;

        private void Awake()
        {
            _handModelRigidbody = GetComponent<Rigidbody>();
            _cachedTransform = transform;

            if (!usePhysicsRotation)
                _handModelRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        private void FixedUpdate()
        {
            float deltaTime = Time.deltaTime;
            Vector3 directionToTarget = handTarget.position - _cachedTransform.position;
            _handModelRigidbody.velocity = directionToTarget * velocityMultiplier * deltaTime;

            if (usePhysicsRotation)
            {
                Quaternion rotationToTarget = _cachedTransform.ToPose().ToLocalRotation(handTarget.ToPose());
                rotationToTarget.ToAngleAxis(out _, out Vector3 axis);
                _handModelRigidbody.angularVelocity = axis * velocityMultiplier * deltaTime;
            }
            else
                _cachedTransform.rotation = handTarget.rotation;
        }
    }
}