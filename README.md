# Sparta_Node6th_Chapter5

## StateSyncNotification

// 타워 정보 갱신
foreach (var tower in response.Towers)
{
    // Debug.Log($"Checking tower: {tower.TowerId}, X: {tower.X}, Y: {tower.Y}");

    if (!GameManager.instance.GetTower(tower.TowerId))
    {
        GameManager.instance.AddTower(new TowerData() { TowerId = tower.TowerId, X = tower.X, Y = tower.Y }, ePlayer.me);
    }
}

// 몬스터 정보 갱신
foreach (var monster in response.Monsters)
{
    if (!GameManager.instance.GetMonster(monster.MonsterId))
    {
        GameManager.instance.AddMonster(new MonsterData() { MonsterId = monster.MonsterId, MonsterNumber = monster.MonsterNumber, Level = GameManager.instance.level }, ePlayer.me);
    }
}
