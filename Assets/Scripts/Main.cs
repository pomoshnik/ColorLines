using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Main : MonoBehaviour
{
    public GameObject[] ball = new GameObject[3];
    private GameObject[,] pole=new GameObject[9, 9];
    public InputMain input;

    void Awake()
    {
        input = new InputMain();
        
        input.Player.Exit.performed += _ => Exit();
        StartTable();
    }
    private void OnEnable()
    {
        input.Player.Enable();
    }
    private void OnDesable()
    {
        input.Player.Disable();
    }

    private void Exit()
    {
        Application.Quit();
        return;
    }

    // Update is called once per frame
    void Update()
    {
        Ray MyRay;
        RaycastHit hit=new RaycastHit();

#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR

        if (Mouse.current.leftButton.ReadValue()!=0)
        {
            var mousePosition = Mouse.current.position.ReadValue();
            MyRay = Camera.main.ScreenPointToRay(mousePosition);
            Debug.DrawRay(MyRay.origin, MyRay.direction * 40, Color.yellow);
            if (Physics.Raycast(MyRay.origin, MyRay.direction, out hit, 100))
            {
                Debug.Log("Ball");
            }
        }

#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE
#endif
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
            var b = Instantiate(ball[c], new Vector3(x-4, y-4, -1f), Quaternion.identity);
            pole[x, y] = b;
        }
    }
}
