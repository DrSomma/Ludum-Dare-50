using System;
using UnityEngine;

namespace Traps
{
    public class Trap : MonoBehaviour
    {
        private bool _wasTriggert;
        
        protected void OnCollisionEnter2D(Collision2D col)
        {
            if (!col.gameObject.CompareTag("Player"))
            {
                return;
            }
            if(_wasTriggert)
                return;
            _wasTriggert = true;

            PlayerLifeController player = col.gameObject.GetComponent<PlayerLifeController>();
            
            HitPlayer(player);
            ThisTrapOnHit(col.GetContact(0).point);
        }

        private void HitPlayer(PlayerLifeController playerLifeController)
        {
            playerLifeController.HurtPlayer();
        }

        protected virtual void ThisTrapOnHit(Vector2 pos)
        {

        }
    }
}
