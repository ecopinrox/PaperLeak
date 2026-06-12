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
    GuardAnimation guardVisualManager;

    GridManager gridMovementMonitor;

    BController behaviourController;

    [SerializeField] Transform patrolPathObject;
    List<Waypoint> patrolPath;

    [SerializeField] DifficultySwitch difficultySettings;

    float investigationRoutineDelay = 1f;
    float examineDelay = 6f;

    bool canCrouch = false;
    bool frozen = false;

    //used to uniquely identify guards when saving
    Vector2Int spawnLoc;

    [Header("Layer masks")]
    [SerializeField] LayerMask crawlableLayerMask;

    //state variables
    IEnumerator<Waypoint> patrolIterator;
    Vector2Int interruptLocation;
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
        guardVisualManager = GetComponent<GuardAnimation>();

        gridMovementMonitor = FindAnyObjectByType<GridManager>();

        patrolPath = new(patrolPathObject.GetComponentsInChildren<Waypoint>(true));
        if(patrolPath.Count == 0)
        {
            Debug.LogError($"Patrol path of guard {gameObject.name} is empty.");
        }
        patrolIterator = patrolPath.GetEnumerator();

        spawnLoc = Vector2Int.RoundToInt(transform.position);
    }

    private void OnEnable()
    {
        DifficultySwitch.loadDifficultySettings += LoadDifficulty;

        LevelManager.OnStateLoad += LoadFrozenState;
        LevelManager.OnStateSave += SaveFrozenState;
    }

    private void OnDisable()
    {
        DifficultySwitch.loadDifficultySettings -= LoadDifficulty;

        LevelManager.OnStateLoad -= LoadFrozenState;
        LevelManager.OnStateSave -= SaveFrozenState;
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
                guardMovement.LookInDirection(patrolIterator.Current.Rotation * Vector2.up);
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
                interruptLocation = guardMovement.GridBasedPosition;
            });

        BAction enterAlertState = new(
            BState.Alert,
            () =>
            {
                //isCrouched = false;
                guardMovement.SetCrouch(false);
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
                //isCrouched = false;
                guardMovement.SetCrouch(false);
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
                    //isCrouched = true;
                    guardMovement.SetCrouch(true);
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
                //isCrouched = false;
                guardMovement.SetCrouch(false);
                guardDistractionSensor.RegisterDistraction(currentDistraction);
                currentDistraction = null;
                guardVisualManager.ChangeToIdleColor();
            });

        //Patrolling states
        getNextWaypoint             .SetNext(goToWaypoint);
        goToWaypoint                .SetNext(waitAtWaypoint);
        waitAtWaypoint              .SetNext(getNextWaypoint);
        returnToInterruptLocation   .SetNext(goToWaypoint);

        //Alert states
        saveInterruptLocation       .SetNext(enterAlertState);
        enterAlertState             .SetNext(waitOnAlert);
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
        if (frozen) return;

        GetOverridingDistraction();
        behaviourController.Tick();
    }

    public void Freeze()
    {
        frozen = true;
        guardMovement.StopMoving();
        guardVisualManager.ChangeToFreezeColor();
    }

    void GetOverridingDistraction()
    {
        Distraction newDistraction = guardDistractionSensor.GetDistraction(guardMovement.IsCrouching);
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

                guardDistractionSensor.RegisterDistraction(currentDistraction);
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
                    guardDistractionSensor.RegisterDistraction(currentDistraction);
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

    void LoadFrozenState(SaveState saveState)
    {
        if(saveState.frozenGuards.ContainsKey(spawnLoc))
        {
            guardMovement.SetPosition(saveState.frozenGuards[spawnLoc].Item1);
            transform.rotation = saveState.frozenGuards[spawnLoc].Item2;
            Freeze();
        }
    }

    void SaveFrozenState(SaveState saveState)
    {
        if(frozen)
        {
            saveState.frozenGuards.TryAdd(spawnLoc, new(guardMovement.GridBasedPosition, transform.rotation));
        }
        else
        {
            saveState.frozenGuards.Remove(spawnLoc);
        }
    }
}