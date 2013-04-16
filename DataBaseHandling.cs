using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System;
using System.Text;

public class DataBaseHandling : MonoBehaviour{


    private String secretKey = "dke4AR1zE.47_en"; // Edit this value and make sure it's the same as the one stored on the server
    public String addScoreURL = "http://www.flyingminutegames.com/wp-includes/add_high_score.php?";
    public String highscoreURL = "http://www.flyingminutegames.com/wp-includes/display_high_score.php";
    public String connectURL = "http://www.flyingminutegames.com/wp-includes/connect_user.php?";

    private String highscoreString;
    private String connectionMessage;
    private bool connectionSuccessFull = false;
    private String validUsername = null;
    private String validPassword = null;

    IEnumerator PostScores(String username, String elapsedtime, String deathcount, String score)
    {
        String hash = CalculateMD5Hash(username + score + secretKey);

        print(" Hash C# " + hash);

        String post_url = addScoreURL 
		+ "username=" + WWW.EscapeURL(username)
        + "&score=" + WWW.EscapeURL(score)
        + "&playtime=" + WWW.EscapeURL(elapsedtime)
        + "&deathcount=" + WWW.EscapeURL(deathcount)  
		+ "&hash=" + hash;
 
        WWW hs_post = new WWW(post_url);
        yield return hs_post;

        if (hs_post.error != null)
        {
            Debug.Log("The high score couldn't have been uploaded :" + hs_post.error);
        }
        else
            Debug.Log("php hash : " + hs_post.text);
    }

    IEnumerator Connect(String username, String password)
    {
        connectionMessage = "Connecting...";
        connectionSuccessFull = false;

        String connect_url = connectURL
        + "username=" + WWW.EscapeURL(username)
        + "&password=" + WWW.EscapeURL(password);

        WWW co_post = new WWW(connect_url);
        yield return co_post;

        if (co_post.error != null)
        {
            Debug.Log("Could not connect user :" + co_post.error);
        }
        else
        {
            connectionMessage = co_post.text;
            if (connectionMessage.Equals("Connection successfull"))
            {
                connectionSuccessFull = true;
                validUsername = username;
                validPassword = password;
            }
        }

    }

    public void UploadScore(String username, String elapsedtime, String deathcount, String score)
    {
        StartCoroutine(PostScores(username, elapsedtime, deathcount, score));
    }

    public void TryConnection(String username, String password)
    {
        StartCoroutine(Connect(username, password));
    }

    public String GetConnectionMessage()
    {
        return connectionMessage;
    }

    public bool HasSuccessfullyConnect()
    {
        return connectionSuccessFull;
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

    public String getValidUsername()
    {
        return validUsername;
    }

    public String getValidPassword()
    {
        return validPassword;
    }

    public string CalculateMD5Hash(string inputSt)
    {
        byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes(inputSt);
        byte[] hashedBytes = MD5CryptoServiceProvider.Create().ComputeHash(asciiBytes);
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();     
    }

}
