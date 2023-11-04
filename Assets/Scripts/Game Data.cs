[System.Serializable]
public class GameData
{
    public int money = 0;
    public int level =1;
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