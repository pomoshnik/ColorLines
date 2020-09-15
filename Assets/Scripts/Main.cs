using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Main : MonoBehaviour
{
    public GameObject[] ball = new GameObject[3];
    private GameObject[,] pole=new GameObject[9, 9];
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
        RaycastHit hit=new RaycastHit();

#if UNITY_STANDALONE || UNITY_WEBPLAYER

        if (Mouse.current.leftButton.ReadValue()!=0)
        {
            var mousePosition = Mouse.current.position.ReadValue();
            MyRay = Camera.main.ScreenPointToRay(mousePosition);
            Debug.DrawRay(MyRay.origin, MyRay.direction * 21, Color.yellow);
            if (Physics.Raycast(MyRay.origin, MyRay.direction, out hit, 21))
            {
                if (hit.collider.gameObject != selectBall)
                {
                    Debug.Log(hit.collider.gameObject.name);
                    if (selectBall != null)
                    {
                        var animatorS = selectBall.GetComponent<Animator>();
                        animatorS.SetTrigger("Stop");
                    }

                    selectBall = hit.collider.gameObject;
                    var animator = selectBall.GetComponent<Animator>();
                    animator.SetTrigger("Jump");
                }

            }
        }

#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE || UNITY_EDITOR

        if (Touchscreen.current.IsPressed(0))
	{
            var position = Touchscreen.current.position.ReadValue();
            MyRay = Camera.main.ScreenPointToRay(position);
            Debug.DrawRay(MyRay.origin, MyRay.direction * 21, Color.yellow);
            if (Physics.Raycast(MyRay.origin, MyRay.direction, out hit, 21))
            {
                if (hit.collider.gameObject != selectBall)
                {
                    Debug.Log(hit.collider.gameObject.name);
                    if (selectBall != null)
                    {
                        var animatorS = selectBall.GetComponent<Animator>();
                        animatorS.SetTrigger("Stop");
                    }

                    selectBall = hit.collider.gameObject;
                    var animator = selectBall.GetComponent<Animator>();
                    animator.SetTrigger("Jump");
                }

            }
        }

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
            var b = Instantiate(ball[c], new Vector3(x-4, y-4, 0f), Quaternion.identity);
            pole[x, y] = b;
        }
    }
}
