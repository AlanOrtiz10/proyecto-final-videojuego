using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Jugador : MonoBehaviour
{
    public float velocidad = 5;
    private Rigidbody2D cuerpo2D;

    private float movimientoHorizontal;

    public float fuerzaSalto = 4;
    private bool enSuelo;
    public Transform detectorSuelo;
    public float radioDeteccion = 0.1f;
    public LayerMask capaSuelo;

    private Animator animator;

    private int monedas;
    public TMP_Text textoMonedas;

    public int vidaMaxima = 3; // Vida inicial del jugador
    private int vidaActual;

    public float tiempoInvencible = 1f; // Tiempo que el jugador no puede recibir daño tras un golpe
    private bool invencible = false;

    void Start()
    {
        cuerpo2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        vidaActual = vidaMaxima; // Inicializar vida
        textoMonedas.text = monedas.ToString();
    }

    void Update()
    {
        // Movimiento horizontal
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");
        cuerpo2D.linearVelocity = new Vector2(movimientoHorizontal * velocidad, cuerpo2D.linearVelocity.y);

        // Cambiar orientación del personaje
        if (movimientoHorizontal != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(movimientoHorizontal), 1, 1);
        }

        // Saltar
        if (Input.GetButtonDown("Jump") && enSuelo)
        {
            cuerpo2D.linearVelocity = new Vector2(cuerpo2D.linearVelocity.x, fuerzaSalto);
        }

        // ANIMACIONES
        animator.SetFloat("VelocidadVertical", cuerpo2D.linearVelocity.y);
        animator.SetBool("enSuelo", enSuelo);

        if (enSuelo)
            animator.SetFloat("Velocidad", Mathf.Abs(movimientoHorizontal));
        else
            animator.SetFloat("Velocidad", 0);
    }

    void FixedUpdate()
    {
        enSuelo = Physics2D.OverlapCircle(detectorSuelo.position, radioDeteccion, capaSuelo);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("moneda"))
        {
            Destroy(collision.gameObject);
            monedas++;
            textoMonedas.text = monedas.ToString();
        }
        else if (collision.CompareTag("trampa"))
        {
            RecibirDanio(1); // Recibe 1 de daño al tocar la trampa
        }
        else if (collision.CompareTag("barril"))
        {
            // Calcular dirección del knockback
            Vector2 knockbackDir = (cuerpo2D.position - (Vector2)collision.transform.position).normalized;

            // Aplicar fuerza
            cuerpo2D.linearVelocity = Vector2.zero;
            cuerpo2D.AddForce(knockbackDir * 3, ForceMode2D.Impulse);

            // Desactivar colliders del barril
            BoxCollider2D[] colliders = collision.gameObject.GetComponents<BoxCollider2D>();
            foreach (BoxCollider2D col in colliders)
            {
                col.enabled = false;
            }

            // Activar Animator y destruir
            collision.GetComponent<Animator>().enabled = true;
            Destroy(collision.gameObject, 0.5f);
        }
    }

    private void RecibirDanio(int cantidad)
    {
        if (!invencible)
        {
            vidaActual -= cantidad;
            animator.SetTrigger("Danio"); // Asegúrate de tener un Trigger "Danio" en tu Animator
            invencible = true;
            Invoke("TerminarInvencible", tiempoInvencible);

            if (vidaActual <= 0)
            {
                // Aquí puedes decidir si reiniciar el nivel o hacer otra cosa
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }

    private void TerminarInvencible()
    {
        invencible = false;
    }
}
