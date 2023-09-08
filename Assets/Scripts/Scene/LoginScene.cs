using UnityEngine;

public class LoginScene : BaseScene
{
    UI_Login ui_Login;
    protected override void Init()
    {
        base.Init();
        Screen.SetResolution(1280, 960, false);
    }

    public override void Clear()
    {

    }
}