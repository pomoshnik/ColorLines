using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
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
    private readonly List<GameObject> array=new List<GameObject>(); 
    private int kolStep=0;
    private int numPath = 0;
    public InputMain input;

    private GameObject selectBall;
    private Vector3 selectHod;

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

        if (Mouse.current.leftButton.wasReleasedThisFrame)
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
            //Debug.DrawRay(MyRay.origin, MyRay.direction * 21, Color.yellow);
            if (Physics.Raycast(MyRay.origin, MyRay.direction, out hit, 23))
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
                }else if ((selectHod.x == hit.collider.gameObject.transform.position.x) &&
                          (selectHod.y == hit.collider.gameObject.transform.position.y))
                {
                    // Ход
                    print("Go");
                }
                else
                
                if (hit.collider.gameObject.CompareTag("Cell") && selectBall!=null)
                {
                    // Выбор хода
                    Debug.Log("Куда пойдем " + hit.collider.gameObject.transform.position.x + " " + hit.collider.gameObject.transform.position.y);
                    var isHod = SearchPathStart(selectBall.transform.position, hit.collider.gameObject.transform.position);
                    if (isHod)
                    {
                        selectHod = hit.collider.gameObject.transform.position;
                    }
                    else
                    {
                        selectHod = Vector3.zero;
                    }
                    var animator = selectBall.GetComponent<Animator>();
                    animator.SetTrigger("JumpingMove");
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

        poleSP[(int)start.x, (int)start.y] = 1;
        poleSP[(int)finish.x, (int)finish.y] = 1000;     

        SearchPath(1);

        kolStep = 0;

        foreach (var o in array)
        {
            Destroy(o);
        }
        array.Clear();
        
        PathResultat((int)finish.x, (int)finish.y, 90);
        
        path[kolStep+1]=new Vector2((int)finish.x, (int)finish.y);
        for (int n = 2; n <= kolStep+1; n++)
        {
            array.Add(Instantiate(metka, path[n], Quaternion.identity));
        }

        return true;
    }

    void PrintPole()
    {
        // Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
        // Type type = assembly.GetType("UnityEditor.LogEntries");
        // MethodInfo method = type.GetMethod("Clear");
        // method.Invoke(new object(), null);
        
        for (int y = 8; y >= 0; y--)
        {
            string s = "";
            for (int x = 0; x <= 8; x++)
            {
                s = s + poleSP[x, y] + " ";
            }
            //Debug.Log(s);
        }
    }

    private bool PathResultat(int fx, int fy, int step)
    {
        int s = step;

        if (fx < 8 && poleSP[fx + 1, fy] < s && poleSP[fx + 1, fy] > 0)
        {
            s = poleSP[fx + 1, fy];
            path[s] = new Vector2(fx + 1, fy);
        }

        if (fy > 0 && poleSP[fx, fy - 1] < s && poleSP[fx, fy - 1] > 0)
        {
            s = poleSP[fx, fy - 1];
            path[s] = new Vector2(fx, fy - 1);
        }

        if (fx > 0 && poleSP[fx - 1, fy] < s && poleSP[fx - 1, fy] > 0)
        {
            s = poleSP[fx - 1, fy];
            path[s] = new Vector2(fx - 1, fy);
        }

        if (fy < 8 && poleSP[fx, fy + 1] < s && poleSP[fx, fy + 1] > 0)
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

        return PathResultat((int) path[s].x, (int) path[s].y, s);

    }

    bool SearchPath(int step)
    {
        PrintPole();
        
        
        
        for (int y = 8; y >= 0; y--)
        {
            for (int x = 0; x <= 8; x++)
            {
                if (poleSP[x,y]==step)
                {
                    if (x < 8 && poleSP[x + 1, y] == 1000)
                    {
                        return true;
                    }
                    
                    if (x < 8 && poleSP[x + 1, y] == 90)
                    {
                        poleSP[x + 1, y] = step + 1;
                    }
                    //////////////////////
                    if (y > 0 && poleSP[x, y - 1] == 1000)
                    {
                        return true;
                    }
                    
                    if (y > 0 && poleSP[x, y - 1] == 90)
                    {
                        poleSP[x, y - 1] = step + 1;
                    }
                    /////////////////////
                    if (x > 0 && poleSP[x - 1, y] == 1000)
                    {
                        return true;
                    }
                    
                    if (x > 0 && poleSP[x - 1, y] == 90)
                    {
                        poleSP[x - 1, y] = step + 1;
                    }
                    /////////////////////
                    if (y < 8 && poleSP[x, y + 1] == 1000)
                    {
                        return true;
                    }
                    
                    if (y < 8 && poleSP[x, y + 1] == 90)
                    {
                        poleSP[x, y + 1] = step + 1;
                    }
                }
            }
        }

        return SearchPath(step + 1);
        
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
            c = Random.Range(1, 5);
            if (pole[x, y] != 0)
            {
                i--;
                continue;
            }
            var b = Instantiate(ball[c-1], new Vector3(x , y , 0f), Quaternion.identity);
            pole[x, y] = c;
        }
    }
}
