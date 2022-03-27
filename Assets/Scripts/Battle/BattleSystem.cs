using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BattleState { Start, PlayerAction, PlayerMove, EnemyMove, Busy } //zmienne do przechowywania stanu walki

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleDialogBox dialogBox;

    public event Action<bool> OnBattleOver;

    BattleState state;
    int currentAction;
    int currentMove;

    EnemiesGroups playerGroups;
    Enemies wildEnemies;

    public void StartBattle(EnemiesGroups playerGroups, Enemies wildEnemies)
    {
        this.playerGroups = playerGroups;
        this.wildEnemies = wildEnemies;
        StartCoroutine(SetupBattle());
    }

    //funkcja ladujaca wszystkie odpowiednie informacje o graczu i przeciwniku oraz informacje w dialog boxie
    public IEnumerator SetupBattle()
    {
        playerUnit.Setup(playerGroups.GetPlayer());
        enemyUnit.Setup(wildEnemies);
        playerHud.SetData(playerUnit.Enemy);
        enemyHud.SetData(enemyUnit.Enemy);

        dialogBox.SetMoveNames(playerUnit.Enemy.Moves);

        yield return dialogBox.TypeDialog($"An angry {enemyUnit.Enemy.Base.Name} is attacking!");

        PlayerAction();
    }

    void PlayerAction()
    {
        state = BattleState.PlayerAction;
        StartCoroutine(dialogBox.TypeDialog("Your turn"));
        dialogBox.EnableActionSelector(true);
    }

    void PlayerMove()
    {
        state = BattleState.PlayerMove;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }

    //ruch gracza
    IEnumerator PerformPlayerMove()
    {
        state = BattleState.Busy;

        var move = playerUnit.Enemy.Moves[currentMove];
        yield return dialogBox.TypeDialog($"{ playerUnit.Enemy.Base.Name } used { move.Base.Name }");

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        enemyUnit.PlayHitAnimation();

        var damageDetails = enemyUnit.Enemy.TakeDamage(move, playerUnit.Enemy);
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Dead)
        {
            yield return dialogBox.TypeDialog($"{ enemyUnit.Enemy.Base.Name } is dead");
            enemyUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);
            OnBattleOver(true);
        }
        else
        {
            StartCoroutine(EnemyMove());
        }
    }

    //ruch przeciwnika
    IEnumerator EnemyMove()
    {
        state = BattleState.EnemyMove;

        var move = enemyUnit.Enemy.GetRandomMove();
        yield return dialogBox.TypeDialog($"{ enemyUnit.Enemy.Base.Name } used { move.Base.Name }");

        enemyUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        playerUnit.PlayHitAnimation();

        var damageDetails = playerUnit.Enemy.TakeDamage(move, playerUnit.Enemy);
        yield return playerHud.UpdateHP();
        yield return ShowDamageDetails(damageDetails);

        if (damageDetails.Dead)
        {
            yield return dialogBox.TypeDialog($"{ playerUnit.Enemy.Base.Name } is dead");
            playerUnit.PlayFaintAnimation();

            yield return new WaitForSeconds(2f);
            OnBattleOver(false);
        }
        else
        {
            PlayerAction();
        }

    }
    
    //petla wyswietlajaca informacje w dialog boxie o krytykach oraz czy typ ataku jest efektwny/nie efektywny
    IEnumerator ShowDamageDetails(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("Critical strike!");

        if (damageDetails.TypeEffectives > 1f)
            yield return dialogBox.TypeDialog("It's super effective!");
        else if (damageDetails.TypeEffectives < 1f)
            yield return dialogBox.TypeDialog("It's not very effective!");
    }

    //poruszanie sie po dialog boxie
    public void HandleUpdate()
    {
        if (state == BattleState.PlayerAction)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.PlayerMove)
        {
            HandleMoveSelection();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
                ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
                --currentAction;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                //walka
                PlayerMove();
            }
            else if (currentAction == 1)
            {
                //ucieczka
            }
        }
    }

    //wybor atakow w dialog boxie
    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Enemy.Moves.Count - 1)
                ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
                --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Enemy.Moves.Count - 2)
                currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Enemy.Moves[currentMove]);

        //aktywacja wybranego ataku
        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableMoveSelector(false);
            dialogBox.EnableDialogText(true);
            StartCoroutine(PerformPlayerMove());
        }
    }
  
}
