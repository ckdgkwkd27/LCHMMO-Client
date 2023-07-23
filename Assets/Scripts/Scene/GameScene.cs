using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        sceneType = SceneType.WORLD;
        //Managers.Map.LoadMap(1);
        var net = Managers.Network;
        Screen.SetResolution(640, 480, false);
    }

    public override void Clear()
    {

    }
}