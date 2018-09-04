using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Garbage : MonoBehaviour {

	public enum TypeGarbage{
		Plastico, PapelCarton, Ordinario	
	};

	public TypeGarbage selectGarbage;

	GarbageController garbageEnlace;

	MeshRenderer tempRenderer;
	BoxCollider tempCollider;

	Transform targetPivot;

	void Start(){
		garbageEnlace = GameObject.FindObjectOfType<GarbageController> ();
		tempRenderer = GetComponent<MeshRenderer> ();
		tempCollider = GetComponent<BoxCollider> ();
		targetPivot = transform.GetChild (0).transform;
	}
	

	void Update(){
		if (targetPivot == null)
			return;
		targetPivot.LookAt (Camera.main.transform);
	}

	void OnTriggerEnter(Collider col){
		if (col.tag.Contains ("PapelCarton")) {
			garbageEnlace.isFlyGarbage = false;
			if (selectGarbage == TypeGarbage.PapelCarton) {
				garbageEnlace.ManagePoints ("PapelCarton", true);
				tempCollider.enabled = false;
			} else {
				garbageEnlace.ManagePoints ("PapelCarton", false);
				tempCollider.enabled = false;
			}
		} else if (col.tag.Contains ("Plastico")) {
			garbageEnlace.isFlyGarbage = false;
			if (selectGarbage == TypeGarbage.Plastico) {
				garbageEnlace.ManagePoints ("Plastico", true);
				tempCollider.enabled = false;
			} else {
				garbageEnlace.ManagePoints ("Plastico", false);
				tempCollider.enabled = false;
			}
		} else if (col.tag.Contains ("Ordinario")) {
			garbageEnlace.isFlyGarbage = false;
			if (selectGarbage == TypeGarbage.Ordinario) {
				garbageEnlace.ManagePoints ("Ordinario", true);
				tempCollider.enabled = false;
			} else {
				garbageEnlace.ManagePoints ("Ordinario", false);
				tempCollider.enabled = false;
			}
		}
	}
}
