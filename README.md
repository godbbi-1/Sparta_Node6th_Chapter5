# Sparta_Node6th_Chapter5

# 회원가입 버튼 [로그인] => [회원가입]

## StateSyncNotification

```bash
# 타워 정보 갱신
foreach (var tower in response.Towers)
{
    // Debug.Log($"Checking tower: {tower.TowerId}, X: {tower.X}, Y: {tower.Y}");

    if (!GameManager.instance.GetTower(tower.TowerId))
    {
        GameManager.instance.AddTower(new TowerData() { TowerId = tower.TowerId, X = tower.X, Y = tower.Y }, ePlayer.me);
    }
}
```

```bash
# 몬스터 정보 갱신
foreach (var monster in response.Monsters)
{
    if (!GameManager.instance.GetMonster(monster.MonsterId))
    {
        GameManager.instance.AddMonster(new MonsterData() { MonsterId = monster.MonsterId, MonsterNumber = monster.MonsterNumber, Level = GameManager.instance.level }, ePlayer.me);
    }
}
```

```bash
# 타워 공격 (버그 수정)
[Tower.cs]
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
```

```bash
# 홈 체력 갱신
[GameManager.cs]
private int _homeHp1 = 100;
public int homeHp1
{
    get => _homeHp1;
    set
    {
        _homeHp1 = value;
        UIManager.Get<UIGame>().SetHpGauge1(value);

        if (_homeHp1 <= 0)
        {
            GamePacket packet = new GamePacket();
            packet.GameEndRequest = new C2SGameEndRequest() { };
            SocketManager.instance.Send(packet);
        }
    }
}
private int _homeHp2 = 100;
public int homeHp2
{
    get => _homeHp2;
    set
    {
        _homeHp2 = value;
        UIManager.Get<UIGame>().SetHpGauge2(value);

        if (_homeHp2 <= 0)
        {
            GamePacket packet = new GamePacket();
            packet.GameEndRequest = new C2SGameEndRequest() { };
            SocketManager.instance.Send(packet);
        }
    }
}
```
