using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Feathersoft.XRI.Whiteboard
{
    [RequireComponent(typeof(XRGrabInteractable))]
    public class MarkerInteractable : MonoBehaviour
    {
        public Color markerColor;
        public int thickness;
        public float rayDistance = 0.01f;
        public Renderer tipRenderer;
        public Transform tipTransform;

        private bool _isGrabbed;

        private void Awake()
        {
            if (tipRenderer != null)
                tipRenderer.material.color = markerColor;
            var interactable = GetComponent<XRBaseInteractable>();
            interactable.selectEntered.AddListener(SelectEntered);
            interactable.selectExited.AddListener(SelectExited);
        }

        private void SelectEntered(SelectEnterEventArgs args) => _isGrabbed = true;
        private void SelectExited(SelectExitEventArgs args) => _isGrabbed = false;

        private void Update()
        {
            if (!_isGrabbed)
                return;

            Ray ray = new Ray(tipTransform.position, tipTransform.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, rayDistance, 1 << 0, QueryTriggerInteraction.Ignore))
                return;

            Whiteboard whiteboard = hit.collider.GetComponent<Whiteboard>();
            if (whiteboard == null)
                return;

            whiteboard.DrawWorldPosition(gameObject, hit.point, markerColor, thickness);
        }
    }
}