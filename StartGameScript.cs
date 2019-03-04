using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class StartGameScript : Photon.MonoBehaviour {

    public Image Health;
    public GameObject SelectShipObj;
    public int _frameRate = 60;
    public GameObject PanelLose;

    int SelectMission;
    int SelectShip;
    public Material Mat;

    public GameObject[] MassShip;
    public string[] MassStringShip;
    public GameObject[] MassMissions;

    public GameObject Regen;
    public string StrRegen;

    private GameObject DontDestroy;
	// Use this for initialization
	void Start () {
        DontDestroy = GameObject.FindGameObjectWithTag("DontDestroy");
        DontDestroy.GetComponent<DontDestroyScript>().isPlay = true;
        if (DontDestroy.GetComponent<DontDestroyScript>().NetworkGame == true)
            GameObject.FindGameObjectWithTag("Finish").SetActive(false);
        QualitySettings.vSyncCount = 0;
        if (DontDestroy.GetComponent<DontDestroyScript>().SelectControll != "Joustick")
            Joustick.SetActive(false);

        SelectMission = DontDestroy.GetComponent<DontDestroyScript>().SelectGame;
        SelectShip = DontDestroy.GetComponent<DontDestroyScript>().SelectShip;
        if (DontDestroy.GetComponent<DontDestroyScript>().NetworkGame == true)  //инициализация создания кораблей игроков в photon
        {
            for (int i = 0; i < MassShip.Length; i++)
            {
                if (i == SelectShip)
                {
                    float X = Random.Range(-60, 60);
                    float Y = Random.Range(-30, 30);
                    SelectShipObj = PhotonNetwork.Instantiate(MassStringShip[i], new Vector3(X, Y, 0f), Quaternion.identity, 0);
                }
            }
        }
        else  //инициализация игроков и миссий в локальной игре
        {
            for (int i = 0; i < MassShip.Length; i++)
            {
                if (i == SelectShip)
                {
                    SelectShipObj = Instantiate(MassShip[i], new Vector3(0f, 0f, 0f), Quaternion.identity);
                }
            }
            for (int i = 0; i < MassMissions.Length; i++)
            {
                if (i == SelectMission)
                {
                    Instantiate(MassMissions[i], new Vector3(0f, 0f, 0f), Quaternion.identity);
                }
            }
        }
	}

    float timeRegen = 5f;
	// Update is called once per frame
	void Update ()
    {
        #if UNITY_ANDROID  //увеличение фпс на андроид, по умолчанию 30.
		        if (_frameRate != Application.targetFrameRate)
			        Application.targetFrameRate = _frameRate;
        #endif

        if(SelectShipObj != null)
        {
            Health.fillAmount = (float)SelectShipObj.GetComponent<MoveScript>().NowHealth / SelectShipObj.GetComponent<MoveScript>().MaxHealth;
            if(SelectShipObj.GetComponent<MoveScript>().NowHealth <=0)
            {
                PhotonNetwork.Destroy(SelectShipObj);
                PanelLose.SetActive(true);
            }
        }

        if(DontDestroy.GetComponent<DontDestroyScript>().isPlay == true)
            if(fireOn == true)
                SelectShipObj.GetComponent<MoveScript>().FireFunction();

        if ((DontDestroy.GetComponent<DontDestroyScript>().SelectGame % 3 == 0)||(DontDestroy.GetComponent<DontDestroyScript>().NetworkGame == true))
        {
            if (DontDestroy.GetComponent<DontDestroyScript>().NetworkGame == false)
            {
                timeRegen -= Time.deltaTime;
                if (timeRegen <= 0)
                {
                    float X = Random.Range(-60, 60);
                    float Y = Random.Range(-30, 30);
                    Instantiate(Regen, new Vector3(X,Y,0f), this.transform.rotation);
                    timeRegen = Random.Range(10, 15);
                }
            }
            else
            {
                if(PhotonNetwork.isMasterClient == true)
                {
                    timeRegen -= Time.deltaTime;
                    if (timeRegen <= 0)
                    {
                        float X = Random.Range(-60, 60);
                        float Y = Random.Range(-30, 30);
                        PhotonNetwork.Instantiate(StrRegen, new Vector3(X, Y, 0f), Quaternion.identity, 0);
                        timeRegen = Random.Range(10, 15);
                    }
                }
            }
        }



	}
    bool fireOn = false;
    public void ClickFireOn()
    {
        fireOn = true;
    }
    public void ClickFireOff()
    {
        fireOn = false;
    }


    public GameObject WinPanel;
    public GameObject LosePanel;
    public GameObject PausePanel;
    public GameObject Joustick;
    public GameObject FireButton;
    public void PauseClick()
    {
        if(DontDestroy.GetComponent<DontDestroyScript>().NetworkGame == false)
        if (DontDestroy.GetComponent<DontDestroyScript>().isPlay == true)
        {
            DontDestroy.GetComponent<DontDestroyScript>().isPlay = false;
            PausePanel.SetActive(true);
        }
    }
    public void ClosePause()
    {
        DontDestroy.GetComponent<DontDestroyScript>().isPlay = true;
        PausePanel.SetActive(false);
    }
    public void BackToMenu()
    {
        if (DontDestroy.GetComponent<DontDestroyScript>().NetworkGame == false)
        {
            Application.LoadLevel(0);
            Destroy(DontDestroy);
        }
        else
        {
            Destroy(DontDestroy);
            PhotonNetwork.Disconnect();
            SceneManager.LoadScene(0);
        }
    }
    public void Reload()
    {
        if (DontDestroy.GetComponent<DontDestroyScript>().NetworkGame == false)
            Application.LoadLevel(1);
        else
        {
            for (int i = 0; i < MassShip.Length; i++)
            {
                if (i == SelectShip)
                {
                    float X = Random.Range(-60, 60);
                    float Y = Random.Range(-30, 30);
                    SelectShipObj = PhotonNetwork.Instantiate(MassStringShip[i], new Vector3(X, Y, 0f), Quaternion.identity, 0);
                }
            }
        }
    }
}
