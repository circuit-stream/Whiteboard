using System.Collections.Generic;
using UnityEngine;

namespace Feathersoft.XRI.Whiteboard
{
    public class Whiteboard : MonoBehaviour
    {
        [SerializeField]
        public Material _targetMat;
        [SerializeField]
        private int _textureYResolution;
        [SerializeField]
        private TextureFormat _textureFormat;

        private Texture2D _drawingTexture;
        private Vector2Int _pixelSize;
        private Bounds _colliderBounds;
        private Transform _cachedTransform;

        private readonly List<DrawAction> _drawActions = new List<DrawAction>();

        private void Awake()
        {
            _cachedTransform = transform;
            Bounds bounds = GetComponent<Collider>().bounds;
            _colliderBounds = new Bounds(
                _cachedTransform.rotation * (bounds.center - _cachedTransform.position),
                _cachedTransform.rotation * bounds.size);

            float colliderAspectRatio = _colliderBounds.size.x / _colliderBounds.size.y;
            _pixelSize = new Vector2Int((int)(_textureYResolution * colliderAspectRatio), _textureYResolution);

            _drawingTexture = new Texture2D(_pixelSize.x, _pixelSize.y, _textureFormat, false);
            _targetMat.SetTexture("_BaseMap", _drawingTexture);

            Color32[] white = _drawingTexture.GetPixels32();
            for (int i = 0; i < white.Length; i++)
                white[i] = Color.clear;

            _drawingTexture.SetPixels32(white);
            _drawingTexture.Apply(false);
        }

        private void Update()
        {
            for (int i = _drawActions.Count - 1; i >= 0; i--)
            {
                DrawAction action = _drawActions[i];
                action.Counter++;
                if (action.Counter >= 2)
                    _drawActions.RemoveAt(i);
            }
        }

        private DrawAction GetDrawAction(GameObject trackedObject, Vector2Int currentCoords)
        {
            foreach (DrawAction item in _drawActions)
            {
                if (item.GameObject == trackedObject)
                    return item;
            }

            DrawAction action = new DrawAction(trackedObject, currentCoords);
            _drawActions.Add(action);
            return action;
        }

        public void DrawWorldPosition(GameObject trackedObject, Vector3 worldPoint, Color color, int thickness)
        {
            worldPoint = _cachedTransform.rotation * (worldPoint - _cachedTransform.position);
            Vector3 colliderMax = _colliderBounds.max;

            // Convert contact to collider bound space.
            float distanceBetweenXPoints = colliderMax.x - worldPoint.x;
            float xRatio = distanceBetweenXPoints / _colliderBounds.size.x;
            int xCoords = (int)(xRatio * _pixelSize.x);

            distanceBetweenXPoints = colliderMax.y - worldPoint.y;
            float yRatio = distanceBetweenXPoints / _colliderBounds.size.y;
            int yCoords = _pixelSize.y - (int)(yRatio * _pixelSize.y);

            xCoords -= thickness;
            yCoords -= thickness;
            int thickSize = thickness * thickness + 1;
            Vector2Int currentCoords = new Vector2Int(xCoords, yCoords);

            DrawAction currentAction = GetDrawAction(trackedObject, currentCoords);

            // Lerp between draws
            if (currentAction.Counter <= 1)
            {
                // Convert to Vector2 (float)
                Vector2 currentPoint = currentCoords;
                Vector2 previousPoint = currentAction.PreviousPixel;

                float distanceBetweenPoints = Vector2.Distance(currentPoint, previousPoint); // max ~20?
                const float maxDistance = 20;
                float ratio = 0;
                float ratioSpeed = Mathf.Clamp(((maxDistance - distanceBetweenPoints) / maxDistance) * 0.1f, 0.01f, 1);

                // Perform pixel smooth update.
                while (ratio <= 1)
                {
                    ratio += ratioSpeed;
                    Vector2 pointAlongPath = Vector2.Lerp(previousPoint, currentPoint, ratio);
                    int tempXCoords = (int)pointAlongPath.x - thickness;
                    int tempYCoords = (int)pointAlongPath.y - thickness;

                    SetTextureColors(tempXCoords, tempYCoords, thickSize, color);
                }
            }

            currentAction.PreviousPixel = currentCoords;
            currentAction.Counter = 0;
            _drawingTexture.Apply(false);
        }

        private void SetTextureColors(int xCoords, int yCoords, int thickSize, Color color)
        {
            var colors = _drawingTexture.GetPixels(xCoords, yCoords, thickSize, thickSize);
            for (int i = 0; i < colors.Length; i++)
                colors[i] = color;

            _drawingTexture.SetPixels(xCoords, yCoords, thickSize, thickSize, colors);
        }
    }
}