using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Main : MonoBehaviour
{
    public GameObject[] ball = new GameObject[6];
    private readonly int[,] pole = new int[9, 9];
    private readonly Vector2[] path = new Vector2[50];
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
                    ClearPole99();
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
                if (pole[x, y] < 0)
                {
                    pole[x, y] = 0;
                }
            }
        }
    }

    bool SearchPathStart(Vector2 start, Vector2 finish)
    {
        numPath = 0;
        path[numPath] = start;
        
        if (SearchPath(new Vector2(start.x + 1, start.y), finish)) return true;
        if (SearchPath(new Vector2(start.x - 1, start.y), finish)) return true;
        if (SearchPath(new Vector2(start.x, start.y + 1), finish)) return true;
        if (SearchPath(new Vector2(start.x, start.y - 1), finish)) return true;

        return false;
    }
    
    bool SearchPath(Vector2 start, Vector2 finish)
    {
        if (start.x < 0 || start.x > 8 || start.y < 0 || start.y > 8 || pole[(int)start.x, (int)start.y] > 0)
        {
            return false;
        }

        numPath += 1;
        if ((pole[(int)start.x, (int)start.y]) >= -numPath && (pole[(int)start.x, (int)start.y] != 0))
        {
            return false;
        }
        path[numPath] = start;
        pole[(int)start.x, (int)start.y] = -numPath;
        if (start == finish)
        {
            return true;
        }
        else
        {
            if (SearchPath(new Vector2(start.x + 1, start.y), finish)) return true;
            if (SearchPath(new Vector2(start.x - 1, start.y), finish)) return true;
            if (SearchPath(new Vector2(start.x, start.y + 1), finish)) return true;
            if (SearchPath(new Vector2(start.x, start.y - 1), finish)) return true;
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
