using UnityEngine;
using System.Collections;
using UnityEditor.AnimatedValues;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class PlayerControl : MonoBehaviour {

	[Header("Configuração")]
	[Tooltip("Velocidade de Movimento")]
	[Range(0,100)]public float speed;

	[Range(0,100)]public int rotationSpeed;

	private float _x;
	private float _z;
	private float _xR;
	private float _yR;
	public GameObject Mesh;

	[Tooltip ("Quantidade de Pulos")]
	[Range (0,5)] public int jumpNumber;
	[Tooltip("Força do Pulo")]
	[Range(0,5)] public int jumpForce;

	private int counterJump;

	private bool _isGrounded;
	private bool Controlle;


	void Start () {
		_isGrounded = true;
		Controlle = false;
	}

	void Update () {

		if( Input.GetJoystickNames ().Length > 0)
		{
			Controlle = true;
		}
		else
		{
			Controlle = false;
		}
	
		if(Controlle)
		{
			_xR = Input.GetAxis ("R_Horizontal") * speed * Time.deltaTime;
			_yR = Input.GetAxis ("R_Vertical") * speed * Time.deltaTime;
			_x = Input.GetAxis ("Horizontal") * speed * Time.deltaTime;
			_z = Input.GetAxis ("Vertical") * speed * Time.deltaTime;

			Vector3 Fixmath = new Vector3 (0, Mathf.Atan2 (-_xR, -_yR) * Mathf.Rad2Deg, 0);
			Mesh.transform.rotation = Quaternion.Euler (-Fixmath);

			transform.Translate (_x,0,_z);
		}
		else
		{
			_x = Input.GetAxis ("Horizontal") * speed * Time.deltaTime;
			_z = Input.GetAxis ("Vertical") * speed * Time.deltaTime;

			Vector2 positionOnScreen = Camera.main.WorldToViewportPoint (transform.position);
			Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);

			float angle = AngleBetweenTwoPoints(-positionOnScreen, -mouseOnScreen);

			Mesh.transform.rotation =  Quaternion.Euler (new Vector3(0f,angle,0f));

			transform.Translate (_x,0,_z);
		}

		if(Input.GetAxisRaw("Jump") > 0 && _isGrounded && counterJump < jumpNumber) 
		{
			GetComponent<Rigidbody> ().AddForce (Vector2.up * jumpForce, ForceMode.Impulse);
			counterJump++;
			if(counterJump >= jumpNumber)
				_isGrounded = false;
		}
	}
		
	float AngleBetweenTwoPoints(Vector3 a, Vector3 b) 
	{
		return Mathf.Atan2(a.x - b.x, a.y - b.y) * Mathf.Rad2Deg;
	}

	void OnCollisionEnter (Collision Coll)
	{
		if(Coll.gameObject.tag == "Scenery")
		{
			_isGrounded = true;
			counterJump = 0;
		}
	}
}
