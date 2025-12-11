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

    public int vidaMaxima = 6;
    private int vidaActual;

    public float tiempoInvencible = 1f;
    private bool invencible = false;

    private VidaUI vidaUI; // Referencia al script de los corazones
    
    public int totalMonedasEnNivel;


    void Start()
    {
        cuerpo2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        vidaActual = vidaMaxima;
        textoMonedas.text = monedas.ToString();

        vidaUI = FindFirstObjectByType<VidaUI>();
        if (vidaUI != null)
        {
            vidaUI.ActualizarVidas(vidaActual); 
        }
       
    }

    void Update()
    {
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");
        cuerpo2D.linearVelocity = new Vector2(movimientoHorizontal * velocidad, cuerpo2D.linearVelocity.y);

        if (movimientoHorizontal != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(movimientoHorizontal), 1, 1);
        }

        if (Input.GetButtonDown("Jump") && enSuelo)
        {
            cuerpo2D.linearVelocity = new Vector2(cuerpo2D.linearVelocity.x, fuerzaSalto);
        }

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

            if (monedas >= totalMonedasEnNivel)
            {
                GanarNivel();
            }
        }
        else if (collision.CompareTag("trampa"))
        {
            RecibirDanio(1);
        }
        else if (collision.CompareTag("barril"))
        {
            Vector2 knockbackDir = (cuerpo2D.position - (Vector2)collision.transform.position).normalized;

            cuerpo2D.linearVelocity = Vector2.zero;
            cuerpo2D.AddForce(knockbackDir * 3, ForceMode2D.Impulse);

            BoxCollider2D[] colliders = collision.gameObject.GetComponents<BoxCollider2D>();
            foreach (BoxCollider2D col in colliders)
            {
                col.enabled = false;
            }

            collision.GetComponent<Animator>().enabled = true;
            Destroy(collision.gameObject, 0.5f);
        }
        else if (collision.CompareTag("bomba"))
        {
            // Hacer da�o al jugador
            RecibirDanio(1);

            // Activar la animaci�n de la bomba
            Animator bombaAnimator = collision.GetComponent<Animator>();
            if (bombaAnimator != null)
            {
                bombaAnimator.enabled = true;
            }

            // Destruir la bomba despu�s de la animaci�n
            float tiempoExplosion = 0.5f;
            Destroy(collision.gameObject, tiempoExplosion);
        }



    }

    public void RecibirDanio(int cantidad)
    {
        if (!invencible)
        {
            vidaActual -= cantidad;
            animator.SetTrigger("Danio");
            invencible = true;
            Invoke("TerminarInvencible", tiempoInvencible);

            if (vidaUI != null)
            {
                vidaUI.ActualizarVidas(vidaActual); 
            }

            if (vidaActual <= 0)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
    }


    private void TerminarInvencible()
    {
        invencible = false;
    }

    void GanarNivel()
    {
        Debug.Log("¡Nivel completado!");

        // Si quieres cargar una escena de victoria:
        SceneManager.LoadScene("PantallaVictoria");
    }
}
