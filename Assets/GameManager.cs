using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;
    public bool inGame = false;
    public GameObject slot;
    public GameObject[] gpObjects = new GameObject[9];
    public Slot[,,] grupos = new Slot[9,3,3];
    public Slot[,] mapa = new Slot[9,9];
    int dificuldade;

    public GameObject menu, gameOver, restartBt, linha, coluna;

    public RectTransform fundo;
    public Text tempoText;
    float tempoI, tempo;
    int segundos, minutos;
    string aux1, aux2;

    // Start is called before the first frame update
    void Start()
    {
        if(gm == null){
            gm = this;
        }

        tempo = 0;

        if(PlayerPrefs.GetString("recFacil").Equals("")){
            PlayerPrefs.SetString("recFacil", "00:00");
        }

        if(PlayerPrefs.GetString("recMedio").Equals("")){
            PlayerPrefs.SetString("recMedio", "00:00");
        }

        if(PlayerPrefs.GetString("recDificil").Equals("")){
            PlayerPrefs.SetString("recDificil", "00:00");
        }

        AttHUD();

        DeviceChange.OnOrientationChange += MyOrientationChangeCode;
        DeviceChange.OnResolutionChange += MyResolutionChangeCode;

        for (int x = 0; x < 9; x++){
            for (int y = 0; y < 9; y++){
                foreach (Slot s in FindObjectsOfType<Slot>()){
                    if(s.GetX() == x && s.GetY() == y){
                        mapa[x,y] = s;
                    }
                }
            }
        }

        DefineGrupos();
    }

    void Update(){
        if(Input.GetKey(KeyCode.Escape)){
            Application.Quit();
        }
    }

    void FixedUpdate(){
        if(inGame){
            tempo += Time.deltaTime;
            
            minutos = (int)tempo/60;

            segundos = (int)tempo%60;

            if(minutos<10){
                aux1 = "0";
            } else{
                aux1 = "";
            }

            if(segundos<10){
                aux2 = "0";
            } else{
                aux2 = "";
            }

            tempoText.text = aux1 + minutos + ":" + aux2 + segundos;
            
        }
    }

    void MyOrientationChangeCode(DeviceOrientation orientation) {
        AttHUD();
    }
 
    void MyResolutionChangeCode(Vector2 resolution) {
        AttHUD();
    }

    void AttHUD(){
        GameObject.Find("background").GetComponent<RectTransform>().sizeDelta = new Vector2(Screen.width, Screen.height);
    }

    public void StartGame(int dificuldade){

        tempo = 0;

        switch(dificuldade){
            case 50:
                GameObject.Find("Dificuldade").GetComponent<Text>().text = "Fácil";

                GameObject.Find("Recorde").GetComponent<Text>().text = PlayerPrefs.GetString("recFacil");
            break;

            case 40:
                GameObject.Find("Dificuldade").GetComponent<Text>().text = "Médio";

                GameObject.Find("Recorde").GetComponent<Text>().text = PlayerPrefs.GetString("recMedio");
            break;

            case 30:
                GameObject.Find("Dificuldade").GetComponent<Text>().text = "Difícil";

                GameObject.Find("Recorde").GetComponent<Text>().text = PlayerPrefs.GetString("recDificil");
            break;
        }

        foreach (Slot slot in FindObjectsOfType<Slot>()){
            slot.SetRevealed(false);
            slot.SetValorReal(0);
            slot.DefineValor(0,true);
        }
        restartBt.SetActive(true);
        inGame = true;
        this.dificuldade = dificuldade;
        menu.SetActive(false);
        DefineMapa();

    }

    public void RestartGame(bool over){
        if(menu.activeSelf){
            menu.SetActive(false);
            inGame = true;
            return;
        }

        if(over){
            gameOver.SetActive(false);
        }
        else{
            inGame = false;
        }
        
        menu.SetActive(true);
    }

    public void InsereNumero(int n){

        foreach(Slot slot in mapa){
            if(slot.GetSelected()){
                slot.DefineValor(n, true);
                slot.Selecionar();
                VerificaTudo();
            }
        }
    }

    public void SelecionaAreas(Slot slot, bool select){
        linha.SetActive(select);
        coluna.SetActive(select);

        if(select){
            linha.GetComponent<RectTransform>().localPosition = new Vector3(linha.GetComponent<RectTransform>().localPosition.x, slot.GetComponent<RectTransform>().localPosition.y + slot.GetComponent<RectTransform>().parent.localPosition.y, linha.GetComponent<RectTransform>().localPosition.z);
            coluna.GetComponent<RectTransform>().localPosition = new Vector3(slot.GetComponent<RectTransform>().localPosition.x + slot.GetComponent<RectTransform>().parent.localPosition.x, coluna.GetComponent<RectTransform>().localPosition.y, coluna.GetComponent<RectTransform>().localPosition.z);
        }

    }


    void DefineMapa(){
        
        DefineValores();

        RevelaNumeros();

    }

    void DefineGrupos(){
        grupos = new Slot[9,3,3];

        int xm, ym;

        for(int g = 0; g < 9; g++){

            for(int x = 0; x < 3; x++){
                for(int y = 0; y < 3; y++){

                    if(g<3){
                        xm = x;
                        ym = y+(g*3);
                    }
                    else if(g<6){
                        xm = x+3;
                        ym = y+((g-3)*3);
                    }
                    else{
                        xm = x+6;
                        ym = y+((g-6)*3);
                    }


                    grupos[g,x,y] = mapa[xm,ym];
                }
            }
        }
    }

    void DefineValores(){
        int v, cont = 0;
        for (int x = 0; x < 9; x++) {
            for (int y = 0; y < 9; y++){
                

                if(x!=0){
                    if(x%6==0){
                        cont = 2;
                    }
                    else if(x%3==0){
                        cont = 1;
                    }
                }

                v = (y+1)+x*3+cont;

                while(v > 9){
                    v -= 9;
                }

                mapa[x,y].SetValorReal(v);
            }
        }

        for(int i = 0; i < 3; i++){
            EmbaralhaLinhas(i*3, i*3+2);
            EmbaralhaColunas(i*3, i*3+2);
        }

        TrocaNumeros();

    }

    void EmbaralhaLinhas(int lMin, int lMax){
        int vz = Random.Range(0,6);
        int l0, l1;

        int[] aux = new int[9];

        for (int i = 0; i < vz; i++){
            do{
                l0 = Random.Range(lMin, (lMax+1));
                l1 = Random.Range(lMin, (lMax+1));
            }while(l0 == l1);

            for(int l = 0; l < 9; l++){
                aux[l] = mapa[l0,l].GetValorReal();
            }

            for(int l = 0; l < 9; l++){
                mapa[l0,l].SetValorReal(mapa[l1,l].GetValorReal());
                //mapa[l0,l].DefineValor(mapa[l1,l].GetValorReal());
            }

            for(int l = 0; l < 9; l++){
                mapa[l1,l].SetValorReal(aux[l]);
                //mapa[l1,l].DefineValor(aux[l]);
            }
        }
    }

    void EmbaralhaColunas(int cMin, int cMax){
        int vz = Random.Range(0,10);
        int c0, c1;

        int[] aux = new int[9];

        for (int i = 0; i < vz; i++){
            do{
                c0 = Random.Range(cMin, (cMax+1));
                c1 = Random.Range(cMin, (cMax+1));
            }while(c0 == c1);

            for(int c = 0; c < 9; c++){
                aux[c] = mapa[c,c0].GetValorReal();
            }

            for(int c = 0; c < 9; c++){
                mapa[c,c0].SetValorReal(mapa[c,c1].GetValorReal());
                //mapa[c,c0].DefineValor(mapa[c,c1].GetValorReal());
            }

            for(int c = 0; c < 9; c++){
                mapa[c,c1].SetValorReal(aux[c]);
                //mapa[c,c1].DefineValor(aux[c]);
            }
        }

    }

    void TrocaNumeros(){
        int trocas, n1, n2;

        trocas = (int) Random.Range(20, 50);

        for(int i = 0; i < trocas; i++){

            do{
                n1 = (int) Random.Range(0,10);
                n2 = (int) Random.Range(0,10);
            }while((n1 == 0) || (n2 == 0) || (n1 == n2));

            foreach (Slot slotT in mapa) {
                if(slotT.GetValorReal() == n1){
                    slotT.SetValorReal(n2);
                }
                else if(slotT.GetValorReal() == n2){
                    slotT.SetValorReal(n1);
                } 
            }
            
        }
    }

    void RevelaNumeros(){
        int x, y;
        bool certo;
        for (int i = 0; i < dificuldade; i++){
            do{
                do{
                    x = Random.Range(0,9);
                    y = Random.Range(0,9);
                } while ((x == 9) || (y == 9));

                
                if(mapa[x,y].GetRevealed()){
                    certo = false;
                }
                else{
                    mapa[x,y].RevelaValor();
                    certo = true;
                }
            } while (!certo);
        }
    }

    void VerificaTudo(){
        bool completo = true, correto = true;

        foreach (Slot slot in mapa){
            slot.MostraErro(false);

            if(slot.GetValor()==0){
                completo= false;
            }
        }

        if(completo){
            foreach (Slot slot in mapa){
                if(!slot.GetRevealed()){
                    for(int x = 0; x < 9; x++){
                        if(slot.GetX() != x){
                            if(mapa[x, slot.GetY()].GetValor() == slot.GetValor()){
                                correto = false;
                                slot.MostraErro(true);
                            }
                        }
                    }

                    for(int y = 0; y < 9; y++){
                        if(slot.GetY() != y){
                            if(mapa[slot.GetX(), y].GetValor() == slot.GetValor()){
                                correto = false;
                                slot.MostraErro(true);
                            }
                        }
                    }

                    for(int x = 0; x < 3; x++){
                        for(int y = 0; y < 3; y++){
                            if(grupos[slot.GetGrupo(), x, y] != slot){
                                if(grupos[slot.GetGrupo(), x, y].GetValor() == slot.GetValor()){
                                    correto = false;
                                    slot.MostraErro(true);
                                }
                            }
                        }
                    }
                }
            }

            if(correto){
                inGame = false;

                float p;
                string n;

                switch(dificuldade){
                    case 50:
                        p = PlayerPrefs.GetFloat("tmpFacil");
                        n = "tmpFacil";
                    break;

                    case 40:
                        p = PlayerPrefs.GetFloat("tmpMedio");
                        n = "tmpMedio";
                    break;

                    case 30:
                        p = PlayerPrefs.GetFloat("tmpDificil");
                        n = "tmpDificil";
                    break;

                    default:
                        p = 0;
                        n = "";
                    break;
                }

                if(tempo < p && n != ""){
                    PlayerPrefs.SetFloat(n,tempo);
                }

                gameOver.SetActive(true);
                restartBt.SetActive(false);
            }
        }
    }

    
}
