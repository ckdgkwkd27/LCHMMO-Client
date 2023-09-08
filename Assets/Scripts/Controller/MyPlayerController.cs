using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static Define;

public class MyPlayerController : PlayerController
{
    bool _moveKeyPressed = false;

    protected override void Init()
    {
        base.Init();
    }

    protected override void UpdateController()
    {
        switch(State)
        {
            case CreatureState.IDLE:
                GetDirInput();
                break;
            case CreatureState.MOVING:
                GetDirInput();
                break;
        }

        base.UpdateController();
    }

    protected override void UpdateIdle()
    {
        if(_moveKeyPressed)
        {
            Debug.Log("Move Key Pressed!");
            State = CreatureState.MOVING;
            return;
        }

        if(_coSkillCooltime == null && Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Skill!");
            //#TODO: Send Skill Packet
        }
    }

    Coroutine _coSkillCooltime;
    IEnumerator CoInputCooltime(float time)
    {
        yield return new WaitForSeconds(time);
        _coSkillCooltime = null;
    }

    void LateUpdate()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    // 키보드 입력
    void GetDirInput()
    {
        _moveKeyPressed = true;

        if (Input.GetKey(KeyCode.W))
        {
            Dir = MoveDir.UP;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Dir = MoveDir.DOWN;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            Dir = MoveDir.LEFT;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Dir = MoveDir.RIGHT;
        }
        else
        {
            _moveKeyPressed = false;
        }
    }

    protected override void MoveToNextPos()
    {
        if (_moveKeyPressed == false)
        {
            State = CreatureState.IDLE;
            CheckUpdatedFlag();
            return;
        }

        Vector3Int destPos = CellPos;

        switch (Dir)
        {
            case MoveDir.UP:
                destPos += Vector3Int.up;
                Debug.Log("MoveToNextPos UP");
                break;
            case MoveDir.DOWN:
                destPos += Vector3Int.down;
                break;
            case MoveDir.LEFT:
                destPos += Vector3Int.left;
                break;
            case MoveDir.RIGHT:
                destPos += Vector3Int.right;
                break;
        }

        if (Managers.Map.CanGo(destPos))
        {
            if (Managers.Object.FindCreature(destPos) == null)
            {
                CellPos = destPos;
            }
        }

        CheckUpdatedFlag();
    }

    protected override void CheckUpdatedFlag()
    {
        if (_updated)
        {
            Protocol.RequestMove movePacket = new Protocol.RequestMove();
            movePacket.PosInfo = PosInfo;
            Managers.Network.Send(movePacket);
            _updated = false;
        }
    }
}
