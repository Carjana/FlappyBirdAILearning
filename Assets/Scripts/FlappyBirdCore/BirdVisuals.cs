using Sirenix.OdinInspector;
using UnityEngine;

namespace FlappyBirdCore
{
    public class BirdVisuals : MonoBehaviour
    {
        [Title("References")]
        [SerializeField] private Bird bird;
        [SerializeField] private SpriteRenderer spriteRenderer;

        [Title("Settings")] 
        [SerializeField] private Vector2 normalSize;
        [SerializeField] private Vector2 deadSize;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color deadColor;

        private void Awake()
        {
            bird.BirdDied += OnBirdDied;
            bird.BirdJumped += OnBirdJumped;
            bird.BirdReset += OnBirdReset;
        }

        private void OnBirdDied()
        {
            spriteRenderer.color = deadColor;
            spriteRenderer.size = deadSize;
        }

        private void OnBirdJumped()
        {
            
        }

        private void OnBirdReset()
        {
            spriteRenderer.color = normalColor;
            spriteRenderer.size = normalSize;
        }
    }
}
