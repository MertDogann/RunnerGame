using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController current;
    public float limitX;
    public float xSpeed;
    public float runningSpeed;
    private float _currentSpeed;
    public GameObject clyinderPrefab;
    public List<RidingClyinder> clyinders;
    private bool _spawningBridge;
    public GameObject bridgePiecePrefabs;
    private BridgeSpawner _bridgeSpawner;
    private float _creatingBridgeTimer;
    public Animator animator; //Oyun ba�lamadan �nce karakterin durma animasyonu olsun startlevel dedikten sonra ko�ma animasyonuna ba�lamas� i�in animat�r ekledik.
    private bool _finished;
    private float _scoreTimer = 0;
    private float _lastTouchedX;

    
    void Start()
    {
        current = this;
    }

    void Update()
    {
        if (LevelController.Current == null || !LevelController.Current.gameActive) // Burada e�er levelcontrollerdeki startgame metodu sayesinde tap to start butonuna bas�lmad�ysa veya oyun aktif edilmemi�se uptade metodunu return yani bo� d�nd�r.
        {
            return;
        }
        float newX = 0;
        float touchXDelta = 0;
            if (Input.touchCount > 0)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    _lastTouchedX = Input.GetTouch(0).position.x;
                }else if ((Input.GetTouch(0).phase == TouchPhase.Began))
                {
                    touchXDelta = 5 * (_lastTouchedX - Input.GetTouch(0).position.x) / Screen.width;
                    _lastTouchedX = Input.GetTouch(0).position.x;
                }
            
            }else if (Input.GetMouseButton(0))
            {
                touchXDelta = Input.GetAxis("Mouse X");
            }

            newX = transform.position.x + xSpeed * touchXDelta * Time.deltaTime;
            newX = Mathf.Clamp(newX, -limitX, limitX);

        Vector3 newPosition = new Vector3(newX, transform.position.y, transform.position.z + _currentSpeed * Time.deltaTime);
        transform.position = newPosition;

        if (_spawningBridge)
        {
            _creatingBridgeTimer -= Time.deltaTime;
            if (_creatingBridgeTimer <0)
            {
                _creatingBridgeTimer = 0.01f;
                IncementClyinderVolume(-0.01f);
                GameObject creatingBridgePeace = Instantiate(bridgePiecePrefabs);
                Vector3 direction = _bridgeSpawner.endeReference.transform.position - _bridgeSpawner.startReference.transform.position;
                float distance = direction.magnitude;
                direction = direction.normalized;
                creatingBridgePeace.transform.forward = direction;
                float characterDistance = transform.position.z - _bridgeSpawner.startReference.transform.position.z;
                characterDistance = Mathf.Clamp(characterDistance, 0, distance);
                Vector3 newPiecePosition = _bridgeSpawner.startReference.transform.position + direction * characterDistance;
                newPiecePosition.x = transform.position.x;
                creatingBridgePeace.transform.position = newPiecePosition;
                if (_finished)
                {
                    _scoreTimer -= Time.deltaTime;
                    if (_scoreTimer < 0)
                    {
                        _scoreTimer = 0.03f;
                        LevelController.Current.ChangeScore(1);
                    }
                }

            }
        }
    }
    public void ChangeSpeed(float value) //Level controllerin playercontrollerin h�z�n� de�i�tirebilmesi i�in yani oyun ba�lamadan h�z�n� s�f�rlayabilmesi i�in metod olu�turuldu.
    {
        _currentSpeed = value;
    }
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.tag == "AddClyinder")
        {
            IncementClyinderVolume(0.1f);
            Destroy(other.gameObject) ;
        }else if (other.tag == "StartSpawnBridge")
        {
            StartSpawnerBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }else if (other.tag == "StopSpawnBridge")
        {
            StopSpawnerBridge();
            if (_finished)
            {
                LevelController.Current.FinishGame();
            }
        }else if (other.tag == "Finish")
        {
            _finished = true;
            StartSpawnerBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }else if (other.tag == "Coin")     //  �arpt���m�z objenin etiketi Coinse scoreyi guncelle ve objeyi yok et sonras�nda coinin etiketini untagged yap. Untagged yap�lmas�n�n sebebi biriken silindirler 2den fazla oldu�u zaman 2 kere puan vermesinden kaynakl�.
        {
            other.tag = "Untagged";
            LevelController.Current.ChangeScore(10);
            Destroy(other.gameObject);
        }
       
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Trap")
        {
            IncementClyinderVolume(-Time.fixedDeltaTime);
        }
    }

    public void IncementClyinderVolume(float value)
    {
        if(clyinders.Count == 0)
        {
            if (value > 0)
            {
                CreateClyinder(value);
            }else
            {
                if (_finished)
                {
                    LevelController.Current.FinishGame();

                }else
                {
                    Die();
                }
            }
        }else
        {
            clyinders[clyinders.Count - 1].IncementClyiderVolume(value);
        }
        
        
    }
    public void Die()//Die diye metod olu�tur. Dead animasyonu olu�tur o animasyonu burada aktif et platformlar�n ve karakteirn layerini duzenle Cameranin takip etmesini engelle 
    {
        animator.SetBool("dead", true);
        gameObject.layer = 6; //Ama� 2 farkl� layer olu�turararak karakterin edit men�s�nden o 2 karakteri birbirinnden etkilenmicek duruma getirerek d��mesini sa�lamak.
        Camera.main.transform.SetParent(null); //Kameran�n oyun bitti�i zaman karakteri takip etmeyi b�rak�r.
        LevelController.Current.GameOver();
    }
    public void CreateClyinder(float value)
    {
        RidingClyinder createdClyinder = Instantiate(clyinderPrefab, transform).GetComponent<RidingClyinder>();
        clyinders.Add(createdClyinder);
        createdClyinder.IncementClyiderVolume(value);
    }
    public void DestroyClyinder(RidingClyinder clyinder)
    {
        clyinders.Remove(clyinder);
        Destroy(clyinder.gameObject);
    }

    private void StartSpawnerBridge(BridgeSpawner spawner)
    {
        _bridgeSpawner = spawner;
        _spawningBridge = true;

    }
    private void StopSpawnerBridge()
    {
        _spawningBridge = false;
    }
}
