using UnityEngine;
using TMPro;

public class VidaUI : MonoBehaviour
{
    public TMP_Text textoVidas;  

    public void ActualizarVidas(int vidaActual)
    {
        if (textoVidas != null)
        {
            textoVidas.text =  vidaActual + " Vidas" ;
        }
    }
}
