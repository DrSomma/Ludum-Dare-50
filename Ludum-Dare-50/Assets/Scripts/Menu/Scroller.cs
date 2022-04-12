using UnityEngine;
using UnityEngine.UI;

namespace Menu
{
    public class Scroller : MonoBehaviour
    {
        [SerializeField]
        private RawImage Img;

        [SerializeField]
        private float X, Y;

        private void Update()
        {
            Img.uvRect = new Rect(position: Img.uvRect.position + new Vector2(x: X, y: Y) * Time.deltaTime, size: Img.uvRect.size);
        }
    }
}