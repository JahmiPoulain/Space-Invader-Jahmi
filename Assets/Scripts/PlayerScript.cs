using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    [Header("speed")]
    public float speed;
    float currentSpeed = 0f;
    float targetSpeed = 0f;
    public AnimationCurve accelValue; //courbe d'acceleration
    float accelTime = 0f;
    public int accelFrameDuration;
    float decellTime = 0f;
    public int decelFrameDuration;
    public AnimationCurve decelValue; //courbe deceleration
    [Header("limites")]
    public float boundary;
    [Range(0f, 1f)]
    private  float boundaryPercentage = 0.55f;

    Vector2 moveInput;
    //[Header("state machine")]
    private enum PlayerState
    {
        Idle,
        Accelerating,
        Decelerating
    }

    private PlayerState currentState = PlayerState.Idle;    

    Rigidbody2D rb;
    InputSystem_Actions controls;
    #region InitializeRegion
    void Awake()
    {
        Application.targetFrameRate = 60; // definir un framerate
        rb = GetComponent<Rigidbody2D>();
        controls = new InputSystem_Actions(); // initialiser input
        controls.Player.Move.performed += ctx => HandheldMovePressed(ctx);
        controls.Player.Move.canceled += ctx => HandheldMoveRelease(ctx);
        CalculateBoundary();
    }



    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }
    #endregion

    void FixedUpdate()
    {
        if (GameManager.instance.isPaused)
        {
            return;
        }

        if (Mathf.Approximately(targetSpeed - currentSpeed, 0) && currentState != PlayerState.Idle) // si on n'a pas de vitesse
        {
            decellTime = 0f;
            currentState = PlayerState.Idle;
        }
        else if (moveInput.x != 0 && currentState != PlayerState.Accelerating) // le moment ou on appuie
        {
            accelTime = 0f;
            currentState = PlayerState.Accelerating;
        }
        else if (moveInput.x == 0 && !Mathf.Approximately(currentSpeed, 0) && currentState != PlayerState.Decelerating)
        {
            decellTime = 0f;
            currentState = PlayerState.Decelerating;
            //Debug.Log("transition vers decelerating");
        }

        switch (currentState) // appliquer le comportement
        {
            case PlayerState.Accelerating:
                HandleAcceleration();
                break;

            case PlayerState.Decelerating:  
                HandleDeceleration();
                break;
            case PlayerState.Idle:
                break;
        }

        float newXPosition = rb.position.x + (currentSpeed * Time.fixedDeltaTime);

        if (newXPosition <= -boundary || newXPosition >= boundary)
        {
            currentSpeed = 0;
            newXPosition = Mathf.Clamp(newXPosition, -boundary, boundary);
        }

        rb.MovePosition(new Vector2(newXPosition, rb.position.y));

       // Debug.Log($"State : {currentState}, Current speed : {");
    }
    void Update()
    {

    }

    private void HandheldMovePressed(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
        targetSpeed = Mathf.Clamp(moveInput.x * speed, -speed, speed);
        accelTime = 0f;
    }
    
    private void HandheldMoveRelease(InputAction.CallbackContext ctx)
    {
        moveInput = Vector2.zero;
        targetSpeed = 0f;
        decellTime = 0f;
    }

    void HandleAcceleration()
    {
        float curveTimeScale = accelValue.keys[accelValue.length - 1].time;
        accelTime += Time.fixedDeltaTime / ((accelFrameDuration * Time.fixedDeltaTime) / curveTimeScale);

        float curveValue = accelValue.Evaluate(accelTime);

        float minCurveValue = 0.2f;
        curveValue = Mathf.Max(curveValue, minCurveValue); // on choisit le plus grand des 2

        currentSpeed += Mathf.Sign(targetSpeed) * curveValue * speed * Time.fixedDeltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, -speed, speed);
    }

    void HandleDeceleration()
    {
        float curveTimeScale = decelValue.keys[decelValue.length - 1].time;
        decellTime += Time.fixedDeltaTime / ((decelFrameDuration * Time.fixedDeltaTime) / curveTimeScale);

        float curveValue = decelValue.Evaluate(decellTime);

        float minCurveValue = 0.2f;
        curveValue = Mathf.Max(curveValue, minCurveValue); // on choisit le plus grand des 2

        if (currentSpeed > 0)
        {
            currentSpeed = Mathf.Clamp(currentSpeed - curveValue * speed * Time.fixedDeltaTime, 0, speed);
        }
        else if (currentSpeed < 0)
        {
            currentSpeed = Mathf.Clamp(currentSpeed + curveValue * speed * Time.fixedDeltaTime, -speed, 0);
        }

        if (Mathf.Abs(currentSpeed) <= 0.5f && Mathf.Approximately(targetSpeed, 0f))
        {
            currentSpeed = 0f;
            decellTime = 0f;
        }
        /*currentSpeed += Mathf.Sign(targetSpeed) * curveValue * speed * Time.fixedDeltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, -speed, speed);*/
    }

    #region CalculateBoundaryAndGizmos
    void CalculateBoundary()
    {
        Vector3 screenBounds = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)); // la largeur de lecran visible en coordonee world

        boundary = screenBounds.x * boundaryPercentage;

       // Debug.Log($"👣 Boundary calculé : {boundary}"); // $ = il y a une variable dans le {} qui suit
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-boundary, -10, 0), new Vector3(-boundary, 10, 0));
        Gizmos.DrawLine(new Vector3(boundary, -10, 0), new Vector3(boundary, 10, 0));
    } 
    #endregion
}

