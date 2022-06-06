using UnityEngine;
using Feathersoft.Tools;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

namespace Feathersoft.XRI.Hands
{
    public class FlipAttachOnGrab : MonoBehaviour
    {
        private void Awake()
        {
            XRBaseInteractor interactor = GetComponent<XRBaseInteractor>();
            interactor.selectEntered.AddListener(SelectEntered);
            interactor.selectExited.AddListener(SelectExited);
        }

        private void SelectEntered(SelectEnterEventArgs args)
        {
            FlipAttachTransform(args);
        }

        private void SelectExited(SelectExitEventArgs args)
        {
            FlipAttachTransform(args);
        }

        private void FlipAttachTransform(BaseInteractionEventArgs args)
        {
            Pose baseTransform = args.interactableObject.transform.ToPose();
            Pose attachTransform = args.interactableObject.GetAttachTransform(args.interactorObject).ToPose();

            // Get the local pose from attach to base.
            Pose localPose = attachTransform.InverseTransformPose(baseTransform);


            // Mirror the local pose on the X axis.
            localPose.position.x *= -1; // flip local position

            localPose.rotation.ToAngleAxis(out float angle, out Vector3 axis); // flip local rotation
            axis.z *= -1;
            axis.y *= -1;
            localPose.rotation = Quaternion.AngleAxis(angle, axis);

            Pose newKeyPose = attachTransform.TransformPose(localPose);
            Pose localNewKeyToAttach = newKeyPose.InverseTransformPose(attachTransform);
            Pose newAttachPose = baseTransform.TransformPose(localNewKeyToAttach);

            args.interactableObject.GetAttachTransform(args.interactorObject).SetPositionAndRotation(newAttachPose.position, newAttachPose.rotation);
        }
    }
}