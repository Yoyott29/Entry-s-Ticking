using UnityEngine;


public enum ActionType { Move, Attack }

public class QueuedAction
{
    public ActionType type;
    public Vector2Int direction;

    public static QueuedAction MoveAction(Vector2Int dir) {
        return new QueuedAction { type = ActionType.Move, direction = dir };
    }

    public static QueuedAction AttackAction() {
        return new QueuedAction { type = ActionType.Attack, direction = Vector2Int.zero };
    }
}
