using UnityEngine;
using FMODUnity;
using FMOD.Studio;


public class S_PlayerFootsteps : MonoBehaviour
{
    //! S_PlayerFootsteps permet de jouer des bruit de pas périodement en fonction du type de sol

    private int MaterialValue;
    private RaycastHit rh;
    private float distance = 1.5f;
    private LayerMask lm;

    [Header("Footstep Settings")]
    [SerializeField] private float walkStepInterval = 0.7f; // Intervalle entre les pas en marche (en secondes)
    [SerializeField] private float runStepInterval = 0.5f; // Intervalle entre les pas en course (en secondes)
    
    private float stepTimer = 0f;
    private bool isMoving = false;

    private bool soundsEnabled = true;

    void Start()
    {
        lm = LayerMask.GetMask("Ground");        
    }

    
    void Update()
    {
        // Shows drawn raycast for debugging
        //Debug.DrawRay(transform.position, Vector3.down * distance, Color.blue);

        // Vérifier si le joueur se déplace
        isMoving = S_UserInput.instance.MoveInput != Vector2.zero;

        if (isMoving && soundsEnabled)
        {
            // Décrémenter le timer
            stepTimer -= Time.deltaTime;

            // Si le timer est écoulé, jouer un pas
            if (stepTimer <= 0f)
            {
                PlayWalkEvent();

                // Réinitialiser le timer (différent selon si marche ou course)
                bool isRunning = CheckIfPlayerIsRunning(); // Vous pouvez implémenter cette fonction
                stepTimer = isRunning ? runStepInterval : walkStepInterval;
            }
        }
        else
        {
            // Reset le timer quand le joueur s'arrête
            stepTimer = 0f;
        }
    }

    //!----------------------------------------------------------------------------
    
    private void PlayWalkEvent() //& Joue un son
    {
        if (S_UserInput.instance.MoveInput == Vector2.zero) return; // Se déplace pas

        // Start with material check then instantiate sound
        MaterialCheck();
        EventInstance Walk = RuntimeManager.CreateInstance(S_FMODEvents.instance.footsteps);
        RuntimeManager.AttachInstanceToGameObject(Walk, gameObject, GetComponent<Rigidbody>());

        // Can be used as alternative to IDs
        Walk.setParameterByName("Terrain", MaterialValue);

        Walk.start();
        Walk.release();
    }


    private void MaterialCheck() //& Choisi le paramètre en fonction du resultat du raycast
    {
        if (Physics.Raycast(transform.position, Vector3.down, out rh, distance, lm))
        {
            //Debug.LogWarning(rh.collider.tag + " " + MaterialValue);
            switch (rh.collider.tag)
            {
                case "Parquet":
                    MaterialValue = 0; // Labeled parameters in FMOD
                    break;
                case "Tile":
                    MaterialValue = 1;
                    break;
                case "Concrete":
                    MaterialValue = 2;
                    break;
                default:
                    MaterialValue = 0;
                    break;

            }
        }
    }

    private bool CheckIfPlayerIsRunning() //& Vérifie si le joueur court
    {
        return S_UserInput.instance.SprintInput; // Par défaut, considère qu'il marche
    }

    //?---------------------------

    public bool CanMakeSound() => soundsEnabled;
    
    public void SetSoundsEnabled(bool enabled)
    {
        soundsEnabled = enabled;
    }
}
