using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {
	public float time, speed;
	public Transform boom;
	private bool flag=true,die=false;
	void Start(){
	}
	void Update () {
		transform.Translate (Vector2.right*speed*Time.deltaTime);
		if (flag) {
			StartCoroutine(Timer ());
			flag=false;
		}
		if (die) {
			boom.gameObject.SetActive(true);
			boom.transform.position=transform.position;
			gameObject.SetActive (false);
		}
	}
	IEnumerator Timer(){
		yield return new WaitForSeconds(time);
		die = true;
	}
}
