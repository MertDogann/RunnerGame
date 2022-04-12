using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    public static LevelController Current; // Levelcontroller scriptinin tüm scriptlerden açýlmasý için static hale getirdik.
    public bool gameActive = false; //Oyunun UI butonlarýna basmasdan baþlamasýný engellemek için gameactivite oluþturduk ve bunu baþta false yaptýk.
    public GameObject startMenu, gameMenu, finishMenu, gameOverMenu; // Menulerin setactivitesini kontrol etmek için.
    public Text scoreText, finishScoreText, currentLevelText, nextLevelText, startingGameMenuMoneyText, gameOverMenuMoneyText, finishGameMenuMoneyText; //UI elemanlarýnýn kontorlünü saðlamak için oluþturuldu.
    int score; //Oyun sonundaki toplam scoremizi ölçmek için eklendi.
    int currentLevel;
    public float maxDistance; //Oyun baþladýðý anda karakterin bitiþ çizgisine olan uzaklýðýný tutmamýz gerekiyor. Çünkü slider içerisineki barýn doðru olmasý için.
    public GameObject finisLine; 
    public Slider levelProgressBar;
    
    void Start()
    {
        Current = this; //Static hale getirdiðimiz scripti kendisine eþitliyoruz.
        currentLevel = PlayerPrefs.GetInt("currentLevel"); // Oyunun level sistemi için oyun hafýzasýna girip bana mevcut level adýnda bir girdi getir diyoruz.Bu deðer 0 sa oyuncu zaten ilk levelden baþlýcak.Girdi girildiyse o levelden baþlýcak.

        if (SceneManager.GetActiveScene().name != "Level " + currentLevel) //Eðer þu anki levelin ismi level + current level deðilse o leveli yükle.    
        {
            SceneManager.LoadScene("Level " + currentLevel); // + current level "level" ile birleþip string oluþturucak.    
            
        }else //Eðer þu anki levelin ismi level + currentse o levelle alakalý düzenlemeler yap.
        {
            currentLevelText.text = (currentLevel + 1).ToString(); //Slider içerisindeki mevcut leveli güncelle.
            nextLevelText.text = (currentLevel + 2).ToString(); //Slider içerisindeki next leveli güncelle.
            UptadeMoneyTexts();
        }
    }
    
    void Update()
    {
        if (gameActive) // Burada oyun aktif olduðu sürece sliderin de sürekli aktif olduðunu bakýyoruz.
        {
            PlayerController player = PlayerController.current;
            float distance = finisLine.transform.position.z - PlayerController.current.transform.position.z;
            levelProgressBar.value = 1 - (distance / maxDistance);
            
        }
    }
    public void StartLevel() //Oyun baþlamadan önce start level butonuna bastýktan sonra olmasý gerekenlerle ilgili metod oluþturuldu.
    {
        maxDistance = finisLine.transform.position.z - PlayerController.current.transform.position.z;  //Karakterin finish çizgisinin z pozisyonundan karakterin þu anki pozisyonun z sini çýkardýk .Finish çizgisini oyun içerisinde ayarlayýp obje olarak eklenmiþtri.
        PlayerController.current.ChangeSpeed(PlayerController.current.runningSpeed); //oyun baþlayýnca karakterin hýzýný playerin maxsimum hýzýna eþitle.
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
        GiveMoneyToPlayer(score); //Oyun sonunda topladýðýmýz score deðerini money ile toplayarak yeni money oluþturduk.
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
    public void UptadeMoneyTexts()//Karakterin parasýný eklemek için tek bir metod oluþturduk.
    {
        int money = PlayerPrefs.GetInt("money");
        startingGameMenuMoneyText.text = money.ToString();
        finishGameMenuMoneyText.text = money.ToString();
        finishGameMenuMoneyText.text = money.ToString();
    }
    public void GiveMoneyToPlayer(int increment) //Burada karakterin oyun sonundaki topladýðý skorlarý moneye ekleyip hepsini money þeklinde yazdýk.
    {
        int money = PlayerPrefs.GetInt("money");
        money = Mathf.Max(0, money +increment);
        PlayerPrefs.SetInt("money", money);
        UptadeMoneyTexts();
    }
    //Oyuncuya para vermeyi yarayan GiveToMoney fonksiyonu oluþtur.Parametre int incretment koyuyuoruz ve mathf.max fonksiyonu ile sýfýrdan büyük olan deðere eþitliyruz..artýþý moneye eþitliyoruz. Fonksiyona uptademoneytexti ekle ve finish game fonksiyonuna þu anki scoreyi yaz. 
}
