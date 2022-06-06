using UnityEngine;

namespace Feathersoft.Tools
{
    public static class PoseExtensions
    {
        /// <summary>
        /// Transforms a pose from world space to local space.
        /// </summary>
        public static Pose InverseTransformPose(this Pose source, Pose target)
        {
            Quaternion inverseTarget = Quaternion.Inverse(target.rotation);
            Vector3 directionFromSourceToTarget = target.position - source.position;
            Vector3 positionOffset = inverseTarget * (directionFromSourceToTarget);
            Quaternion rotationOffset = Quaternion.Inverse(inverseTarget * source.rotation);
            return new Pose(positionOffset, rotationOffset);
        }

        /// <summary>
        /// Used to get a position offset between two Pose.
        /// </summary>
        /// <returns>The local position from Pose to Pose.</returns>
        public static Vector3 ToLocalPosition(this Pose from, Pose to) =>
            Quaternion.Inverse(to.rotation) * (to.position - from.position);

        /// <summary>
        /// Used to get a rotation offset between two Pose.
        /// </summary>
        /// <returns>The local rotation from Pose to Pose.</returns>
        public static Quaternion ToLocalRotation(this Pose from, Pose to) =>
            Quaternion.Inverse(Quaternion.Inverse(to.rotation) * from.rotation);

        /// <summary>
        /// Transforms a pose from local space to world space.
        /// </summary>
        public static Pose TransformPose(this Pose source, Pose offsetFromSourceToTarget)
        {
            Quaternion rotation = source.rotation * offsetFromSourceToTarget.rotation;
            Vector3 position = source.position + rotation * offsetFromSourceToTarget.position;
            return new Pose(position, rotation);
        }

        /// <summary>
        /// Converts a <see cref="Transform"/> to a <see cref="Pose"/>.
        /// </summary>
        public static Pose ToPose(this Transform transform) => new Pose(transform.position, transform.rotation);
    }
}