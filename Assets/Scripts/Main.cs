using System;
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
    public GameConfig GameConfig;
    public GameState GameState;

    public bool IsNight;

    private bool skip;

    //init
    private void Awake()
    {
        GameState = new GameState();
        GameState.WorldGrid = new(GameConfig.WorldSize);
        GameState.MoneyCount = GameConfig.StartMoney;

        G.Main = this;
    }

    //game start
    private void Start()
    {
        StartCoroutine(BeginFirstDay());
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

    private IEnumerator BeginFirstDay()
    {
        G.BuildSystem.SetBuildingActive(false);

        yield return SmartWait(1);

        G.BuildSystem.SetBuildingActive(true);
    }
    private IEnumerator BeginDay()
    {
        //DayNightSystem -> ShowDay
        //wait for it

        IsNight = false;
        EventBus.Invoke(new DayBeginEvent());

        yield return SmartWait(1);

        G.BuildSystem.ChangeBuildingActive(true);
    }
    private IEnumerator EndDay()
    {
        //DayNightSystem -> ShowNight
        //wait for it

        yield return SmartWait(1);

        GameState.Day++;
        IsNight = true;
        EventBus.Invoke(new DayCompleteEvent());
        G.StatsDrawer.UpdateDraw();

        G.BuildSystem.ChangeBuildingActive(false);
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
