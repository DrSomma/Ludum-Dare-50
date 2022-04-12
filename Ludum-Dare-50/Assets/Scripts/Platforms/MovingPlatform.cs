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
        
        private Vector3 CalcEndPos => new Vector3(x: transform.position.x + endPosRelativ.x, y: transform.position.y + endPosRelativ.y);
        private Vector3 _endPos;
        private void Start()
        {
            _endPos = CalcEndPos;
            transform.DOMove(endValue: _endPos, duration: speed).SetEase(Ease.InOutSine).SetSpeedBased().SetLoops(loops: -1,loopType: LoopType.Yoyo);
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            col.transform.SetParent(transform);
        }

        private void OnCollisionExit2D(Collision2D col)
        {
            col.transform.SetParent(null);
        }

        #region Debug
        private SpriteRenderer _renderer; 
        private void OnDrawGizmos()
        {
            if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();
            Gizmos.DrawWireCube(center: Application.isPlaying ? _endPos : CalcEndPos ,_renderer.size);
        }
        #endregion
       
    }
}
