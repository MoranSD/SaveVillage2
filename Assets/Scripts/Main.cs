using System.Collections;
using System.Linq;
using UnityEngine;

public static class G
{
    public static Main Main;
    public static CameraController CameraController;
    public static BuildSystem BuildSystem;
    public static Wallet Wallet;
    public static PopulationSystem PopulationSystem;
    public static StatsDrawer StatsDrawer;
    public static HungrySystem HungrySystem;
    public static EnemySystem EnemySystem;
}

public class GameState
{
    public WorldGrid WorldGrid;
    public int MoneyCount;
    public int Population;
    public int Day;
    public int FoodCount;
}

public struct DayBeginEvent { }
public struct DayCompleteEvent { }

public class Main : MonoBehaviour
{
    public GameState GameState;

    public Vector2Int WorldSize;
    public int StartMoney;
    public int StartFood;
    public float DayDuration;
    public BuildingConfig[] BuildingConfigs;

    public bool IsNight;

    private bool skip;

    //init
    private void Awake()
    {
        GameState = new GameState();
        GameState.WorldGrid = new(WorldSize);
        GameState.MoneyCount = StartMoney;
        GameState.FoodCount = StartFood;

        G.Main = this;
    }

    //game start
    private void Start()
    {
        StartCoroutine(BeginDay(true));
    }

    private void Update()
    {
        ReadDebugHotKeys();
    }

    private void ReadDebugHotKeys()
    {
        if (Input.GetKeyDown(KeyCode.T))
            skip = true;

        if (Input.GetKeyDown(KeyCode.M))
            G.Wallet.AddMoney(10);

        if (Input.GetKeyDown(KeyCode.D))
        {
            //skip day
            GameState.Day++;
            IsNight = true;
            EventBus.Invoke(new DayCompleteEvent());
            IsNight = false;
            EventBus.Invoke(new DayBeginEvent());
            G.StatsDrawer.UpdateDraw();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            var b = G.BuildSystem.Buildings.FirstOrDefault(x => x.transform.position == (Vector3)(Vector2)GameMath.MouseToGridWorld());

            if (b != null)
                b.Kill();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            var b = G.BuildSystem.Buildings.FirstOrDefault(x => x.transform.position == (Vector3)(Vector2)GameMath.MouseToGridWorld());

            if (b != null && b is PopulationBuilding)
                ((PopulationBuilding)b).RemoveOnePopulation();
        }
    }

    private IEnumerator BeginDay(bool IsFirst)
    {
        if (IsFirst)
        {
            G.BuildSystem.SetBuildingActive(false);
        }
        else
        {
            //DayNightSystem -> ShowDay
            //wait for it

            IsNight = false;
            GameState.Day++;
            EventBus.Invoke(new DayBeginEvent());
            G.StatsDrawer.UpdateDraw();
        }

        yield return SmartWait(1);

        G.BuildSystem.ChangeBuildingActive(true);

        yield return SmartWait(DayDuration);

        yield return EndDay();
    }
    private IEnumerator EndDay()
    {
        //DayNightSystem -> ShowNight
        //wait for it

        yield return SmartWait(1);

        IsNight = true;
        EventBus.Invoke(new DayCompleteEvent());

        G.BuildSystem.ChangeBuildingActive(false);

        G.EnemySystem.SpawnWave();

        yield return new WaitUntil(() => G.EnemySystem.IsWaveDead);

        yield return SmartWait(1);

        yield return BeginDay(false);
    }
    private IEnumerator SmartWait(float seconds)
    {
        skip = false;
        float s = 0;
        while(skip == false && s <= seconds)
        {
            yield return new WaitForEndOfFrame();
            s += Time.deltaTime;
        }
    }
}
