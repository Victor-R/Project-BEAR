using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    #region Variáveis Globais
    
    #region Publicas
    public Transform TransformDoBear;
    public Transform TransformDoChar;
    public GameObject Lupa;
    #endregion

    double Distancia = 10;
    
    #endregion
    
    #region OnGui
    public GUIStyle StyleProGui;

    void OnGUI()
    {
        #region Cortar Ultimos Digitos
        char[] tal = VelocidadeChar.ToString().ToCharArray();
        int index = Array.IndexOf(tal, ',');
        //Debug.Log(index);
        string stringtal = "";
        

        for (int i = 0; i < index+1; i++)
        {
            stringtal += tal[i];
        }

        if (tal.Length >= index + 2)
        {
            stringtal += tal[index + 1];

        }
        if (tal.Length >= index + 3)
        {
            stringtal += tal[index + 2];

        }
        if (tal.Length >= index + 4)
        {
            stringtal += tal[index + 3];

        }
        #endregion

        string ParaMostrar = string.Format("Velocidade: {0}km/h  \tDistância: {1}m", CortarUltimosDigitos(VelocidadeChar), CortarUltimosDigitos(Distancia));

        GUI.Label(new Rect(150, 10, 100, 20), ParaMostrar, StyleProGui);
    }
    #endregion

    #region Variáveis FixedUpdate
    private float tempoAnterior1;
    private float tempoAnterior2;
    private float tempoAnterior3;
    private float tempoAnterior4;

    // Fração de segundo utilizada para os Incrementos do FixedUpdate:
    float fracaoDeSegundoParaUpdate = 0.05f;
    #endregion
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        #region ChecarSeGameOver
        if (Distancia <= 0)
        {
            GameOver();
        }
        #endregion

        #region Decrementador de Velocidade
        if (tempoAnterior1 + 0.2f < Time.time)
        {
            VelocidadeChar = DecrementarVelocidadePersonagem(VelocidadeChar, 0.2f);
            tempoAnterior1 = Time.time;
        }
        #endregion

        #region Atualizar Distância
        if (tempoAnterior2 + 0.05 < Time.time)
        {
            // Distância "Perde" ((Velocidade atual do Bear convertida de km/h para m/s) multiplicado pela fração de segundo em que esse For está rodando).
            Distancia -= (VelocidadeBear * 0.277778) * fracaoDeSegundoParaUpdate;
            Distancia += (VelocidadeChar * 0.277778) * fracaoDeSegundoParaUpdate;
            tempoAnterior2 = Time.time;
        }
        #endregion

        #region Atualizador De Animação
        AnimatorDeMovimentacaoDoPersonagem.speed = Convert.ToSingle(1 + VelocidadeChar/100);
        AnimatorDoTerreno.speed = Convert.ToSingle(1 + VelocidadeChar / 100);
        #endregion

        #region Atualizar Posição Bear
        if (Distancia < 11)
        {
            if (tempoAnterior3 + 0.05f < Time.time)
            {
                TransformDoBear.localPosition = new Vector2(Convert.ToSingle((Distancia * -105) +320), TransformDoBear.localPosition.y);
                tempoAnterior3 = Time.time;

                TransformDoChar.localPosition = new Vector2(Convert.ToSingle((Distancia * -50) +550), TransformDoChar.localPosition.y);
                tempoAnterior3 = Time.time;
            }
        }
        #endregion

        #region Mostrar Lupa
        if (Distancia > 11)
        {
            Lupa.SetActive(true);
        }
        else
        {
            Lupa.SetActive(false);
        }
        #endregion

        #region Salvar e agendar notificação a cada 10 segundos
        if (tempoAnterior4 + 10 < Time.time)
        {
            SalvarParametros();
            AgendarNotificacao();
            tempoAnterior4 = Time.time;
        }
        #endregion
    }
    

    #region Variáveis Bear
    double VelocidadeBear = 4.123;
    #endregion

    #region Variáveis Personagem
    double VelocidadeChar = 0;
    public static double VelocidadeMinimaAtual = 2;
    public static double DecrementoDeVelocidadeAtual = 0.3;
    public static double IncrementoDeVelocidadeAtual = 0.5f;
    public Animator AnimatorDeMovimentacaoDoPersonagem;
    public Animator AnimatorDoTerreno;
    #endregion

    #region Métodos de Incremento
    public void IncrementarVelocidadePersonagem()
    {
        VelocidadeChar += (IncrementoDeVelocidadeAtual / AnimatorDeMovimentacaoDoPersonagem.speed/2);
        //Debug.Log(IncrementoDeVelocidadeAtual / AnimatorDeMovimentacaoDoPersonagem.speed/2);
    }
    /// <summary>
    /// Método para uniformizar o modo como se decrementa a velocidade do personagem em razao a alguma fração de segundo.
    /// </summary>
    /// <param name="Velocidade">Velocidade que será decrementada</param>
    /// <param name="fracaoUtilizada">Fração de segundo utilizada no contexto atual, (1 = o segundo todo)</param>
    /// <returns></returns>
    public double DecrementarVelocidadePersonagem(double Velocidade, float fracaoUtilizada)
    {
        if ((Velocidade - DecrementoDeVelocidadeAtual) < VelocidadeMinimaAtual)
        {
            Velocidade = VelocidadeMinimaAtual;
        }
        else
        {
            Velocidade -= DecrementoDeVelocidadeAtual * fracaoUtilizada;
        }

        return Velocidade;
    }
    #endregion

    #region Mono e Outros Métodos
    private void Start()
    {
        //APAGAR NOTIFICAÇÕES AGENDADAS
        LocalNotification.ClearNotifications();

        #region Retrocalcular tudo
        // SE DISTÂNCIA JÁ FOI ALGUMA VEZ SALVA ENTÃO USAR RETROCALCULADORA
        if (PlayerPrefs.GetFloat("Distancia") > 0)
        {
            RetroCalculadora();
            LocalNotification.ClearNotifications();
        }
        #endregion
    }

    private void OnApplicationQuit()
    {
        SalvarParametros();
        AgendarNotificacao();
    }

    private void OnApplicationFocus(bool focus)
    {
        // SE O APP GANHAR FOCO:
        if (focus)
        {
            RetroCalculadora();
            LocalNotification.ClearNotifications();
        }
        // SE O APP PERDER O FOCO: SALVAR PARAMETROS.
        else
        {
            SalvarParametros();
            AgendarNotificacao();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SalvarParametros();
            AgendarNotificacao();
        }
        else
        {
            RetroCalculadora();
            LocalNotification.ClearNotifications();
        }
    }

    IEnumerator TimerParaFecharEmSegundoPlano()
    {
        Debug.Log("timer para fechar iniciado");
        yield return new WaitForSeconds(10);
        Application.Quit();
    }

    private string CortarUltimosDigitos(double valor)
    {
        char[] valorEmCharArray = valor.ToString().ToCharArray();
        int index = Array.IndexOf(valorEmCharArray, ',');
        
        string srtringParaRetornar = "";


        for (int i = 0; i < index + 1; i++)
        {
            srtringParaRetornar += valorEmCharArray[i];
        }

        if (valorEmCharArray.Length >= index + 2)
        {
            srtringParaRetornar += valorEmCharArray[index + 1];

        }
        if (valorEmCharArray.Length >= index + 3)
        {
            srtringParaRetornar += valorEmCharArray[index + 2];

        }
        if (valorEmCharArray.Length >= index + 4)
        {
            srtringParaRetornar += valorEmCharArray[index + 3];

        }

        return srtringParaRetornar;
    }

    #region Variáveis Gameover
    public GameObject MainCanvas;
    public GameObject CanvasGameover;
    #endregion
    public void GameOver()
    {
        CanvasGameover.SetActive(true);
        this.enabled = false;
        MainCanvas.SetActive(false);
    }

    #region Variáveis Reset e Restart Main Canvas
    double defVelocidade;
    double defDistancia;
    Transform defTransformBear;
    #endregion
    public void ResetRestartMainCanvas()
    {
        VelocidadeChar = 2;
        Distancia = 10;
        TransformDoBear.localPosition = new Vector2(-900,-45);

        CanvasGameover.SetActive(false);
        this.enabled = true;
        MainCanvas.SetActive(true);
    }

    public void SalvarParametros()
    {
        PlayerPrefs.SetFloat("Distancia", Convert.ToSingle(Distancia));
        PlayerPrefs.SetFloat("VelocidadeChar", Convert.ToSingle(VelocidadeChar));
        PlayerPrefs.SetInt("TempoOnQuitAno", DateTime.Now.Year);
        PlayerPrefs.SetInt("TempoOnQuitMes", DateTime.Now.Month);
        PlayerPrefs.SetInt("TempoOnQuitDia", DateTime.Now.Day);
        PlayerPrefs.SetInt("TempoOnQuitHora", DateTime.Now.Hour);
        PlayerPrefs.SetInt("TempoOnQuitMinuto", DateTime.Now.Minute);
        PlayerPrefs.SetInt("TempoOnQuitSegundo", DateTime.Now.Second);
    }

    public void RetroCalculadora()
    {
        // CARREGAR VARIÁVEIS
        double distanciaCarregada = PlayerPrefs.GetFloat("Distancia");
        double velocidadeCharCarregada = PlayerPrefs.GetFloat("VelocidadeChar");
        int anoCarregado = PlayerPrefs.GetInt("TempoOnQuitAno");
        int mesCarregado = PlayerPrefs.GetInt("TempoOnQuitMes");
        int diaCarregado = PlayerPrefs.GetInt("TempoOnQuitDia");
        int horaCarregado = PlayerPrefs.GetInt("TempoOnQuitHora");
        int minutoCarregado = PlayerPrefs.GetInt("TempoOnQuitMinuto");
        int segundoCarregado = PlayerPrefs.GetInt("TempoOnQuitSegundo");

        // MONTAR O DATETIME CARREGADO
        DateTime dataCarregada = new DateTime
            (
                anoCarregado,
                mesCarregado,
                diaCarregado,
                horaCarregado,
                minutoCarregado,
                segundoCarregado
            );

        // COMPARAR O DATETIME CARREGADO COM O ATUAL EM SEGUNDOS
        TimeSpan intervalo = DateTime.Now - dataCarregada;
        int segundosPassados = Convert.ToInt32(intervalo.TotalSeconds);
        Debug.Log("segundos passados: " + segundosPassados);

        // CALCULA A DISTÂNCIA PERCORRIDA ENQUANTO DECREMENTA A VELOCIDADE DURANTE O INTERVALO EM SEGUNDOS
        for (int i = 0; i <= segundosPassados - 2; i++)
        {       
            if(velocidadeCharCarregada > VelocidadeMinimaAtual)
            {
                distanciaCarregada += velocidadeCharCarregada * 0.277778;

                velocidadeCharCarregada = DecrementarVelocidadePersonagem(velocidadeCharCarregada, 1);
            }
            else
            {
                distanciaCarregada -= (VelocidadeBear - VelocidadeMinimaAtual) * 0.277778;
            }
        }

        // APLICAR CALCULO AO JOGO
        Distancia = distanciaCarregada;
        VelocidadeChar = velocidadeCharCarregada;

        /*
        // APAGAR VARIAVEIS SALVAS
        PlayerPrefs.DeleteKey("Distancia");
        PlayerPrefs.DeleteKey("VelocidadeChar");
        PlayerPrefs.DeleteKey("TempoOnQuitAno");
        PlayerPrefs.DeleteKey("TempoOnQuitMes");
        PlayerPrefs.DeleteKey("TempoOnQuitDia");
        PlayerPrefs.DeleteKey("TempoOnQuitHora");
        PlayerPrefs.DeleteKey("TempoOnQuitMinuto");
        PlayerPrefs.DeleteKey("TempoOnQuitSegundo");
        */
    }

    public void AgendarNotificacao()
    {
        LocalNotification.ClearNotifications();

        // CALCULAR QUANDO O BEAR VAI ALCANÇAR O CHAR
        /*
        if (Distancia > 100)
        {
            double distanciaPrevista = Distancia;
            double velocidadePrevista = VelocidadeChar;
            int tempoEmSegundos = 0;

            while (distanciaPrevista > 100)
            {
                if (velocidadePrevista > VelocidadeMinimaAtual)
                {
                    distanciaPrevista += velocidadePrevista * 0.277778;

                    velocidadePrevista = DecrementarVelocidadePersonagem(velocidadePrevista, 1);

                    tempoEmSegundos++;
                }
                else
                {
                    distanciaPrevista -= (VelocidadeBear - VelocidadeMinimaAtual) * 0.277778;

                    tempoEmSegundos++;
                }
            }

            string[] MensagensDeAproximacao = new string[]
            {
                "Faltam menos de 100m pro Bear te pegar!",
                "Not today, not tomorrow, but i will kill you"
            };

            LocalNotification.SendNotification(3, tempoEmSegundos * 1000, "Alerta de Aproximação! (100m)", MensagensDeAproximacao[UnityEngine.Random.Range(0, MensagensDeAproximacao.Length - 1)], new Color32(0, 0, 0, 255), true, true, true, "ic_launcher");
        }
        if (Distancia > 50)
        {
            double distanciaPrevista = Distancia;
            double velocidadePrevista = VelocidadeChar;
            int tempoEmSegundos = 0;

            while (distanciaPrevista > 50)
            {
                if (velocidadePrevista > VelocidadeMinimaAtual)
                {
                    distanciaPrevista += velocidadePrevista * 0.277778;

                    velocidadePrevista = DecrementarVelocidadePersonagem(velocidadePrevista, 1);

                    tempoEmSegundos++;
                }
                else
                {
                    distanciaPrevista -= (VelocidadeBear - VelocidadeMinimaAtual) * 0.277778;

                    tempoEmSegundos++;
                }
            }

            string[] MensagensDeAproximacao = new string[]
            {
                "Faltam menos de 50m pro Bear te pegar!",
                "Falta pouco pro Bear te pegar.",
                "The life snake.",
                "Hora de procastinar mais um pouco."
            };

            LocalNotification.SendNotification(2, tempoEmSegundos * 1000, "Alerta de Aproximação! (50m)", MensagensDeAproximacao[UnityEngine.Random.Range(0, MensagensDeAproximacao.Length - 1)], new Color32(0, 0, 0, 255), true, true, true, "ic_launcher");
        }
        */
        if (Distancia > 25)
        {
            double distanciaPrevista = Distancia;
            double velocidadePrevista = VelocidadeChar;
            int tempoEmSegundos = 0;

            while (distanciaPrevista > 25)
            {

                // SIMULA O GANHO E PERDA DE DISTANCIA ATRAVES DO TEMPO
                if (velocidadePrevista > VelocidadeMinimaAtual)
                {
                    distanciaPrevista += velocidadePrevista * 0.277778;

                    velocidadePrevista = DecrementarVelocidadePersonagem(velocidadePrevista, 1);

                    tempoEmSegundos++;
                }
                else
                {
                    distanciaPrevista -= (VelocidadeBear - VelocidadeMinimaAtual) * 0.277778;

                    tempoEmSegundos++;

                    // AGENDAR NOTIFICAÇÃO AOS 100M RESTANTES PARA ALCANÇAR
                    if (distanciaPrevista > 99 && distanciaPrevista < 101)
                    {
                        string[] MensagensDeAproximacao100 = new string[]
                        {
                        "Faltam menos de 100m pro Bear te pegar!",
                        "Not today, not tomorrow, but i will kill you"
                        };

                        LocalNotification.SendNotification(3, tempoEmSegundos * 1000, "Alerta de Aproximação! (100m)", MensagensDeAproximacao100[UnityEngine.Random.Range(0, MensagensDeAproximacao100.Length - 1)], new Color32(0, 0, 0, 255), true, true, true, "ic_launcher");
                    }

                    // AGENDAR NOTIFICAÇÃO AOS 50M RESTANTES PARA ALCANÇAR
                    if (distanciaPrevista > 49 && distanciaPrevista < 51)
                    {
                        string[] MensagensDeAproximacao50 = new string[]
                        {
                        "Faltam menos de 50m pro Bear te pegar!",
                        "Falta pouco pro Bear te pegar.",
                        "The life snake.",
                        "Hora de procastinar mais um pouco."
                        };

                        LocalNotification.SendNotification(3, tempoEmSegundos * 1000, "Alerta de Aproximação! (50m)", MensagensDeAproximacao50[UnityEngine.Random.Range(0, MensagensDeAproximacao50.Length - 1)], new Color32(0, 0, 0, 255), true, true, true, "ic_launcher");
                    }
                }
            }

            // AGENDAR NOTIFICAÇÃO AOS 25M RESTANTES PARA ALCANÇAR
            string[] MensagensDeAproximacao = new string[]
            {
                "Faltam menos de 25m pro Bear te pegar!",
                "O Bear vai te pegar!",
                "Ó o bicho vindo!",
                "Cachoooooooorro!",
                "Two minutes to miiiiiiiidniiiiight"
            };
            LocalNotification.SendNotification(1, tempoEmSegundos * 1000, "Alerta de Aproximação! (25m)", MensagensDeAproximacao[UnityEngine.Random.Range(0, MensagensDeAproximacao.Length - 1)], new Color32(0, 0, 0, 255), true, true, true, "ic_launcher");
        }
        else
        {
            LocalNotification.SendNotification(1, 15000, "Alerta de Aproximação!", "Tem certeza que já vai? O Bear logo vai te alcançar!", new Color32(0, 0, 0, 255), true, true, true, "ic_launcher");
        }
    }
    #endregion
}
