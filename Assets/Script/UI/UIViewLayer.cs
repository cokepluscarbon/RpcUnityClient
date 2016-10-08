using System;
using System.Collections.Generic;

public enum UIViewLayer
{
    None = 0,
    Scene = 1,      // 场景层（场景物品到UI的映射、血条、飘字，拾取道具的展示等）
    View = 2,       // View层（普通的养成页面）                      ** 入栈
    Toolbar = 3,    // 工具栏层（返回按钮、货币栏、导航栏等）
    FullView = 4,   // FullView层（需要全屏显示的页面）              ** 入栈
    Prompt = 5,     // 提示层（对话框、确认框、Tips等）
    Guide = 6,      // 引导层（显示引导相关页面）
    System = 7,     // 系统层（Loading页面、退出框、错误、重连提示等）** 这层的页面不会被UIManager销毁，需要自行处理生命周期
}
