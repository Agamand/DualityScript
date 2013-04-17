/**
 *  DataBaseHandling
 *      --> This script handles every call made to the server by executing different php scripts server side
 *  
 *  Members: 
 *      private String secretKey: a static key used to check if any call of a script altering the content of the DB has been legitimately made by the game
 *	    public String addScoreURL: the URL of the php script to add an high score
 *      public String highscoreURL: the URL of the php script to display high scores
 *	    public String connectURL: the URL of the php script to test wether or not the user is registered in the DB and has entered the valid password
 *      
 *      private String highscoreString: the String returned by the php script which contains either an error message or the top ten high score
 *      private String connectionMessage: the String returned by the php connection script which states wheter or not the user is registered
 *      private bool connectionSuccessFull: boolean allowing to know if the previous call to the connection script has been successfull
 *      private String validUsername: the String containing the valid username when the user has connect successfully
 *      private String validPassword: the String containing the valid password when the user has connect successfully
 *      
 *  Authors: Jean-Vincent Lamberti
 **/

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

    /**
     * PostScores(String username, String elapsedtime, String deathcount, String score)
     *  --> Coroutine used to upload a high score
     *  
     * Arguments: 
     *  - String username : the username of the player
     *  - String elapsedtime : the time the player has taken to beat the game
     *  - String deathcount : the number of times the player died
     *  - String score : the score of the player
     * */
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
            Debug.Log("Error uploading the high score :" + hs_post.error);
        }
        else
            Debug.Log(hs_post.text);
    }

    /**
     * Connect(String username, String password)
     *  --> Coroutine used to test whether or not the player has entered a valid user name and password
     *  
     * Arguments: 
     *  - String username : the username of the player
     *  - String elapsedtime : the password of the player
     * */
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

    /**
     * Starts the PostScores coroutine, call this to upload a new score
     * */
    public void UploadScore(String username, String elapsedtime, String deathcount, String score)
    {
        StartCoroutine(PostScores(username, elapsedtime, deathcount, score));
    }

    /**
     * Starts the Connect coroutine, call this to test user account info
     * */
    public void TryConnection(String username, String password)
    {
        StartCoroutine(Connect(username, password));
    }

    /**
     * GetConnectionMessage():
     *  --> returns the string returned by the php connection script
     * */
    public String GetConnectionMessage()
    {
        return connectionMessage;
    }

    /**
     * HasSuccessfullyConnect()
     *  --> returns wether or not the player has entered valid account info
     * */
    public bool HasSuccessfullyConnect()
    {
        return connectionSuccessFull;
    }
 
    /**
     * GetScores()
     *  --> coroutine allowing to fetch the top ten high scores
     * 
     * */
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
        }
    }

    /**
     * Calls the GetScores coroutine, call this to get the top ten high scores
     * */
    public void FetchScores()
    {
        StartCoroutine(GetScores());

    }

    /**
     * GetScoresTab()
     *  --> get a String array of the top ten high scores
     * */
    public String[] GetScoresTab()
    {
        String[] tab = highscoreString.Split('\\');
        return tab;
    }

    /**
     * GetValidUsername()
     *  --> returns the username of the player if he has connected successfully
     * 
     * */
    public String GetValidUsername()
    {
        return validUsername;
    }

    /**
     * GetValidPassword()
     *  --> returns the password of the player if he has connected successfully
     * 
     * */
    public String GetValidPassword()
    {
        return validPassword;
    }

    /**
     * CalculateMD5Hash(String inputs)
     *  --> returns a md5 hashed String of the string given in parameters
     * */
    public String CalculateMD5Hash(String inputSt)
    {
        byte[] asciiBytes = ASCIIEncoding.ASCII.GetBytes(inputSt);
        byte[] hashedBytes = MD5CryptoServiceProvider.Create().ComputeHash(asciiBytes);
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();     
    }

}
