using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public static LevelController Current; // Levelcontroller scriptinin t�m scriptlerden a��lmas� i�in static hale getirdik.
    public bool gameActive = false; //Oyunun UI butonlar�na basmasdan ba�lamas�n� engellemek i�in gameactivite olu�turduk ve bunu ba�ta false yapt�k.
    public GameObject startMenu, gameMenu, finishMenu, gameOverMenu; // Menulerin setactivitesini kontrol etmek i�in.
    public Text scoreText, finishScoreText, currentLevelText, nextLevelText, startingGameMenuMoneyText, gameOverMenuMoneyText, finishGameMenuMoneyText; //UI elemanlar�n�n kontorl�n� sa�lamak i�in olu�turuldu.
    int score; //Oyun sonundaki toplam scoremizi �l�mek i�in eklendi.
    int currentLevel;
    public float maxDistance; //Oyun ba�lad��� anda karakterin biti� �izgisine olan uzakl���n� tutmam�z gerekiyor. ��nk� slider i�erisineki bar�n do�ru olmas� i�in.
    public GameObject finisLine; 
    public Slider levelProgressBar;
    
    void Start()
    {
        Current = this; //Static hale getirdi�imiz scripti kendisine e�itliyoruz.
        currentLevel = PlayerPrefs.GetInt("currentLevel"); // Oyunun level sistemi i�in oyun haf�zas�na girip bana mevcut level ad�nda bir girdi getir diyoruz.Bu de�er 0 sa oyuncu zaten ilk levelden ba�l�cak.Girdi girildiyse o levelden ba�l�cak.

        if (SceneManager.GetActiveScene().name != "Level " + currentLevel) //E�er �u anki levelin ismi level + current level de�ilse o leveli y�kle.    
        {
            SceneManager.LoadScene("Level " + currentLevel); // + current level "level" ile birle�ip string olu�turucak.    
            
        }else //E�er �u anki levelin ismi level + currentse o levelle alakal� d�zenlemeler yap.
        {
            currentLevelText.text = (currentLevel + 1).ToString(); //Slider i�erisindeki mevcut leveli g�ncelle.
            nextLevelText.text = (currentLevel + 2).ToString(); //Slider i�erisindeki next leveli g�ncelle.
            UptadeMoneyTexts();
        }
    }
    
    void Update()
    {
        if (gameActive) // Burada oyun aktif oldu�u s�rece sliderin de s�rekli aktif oldu�unu bak�yoruz.
        {
            PlayerController player = PlayerController.current;
            float distance = finisLine.transform.position.z - PlayerController.current.transform.position.z;
            levelProgressBar.value = 1 - (distance / maxDistance);
            
        }
    }
    public void StartLevel() //Oyun ba�lamadan �nce start level butonuna bast�ktan sonra olmas� gerekenlerle ilgili metod olu�turuldu.
    {
        maxDistance = finisLine.transform.position.z - PlayerController.current.transform.position.z;  //Karakterin finish �izgisinin z pozisyonundan karakterin �u anki pozisyonun z sini ��kard�k .Finish �izgisini oyun i�erisinde ayarlay�p obje olarak eklenmi�tri.
        PlayerController.current.ChangeSpeed(PlayerController.current.runningSpeed); //oyun ba�lay�nca karakterin h�z�n� playerin maxsimum h�z�na e�itle.
        startMenu.SetActive(false);
        gameMenu.SetActive(true);
        PlayerController.current.animator.SetBool("running", true);
        gameActive = true;
    }
    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void NextLevel()
    {
        SceneManager.LoadScene("Level " + (currentLevel + 1));
    }
    public void GameOver()
    {
        UptadeMoneyTexts();
        gameMenu.SetActive(false);
        gameOverMenu.SetActive(true);
        gameActive = false;

    }
    public void FinishGame()
    {
        GiveMoneyToPlayer(score); //Oyun sonunda toplad���m�z score de�erini money ile toplayarak yeni money olu�turduk.
        PlayerPrefs.SetInt("Level " , currentLevel + 1);
        finishScoreText.text = score.ToString();
        finishMenu.SetActive(true);
        gameMenu.SetActive(false);
        gameActive = false;
    }
    public void ChangeScore(int incement)
    {
        score += incement;
        scoreText.text = score.ToString();
    }
    public void UptadeMoneyTexts()//Karakterin paras�n� eklemek i�in tek bir metod olu�turduk.
    {
        int money = PlayerPrefs.GetInt("money");
        startingGameMenuMoneyText.text = money.ToString();
        finishGameMenuMoneyText.text = money.ToString();
        finishGameMenuMoneyText.text = money.ToString();
    }
    public void GiveMoneyToPlayer(int increment) //Burada karakterin oyun sonundaki toplad��� skorlar� moneye ekleyip hepsini money �eklinde yazd�k.
    {
        int money = PlayerPrefs.GetInt("money");
        money = Mathf.Max(0, money +increment);
        PlayerPrefs.SetInt("money", money);
        UptadeMoneyTexts();
    }
    //Oyuncuya para vermeyi yarayan GiveToMoney fonksiyonu olu�tur.Parametre int incretment koyuyuoruz ve mathf.max fonksiyonu ile s�f�rdan b�y�k olan de�ere e�itliyruz..art��� moneye e�itliyoruz. Fonksiyona uptademoneytexti ekle ve finish game fonksiyonuna �u anki scoreyi yaz. 
}
