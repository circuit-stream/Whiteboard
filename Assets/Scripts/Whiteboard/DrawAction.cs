using UnityEngine;

namespace Feathersoft.XRI.Whiteboard
{
    public class DrawAction
    {
        public Vector2Int PreviousPixel;
        public GameObject GameObject;
        public int Counter;

        public DrawAction(GameObject gameObject, Vector2Int currentPixel)
        {
            GameObject = gameObject;
            PreviousPixel = currentPixel;
        }
    }
}