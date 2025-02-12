using UnityEngine;

public class ArmyBuilding : PopulationNeedBuilding
{
    [SerializeField] private float respawnRate;

    private float respawnTime;

    protected override void Awake()
    {
        base.Awake();
        EventBus.Subscribe<DayCompleteEvent>(OnDayComplete);
        EventBus.Subscribe<DayBeginEvent>(OnDayBegin);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventBus.Unsubscribe<DayCompleteEvent>(OnDayComplete);
        EventBus.Unsubscribe<DayBeginEvent>(OnDayBegin);
    }

    private void Update()
    {
        if (G.Main.IsNight == false) return;

        //check for deads

        respawnTime -= Time.deltaTime;
        if(respawnTime <= 0)
        {
            respawnTime = respawnRate;
        }
    }

    private void OnDayComplete(DayCompleteEvent dayComplete)
    {
        respawnTime = respawnRate;
        //spawn warriors
    }

    private void OnDayBegin(DayBeginEvent dayBegin)
    {
        //destroy warriors
    }
}
