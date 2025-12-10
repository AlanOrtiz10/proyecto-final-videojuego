using UnityEngine;

public class Bomba : MonoBehaviour
{
    private Animator animator;
    private bool activada = false;

    public int danio = 1;              // Daño que hace la bomba al jugador
    public float tiempoExplosion = 0.5f; // Duración de la animación antes de desaparecer

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Solo activar si colisiona con el jugador y no está activada
        if (!activada && collision.CompareTag("Jugador"))
        {
            activada = true;

            // Llamar al método del jugador para hacer daño
            collision.GetComponent<Jugador>()?.RecibirDanio(danio);

            // Reproducir animación de explosión
            if (animator != null)
            {
                animator.SetTrigger("Explota");
            }

            // Destruir la bomba después del tiempo de animación
            Destroy(gameObject, tiempoExplosion);
        }
    }
}
