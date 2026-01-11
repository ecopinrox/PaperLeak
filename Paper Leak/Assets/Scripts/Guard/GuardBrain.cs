using System.Collections.Generic;
using UnityEngine;
using Behaviour;

public class GuardBrain : MonoBehaviour
{
    //this is the brain of the guard object
    //this script does nothing except call functions from other scripts at the correct times
    //this script has no functionality of its own
    //no other script should take any action

    GuardMovement guardMovement;
    GuardDistractionSensor guardDistractionSensor;
    GuardSpriteManager guardVisualManager;

    GridManager gridMovementMonitor;
    Pathfinder pathfinder;

    BController behaviourController;

    [SerializeField] Transform patrolPathObject;
    List<Waypoint> patrolPath;

    [SerializeField] DifficultySwitch difficultySettings;

    float investigationRoutineDelay = 1f;
    float examineDelay = 6f;

    bool canCrouch = false;

    [Header("Layer masks")]
    [SerializeField] LayerMask crawlableLayerMask;

    //state variables
    IEnumerator<Waypoint> patrolIterator;
    Vector2Int interruptLocation;
    bool isCrouched;
    Distraction currentDistraction = null;
    Vector2Int lastSeenDistractionPosition;

    //transition states
    BTask patrolToAlertTask;
    BTask alertOrInvestigatingToAlertTask;
    BTask investigatingToInvestigatingTask;

    private void Awake()
    {
        guardMovement = GetComponent<GuardMovement>();
        guardDistractionSensor = GetComponent<GuardDistractionSensor>();
        guardVisualManager = GetComponent<GuardSpriteManager>();

        gridMovementMonitor = FindAnyObjectByType<GridManager>();
        pathfinder = FindAnyObjectByType<Pathfinder>();

        patrolPath = new(patrolPathObject.GetComponentsInChildren<Waypoint>(true));
        if(patrolPath.Count == 0)
        {
            Debug.LogError($"Patrol path of guard {gameObject.name} is empty.");
        }
        patrolIterator = patrolPath.GetEnumerator();
    }

    private void OnEnable()
    {
        DifficultySwitch.loadDifficultySettings += LoadDifficulty;
    }

    private void OnDisable()
    {
        DifficultySwitch.loadDifficultySettings -= LoadDifficulty;
    }

    private void Start()
    {
        //patrol states
        BAction getNextWaypoint = new(
            BState.Patrolling,
            () =>
            {
                if (!patrolIterator.MoveNext())
                {
                    patrolIterator.Reset();
                    patrolIterator.MoveNext();
                }
            });

        BWait goToWaypoint = new(
            BState.Patrolling,
            () => 
            { 
                guardMovement.SetDestination(patrolIterator.Current.Position); 
            },
            () => 
            { 
                return guardMovement.PathComplete; 
            });

        BTimer waitAtWaypoint = new(
            BState.Patrolling,
            () => 
            {
                transform.rotation = patrolIterator.Current.Rotation; 
            }, 
            () =>
            {
                return patrolIterator.Current.WaitTime;
            });

        BWait returnToInterruptLocation = new(
            BState.Patrolling,
            () =>
            {
                guardMovement.SetDestination(interruptLocation);
            },
            () => 
            { 
                return guardMovement.PathComplete; 
            });

        //alert states
        BAction saveInterruptLocation = new(
            BState.Alert,
            () =>
            {
                interruptLocation = guardMovement.CurrentLocation;
            });

        BAction enterAlertState = new(
            BState.Alert,
            () =>
            {
                isCrouched = false;
                guardMovement.StopMoving();
                guardVisualManager.ChangeToCautionColor();
            });

        BTimer waitOnAlert = new(
            BState.Alert,
            () => { },
            () => investigationRoutineDelay
            );

        BWait goToDistraction = new(
            BState.Investigating,
            () =>
            {
                isCrouched = false;
                guardVisualManager.ChangeToCautionColor();
                guardMovement.SetCardinalDestination(lastSeenDistractionPosition);
            },
            () =>
            {
                return guardMovement.PathComplete;
            });

        BTimer waitAtDistraction = new(
            BState.Investigating,
            () =>
            {
                guardMovement.LookAt(lastSeenDistractionPosition);

                if (canCrouch && gridMovementMonitor.IsLocationInMask(lastSeenDistractionPosition, crawlableLayerMask))
                {
                    isCrouched = true;
                    guardVisualManager.ChangeToCrouchColor();
                }
            },
            () =>
            {
                return examineDelay;
            });

        BAction exitInvestigatingState = new(
            BState.Investigating,
            () =>
            {
                isCrouched = false;
                guardDistractionSensor.RegisterDistraction(currentDistraction);
                currentDistraction = null;
                guardVisualManager.ChangeToIdleColor();
            });

        //Patrolling states
        getNextWaypoint.SetNext(goToWaypoint);
        goToWaypoint                .SetNext(waitAtWaypoint);
        waitAtWaypoint              .SetNext(getNextWaypoint);
        returnToInterruptLocation   .SetNext(goToWaypoint);

        //Alert states
        saveInterruptLocation       .SetNext(enterAlertState);
        enterAlertState             .SetNext(waitOnAlert);
        //waitOnAlert                 .SetNext(isDistractionUnderTable);
        waitOnAlert                 .SetNext(goToDistraction);

        //Investigating states
        goToDistraction             .SetNext(waitAtDistraction);
        waitAtDistraction           .SetNext(exitInvestigatingState);
        exitInvestigatingState      .SetNext(returnToInterruptLocation);

        //Set transitions
        patrolToAlertTask = saveInterruptLocation;
        alertOrInvestigatingToAlertTask = enterAlertState;
        investigatingToInvestigatingTask = goToDistraction;

        behaviourController = new(getNextWaypoint);
    }

    private void FixedUpdate()
    {
        GetOverridingDistraction();
        behaviourController.Tick();
    }

    void GetOverridingDistraction()
    {
        Distraction newDistraction = guardDistractionSensor.GetDistraction(isCrouched);
        if (newDistraction == null) return;

        if (currentDistraction != null && currentDistraction.Priority > newDistraction.Priority) return;

        //transitions can be applied here
        BTask nextTask = null;
        switch(behaviourController.CurrentState)
        {
            case BState.Patrolling:
                nextTask = patrolToAlertTask;
                break;

            case BState.Alert:
                if (currentDistraction == newDistraction) 
                {
                    break; 
                }
                nextTask = alertOrInvestigatingToAlertTask;
                break;

            case BState.Investigating:
                if (currentDistraction == newDistraction)
                {
                    if (newDistraction.Position != lastSeenDistractionPosition)
                    {
                        nextTask = investigatingToInvestigatingTask;
                    }
                }
                else 
                { 
                    nextTask = alertOrInvestigatingToAlertTask; 
                }
                break;
        }

        currentDistraction = newDistraction;
        lastSeenDistractionPosition = currentDistraction.Position;

        if (nextTask != null)
        {
            behaviourController.SetCurrentTask(nextTask);
        }
    }

    void LoadDifficulty(int level)
    {
        GuardSettings settings = difficultySettings.GetDifficultySettings<GuardSettings>(level);

        investigationRoutineDelay = settings.investigationRoutineDelay;
        examineDelay = settings.examineDelay;

        canCrouch = settings.canCrouch;

        guardDistractionSensor.LoadSettings(settings);
        guardMovement.LoadSettings(settings);
    }
}