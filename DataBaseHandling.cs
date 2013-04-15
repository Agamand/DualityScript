using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System;

public class DataBaseHandling : MonoBehaviour{


    private String secretKey = "dke4AR1zE.47_en"; // Edit this value and make sure it's the same as the one stored on the server
    public String addScoreURL = "http://www.flyingminutegames.com/wp-includes/add_high_score.php?";
    public String highscoreURL = "http://www.flyingminutegames.com/wp-includes/display_high_score.php";
    public String highscoreString;

    IEnumerator PostScores(String username, float elapsedtime, int deathcount, int score)
    {
        //This connects to a server side php script that will add the name and score to a MySQL DB.
        // Supply it with a string representing the players name and the players score.
        String hash = MD5.Create(username + score + secretKey).ToString();

        String post_url = addScoreURL 
		+ "username=" + WWW.EscapeURL(username) 
		+ "&score=" + score 
		+ "&playtime=" + score 
		+ "&deathcount=" + score 
		+ "&hash=" + hash;
 
        WWW hs_post = new WWW(post_url);
        yield return hs_post;

        if (hs_post.error != null)
        {
            Debug.Log("The high score couldn't have been uploaded :" + hs_post.error);
        }
    }
 
    IEnumerator GetScores()
    {
        highscoreString = "Loading Scores";
        WWW hs_get = new WWW(highscoreURL);
        yield return hs_get;
 
        if (hs_get.error != null)
        {
            Debug.Log("An error occured, while retrieving the High Scores " + hs_get.error);
        }
        else
        {
           highscoreString = hs_get.text;
           print("success " + hs_get.text);
        }
    }

    public void FetchScores()
    {
        StartCoroutine(GetScores());

    }

    public String[] getScoresTab()
    {
        String[] tab = highscoreString.Split('\\');
        return tab;
    }
}
