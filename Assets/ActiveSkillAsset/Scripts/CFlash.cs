﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "CFlash", menuName = "SkillData/CFlash")]
public class CFlash : SkillData
{
    public override BT.State Run(Character character, object temporary, int idx)
    {
        base.Run(character, temporary, idx);

        return Run(RoomManager.Instance.GetNearestAvailableArea((temporary as Character).transform.position));
    }

    private BT.State Run(object position)
    {
        if (!character || delay < 0 || amount < 0)
        {
            return BT.State.FAILURE;
        }
        character.isCasting = true;
        GameObject gameObject = ResourceManager.Instance.skillPool.GetPooledObject();
        gameObject.transform.position = character.transform.position;
        gameObject.AddComponent<CollisionSkill>().Init(character as Character, position, amount, Flash);

        character.GetCharacterComponents().AnimationHandler.SetLapsedAction(gameObject.GetComponent<CollisionSkill>().LapseAnimation);
        character.GetCharacterComponents().AnimationHandler.SetEndAction(gameObject.GetComponent<CollisionSkill>().EndAnimation);
        character.GetCharacterComponents().AnimationHandler.Skill(idx);
        return BT.State.SUCCESS;
    }

    private void Flash(Character user, object position, float amount)
    {
        user.transform.position = (Vector2)position;
    }

}
