
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public Text TextoGameOver;
    readonly string[] MensagensDeGameover = new string[] 
    {
        "Você é Ruim!",
        "Voecê Está Realmente Tentando?",
        "Desiste!",
        "Mensagem Desmotivacional Genérica",
        "Mensagem Aleatória de {GameOver}",
        "Gameover.txt",
        "O Bear Te Alcançou! \nna verdade esse é o objetivo desse jogo..."
    };

    public void OnEnable()
    {
        TextoGameOver.text = MensagensDeGameover[Random.Range(0, MensagensDeGameover.Length - 1)];
    }
}
