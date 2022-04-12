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

        [SerializeField]
        private ParticleSystem dust;
        
        public void Start()
        {
            playerLifeController.OnPlayerHit += OnPlayerHit;
        }

        private void OnPlayerHit()
        {
            GameObject ps = Instantiate(particleHurt);
            ps.transform.position = playerLifeController.transform.position;
        }

        public void SpawnDustParticels()
        {
            dust.Play();
        }
    }
}
