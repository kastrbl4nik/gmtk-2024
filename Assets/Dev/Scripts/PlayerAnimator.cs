using UnityEngine;


public class PlayerAnimator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator anim;

    [SerializeField] private SpriteRenderer sprite;

    [Header("Settings")]
    [SerializeField] [Range(1f, 3f)]
    private float maxIdleSpeed = 2;

    [SerializeField] private float maxTilt = 5;
    [SerializeField] private float tiltSpeed = 20;

    [Header("Particles")]
    [SerializeField] private ParticleSystem jumpParticles;
    [SerializeField] private ParticleSystem launchParticles;
    [SerializeField] private ParticleSystem moveParticles;
    [SerializeField] private ParticleSystem landParticles;

    [Header("Audio Clips")] [SerializeField]
    private AudioClip[] footsteps;

    private AudioSource source;
    private IPlayerController player;
    private bool grounded;
    private ParticleSystem.MinMaxGradient currentGradient;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        player = GetComponent<IPlayerController>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        player.Jumped += OnJumped;
        player.GroundedChanged += OnGroundedChanged;

        moveParticles.Play();
    }

    private void OnDisable()
    {
        player.Jumped -= OnJumped;
        player.GroundedChanged -= OnGroundedChanged;

        moveParticles.Stop();
    }

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        DetectGroundColor();

        HandleSpriteFlip();

        HandleIdleSpeed();

        HandleCharacterTilt();
    }

    private void HandleSpriteFlip()
    {
        if (player.FrameInput.x != 0)
        {
            sprite.flipX = player.FrameInput.x > 0;
        }
    }

    private void HandleIdleSpeed()
    {
        var inputStrength = Mathf.Abs(player.FrameInput.x);
        anim.SetFloat(IdleSpeedKey, Mathf.Lerp(1, maxIdleSpeed, inputStrength));
        moveParticles.transform.localScale = Vector3.MoveTowards(moveParticles.transform.localScale,
            Vector3.one * inputStrength, 2 * Time.deltaTime);
    }

    private void HandleCharacterTilt()
    {
        var runningTilt = grounded ? Quaternion.Euler(0, 0, maxTilt * player.FrameInput.x) : Quaternion.identity;
        anim.transform.up =
            Vector3.RotateTowards(anim.transform.up, runningTilt * Vector2.up, tiltSpeed * Time.deltaTime, 0f);
    }

    private void OnJumped()
    {
        anim.SetTrigger(JumpKey);
        anim.ResetTrigger(GroundedKey);


        if (grounded) // Avoid coyote
        {
            SetColor(jumpParticles);
            SetColor(launchParticles);
            jumpParticles.Play();
        }
    }

    private void OnGroundedChanged(bool newGrounded, float impact)
    {
        grounded = newGrounded;

        if (grounded)
        {
            DetectGroundColor();
            SetColor(landParticles);

            anim.SetTrigger(GroundedKey);
            // todo(kilativ-dotcom): uncomment when implemented
            // source.PlayOneShot(footsteps[Random.Range(0, footsteps.Length)]);
            moveParticles.Play();

            landParticles.transform.localScale = Vector3.one * Mathf.InverseLerp(0, 40, impact);
            landParticles.Play();
        }
        else
        {
            moveParticles.Stop();
        }
    }

    private void DetectGroundColor()
    {
        var hit = Physics2D.Raycast(transform.position, Vector3.down, 2);

        if (!hit || hit.collider.isTrigger || !hit.transform.TryGetComponent(out SpriteRenderer r))
        {
            return;
        }

        var color = r.color;
        currentGradient = new ParticleSystem.MinMaxGradient(color * 0.9f, color * 1.2f);
        SetColor(moveParticles);
    }

    private void SetColor(ParticleSystem ps)
    {
        var main = ps.main;
        main.startColor = currentGradient;
    }

    private static readonly int GroundedKey = Animator.StringToHash("Grounded");
    private static readonly int IdleSpeedKey = Animator.StringToHash("IdleSpeed");
    private static readonly int JumpKey = Animator.StringToHash("Jump");
}
