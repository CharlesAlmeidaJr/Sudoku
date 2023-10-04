using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{
    public int valorReal, valor = 0;
    public int g, x, y;

    bool revealed = false, selected = false, errado = false;

    public Sprite[] sprites;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetRevealed(bool revealed){
        this.revealed = revealed;
    }

    public bool GetRevealed(){
        return revealed;
    }

    public void SetSelected(bool selected){
        this.selected = selected;
    }

    public bool GetSelected(){
        return selected;
    }

    public void SetXY(int x, int y){
        this.x = x;
        this.y = y;
    }

    public void SetGrupo(int g){
        this.g = g;

        this.transform.SetParent(GameManager.gm.gpObjects[g].transform);
    }

    public void SetValorReal(int valorReal){
        this.valorReal = valorReal;
    }

    public int GetValorReal(){
        return valorReal;
    }

    public int GetValor(){
        return valor;
    }

    public int GetX(){
        return x;
    }

    public int GetY(){
        return y;
    }

    public int GetGrupo(){
        return g;
    }

    public bool GetErrado(){
        return errado;
    }

    public void DefineValor(int valor, bool tentativa){
        this.valor = valor;

        if(tentativa){
            GetComponent<Image>().sprite = sprites[valor];
        }
    }

    public void RevelaValor(){
        revealed = true;

        GetComponent<Image>().sprite = sprites[valorReal+19];
        valor = valorReal;
    }

    public void Selecionar(){
        if(!revealed && GameManager.gm.inGame){
            if(!selected){
                foreach(Slot slot in FindObjectsOfType<Slot>()){
                    if(slot.GetSelected()){
                        slot.Selecionar();
                    }
                }
                
                selected = true;
                GetComponent<Image>().sprite = sprites[valor+10];


                GameManager.gm.SelecionaAreas(this, true);
                return;
            }
            else{
                selected = false;
                GetComponent<Image>().sprite = sprites[valor];
                GameManager.gm.SelecionaAreas(this, false);
                return;
            }
        }
    }

    public void MostraErro(bool errado){
        this.errado = errado;
        
        if(errado){
            
            if(revealed){
                GetComponent<Image>().sprite = sprites[valorReal+28];
            }
            else{
                GetComponent<Image>().sprite = sprites[valor+28];
            }
        }
        else{

            if(revealed){
                GetComponent<Image>().sprite = sprites[valorReal+19];
            }
            else{
                GetComponent<Image>().sprite = sprites[valor];
            }

        }
    }
}
