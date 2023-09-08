using UnityEngine;
using Google.Protobuf;
using static Define;

public class BaseController : MonoBehaviour
{
    public ulong Id { get; set; }
    Protocol.StatInfo _stat = new Protocol.StatInfo();
    public virtual Protocol.StatInfo Stat
    {
        get { return _stat; }
        set
        {
            if (_stat.Equals(value))
                return;

            _stat.Hp = value.Hp;
            _stat.MaxHp = value.MaxHp;
            _stat.Speed = value.Speed;
        }
    }

    public float Speed
    {
        get { return Stat.Speed; }
        set { Stat.Speed = value; }
    }

    public virtual uint Hp
    {
        get { return Stat.Hp; }
        set
        {
            Stat.Hp = value;
        }
    }

    protected bool _updated = false;
    Protocol.PositionInfo _positionInfo = new Protocol.PositionInfo();
    public Protocol.PositionInfo PosInfo
    {
        get { return _positionInfo; }
        set
        {
            if (_positionInfo.Equals(value))
                return;

            CellPos = new Vector3Int(value.PosX, value.PosY, 0);
            State = (CreatureState)value.State;
            //Dir = (MoveDir)value.moveDir;
        }
    }

    public void SyncPos()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = destPos;
    }

    public Vector3Int CellPos
    {
        get
        {
            return new Vector3Int(PosInfo.PosX, PosInfo.PosY, 0);
        }

        set
        {
            if (PosInfo.PosX == value.x && PosInfo.PosY == value.y)
                return;

            PosInfo.PosX = value.x;
            PosInfo.PosY = value.y;
            _updated = true;
        }
    }

    protected Animator _animator;
    protected SpriteRenderer _sprite;

    public virtual CreatureState State
    {
        get { return (CreatureState)PosInfo.State; }
        set
        {
            if ((CreatureState)PosInfo.State == value)
                return;

            PosInfo.State = (ushort)value;
            UpdateAnimation();
            _updated = true;
        }
    }

    public MoveDir Dir;
    public MoveDir GetDirFromVec(Vector3Int dir)
    {
        if (dir.x > 0)
            return MoveDir.RIGHT;
        else if (dir.x < 0)
            return MoveDir.LEFT;
        else if (dir.y > 0)
            return MoveDir.UP;
        else
            return MoveDir.DOWN;
    }

    public Vector3Int GetFrontCellPos()
    {
        Vector3Int cellPos = CellPos;

        switch (Dir)
        {
            case MoveDir.UP:
                cellPos += Vector3Int.up;
                break;
            case MoveDir.DOWN:
                cellPos += Vector3Int.down;
                break;
            case MoveDir.LEFT:
                cellPos += Vector3Int.left;
                break;
            case MoveDir.RIGHT:
                cellPos += Vector3Int.right;
                break;
        }

        return cellPos;
    }

    protected virtual void UpdateAnimation()
    {
        if (_animator == null || _sprite == null)
            return;

        if (State == CreatureState.IDLE)
        {
            switch (Dir)
            {
                case MoveDir.UP:
                    _animator.Play("IDLE_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.DOWN:
                    _animator.Play("IDLE_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.LEFT:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.RIGHT:
                    _animator.Play("IDLE_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (State == CreatureState.MOVING)
        {
            switch (Dir)
            {
                case MoveDir.UP:
                    _animator.Play("WALK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.DOWN:
                    _animator.Play("WALK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.LEFT:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.RIGHT:
                    _animator.Play("WALK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else if (State == CreatureState.SKILL)
        {
            switch (Dir)
            {
                case MoveDir.UP:
                    _animator.Play("ATTACK_BACK");
                    _sprite.flipX = false;
                    break;
                case MoveDir.DOWN:
                    _animator.Play("ATTACK_FRONT");
                    _sprite.flipX = false;
                    break;
                case MoveDir.LEFT:
                    _animator.Play("ATTACK_RIGHT");
                    _sprite.flipX = true;
                    break;
                case MoveDir.RIGHT:
                    _animator.Play("ATTACK_RIGHT");
                    _sprite.flipX = false;
                    break;
            }
        }
        else
        {

        }
    }

    void Start()
    {
        Init();
    }

    void Update()
    {
        UpdateController();
    }

    protected virtual void Init()
    {
        _animator = GetComponent<Animator>();
        _sprite = GetComponent<SpriteRenderer>();
        Vector3 pos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        transform.position = pos;

        UpdateAnimation();
    }

    protected virtual void UpdateController()
    {
        switch (State)
        {
            case CreatureState.IDLE:
                UpdateIdle();
                break;
            case CreatureState.MOVING:
                UpdateMoving();
                break;
            case CreatureState.SKILL:
                UpdateSkill();
                break;
            case CreatureState.DEAD:
                UpdateDead();
                break;
        }
    }

    protected virtual void UpdateIdle()
    {
    }

    protected virtual void UpdateMoving()
    {
        Vector3 destPos = Managers.Map.CurrentGrid.CellToWorld(CellPos) + new Vector3(0.5f, 0.5f);
        Vector3 moveDir = destPos - transform.position;

        // 도착 여부 체크
        float dist = moveDir.magnitude;
        if (dist < Speed * Time.deltaTime)
        {
            transform.position = destPos;
            MoveToNextPos();
        }
        else
        {
            transform.position += moveDir.normalized * Speed * Time.deltaTime;
            State = CreatureState.MOVING;
        }
    }

    protected virtual void MoveToNextPos()
    {

    }

    protected virtual void UpdateSkill()
    {

    }

    protected virtual void UpdateDead()
    {

    }
}