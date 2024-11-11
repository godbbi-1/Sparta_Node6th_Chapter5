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

[Tower.js]
public void OnAttackMonster(Monster monster)
    {
        // towerId == 0 으로 버그 수정
        if (monster == null || towerId == 0) return;
        var beam = Instantiate(ResourceManager.instance.LoadAsset<BeamObject>("BeamObject"), beamPosition).SetTimer().SetTarget(monster);
        isAttackDelay = true;
        monster.SetDamage(power);
        if (player == ePlayer.me)
        {
            StartCoroutine(OnCooldown());

            GamePacket packet = new GamePacket();
            packet.TowerAttackRequest = new C2STowerAttackRequest() { TowerId = towerId, MonsterId = monster.monsterId };
            SocketManager.instance.Send(packet);
        }
    }
