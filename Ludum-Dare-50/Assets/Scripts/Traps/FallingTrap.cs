using System;
using System.Collections;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Traps
{
    public class FallingTrap : Trap
    {
        [SerializeField]
        private float speedDown = 20f;

        [SerializeField]
        private float speedUp = 20f;
        
        [SerializeField]
        private float delayBetween = 2f;
        
        [SerializeField]
        private float delayGround = 1f;
        
        [SerializeField]
        private LayerMask layerMask;
        
        [SerializeField]
        private Collider2D trapCollider;
        
        private const float DistanceRaycast = 10f;
        
        private Vector3 _target;
        private Vector3 _startPos;
        private bool _doMove;
        
        private void Start()
        {
            _target = GetTargetPos();
            _startPos = transform.position;

            StartTween();
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.V))
                StartTween();
        }

        private void StartTween()
        {
            _doMove = true;
            StartCoroutine(DoTween());
        }
        
        IEnumerator DoTween()
        {
            while (_doMove)
            {
                transform.DOMove(_target, speedDown).SetSpeedBased().SetEase(Ease.OutBounce);
                yield return new WaitForSeconds(delayGround);
                transform.DOMove(_startPos, speedUp).SetSpeedBased().SetEase(Ease.InOutSine);
                yield return new WaitForSeconds(delayBetween);
            }
            
            yield return null;
        }
        
        protected override void ThisTrapOnHit(Vector2 vector2)
        {
            //Todo: effect
        }

        private void OnDrawGizmos()
        {
            if(trapCollider == null)
                return;
            _target = GetTargetPos();
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(_target,Vector3.one * 0.2f);
        }

        private Vector2 GetTargetPos()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down,DistanceRaycast,layerMask);

            if (hit.collider == null)
            {
                Debug.LogError("FallingTrap cant find target");
                return Vector2.down * 5f;
            }

            float offsetY = trapCollider.bounds.size.y / 2;
            return new Vector2(transform.position.x, hit.point.y + offsetY);
        }
    }
}
