using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    public GameObject[] ball = new GameObject[3];
    private GameObject[,] pole=new GameObject[9, 9];
    void Start()
    {
        StartTable();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
            return;
        }
    }

    void StartTable() 
    {
        int x;
        int y;
        int c;
        for (int i=1;i<6;i++)
        {
            x = Random.Range(1, 9);
            y = Random.Range(1, 9);
            c = Random.Range(0, 4);
            if (pole[x, y]!=null)
            {
                i--;
                continue;
            }
            var b = Instantiate(ball[c], new Vector3(x-4, y-4,-40f), Quaternion.identity);
            pole[x, y] = b;
        }
    }
}
