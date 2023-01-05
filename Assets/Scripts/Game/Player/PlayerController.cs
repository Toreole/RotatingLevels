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

    [InputAxis, SerializeField]
    private string horizontalInput, jumpInput, rotateInput, pauseInput;
    [SerializeField]
    private float rotationTime = 1.5f;

    private Coroutine crRotate = null;
    private float levelRotation = 0;

    private void Start()
    {
        transform.SetParent(levelPivot);
        physicsController.Simulate = true;
    }

    void Update()
    {
        if(crRotate == null)
        {
            if (Input.GetButton(rotateInput))
            {
                crRotate = StartCoroutine(CoRotateLevel());
            }

            //quick and dirty ground check.
            if(Physics2D.Raycast(transform.position, Vector2.down, 0.54f))
            {
                Vector2 v = new Vector2(Input.GetAxis(horizontalInput), 0);
                v *= 3f; //replace with speed var
                body.velocity = v;
            }
        }
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