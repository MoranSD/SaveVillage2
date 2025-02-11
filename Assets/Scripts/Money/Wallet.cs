using UnityEngine;

public class Wallet : MonoBehaviour
{
    private void Awake()
    {
        G.Wallet = this;
    }

    public bool TryGetMoney(int count)
    {
        if(G.Main.GameState.MoneyCount < count)
            return false;

        G.Main.GameState.MoneyCount -= count;
        G.StatsDrawer.UpdateDraw();
        return true;
    }

    public void AddMoney(int count)
    {
        G.Main.GameState.MoneyCount += count;
        G.StatsDrawer.UpdateDraw();
    }
}
