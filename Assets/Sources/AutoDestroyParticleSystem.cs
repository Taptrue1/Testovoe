using UnityEngine;

namespace Sources
{
    public class AutoDestroyParticleSystem : MonoBehaviour
    {
        private ParticleSystem _particleSystem;

        private void Start()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }
        private void Update()
        {
            if (_particleSystem && !_particleSystem.IsAlive())
            {
                Destroy(gameObject);
            }
        }
    }
}