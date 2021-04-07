using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;
using Random = UnityEngine.Random;

public class Main : MonoBehaviour
{
    public GameObject[] ball = new GameObject[6];
    public GameObject metka;
    
    
    private readonly int[,] pole = new int[9, 9];
    private readonly int[,] poleSP = new int[9, 9];
    private readonly Vector2[] path = new Vector2[50];
    private int kolStep=0;
    private int numPath = 0;
    public InputMain input;

    private GameObject selectBall;

    void Awake()
    {
        input = new InputMain();

        input.Player.Exit.performed += _ => Exit();
        StartTable();
    }

    void Start()
    {

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
        RaycastHit hit = new RaycastHit();
        Vector2 position=new Vector2();

#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR

        if (Mouse.current.leftButton.ReadValue() != 0)
        {
            position = Mouse.current.position.ReadValue();

        }

#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE 

        if (Touchscreen.current.IsPressed(0))
	    {
            position = Touchscreen.current.position.ReadValue();
        }

#endif
        if (position != null)
        {
            MyRay = Camera.main.ScreenPointToRay(position);
            Debug.DrawRay(MyRay.origin, MyRay.direction * 21, Color.yellow);
            if (Physics.Raycast(MyRay.origin, MyRay.direction, out hit, 21))
            {
                if (hit.collider.gameObject.CompareTag("Ball") && hit.collider.gameObject != selectBall)
                {
                    Debug.Log("Выбрал шарик "+hit.collider.gameObject.transform.position.x+" "+hit.collider.gameObject.transform.position.y);
                    if (selectBall != null)
                    {
                        var animatorS = selectBall.GetComponent<Animator>();
                        animatorS.SetTrigger("Stop");
                    }

                    selectBall = hit.collider.gameObject;
                    var animator = selectBall.GetComponent<Animator>();
                    animator.SetTrigger("Jump");
                }
                if (hit.collider.gameObject.CompareTag("Cell") && selectBall!=null)
                {
                    Debug.Log("Куда пойдем " + hit.collider.gameObject.transform.position.x + " " + hit.collider.gameObject.transform.position.y);
                    var isHod = SearchPathStart(selectBall.transform.position, hit.collider.gameObject.transform.position);
                    //ClearPole99();
                    Debug.Log(isHod);
                }
            }
        }
    }

    private void ClearPole99()
    {
        for (int x = 0; x < 9; x++)
        {
            for (int y = 0; y < 9; y++)
            {
                if (poleSP[x, y] < 0)
                {
                    pole[x, y] = 0;
                }
            }
        }
    }

    bool SearchPathStart(Vector2 start, Vector2 finish)
    {
        int sx = (int)start.x;
        int sy = (int)start.y;
        int fx = (int)finish.x;
        int fy = (int)finish.y;

        numPath = 0;
        
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (pole[i,j]>0)
                {
                    poleSP[i, j] = -pole[i, j];                    
                }
                else
                {
                    poleSP[i, j] = 90;
                }
                
            }
        }

        SearchPath(sx, sy, fx, fy);

        kolStep = 0;
        
        PathResultat(sx, sy, fx, fy, 100);


        for (int n = 0; n <= kolStep; n++)
        {
            Instantiate(metka, path[n], Quaternion.identity);
            n += 1;
        }

        return true;
    }

    void PrintPole()
    {
        Debug.ClearDeveloperConsole();
        for (int y = 8; y >= 0; y--)
        {
            string s = "";
            for (int x = 0; x <= 8; x++)
            {
                s = s + poleSP[x, y] + " ";
            }
            Debug.Log(s);
        }
    }
    
    private bool PathResultat(int sx, int sy, int fx, int fy, int step)
    {
        int s = step;

        if (poleSP[fx + 1, fy] < s && poleSP[fx + 1, fy] > 0)
        {
            s = poleSP[fx + 1, fy];
            path[s] = new Vector2(fx + 1, fy);
        }

        if (poleSP[fx, fy - 1] < s && poleSP[fx, fy - 1] > 0)
        {
            s = poleSP[fx, fy - 1];
            path[s] = new Vector2(fx, fy - 1);
        }

        if (poleSP[fx - 1, fy] < s && poleSP[fx - 1, fy] > 0)
        {
            s = poleSP[fx - 1, fy];
            path[s] = new Vector2(fx - 1, fy);
        }

        if (poleSP[fx, fy + 1] < s && poleSP[fx, fy + 1] > 0)
        {
            s = poleSP[fx, fy + 1];
            path[s] = new Vector2(fx, fy + 1);
        }

        if (kolStep < s)
        {
            kolStep = s;
        }

        if (s == 1)
        {
            return true;
        }

        return PathResultat(sx, sy, (int) path[s].x, (int) path[s].y, s);

    }

    bool SearchPath(int sx, int sy, int fx, int fy)
    {
        if (sx < 0 || sx > 8 || sy < 0 || sy > 8)
        {
            return false;
        }

        PrintPole();
        
        // if ((sx == fx) && (sy == fy))
        // {
        //     return true;
        // }
        
        numPath += 1;
        // if ((poleSP[sx, sy]) <= numPath && (poleSP[sx, sy] != 0))
        // {
        //     return false;
        // }

        if (sx < 8 && poleSP[sx + 1, sy] > numPath)
        {
            poleSP[sx + 1, sy] = numPath;
            SearchPath(sx + 1, sy, fx, fy);
        }

        if (sy > 0 && poleSP[sx, sy - 1] > numPath)
        {
            poleSP[sx, sy - 1] = numPath;
            SearchPath(sx, sy - 1, fx, fy);
        }

        if (sx > 0 && poleSP[sx - 1, sy] > numPath)
        {
            poleSP[sx - 1, sy] = numPath;
            SearchPath(sx - 1, sy, fx, fy);
        }

        if (sy < 8 && poleSP[sx, sy + 1] > numPath)
        {
            poleSP[sx, sy + 1] = numPath;
            SearchPath(sx, sy + 1, fx, fy);
        }

        numPath -= 1;
        return false;
    }

    void StartTable()
    {
        int x;
        int y;
        int c;
        for (int i = 1; i < 6; i++)
        {
            x = Random.Range(1, 9);
            y = Random.Range(1, 9);
            c = Random.Range(0, 4);
            if (pole[x, y] != 0)
            {
                i--;
                continue;
            }
            var b = Instantiate(ball[c], new Vector3(x , y , 0f), Quaternion.identity);
            pole[x, y] = c;
        }
    }
}
