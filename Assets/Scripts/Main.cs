using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Main : MonoBehaviour
{
    public GameObject[] ball = new GameObject[3];
    private GameObject[,] pole = new GameObject[9, 9];
    private Vector2[] path = new Vector2[50];
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
                if (hit.collider.gameObject.tag == "Ball" && hit.collider.gameObject != selectBall)
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
                if (hit.collider.gameObject.tag == "Cell" && selectBall!=null)
                {
                    Debug.Log("Куда пойдем " + hit.collider.gameObject.transform.position.x + " " + hit.collider.gameObject.transform.position.y);
                }
            }
        }
    }

    bool SearchPath(Vector2 start, Vector2 finish)
    {
        return true;
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
            if (pole[x, y] != null)
            {
                i--;
                continue;
            }
            var b = Instantiate(ball[c], new Vector3(x - 4, y - 4, 0f), Quaternion.identity);
            pole[x, y] = b;
        }
    }
}
