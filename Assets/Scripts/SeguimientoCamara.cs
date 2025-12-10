using UnityEngine;

public class SeguimientoCamara : MonoBehaviour
{
    private Transform target;

    // Offset solo horizontal (la altura la fijamos)
    private Vector3 offset = new Vector3(0f, 0f, -10f); // Z = -10

    private float suavizado = 0.2f;

    // Posición inicial exacta
    private Vector3 posicionInicial = new Vector3(-0.466f, 1.235f, -10f);

    private void Start()
    {
        // Buscar al jugador automáticamente por tag
        GameObject jugador = GameObject.FindGameObjectWithTag("Jugador");
        if (jugador != null)
        {
            target = jugador.transform;
        }
        else
        {
            Debug.LogWarning("No se encontró un GameObject con tag 'Jugador'");
        }

        // Colocar la cámara en la posición inicial antes de seguir al jugador
        transform.position = posicionInicial;

        // Ajustamos el offset horizontal respecto al jugador
        if (target != null)
        {
            offset.x = transform.position.x - target.position.x;
            offset.z = -10f; // mantener cámara en Z
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            // Mantener Y fijo, solo mover X del jugador
            Vector3 objetivo = new Vector3(target.position.x + offset.x, posicionInicial.y, offset.z);

            // Suavizar movimiento
            transform.position = Vector3.Lerp(transform.position, objetivo, suavizado);
        }
    }
}
