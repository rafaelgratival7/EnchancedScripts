using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

 public class TecladoPin : MonoBehaviour {
    #region Declaração
	[Header("Configuração")]
	[Tooltip("Número de clips, faltando")]
	[Range(0,5)]public int Corretivo;
	private float Timer;
	private Animator Controller;
	private int Contador;
	private AnimationClip[] AnimationList;
	private string[] AnimationName;
	private float[] AnimationTime;
	public GUIStyle Estilo;
    #endregion
	void Awake(){
		Controller = GetComponent <Animator> ();
    }
	void Start(){
		transform.position = Vector3.zero;
		AnimationList = Controller.runtimeAnimatorController.animationClips;
		AnimationName = new string[AnimationList.Length];
		AnimationTime = new float[AnimationList.Length];
		for(int i = 0; i < AnimationList.Length; i++){
			AnimationName[i] = AnimationList[i].name;
			AnimationTime [i] = AnimationList [i].length;
		}
    }
	void Update(){
		Timer = Controller.GetCurrentAnimatorStateInfo (0).normalizedTime * Controller.GetCurrentAnimatorClipInfo (0).Length;
		for(int i = 0; i <AnimationList.Length + Corretivo;i++)
			if(Input.GetKey ((i+ 1).ToString ()))
				Contador = i + 1;
		for(int a = 0;a <AnimationList.Length + Corretivo;a++)
			if(Contador == a)
				Controller.SetInteger ("Numero", a);
		if(Controller.GetCurrentAnimatorStateInfo (0).normalizedTime <= Controller.GetCurrentAnimatorStateInfo (0).length)
			Contador = 0;
    }
	void OnGUI(){
		for(int i = 0; i < AnimationList.Length;i++){
			GUI.Box (new Rect(0,i * 20,150,20),AnimationName[i],Estilo);
			GUI.Box(new Rect (150, i * 20, 150,20),"Tempo Total: " + AnimationTime[i],Estilo);
			if(GUI.Button (new Rect (300, i * 20, 30, 20), (i + 1).ToString ()))
				Contador = i + 1;
		}
		Rect Janela = new Rect (Screen.width - 100, Screen.height - 50, 100, 50);
		GUI.Window (0,Janela,ShowTime,"Timer");
		GUI.TextArea (new Rect(0,Screen.height -40,650,40),"Para tocar animação uma vez pressione os botões ao lado do nome \nPara tocar animação com loop segure os númericos do teclado correspondentes a ordem da lista acima"); 
	}
	void ShowTime(int WindowID){
		GUI.color = Color.white;
		GUI.Label (new Rect(20,25,100,30), Timer.ToString ());
	}
}

