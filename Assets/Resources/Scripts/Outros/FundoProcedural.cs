using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FundoProcedural : MonoBehaviour
{
    public Sprite[] ElementosVisuaisParaUtilizar;
    public float intervaloMaxGerar = 1;
    public float intervaloMinGerar = 1;

    float tempoanterior = 0;
    private void Update()
    {
        if (tempoanterior + Random.Range(intervaloMinGerar, intervaloMaxGerar) < Time.time)
        {
            GameObject nuvem = new GameObject();
            nuvem.transform.SetParent(gameObject.transform);
            SpriteRenderer SR = nuvem.AddComponent<SpriteRenderer>();
            SR.sprite = ElementosVisuaisParaUtilizar[Random.Range(0, ElementosVisuaisParaUtilizar.Length - 1)];
            SR.sortingOrder = -7;
            ParalaxObject PO = nuvem.AddComponent<ParalaxObject>();
            PO.velocidadeX = 0.05f;
            PO.randonMaxY = 150;
            PO.randonMinY = -150;
            PO.randonMaxSize = 40;
            PO.randonMinSize = 40;

            


            tempoanterior = Time.time;
        }
    }
}
