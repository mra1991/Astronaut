using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locomotion : MonoBehaviour
{
    //variables for ground
    private bool isGrounded = false;
    [Tooltip("Attach your character's GroundCheck children object here.")]
    [SerializeField] private Transform groundCheck;
    [Tooltip("Include all layers containing objects that the character can jump while standing on them.")]
    [SerializeField] private LayerMask whatIsGround;
    [Tooltip("Include all layers containing objects that the character would die if they land on them")]
    [SerializeField] private LayerMask whatIsDeath;
    const float checkRadius = 0.12f; //radius of the overlap circle

    //variables for jump
    private Rigidbody2D rigid;
    [SerializeField] private float jumpForce = 2f;
    [SerializeField] private float thrustForce = 4f;

    //variables for movement
    private Vector3 initPos; //To save the initial position of the character in the world
    [SerializeField] private float speed = 2f;
    private Vector2 refVelocity = Vector2.zero;
    [Range(0f, 0.3f)][Tooltip("How long it takes to reach full speed.")]
    [SerializeField] private float smoothing = 0.05f;
    private bool faceLeft = false; //to change derection
    private SpriteRenderer sprite;

    public bool IsGrounded { get => isGrounded;}

    public void Move(float move, bool jump, bool thrust)
    {
        Movement(move);
        Jump(jump);
        Thrust(thrust);
        CheckOrientation(move);
    }

    public void CheckOrientation(float move)
    {
        //if input is in one direction and the character is facing the opposite direction
        if ((move > 0 && faceLeft) || (move < 0 && !faceLeft)) 
        {
            faceLeft = !faceLeft;
            sprite.flipX = faceLeft;
        }
    }

    public void CheckGround()
    {
        //checking if the character is on floor
        Collider2D collider = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
        isGrounded = (collider != null) ? true : false;
        //checking if the character is on a spike (or other death-traps!)
        Collider2D collider2 = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsDeath);
        if (collider2 != null)
        {
            GameManager.instance.Death(); //signal death to GameManager
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Gem")
        {
            Destroy(other.gameObject); //destroy the gem
            GameManager.instance.AddPoints(); //signal GameManager to add points
        }
    }

    void Movement(float move)
    {
        Vector2 targetVelocity = new Vector2(move * speed, rigid.velocity.y);
        rigid.velocity = Vector2.SmoothDamp(rigid.velocity, targetVelocity, ref refVelocity, smoothing);
    }

    void Jump(bool jump)
    {
        if (isGrounded && jump)
        {
            rigid.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    // Thrust will add the thrust force tho the ridigbody component
    void Thrust(bool thrust)
    {
        if (!isGrounded && thrust) //if astronaut is under thrust and in the air
        {
            rigid.AddForce(Vector3.up * thrustForce, ForceMode2D.Force);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        initPos = transform.position; //saving initial position
    }

    //initialize the character
    public void Init()
    {
        transform.position = initPos; //restoring initial position
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        CheckGround();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isGrounded ? Color.red : Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }
}
