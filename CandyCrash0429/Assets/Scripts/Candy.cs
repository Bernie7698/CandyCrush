using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candy : MonoBehaviour {

	public int ColunmIndex = 0;
	public int RowIndex = 0;
	public float xoff = -4.5f;   //x 的偏移量
	public float yoff = -3.0f;   //y 的偏移量

	public GameObject[] bgs;     //糖果圖形陣列
	private GameObject bg;
	public int type;             //產生的 Candy 編號
	public GameController game;
	public int CandyTypeNumber = 6;   //Candy 種類

	private SpriteRenderer sr;
	public bool selected{
		set{
			if (sr != null) {
				sr.color = value ? Color.blue : Color.white;
			}
		}
	}
	void Start(){
		sr = bg.GetComponent<SpriteRenderer> ();
	}

	void OnMouseDown(){
		game.Select (this);
	}

	private void AddRandomBG(){
		if (bg != null)
			return;
		type = Random.Range (0, Mathf.Min(CandyTypeNumber, bgs.Length));
		bg = Instantiate (bgs [type]);
		bg.transform.parent = this.transform;
	}

	public void UpdatePosition(){
		AddRandomBG ();
		transform.position = new Vector3 (ColunmIndex + xoff, 
			                              RowIndex + yoff, 0);
	}

	public void TweenToPosition(){
		AddRandomBG ();
		iTween.MoveTo (this.gameObject, 
		  iTween.Hash ("x", ColunmIndex + xoff, "y", RowIndex + yoff, "time", 0.3f));
	}

	//讓 Candy  銷毀掉
	public void Dispose(){
		game = null;
		Destroy (bg.gameObject);
		Destroy (this.gameObject);
	}
}
