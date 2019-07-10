using UnityEngine;

/// <summary>
/// Componente que controla a movimentação de objetos em paralax no cenário
/// </summary>
public class ParalaxObject : MonoBehaviour
{
    /// <summary>
    /// Onde o objeto vai começar
    /// </summary>
    public float startX = 700;

    /// <summary>
    /// Até onde o objeto vai andar até ser destruído
    /// </summary>
    public float limiteX = -700;

    /// <summary>
    /// Gerar altura aleatória dentro desse parâmetros
    /// </summary>
    public float randonMaxY;
    public float randonMinY;

    /// <summary>
    /// Velocidade com que o objeto passará na tela
    /// </summary>
    public float velocidadeX = 1;

    /// <summary>
    /// Gerar tamanho aleatório dentro desses parâmetros
    /// </summary>
    public float randonMaxSize = 1;
    public float randonMinSize = 1;


    private void Start()
    {
        // Iniciar objeto na posição desejada
        gameObject.transform.localPosition = new Vector2(startX, Random.Range(randonMinY, randonMaxY));
        float tamanho = Random.Range(randonMinSize, randonMaxSize);
        gameObject.transform.localScale = new Vector2(tamanho, tamanho);

    }

    private void FixedUpdate()
    {
        // Verificar se o objeto atingiu o limite
        if (gameObject.transform.localPosition.x < -700)
        {
            // Se sim, destrui-lo
            GameObject.Destroy(gameObject);
        }

        // Modificar posição do objeto
        gameObject.transform.position = new Vector2(gameObject.transform.position.x - velocidadeX, gameObject.transform.position.y);
    }
}
