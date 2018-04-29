using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	public Candy candy;
	public int colunmNum = 10;   //定義行個數
	public int rowNum = 7;       //定義列個數
	public GameObject game;       
	private ArrayList CandyArr;

	public AudioClip swapClip;
	public AudioClip match3Clip;
	public AudioClip explodeClip;
	public AudioClip wrongClip;

	// Use this for initialization
	void Start () {
		CandyArr = new ArrayList ();
		for (int rowIndex = 0; rowIndex < rowNum; rowIndex++) {
			ArrayList temp = new ArrayList ();
			for (int colunmIndex = 0; colunmIndex < colunmNum; colunmIndex++) {
				Candy c = AddCandy (colunmIndex,rowIndex);
				temp.Add (c);
			}
			CandyArr.Add (temp);
		}
		if (CheckMatches ())      //一開始就做檢測
			RemoveMatches ();
	}

	private Candy AddCandy(int colIndex, int rowIndex){
		Object o = Instantiate (candy);
		Candy c = o as Candy;
		c.transform.parent = game.transform;
		c.ColunmIndex = colIndex;
		c.RowIndex = rowIndex;
		c.UpdatePosition ();
		c.game = this;
		return c;
	}

	//獲取二維組數元素
	private Candy GetCandy(int colIndex, int rowIndex){
		ArrayList tmp = CandyArr [rowIndex] as ArrayList;
		Candy c = tmp [colIndex] as Candy;
		return c;
	}
	//設定二維組數元素
	private void SetCandy(int colIndex, int rowIndex, Candy c){
		ArrayList tmp = CandyArr [rowIndex] as ArrayList;
		tmp [colIndex] = c;
	}

	private Candy crt;
	public void Select(Candy c){
		//Remove (c);
		//return;
		if (crt == null) {
			crt = c;
			crt.selected = true;
			return;
		} else {
			if ((Mathf.Abs (crt.RowIndex - c.RowIndex) + Mathf.Abs (crt.ColunmIndex - c.ColunmIndex)) == 1) {
				/*Exchange (crt, c);
				CheckMatches ();
				RemoveMatches ();*/
				StartCoroutine (Exchange2 (crt, c));
			} else {
				GetComponent<AudioSource> ().PlayOneShot (wrongClip);
			}
			crt.selected = false;
			crt = null;
		}
	}

	IEnumerator Exchange2(Candy c1, Candy c2){
		Exchange (c1, c2);
		yield return new WaitForSeconds (0.4f);
		if (CheckMatches ()) {
			RemoveMatches ();
		} else {
			Exchange (c1, c2);
		}
	}


	private void Exchange(Candy c1, Candy c2){

		GetComponent<AudioSource> ().PlayOneShot (swapClip);
		SetCandy (c1.ColunmIndex, c1.RowIndex, c2);
		SetCandy (c2.ColunmIndex, c2.RowIndex, c1);

		int ColIndex = c1.ColunmIndex;
		c1.ColunmIndex = c2.ColunmIndex;
		c2.ColunmIndex = ColIndex;

		int RowIndex = c1.RowIndex;
		c1.RowIndex = c2.RowIndex;
		c2.RowIndex = RowIndex;
		//c1.UpdatePosition ();
		//c2.UpdatePosition ();
		c1.TweenToPosition();
		c2.TweenToPosition();
	}

	//檢測是否可以消除
	private bool CheckMatches(){
	
		return CheckHorizontalMatches ()||CheckVerticalMatches ();
	}
	//檢查水平方向
	private bool CheckHorizontalMatches(){
		bool result = false;
		for (int rowIndex = 0; rowIndex < rowNum; rowIndex++) {   //檢測從最上面那一列開始檢測
			for (int columIndex = 0; columIndex < colunmNum - 2; columIndex++) {
				if ((GetCandy (columIndex, rowIndex).type == GetCandy (columIndex + 1, rowIndex).type) &&
				   (GetCandy (columIndex + 2, rowIndex).type == GetCandy (columIndex + 1, rowIndex).type)) {
					//Debug.Log (rowIndex + " " + columIndex + " " + (columIndex + 1) + " " + (columIndex + 2));
					GetComponent<AudioSource>().PlayOneShot(match3Clip);
					result = true;
					AddMatch (GetCandy (columIndex, rowIndex));
					AddMatch (GetCandy (columIndex + 1, rowIndex));
					AddMatch (GetCandy (columIndex + 2, rowIndex));	  		
				}
			}
		}
		return result;
	}
	//檢查鉛直方向
	private bool CheckVerticalMatches(){
		bool result = false;
		for (int columIndex  = 0; columIndex < colunmNum ; columIndex++) {   //檢測從最上面那一列開始檢測
			for (int rowIndex = 0; rowIndex < rowNum - 2; rowIndex++) {
				if ((GetCandy (columIndex, rowIndex+1).type == GetCandy (columIndex, rowIndex).type) &&
					(GetCandy (columIndex, rowIndex+1).type == GetCandy (columIndex, rowIndex+2).type)) {
					//Debug.Log (rowIndex + " " + columIndex + " " + (columIndex + 1) + " " + (columIndex + 2));
					GetComponent<AudioSource>().PlayOneShot(match3Clip);
					result = true;
					AddMatch (GetCandy (columIndex, rowIndex));
					AddMatch (GetCandy (columIndex, rowIndex+1));
					AddMatch (GetCandy (columIndex, rowIndex+2));	  		
				}
			}
		}
		return result;
	}
	private ArrayList matches;
	private void AddMatch(Candy c){
		if (matches == null)
			matches = new ArrayList ();
		int index = matches.IndexOf (c);
		if (index == -1)
			matches.Add (c);
	}
	private void AddEffect(Vector3 pos){
		Instantiate (Resources.Load ("Prefabs/Explosion"), pos, Quaternion.identity);
		CameraShake.shakeFor (0.5f, 0.1f);
	}

	//刪除
	private void Remove(Candy c){
		AddEffect (c.transform.position);
		GetComponent<AudioSource>().PlayOneShot(explodeClip);
		c.Dispose ();

		int colIndex = c.ColunmIndex;
		for (int rowIndex = c.RowIndex+1; rowIndex < rowNum; rowIndex++) {
			Candy c2 = GetCandy (colIndex, rowIndex);
			c2.RowIndex--;
			c2.TweenToPosition();
			SetCandy (colIndex, rowIndex - 1, c2);
		}
		//增加新的 candy 至 此行上方
		Candy newCandy = AddCandy(colIndex,rowNum-1);
		newCandy.RowIndex = rowNum;
		newCandy.UpdatePosition ();
		newCandy.RowIndex--;
		newCandy.TweenToPosition();
		SetCandy (colIndex, rowNum - 1, newCandy);

	}

	private void RemoveMatches(){
		Candy tmp;
		for (int i = 0; i < matches.Count; i++) {
			tmp = matches [i] as Candy;
			Remove (tmp);
		}
		matches = new ArrayList ();
		/*if (CheckMatches ()) {
			RemoveMatches ();
		}*/
		StartCoroutine (WaitAndCheck());
	}
	IEnumerator WaitAndCheck(){
		yield return new WaitForSeconds (0.5f);
		if (CheckMatches ()) {
			RemoveMatches ();
		}
	}
}
