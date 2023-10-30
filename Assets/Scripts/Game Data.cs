[System.Serializable]
public class GameData
{
    public int money;
    public int level;
    public int tableCount = 3;
    public float prestige = 1;

    public GameData(int level, int money, int tableCount, float prestige)
    {
        this.money = money;
        this.level = level;
        this.tableCount = tableCount;
        this.prestige = prestige;
    }

}