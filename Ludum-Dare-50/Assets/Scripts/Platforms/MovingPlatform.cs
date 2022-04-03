using DG.Tweening;
using UnityEngine;

namespace Platforms
{
    public class MovingPlatform : MonoBehaviour
    {
        [SerializeField]
        private float speed;

        [SerializeField]
        private Vector2 endPosRelativ;

        private void Start()
        {
            Vector3 endPos = new Vector3(x: transform.position.x + endPosRelativ.x, y: transform.position.y + endPosRelativ.y);
            transform.DOMove(endValue: endPos, duration: speed).SetEase(Ease.InOutSine).SetSpeedBased().SetLoops(loops: -1,loopType: LoopType.Yoyo);
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            col.transform.SetParent(transform);
        }

        private void OnCollisionExit2D(Collision2D col)
        {
            col.transform.SetParent(null);
        }

        private void OnDrawGizmos()
        {
            Vector3 position = transform.position;
            Vector3 endPos = new Vector3(x: position.x + endPosRelativ.x, y: position.y + endPosRelativ.y);
            Gizmos.DrawSphere(center: endPos,radius: 0.4f);
        }
    }
}
