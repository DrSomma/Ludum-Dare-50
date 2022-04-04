using UnityEngine;

namespace Player
{ 
    [RequireComponent(typeof(PlayerLifeController))]
    public class PlayerParticleController : MonoBehaviour
    {
        [SerializeField]
        private GameObject particleHurt;
        
        [SerializeField]
        private PlayerLifeController playerLifeController;
        
        public void Start()
        {
            playerLifeController.OnPlayerHurt += OnPlayerHurt;
        }

        private void OnPlayerHurt()
        {
            GameObject ps = Instantiate(particleHurt);
            ps.transform.position = playerLifeController.transform.position;
        }
    }
}
