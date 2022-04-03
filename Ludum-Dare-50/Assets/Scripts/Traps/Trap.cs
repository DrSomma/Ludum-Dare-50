using System;
using UnityEngine;

namespace Traps
{
    public class Trap : MonoBehaviour
    {
        protected void OnCollisionEnter2D(Collision2D col)
        {
            if (!col.gameObject.CompareTag("Player"))
            {
                return;
            }

            KillPlayer();
            ThisTrapOnHit();
        }

        private void KillPlayer()
        {
            GameManager.Instance.UpdateGameState(GameState.Dead);
        }

        protected virtual void ThisTrapOnHit()
        {
            //Todo: effect
        }
    }
}
