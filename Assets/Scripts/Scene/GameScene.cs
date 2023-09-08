using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
        sceneType = SceneType.WORLD;
        Screen.SetResolution(1280, 960, false);
    }

    public override void Clear()
    {

    }
}