using System;
using System.Collections.Generic;

public class UIBaseView : UIBase
{
    public object[] args;

    public void CloseView()
    {
        UIManager.BackView();
    }

    public bool HasArgs()
    {
        return GetArgsLength() != 0;
    }

    public int GetArgsLength()
    {
        if (args != null)
        {
            return args.Length;
        }

        return 0;
    }
}

