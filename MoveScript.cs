using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///скрипт для игрока

public class MoveScript : Photon.MonoBehaviour {

    private ControllerScript contScript;
    private Transform thisTramsform;
    private Transform spawner1;
    private Transform spawner2;
    private Transform spawner3;
    private Vector3 ForwardVector;

    public int MaxHealth;
    public int NowHealth;

    public float Speed;
    public float SpeedFire;
    public float Damage;
    float NowTimeFire = 0;
    public Material Mat;
    public GameObject ShipInclude;
    public GameObject Trail;
    public GameObject Particles;
    public GameObject SpawnerFire1;
    public GameObject SpawnerFire2;
    public GameObject SpawnerFire3;
    public GameObject Fire;
    public string StrFire;
    public GameObject FireCollision;
    private float moveHorizontal = 0f;
    private float moveVertical = 0f;

    private GameObject MainCamera;
    private GameObject DontDestroy;
    private string SelectControl;
	
	
	// Use this for initialization
	void Start () {
        DontDestroy = GameObject.FindGameObjectWithTag("DontDestroy");
        MainCamera = Camera.main.gameObject;
        SelectControl = DontDestroy.GetComponent<DontDestroyScript>().SelectControll;
		
        if(SelectControl == "Joustick")
            contScript = GameObject.FindGameObjectWithTag("Joustick").GetComponent<ControllerScript>();
        thisTramsform = this.transform;
        if (SpawnerFire1 != null)
            spawner1 = SpawnerFire1.transform;
        if (SpawnerFire2 != null)
            spawner2 = SpawnerFire2.transform;
        if (SpawnerFire3 != null)
            spawner3 = SpawnerFire3.transform;
		
        ForwardVector = Vector3.forward;

        Gradient gr = new Gradient();
        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;

        colorKey = new GradientColorKey[2];
        colorKey[0].color = Mat.color;
        colorKey[0].time = 0.0f;
        colorKey[1].color = Mat.color;
        colorKey[1].time = 1.0f;

        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 0.5f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 0.0f;
        alphaKey[1].time = 1.0f;

        gr.SetKeys(colorKey, alphaKey);
        Trail.GetComponent<TrailRenderer>().colorGradient = gr;
        ShipInclude.GetComponent<SpriteRenderer>().color = Mat.color;
        Particles.GetComponent<ParticleSystem>().startColor = Mat.color;
        Fire.GetComponent<SpriteRenderer>().color = Mat.color;
        FireCollision.GetComponentInChildren<ParticleSystem>().startColor = Mat.color;

        Debug.Log(Mat.color.r);
	}

    float tineRestart = 3f;
	// Update is called once per frame
	void FixedUpdate () {

        if (NowHealth > MaxHealth)
            NowHealth = MaxHealth;


        if (DontDestroy.GetComponent<DontDestroyScript>().isPlay == true)  //выбов вида управления
        {
            if (SelectControl == "Joustick")
            {
                if (contScript.Horizontal() != 0 || contScript.Vertical() != 0)
                {
                    if (DontDestroy.GetComponent<DontDestroyScript>().NetworkGame == true)
                        photonView.RPC("CharacterMove", PhotonTargets.All);
                    else
                        CharacterMove();
                }
            }
            else
            {
                if (Input.acceleration.x != 0 || Input.acceleration.y != 0)
                {
                    if (DontDestroy.GetComponent<DontDestroyScript>().NetworkGame == true)
                        photonView.RPC("CharacterMove", PhotonTargets.All);
                    else
                        CharacterMove();
                }
            }

            if (NowTimeFire < SpeedFire)
            {
                NowTimeFire += Time.deltaTime;
            }
            else
            {
                NowTimeFire = SpeedFire;
            }
            if (Input.GetKey(KeyCode.Space))
            {
                FireFunction();
            }
        }
	}

    Vector3 moveVector;
    [PunRPC] 
    void CharacterMove()  //rpc фунция перемещения
    {
        if (GameObject.Find("DontDestroy").GetComponent<DontDestroyScript>().NetworkGame == false)
        {
            if (SelectControl == "Joustick")
            {
                Vector3 direction = new Vector3(contScript.Horizontal(), contScript.Vertical(), 0);
                thisTramsform.position = Vector3.MoveTowards(thisTramsform.position, thisTramsform.position + direction, Speed * Time.deltaTime);

                moveVector.x = contScript.Horizontal() * Speed;
                moveVector.y = contScript.Vertical() * Speed;

                if (contScript.Horizontal() != 0 || contScript.Vertical() != 0)
                {
                    Vector3 direct = Vector3.RotateTowards(thisTramsform.forward, moveVector, Speed, 0.0f);
                    thisTramsform.rotation = Quaternion.LookRotation(ForwardVector, ForwardVector + direct);
                }
            }
            else
            {
                float inpX = 0;  //калибровка акселерометра
                if(Input.acceleration.x>0.1)
                    inpX = 1;
                if (Input.acceleration.x < -0.1)
                    inpX = -1;
                float inpY = 0;
                if (Input.acceleration.y > 0.1)
                    inpY = 1;
                if (Input.acceleration.y < -0.1)
                    inpY = -1;

                Vector3 direction = new Vector3(inpX, inpY, 0);
                thisTramsform.position = Vector3.MoveTowards(thisTramsform.position, thisTramsform.position + direction, Speed * Time.deltaTime);

                moveVector.x = inpX * Speed;
                moveVector.y = inpY * Speed;

                if (inpX != 0 || inpY != 0)
                {
                    Vector3 direct = Vector3.RotateTowards(thisTramsform.forward, moveVector, Speed, 0.0f);
                    thisTramsform.rotation = Quaternion.LookRotation(ForwardVector, ForwardVector + direct);
                }
            }
        }
        else
        {
            if (photonView.isMine)
            {
                if (SelectControl == "Joustick")
                {
                    Vector3 direction = new Vector3(contScript.Horizontal(), contScript.Vertical(), 0);
                    thisTramsform.position = Vector3.MoveTowards(thisTramsform.position, thisTramsform.position + direction, Speed * Time.deltaTime);

                    moveVector.x = contScript.Horizontal() * Speed;
                    moveVector.y = contScript.Vertical() * Speed;

                    if (contScript.Horizontal() != 0 || contScript.Vertical() != 0)
                    {
                        Vector3 direct = Vector3.RotateTowards(thisTramsform.forward, moveVector, Speed, 0.0f);
                        thisTramsform.rotation = Quaternion.LookRotation(ForwardVector, ForwardVector + direct);
                    }
                }
                else
                {
                    Vector3 direction = new Vector3(Input.acceleration.x, Input.acceleration.y, 0);
                    thisTramsform.position = Vector3.MoveTowards(thisTramsform.position, thisTramsform.position + direction, Speed * Time.deltaTime);

                    moveVector.x = Input.acceleration.x * Speed;
                    moveVector.y = Input.acceleration.y * Speed;

                    if (Input.acceleration.x != 0 || Input.acceleration.y != 0)
                    {
                        Vector3 direct = Vector3.RotateTowards(thisTramsform.forward, moveVector, Speed, 0.0f);
                        thisTramsform.rotation = Quaternion.LookRotation(ForwardVector, ForwardVector + direct);
                    }
                }
            }


            if (isSinch)  //перемещение иных объектов, не созданных в этом клиенте
            {
                if (Vector3.Distance(oldPosition, newPosition) > 1f)
                {
                    this.transform.position = oldPosition = newPosition;
                }
                else
                {
                    offsetTime += Time.deltaTime * 1f;
                    this.transform.position = Vector3.Lerp(oldPosition, oldPosition, offsetTime);
                }
            }
            if (isSinchRot)
            {
                if (Quaternion.Angle(oldRotation, newRotation) > 1f)
                {
                    this.transform.rotation = oldRotation = newRotation;
                }
                else
                {
                    offsetTimeRot += Time.deltaTime * 1f;
                    this.transform.rotation = Quaternion.Lerp(oldRotation, newRotation, offsetTimeRot);
                }
            }
        }
    }

    [PunRPC]  //функция получения урона
    void EnterDamage(int damage)
    {
        NowHealth -= damage;
    }

    public void FireFunction()
    {
        if (NowTimeFire == SpeedFire)
        {
            if (SpawnerFire1 != null)
            {
                GameObject InstObj;
                if (GameObject.Find("DontDestroy").GetComponent<DontDestroyScript>().NetworkGame == false)
                {
                    InstObj = Instantiate(Fire, SpawnerFire1.transform.position, spawner1.transform.rotation);
                    InstObj.GetComponent<GunScript>().Creator = "Player";
                    InstObj.GetComponent<GunScript>().Mat = Mat;
                }
                else
                {
                    InstObj = PhotonNetwork.Instantiate(StrFire, SpawnerFire1.transform.position, spawner1.transform.rotation, 0);
                    InstObj.GetComponent<GunScript>().Creator = PhotonNetwork.playerName;
                    InstObj.GetComponent<GunScript>().Mat = Mat;
                }
            }
            if (SpawnerFire2 != null)
            {
                GameObject InstObj;
                if (GameObject.Find("DontDestroy").GetComponent<DontDestroyScript>().NetworkGame == false)
                {
                    InstObj = Instantiate(Fire, SpawnerFire2.transform.position, spawner2.transform.rotation);
                    InstObj.GetComponent<GunScript>().Creator = "Player";
                    InstObj.GetComponent<GunScript>().Mat = Mat;
                }
                else
                {
                    InstObj = PhotonNetwork.Instantiate(StrFire, SpawnerFire2.transform.position, spawner2.transform.rotation, 0);
                    InstObj.GetComponent<GunScript>().Creator = PhotonNetwork.playerName;
                    InstObj.GetComponent<GunScript>().Mat = Mat;
                }
            }
            if (SpawnerFire3 != null)
            {
                GameObject InstObj;
                if (GameObject.Find("DontDestroy").GetComponent<DontDestroyScript>().NetworkGame == false)
                {
                    InstObj = Instantiate(Fire, SpawnerFire3.transform.position, spawner3.transform.rotation);
                    InstObj.GetComponent<GunScript>().Creator = "Player";
                    InstObj.GetComponent<GunScript>().Mat = Mat;
                }
                else
                {
                    InstObj = PhotonNetwork.Instantiate(StrFire, SpawnerFire3.transform.position, spawner3.transform.rotation, 0);
                    InstObj.GetComponent<GunScript>().Creator = PhotonNetwork.playerName;
                    InstObj.GetComponent<GunScript>().Mat = Mat;
                }
            }
            
            NowTimeFire = 0;
        }
    }



    Vector3 oldPosition = Vector3.zero;
    Vector3 newPosition = Vector3.zero;
    float offsetTime = 0;
    bool isSinch = false;
    Quaternion oldRotation = new Quaternion(0f, 0f, 0f, 0f);
    Quaternion newRotation = new Quaternion(0f, 0f, 0f, 0f);
    float offsetTimeRot = 0;
    bool isSinchRot = false;

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)  //функция синхронизации объектов в сети photon
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
            oldPosition = this.transform.position;
            newPosition = pos;
            offsetTime = 0;
            isSinch = true;

            oldRotation = this.transform.rotation;
            newRotation = rot;
            offsetTimeRot = 0;
            isSinchRot = true;
            this.transform.localScale = scale;

            Color NewColor = new Color(colorR, colorG, colorB);
            Gradient gr = new Gradient();
            GradientColorKey[] colorKey;
            GradientAlphaKey[] alphaKey;

            colorKey = new GradientColorKey[2];
            colorKey[0].color = NewColor;
            colorKey[0].time = 0.0f;
            colorKey[1].color = NewColor;
            colorKey[1].time = 1.0f;

            alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 0.5f;
            alphaKey[0].time = 0.0f;
            alphaKey[1].alpha = 0.0f;
            alphaKey[1].time = 1.0f;

            gr.SetKeys(colorKey, alphaKey);
            Trail.GetComponent<TrailRenderer>().colorGradient = gr;
            ShipInclude.GetComponent<SpriteRenderer>().color = NewColor;
            Particles.GetComponent<ParticleSystem>().startColor = NewColor;
        }
    }
}
