using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ironcow;
using UnityEngine.UI;
using Ironcow.WebSocketPacket;
using Google.Protobuf;
using static GamePacket;
using System;

public class SocketManager : TCPSocketManagerBase<SocketManager>
{
    public int level = 1;
    public void RegisterResponse(GamePacket gamePacket)
    {
        var response = gamePacket.RegisterResponse;
        UIManager.Get<PopupRegister>().OnRegisterResult(response.Success);
    }

    public void LoginResponse(GamePacket gamePacket)
    {
        var response = gamePacket.LoginResponse;
        UIManager.Get<PopupLogin>().OnLoginResult(response.Success);
    }

    public void MatchStartNotification(GamePacket gamePacket)
    {
        var response = gamePacket.MatchStartNotification;
        UIManager.Get<UIMain>().OnMatchResult(response);
    }

    public void EnemyMonsterDeathNotification(GamePacket gamePacket)
    {
        var response = gamePacket.EnemyMonsterDeathNotification;
        GameManager.instance.RemoveMonster(response.MonsterId);
    }

    public void EnemyTowerAttackNotification(GamePacket gamePacket)
    {
        var response = gamePacket.EnemyTowerAttackNotification;
        GameManager.instance.OnAttackMonster(response.TowerId, response.MonsterId);
    }

    public void GameOverNotification(GamePacket gamePacket)
    {
        var response = gamePacket.GameOverNotification;
        UIManager.Get<UIGame>().SetWinner(response.IsWin);
    }

    public void SpawnEnemyMonsterNotification(GamePacket gamePacket)
    {
        var response = gamePacket.SpawnEnemyMonsterNotification;
        GameManager.instance.AddMonster(new MonsterData() { MonsterId = response.MonsterId, MonsterNumber = response.MonsterNumber, Level = GameManager.instance.level }, ePlayer.another);
    }

    public void StateSyncNotification(GamePacket gamePacket)
    {
        var response = gamePacket.StateSyncNotification;
        GameManager.instance.level = response.MonsterLevel;
        GameManager.instance.homeHp1 = response.BaseHp;
        GameManager.instance.score = response.Score;
        GameManager.instance.gold = response.UserGold;

        // Debug.Log("towers: " + response.Towers);

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
    }

    public void UpdateBaseHpNotification(GamePacket gamePacket)
    {
        var response = gamePacket.UpdateBaseHpNotification;
        if (!response.IsOpponent)
            GameManager.instance.homeHp1 = response.BaseHp;
        else
            GameManager.instance.homeHp2 = response.BaseHp;

    }

    public void SpawnMonsterResponse(GamePacket gamePacket)
    {
        var response = gamePacket.SpawnMonsterResponse;
        GameManager.instance.AddMonster(new MonsterData() { MonsterId = response.MonsterId, MonsterNumber = response.MonsterNumber, Level = GameManager.instance.level }, ePlayer.me);
    }

    public void TowerPurchaseResponse(GamePacket gamePacket)
    {
        var response = gamePacket.TowerPurchaseResponse;
        GameManager.instance.SetTower(response.TowerId);
    }

    public void AddEnemyTowerNotification(GamePacket gamePacket)
    {
        var response = gamePacket.AddEnemyTowerNotification;
        GameManager.instance.AddTower(new TowerData() { TowerId = response.TowerId, X = response.X, Y = response.Y }, ePlayer.another);
    }


    protected override void InitPackets()
    {
        var payloads = Enum.GetNames(typeof(PayloadOneofCase));
        var methods = GetType().GetMethods();
        foreach (var payload in payloads)
        {
            var val = (PayloadOneofCase)Enum.Parse(typeof(PayloadOneofCase), payload);
            var method = GetType().GetMethod(payload);
            if (method != null)
            {
                var action = (Action<GamePacket>)Delegate.CreateDelegate(typeof(Action<GamePacket>), this, method);
                _onRecv.Add(val, action);
            }
        }
    }
}