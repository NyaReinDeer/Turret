using UnityEngine;
using System.Collections;

public class Boom : MonoBehaviour {
	public float timer=0.5f;
	void OnEnable(){
		StartCoroutine (TimerBoom ());
	}
	IEnumerator TimerBoom(){
		yield return new WaitForSeconds(timer);
		gameObject.SetActive (false);
	}
}
