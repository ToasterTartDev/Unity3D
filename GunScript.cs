using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///скрипт на пуле в игре

public class GunScript : Photon.MonoBehaviour {


    public string Creator;
    public int Damage = 0;
    public float lifeTime = 1f;
    public float Speed = 30f;
    public GameObject PartGun;
    public string StrPartGun;
    public Material Mat;

    private GameObject DontDestroy;
	// Use this for initialization
	void Start () {
        DontDestroy = GameObject.FindGameObjectWithTag("DontDestroy");
	}
	
	//// Update is called once per frame
	void Update () {
        if (DontDestroy.GetComponent<DontDestroyScript>().isPlay == true)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
            {
                Destroy(this.gameObject);
            }
            this.transform.Translate(Vector3.up * Speed * Time.deltaTime);
        }
	}

    public void OnTriggerEnter2D(Collider2D col)  //столкновение
    {
        if(Creator == "Player")
            if(col.gameObject.tag == "Enemy")
            {
                Destroy(this.gameObject);
                if (DontDestroy.GetComponent<DontDestroyScript>().NetworkGame == false)
                {
                    Instantiate(PartGun, this.transform.position, Quaternion.identity);
                    col.gameObject.GetComponent<EnemyScript>().NowHealth -= Damage;
                }
                else
                {
                    PhotonNetwork.Instantiate("FireCollision", this.transform.position, Quaternion.identity, 0);

                }
            }
        if (Creator == "Enemy")
            if (col.gameObject.tag == "Player")
            {
                Destroy(this.gameObject);
                Instantiate(PartGun, this.transform.position, Quaternion.identity);
                col.gameObject.GetComponent<MoveScript>().NowHealth -= Damage;
                col.gameObject.GetComponent<AudioSource>().Play();
            }

        if (col.gameObject.tag == "Player")  //столконовение с др игроком
        {
            if(DontDestroy.GetComponent<DontDestroyScript>().NetworkGame == true)
                if(Creator != PhotonNetwork.playerName)
                {
                    PhotonView photonView1 = PhotonView.Find(col.gameObject.GetComponent<PhotonView>().viewID);
                    photonView1.RPC("EnterDamage", PhotonTargets.All, Damage);
                    PhotonView.DestroyObject(this.gameObject);
                    GameObject Part = PhotonNetwork.Instantiate(StrPartGun, this.transform.position, Quaternion.identity, 0);
                    col.gameObject.GetComponent<AudioSource>().Play();
                }
        }
    }


    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)  //синхронизация позиции, вектора поворота и цвета пули в мультиплеере photon
    {

        Vector3 pos = this.transform.position;
        Quaternion rot = this.transform.rotation;
        Vector3 scale = this.transform.localScale;
        float colorR = Mat.color.r;
        float colorG = Mat.color.g;
        float colorB = Mat.color.b;

        stream.Serialize(ref pos);
        stream.Serialize(ref rot);
        stream.Serialize(ref scale);
        stream.Serialize(ref colorR);
        stream.Serialize(ref colorG);
        stream.Serialize(ref colorB);

        if (stream.isReading)
        {
            this.transform.position = pos;
            this.transform.rotation = rot;
            this.transform.localScale = scale;

            Color NewColor = new Color(colorR, colorG, colorB);
            this.GetComponent<SpriteRenderer>().color = NewColor;
        }
    }
}
