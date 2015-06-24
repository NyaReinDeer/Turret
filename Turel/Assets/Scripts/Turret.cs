using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class Turret : MonoBehaviour {
	[Tooltip("Нач.позиции пушки и цели")]
	public Vector2 cannonPosition, targetPosition;
	[Tooltip("Скорость цели")]
	public float targetSpeed;
	[Tooltip("Скорость пули")]
	public float startSpeed;
	[Tooltip("Скорострельность в секунду")]
	public float speedShoot;
	[Tooltip("Максимальное количество патронов")]
	public int max;
	[Tooltip("Первый поворот")]
	public int firstAngle;
	
	public Transform aim,bullet,turret,enemy,flash,boom;
	public Text realCount;
	private int real;
	private Quaternion actualRotation;
	private float speedRotation=10.0f,deltaScope=0.01f,time,timerFlash=1f;
	private bool started=true,lowFire=true;
	private Vector2 dif;
	void Start () {
		real = max;realCount.text=real+"/"+max;
		turret.position = cannonPosition;
		enemy.position = targetPosition;
		actualRotation=Quaternion.Euler(new Vector3(0.0f,0.0f,firstAngle));
		speedShoot = 1 / speedShoot;
	}
	void Update () {
		/*Подсчет времени, обновление позиции прицела, вычисление угла*/
		time=CalculateTime();
		Snipe ();
		actualRotation=Calculate();
		/*Движение цели и обновление угла у турели в рантайме*/
		enemy.Translate (new Vector2 (targetSpeed, 0.0f) * Time.deltaTime);
		turret.localRotation = Quaternion.RotateTowards (turret.transform.localRotation, actualRotation, speedRotation);
		/*Первое ветвление, для разделения первого поворота и последующих действий*/
		if (started) {
			if(Mathf.Abs(turret.localRotation.z - actualRotation.z)<deltaScope){
				started = false;
			}
		} else if(real>0) {
			/*Условие статуса "наведен"*/
			if(Mathf.Abs(turret.rotation.z - actualRotation.z)<deltaScope){
				if(lowFire){
					Shoot ();
					real--;
					realCount.text=real+"/"+max;//UI
					if(!flash.gameObject.activeInHierarchy){//включение партикла
						flash.gameObject.SetActive(true);
						StartCoroutine(TimerFlash());
					}
					lowFire=false;
					StartCoroutine(TimerShoot());
				}
			}
		}

	}
	float CalculateTime(){//t^2*(v^2-v0^2)+t*2*(x-x0)+(x-x0)^2+(y-y0)^2=0
		float a, b, c, d,t1,t2,dx,dy;
		//float distanse = Vector3.Distance (turret.position, enemy.position);
		//Вычисляем коэффициенты
		dx = enemy.position.x - turret.position.x;//локализуем врага относительно турели
		dy = enemy.position.y - turret.position.y;
		a=targetSpeed*targetSpeed-startSpeed*startSpeed;
		b = 2 * dx;
		c = dx * dx + dy * dy;
		if (a == 0) {//если уравнение не квадр.
			return -b / c;
		} else {
			d = b * b - 4 * a * c;
			if(d==0){//если дискр. равен нулю - решение одно
				return -b / 2*a;
			}else if(d>0){
				float sqrD=Mathf.Sqrt(d);
				t1=(-b+sqrD)/2/a;
				t2=(-b-sqrD)/2/a;
				if(t2>0)
					t1=t2;
				if(t1>=0)
					return t1;
			}
		}
		return 0;
	}
	void Snipe(){
		Vector3 buf = enemy.position;
		buf.x += time;
		aim.position = buf;
	}
	Quaternion Calculate(){
		dif=aim.position - turret.position;
		dif.Normalize ();
		float rotZ = Mathf.Atan2 (dif.y, dif.x) * Mathf.Rad2Deg;
		return Quaternion.Euler (new Vector3(0.0f,0.0f,rotZ));
	}
	void Shoot(){
		Transform buf = Instantiate (bullet, flash.position, actualRotation)as Transform;
		Bullet obj = buf.GetComponent<Bullet> ();
		obj.time = time;
		obj.speed = startSpeed;
		obj.boom = boom;
	}
	IEnumerator TimerShoot(){
		yield return new WaitForSeconds(speedShoot);
		lowFire = true;
	}
	IEnumerator TimerFlash(){
		yield return new WaitForSeconds(timerFlash);
		flash.gameObject.SetActive (false);
	}
}
