using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    #region Variáveis Globais
    
    #region Publicas
    public Transform TransformDoBear;
    public GameObject Lupa;
    public Image tal;
    #endregion

    double Distancia = 10;
    
    #endregion
    
    #region OnGui
    public GUIStyle StyleProGui;

    void OnGUI()
    {
        #region Cortar Ultimos Digitos
        char[] tal = Velocidade.ToString().ToCharArray();
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

        string ParaMostrar = string.Format("Velocidade: {0}km/h  \tDistância: {1}m", CortarUltimosDigitos(Velocidade), CortarUltimosDigitos(Distancia));

        GUI.Label(new Rect(150, 10, 100, 20), ParaMostrar, StyleProGui);
    }
    #endregion

    #region FixedUpdate
    private float tempoAnterior1;
    private float tempoAnterior2;
    private float tempoAnterior3;

    private void FixedUpdate()
    {
        #region ChecarSeGameOver
        if (Distancia <= 0)
        {
            GameOver();
        }
        #endregion

        #region Decrementador de Velocidade
        if (tempoAnterior1 + 0.2f < Time.time)
        {
            DecrementarVelocidadePersonagem();
            tempoAnterior1 = Time.time;
        }
        #endregion

        #region Atualizar Distância
        if (tempoAnterior2 + 0.05 < Time.time)
        {
            Distancia -= VelocidadeBear * 0.05;
            Distancia += Velocidade * 0.05;
            tempoAnterior2 = Time.time;
        }
        #endregion

        #region Atualizador De Animação
        AnimatorDeMovimentacaoDoPersonagem.speed = System.Convert.ToSingle(1 + Velocidade/100);
        #endregion

        #region Atualizar Posição Bear
        if (Distancia < 11)
        {
            if (tempoAnterior3 + 0.05f < Time.time)
            {
                TransformDoBear.localPosition = new Vector2(Convert.ToSingle((Distancia * -105) +250), TransformDoBear.localPosition.y);
                tempoAnterior3 = Time.time;
            }
        }
        #endregion

        #region Atualizar Cenário

        #endregion
        //tal.material.se
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
    }
    #endregion

    #region Variáveis Bear
    double VelocidadeBear = 4.123;
    #endregion

    #region Variáveis e Métodos do Personagem
    double Velocidade = 0;
    public static double DecrementoDeVelocidadeAtual = 0.123;
    public static double IncrementoDeVelocidadeAtual = 0.5f;
    public Animator AnimatorDeMovimentacaoDoPersonagem;

    #region Métodos de Incremento
    public void IncrementarVelocidadePersonagem()
    {
        Velocidade += (IncrementoDeVelocidadeAtual / AnimatorDeMovimentacaoDoPersonagem.speed/2);
        //Debug.Log(IncrementoDeVelocidadeAtual / AnimatorDeMovimentacaoDoPersonagem.speed/2);
    }

    public void DecrementarVelocidadePersonagem()
    {
        if ((Velocidade - DecrementoDeVelocidadeAtual) < 2)
        {
            Velocidade = 2;
        }
        else
        {
            Velocidade -= DecrementoDeVelocidadeAtual;
        }
    }
    #endregion

    #endregion

    #region Mono e Outros Métodos
    private void Start()
    {
        #region Setar Valores Iniciais do Jogo
        defVelocidade = Velocidade;
        defDistancia = Distancia;
        defTransformBear = TransformDoBear;

        #endregion
    }

    private string CortarUltimosDigitos(double valor)
    {
        char[] tal = valor.ToString().ToCharArray();
        int index = Array.IndexOf(tal, ',');
        
        string stringtal = "";


        for (int i = 0; i < index + 1; i++)
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

        return stringtal;
    }

    #region Gameover
    public GameObject MainCanvas;
    public GameObject CanvasGameover;

    public void GameOver()
    {
        CanvasGameover.SetActive(true);
        this.enabled = false;
        MainCanvas.SetActive(false);
    }

    #region Reset e Restart Main Canvas
    double defVelocidade;
    double defDistancia;
    Transform defTransformBear;
    public void ResetRestartMainCanvas()
    {
        Velocidade = defVelocidade;
        Distancia = defDistancia;
        TransformDoBear = defTransformBear;

        CanvasGameover.SetActive(false);
        this.enabled = true;
        MainCanvas.SetActive(true);
    }
    #endregion

    #endregion

    #endregion


}
