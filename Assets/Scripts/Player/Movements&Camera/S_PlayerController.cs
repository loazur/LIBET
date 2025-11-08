using UnityEngine;

public class S_PlayerController : MonoBehaviour
{
    //~ Références
    [Header("References")]
    [HideInInspector] public Rigidbody playerRigidbody;
    [HideInInspector] public MeshRenderer meshRenderer;
    public CapsuleCollider capsuleCollider;
    public Material baseMat;
    public Material sprintMat;

    //~ Scripts de mouvements
    private S_PlayerSprint playerSprint;
    private S_PlayerCrouch playerCrouch;
    private S_PlayerNoClip playerNoClip;

    //~ Variables de mouvements
    public float movementSpeed = 3.5f; 
    private float gravity = 10f;

    private bool isMovingEnabled = true;


    //~ Gestion Slopes
    [Header("Gestion Slopes")]
    private RaycastHit slopeHit;
    [SerializeField] private float maxSlopeAngle; // 60f

    //~ Gestion Stairs
    [Header("Gestion Stairs")]
    public GameObject stepRayUpper;
    public GameObject stepRayLower;
    [SerializeField] private float stepHeight = 0.6f; 
    [SerializeField] private float stepSmooth = 0.1f;


    //~ Booleans (Au dessus tête et au sol)
    [Header("Colliders")]
    public GameObject overheadCheck;
    [HideInInspector] public SphereCollider colliderOverhead;

    public GameObject groundCheck;
    [HideInInspector] public BoxCollider colliderGround;

    void Start() //& INITIALISATION VARIABLES
    {
        meshRenderer = GetComponent<MeshRenderer>();
        playerRigidbody = GetComponent<Rigidbody>();

        colliderOverhead = overheadCheck.GetComponent<SphereCollider>();
        colliderGround = groundCheck.GetComponent<BoxCollider>();

        playerSprint = GetComponent<S_PlayerSprint>();
        playerCrouch = GetComponent<S_PlayerCrouch>();
        playerNoClip = GetComponent<S_PlayerNoClip>();

        stepRayUpper.transform.localPosition = new Vector3(stepRayUpper.transform.localPosition.x, -stepHeight, stepRayUpper.transform.localPosition.z); // Hauteur = hauteur steps

        overheadCheck.SetActive(false); // On désactive overheadCheck dès qu'on spawn
    }

    void FixedUpdate() //& PHYSICS 
    {
        if (playerNoClip.isNoClipping)
        {
            return;
        }

        //! Tout ce qui en dessous ne sera pas actif en Mode NoClip

        
        Move(S_UserInput.instance.MoveInput); // Gestion Mouvements
        HandleGravity(); // Gestion de la gravité
        StepClimb(); // Gestion Stairs
    }

    //! --------------- Fonctions privés ---------------

    private void Move(Vector2 movementVector) //& Gére les mouvements du joueur et la gravité
    {
        if (!canMove()) // Si désactivé
        {
            return;
        }

        // Variables pour les mouvements horizontaux
        Vector3 move = transform.forward * movementVector.y + transform.right * movementVector.x;
        move.Normalize();

        // Réduction de vitesse quand accroupi
        if (playerCrouch.isCrouching)
        {
            move /= playerCrouch.speedDecreaser;
        }

        //* Gestion du sprint
        if (S_UserInput.instance.SprintInput && !playerCrouch.isCrouching)
        {
            playerSprint.Sprint(ref move); // Modifie la variable initial
        }
        else
        {
            meshRenderer.material = baseMat; // Remet le material de base
        }

        //* Gestions des slopes
        if (OnSlope(out float slopeAngle)) // Si on est sur une slope en dessous du maxAngle
        {
            Vector3 slopesMove = GetSlopeMoveDirection(move) * movementSpeed;

            playerRigidbody.linearVelocity = slopesMove;
            return;
        }

        // Applique le mouvement
        playerRigidbody.linearVelocity = new Vector3(move.x * movementSpeed, playerRigidbody.linearVelocity.y, move.z * movementSpeed);
    }

    public bool isGrounded() //& Vérifie si le joueur est au sol
    {
        // Position du centre du BoxCollider
        Vector3 checkPos = groundCheck.transform.position;

        // La moitié des dimensions du BoxCollider (en tenant compte de l'échelle globale)
        Vector3 halfExtents = Vector3.Scale(colliderGround.size * 0.5f, groundCheck.transform.lossyScale);

        // Vérifie si la box touche un layer "Default"
        return Physics.CheckBox(checkPos, halfExtents, groundCheck.transform.rotation, LayerMask.GetMask("Default"));
    }

    private void HandleGravity() //& Application de la gravité
    {
        //! La gravité est globalement gérée par le RigidBody de base, ApplyGravity sert juste à faire certaines corrections

        // Pour eviter les rebonds après avoir atterit sur le sol 
        if (isGrounded())
        {
            if (OnSlope(out float slopeAngle)) // Sur une slope et essaye pas de sauter
            {
                Vector3 slopeParallel = Vector3.ProjectOnPlane(playerRigidbody.linearVelocity, slopeHit.normal);

                playerRigidbody.linearVelocity = slopeParallel; // Annule la vitesse perpendiculaire a la slope
                playerRigidbody.AddForce(Vector3.down * gravity, ForceMode.Acceleration); // Colle a la slope

                float slopeGravityMultiplier = 7.5f;

                if (S_UserInput.instance.MoveInput != Vector2.zero) // Lors d'un mouvement sur slope
                {
                    playerRigidbody.AddForce(Vector3.down * gravity * slopeGravityMultiplier, ForceMode.Acceleration); // Permet de coller de manière plus efficace
                }
            }
            else
            {
                Vector3 velocity = playerRigidbody.linearVelocity;

                if (velocity.y < 0f) // Si encore velocity vers le bas
                {
                    velocity.y = 0f; // colle au sol
                    playerRigidbody.linearVelocity = velocity;
                }
            }

        }

        playerRigidbody.useGravity = !OnSlope(out _); // On désactive la gravité géré par RigidBody sur les slopes
        playerRigidbody.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }

    public bool OnSlope(out float slopeAngle) //& Détection si slope en fonction de maxSlopeAngle
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, capsuleCollider.height * 0.5f + 0.3f))
        {
            slopeAngle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return slopeAngle != 0;
        }

        slopeAngle = 0; // Pas d'angle
        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 movementVector3) //& Calcul un nouveau vector quand le joueur est un sur un slope
    {
        return Vector3.ProjectOnPlane(movementVector3, slopeHit.normal).normalized;
    }

    private void StepClimb() //& Gestion Montée Escaliers
    {
        if (OnSlope(out _) || !isGrounded()) // Réglages de bugs : Montée escalier sur slope et walljump, Montée d'escalier dans les airs
        {
            return;
        }

        // En face
        RaycastHit hitLower;

        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f))
        {
            RaycastHit hitUpper;

            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f))
            {
                playerRigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }

        // 45 Degree
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitLower, 0.1f))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitUpper, 0.2f))
            {
                playerRigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }

        // -45 Degree
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitLower, 0.1f))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitUpper, 0.2f))
            {
                playerRigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
    }

    //? ------------------------------------------------    

    public bool canMove() //& Retourne si le joueur peut se déplacer
    {
        return isMovingEnabled;
    }

    public void setMovementsEnabled(bool isEnabled) //& Active/Désactive les mouvements
    {
        if (isEnabled)
        {
            isMovingEnabled = true;
        }
        else
        {
            isMovingEnabled = false;
            playerRigidbody.linearVelocity = Vector3.zero; // Désactive la vélocity actuel
        }
        
    }
}
