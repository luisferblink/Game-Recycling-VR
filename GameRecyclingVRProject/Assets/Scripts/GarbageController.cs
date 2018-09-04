using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GarbageController : MonoBehaviour {


	public List<Transform> trashCanRenderers = new List<Transform> ();
	List<Transform> garbagesRenderers = new List<Transform> ();

	List<GameObject> messages = new List<GameObject>();

	public GameObject popUpWin, popUpLose;

	public Text pointsPapelCarton, pointsOrdinarios, pointsPlastico;

	public Color normalColor, selectColor;

	public Transform shotPointGarbage;
	Transform tempGarbage;

	public float firingAngle = 45.0f;
	public float gravity = 9.8f;

	Vector3 tagetPos;

	int pointPapelCarton, pointOrdinarios, pointPlastico, numberCollector;

	[HideInInspector]
	public bool isFlyGarbage;

	void Start(){
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("Garbage")) {
			garbagesRenderers.Add (g.transform);
			messages.Add (g.transform.GetChild (0).GetChild(0).gameObject);
			g.transform.GetChild (0).GetChild (0).gameObject.SetActive (false);
		}
	}
	

	void Update(){
		foreach (Transform g in garbagesRenderers) {
			if (tempGarbage != null) {
				if (!garbagesRenderers.Contains (tempGarbage)) {
					g.GetComponent<Renderer> ().material.SetColor ("_OutlineColor", normalColor);
				}
			} else {
				g.GetComponent<Renderer> ().material.SetColor ("_OutlineColor", normalColor);
			}
		}
		foreach (Transform t in trashCanRenderers) {
			if (!isFlyGarbage) {
				t.GetComponent<Animator> ().SetBool ("Open", false);
			}
		}

		foreach (GameObject m in messages) {
			m.SetActive (false);
		}

		Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit)) {
			if (hit.transform.name.Contains ("Garbage")) {
				hit.transform.GetComponent<Renderer> ().material.SetColor ("_OutlineColor", selectColor);
				hit.transform.GetChild (0).GetChild (0).gameObject.SetActive (true);
				if (Input.GetMouseButtonDown (0)) {
					if (tempGarbage == null && !isFlyGarbage) {
						tempGarbage = hit.transform;//cuando agarra el objeto lo asigana a un variable temporal
						tagetPos = tempGarbage.position;//almacena la posicion del objeto ya que si no se lanza a la basura este volvera a caer donde estaba 
						tempGarbage.parent = shotPointGarbage;//lo asigna al lanzador 
						tempGarbage.localPosition = Vector3.zero;//restablece la posicion para tenerlo en el lanzador
						tempGarbage.GetComponent<Renderer> ().material.SetColor ("_OutlineColor", selectColor);
						tempGarbage.GetComponent<BoxCollider> ().enabled = false;
					}
				}
			}
			if (hit.transform.name.Contains ("TrashCan") && !isFlyGarbage) {//pregunta si detecto una basura 
				hit.transform.GetComponent<Animator> ().SetBool ("Open", true);
				if (Input.GetMouseButtonUp (0)) {
					if(tempGarbage != null){
						tagetPos = hit.transform.Find ("Target").transform.position;
						tempGarbage.GetComponent<BoxCollider> ().enabled = true;
						isFlyGarbage = true;
						StartCoroutine (ShootGarbage (tempGarbage, tagetPos));
					}
				}
			}
		}

		if (Input.GetMouseButtonUp (0)) {
			if(tempGarbage != null){
				tempGarbage.GetComponent<Renderer> ().material.SetColor ("_OutlineColor", normalColor);
				tempGarbage.GetComponent<BoxCollider> ().enabled = true;
				isFlyGarbage = true;
				Invoke ("NormaliceFly", 3.0f);
				StartCoroutine (ShootGarbage (tempGarbage, tagetPos));
			}
		}
	}


	public IEnumerator ShootGarbage(Transform projectile, Vector3 target){
		projectile.position = shotPointGarbage.position;
		projectile.parent = null;
		tempGarbage = null;
		float target_Distance = Vector3.Distance(projectile.position, target);
		float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

		float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
		float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

		float flightDuration = target_Distance / Vx;

		projectile.rotation = Quaternion.LookRotation(target - projectile.position);
		float elapse_time = 0;
		while (elapse_time < flightDuration){
			projectile.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);
			elapse_time += Time.deltaTime;
			yield return null;
		}
	}

	void NormaliceFly(){
		CancelInvoke ("NormaliceFly");
		isFlyGarbage = false;
	}

	public void ManagePoints(string garbage, bool getpoint){
		numberCollector++;
		if (getpoint) {
			if (garbage == "PapelCarton") {
				pointPapelCarton++;
			} else if (garbage == "Ordinario") {
				pointOrdinarios++;
			} else if (garbage == "Plastico") {
				pointPlastico++;
			}
		}

		if (numberCollector >= 18) {
			if (pointPapelCarton >= 6 && pointOrdinarios >= 6 && pointPlastico >= 6) {
				popUpWin.SetActive (true);
				Invoke ("RestartLevel", 3.0f);
			} else {
				popUpLose.SetActive (true);
				Invoke ("RestartLevel", 3.0f);
			}
		}

		pointsPapelCarton.text = pointPapelCarton.ToString ();
		pointsOrdinarios.text = pointOrdinarios.ToString ();
		pointsPlastico.text = pointPlastico.ToString ();
	}

	void RestartLevel(){
		CancelInvoke ("RestartLevel");
		SceneManager.LoadScene ("Kitchen");
	}
}
