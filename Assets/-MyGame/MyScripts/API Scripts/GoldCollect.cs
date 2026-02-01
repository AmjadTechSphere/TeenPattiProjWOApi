using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GoldWinLoose;

public class GoldCollect : MonoBehaviour
{
    public int counter;
    int target;
    float deltaTimer;
    bool isPressed;
    // Start is called before the first frame update
    void Start()
    {
        target = 60;
    }

    // Update is called once per frame
    void Update()
    {
        if (isPressed)
        {
            if (deltaTimer > 0)
            {
                counter++;
                if (counter >= target)
                {
                    // gold add
                    string randomString = GenerateRandomAlphaNumericString();
                    GoldWinLoose.Instance.SendGold(randomString, "POKER", "POKER10000000", Trans.win, "90000000000");
                    counter = 0;
                }
            }
            deltaTimer = 2;
            isPressed = false;
        }
        else
        {
            if (deltaTimer > 0)
            {
                deltaTimer -= Time.deltaTime;
            }
            else if (deltaTimer < 0)
            {
                deltaTimer = 0;
                counter = 0;
            }
        }
    }
    private void OnEnable()
    {
        deltaTimer = 0;
        counter = 0;
    }
    public void countClick()
    {
        isPressed = true;
    }

    void getAlphaNumericString()
    {
        string randomString = GenerateRandomAlphaNumericString();
        Debug.Log(randomString);
    }

    private char RandomAlphanumericCharacter()
    {
        string characters = "abcdefghijklmnopqrstuvwxyz0123456789";
        int randomIndex = Random.Range(0, characters.Length);
        return characters[randomIndex];
    }
    private string GenerateRandomAlphaNumericString()
    {
        // Define the format
        string format = "ab8cd632-14af-4fba-a2d8-1e02e4e54c78";

        // Use a StringBuilder to build the result string
        System.Text.StringBuilder result = new System.Text.StringBuilder();

        // Loop through each character in the format
        foreach (char c in format)
        {
            if (c == 'a' || c == 'b' || c == 'c' || c == 'd' || c == 'f' || c == '2' || c == '4' || c == '8')
            {
                // If the character is a placeholder for a random alphanumeric character, add a random character
                result.Append(RandomAlphanumericCharacter());
            }
            else
            {
                // If the character is a fixed character, add it as is
                result.Append(c);
            }
        }

        return result.ToString();
    }
}
