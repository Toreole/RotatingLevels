using UnityEngine;
using NaughtyAttributes;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    PhysicsController physicsController;
    [SerializeField]
    private Transform levelPivot;

    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private bool startsAwake = true;
    [SerializeField]
    private Color sleepColor, awakeColor;
    [SerializeField]
    private float wakeUpTime = 0.75f;

    [SerializeField]
    private Rigidbody2D body;
    [SerializeField]
    private float moveSpeed = 3;

    [InputAxis, SerializeField]
    private string horizontalInput, jumpInput, rotateInput, pauseInput;
    [SerializeField]
    private float rotationTime = 1.5f;

    private Coroutine crRotate = null;
    private float levelRotation = 0;

    private bool awake = true;

    private void Start()
    {
        awake = startsAwake;
        transform.SetParent(levelPivot);
        physicsController.Simulate = true;
        spriteRenderer.color = awake ? awakeColor : sleepColor;
    }

    void Update()
    {
        if(!awake)
        {
            WaitToWakeUp();
            return;
        }
        if(crRotate == null)
        {
            //quick and dirty ground check.
            if(IsOnGround())
            {
                if (Input.GetButton(rotateInput))
                {
                    crRotate = StartCoroutine(CoRotateLevel());
                    return;
                }

                Vector2 v = new Vector2(Input.GetAxisRaw(horizontalInput) * moveSpeed, 0);
               
                body.velocity = v;
            }
            else
            {
                Vector2 v = body.velocity;
                v.x = 0;
                v.y = Mathf.Min(-0.5f, v.y);
                body.velocity = v;
            }
        }
    }

    private bool IsOnGround()
    {
        Vector2 pos = transform.position;
        Vector2 widthOffset = new Vector2(0.5f, 0);
        RaycastHit2D hit = Physics2D.CircleCast(pos + widthOffset, 0.05f, Vector2.down, 0.54f);
        RaycastHit2D hit2 = Physics2D.CircleCast(pos - widthOffset, 0.05f, Vector2.down, 0.54f);

        bool ground = hit && hit.normal.y > 0.75f;
        ground |= hit2 && hit2.normal.y > 0.75f;

        return ground;
    }

    private void WaitToWakeUp()
    {
        //any key that isnt pause
        if(Input.anyKeyDown && !Input.GetButtonDown(pauseInput))
        {
            if(crRotate == null)
            {
                crRotate = StartCoroutine(CoWakeUp());
            }
        }
    }

    private IEnumerator CoWakeUp()
    {
        for(float t = 0; t < wakeUpTime; t += Time.deltaTime)
        {
            Color col = Color.Lerp(sleepColor, awakeColor, t / wakeUpTime);
            spriteRenderer.color = col;
            yield return null;
        }
        spriteRenderer.color = awakeColor;
        awake = true;
        crRotate = null;
    }

    private IEnumerator CoRotateLevel()
    {
        physicsController.Simulate = false;
        body.velocity = Vector2.zero;

        float startRotation = levelRotation;
        float goalRotation = levelRotation + 90 * Input.GetAxisRaw(rotateInput);

        for(float t = 0; t < rotationTime; t += Time.deltaTime)
        {
            ApplyLevelRotation(Mathf.Lerp(startRotation, goalRotation, t / rotationTime));
            yield return null;
        }
        ApplyLevelRotation(goalRotation);
        levelRotation = goalRotation;
        crRotate = null;
        physicsController.Simulate = true;
    }

    private void ApplyLevelRotation(float rotation)
    {
        levelPivot.rotation = Quaternion.Euler(0, 0, rotation);
    }
}