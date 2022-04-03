using System;
using UnityEngine;

namespace Traps
{
    public class Trap : MonoBehaviour
    {
        public static event Action OnTrapHit;
    
        protected void OnCollisionEnter2D(Collision2D col)
        {
            Debug.Log("lol");
            if (!col.gameObject.CompareTag("Player"))
            {
                return;
            }

            ThisTrapOnHit();
            
            OnTrapHit?.Invoke();
        }

        protected virtual void ThisTrapOnHit()
        {
            //Todo: effect
        }
    }
}
