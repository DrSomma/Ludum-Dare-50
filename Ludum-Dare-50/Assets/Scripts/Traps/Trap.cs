using System;
using UnityEngine;

namespace Traps
{
    public class Trap : MonoBehaviour
    {
        [SerializeField]
        private GameObject particleSystem;
        
        protected void OnCollisionEnter2D(Collision2D col)
        {
            if (!col.gameObject.CompareTag("Player"))
            {
                return;
            }

            KillPlayer();
            ThisTrapOnHit(col.GetContact(0).point);
        }

        private void KillPlayer()
        {
            GameManager.Instance.UpdateGameState(GameState.Dead);
        }

        protected virtual void ThisTrapOnHit(Vector2 pos)
        {
            var ps = Instantiate(particleSystem);
            ps.transform.position = pos;

        }
    }
}
