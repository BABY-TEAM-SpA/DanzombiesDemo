using UnityEngine;

public class LevelProgressTracker : Service<LevelProgressTracker>
{
    #region [VARIABLES]
    #region Stats
    public int HordesCleared { get; private set; }

    public int PerfectSteps { get; private set; }
    public int GoodSteps { get; private set; }
    public int OkSteps { get; private set; }
    public int BadSteps { get; private set; }

    public float GameTime { get; private set; }
    #endregion

    #region Expositions
    [SerializeField] private DanceZone[] hordes;
    #endregion

    #region Interns
    private bool isTracking;
    private float elapsed;
    #endregion
    #endregion

    #region [UNITY]
    private void Update()
    {
        if (isTracking)
            Track();
    }
    #endregion

    #region [METHODS]
    #region Track
    public void StartTracking()
    {
        Reset();

        foreach (DanceZone horde in hordes)
            horde.OnCompleted += () => HordesCleared++;

        isTracking = true;
    }

    public void StopTracking()
    {
        isTracking = false;

        foreach (DanceZone horde in hordes)
            horde.OnCompleted -= () => HordesCleared++;

        GameTime = elapsed;
    }

    private void Track()
    {
        elapsed += Time.deltaTime;
    }
    #endregion

    #region Helpers
    private void Reset()
    {
        HordesCleared = 0;

        PerfectSteps = 0;
        GoodSteps = 0;
        OkSteps = 0;
        BadSteps = 0;

        GameTime = 0f;
    }
    #endregion
    #endregion
}
