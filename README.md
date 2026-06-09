# 冰冷宙域 / Cold Space
## 简介
[![演示视频](ReadMe_Content/Cover.png)](https://github.com/Find-A-Name-Is-Hard/Cold-Space/releases/download/Video/TrailerForColdSpace2.mp4)


本游戏尝试将弹幕射击玩法与医学诊断主题主题结合，玩家扮演一名驾驶飞船的医生，需要尝试检测敌人弱点，推理出正确的弱点组合再发动最终一击。

> 游戏链接：https://nvll-reference.itch.io/cold-space

> 游戏PV：[点击观看演示视频](https://github.com/Find-A-Name-Is-Hard/Cold-Space/releases/download/Video/TrailerForColdSpace2.mp4)


## 操控方式
- 方向键↑↓←→：移动飞船

- 按住Shift：降低移动速度

- Z：射击，命中敌人积攒能量，但不造成伤害

- X+数字1~4：发射一种弱点测试攻击，命中后告知敌人是否有这种弱点

- C+数字1~6：针对不同弱点组合，选择不同最终攻击，如果敌人确实带有这种弱点组合，将成功杀死敌人，否则将会激怒敌人

## 技术亮点

- **MVC 架构**：玩家控制器采用 MVC 分层解耦，分离数据、表现与逻辑，提升可维护性。
- **对象池**：基于 `Dictionary<string, Stack<GameObject>>` 实现 GameObject 复用，支持协程分批预加载，避免单帧卡顿。
- **单例模式**：关卡管理器、输入管理器等核心模块采用单例，保证全局唯一实例。
- **弹幕系统**：支持直线弹、旋转弹、追踪弹等多类型，实现自机狙与随机弹算法；通过 `ScriptableObject` 可视化配置攻击数据，便于灵活扩展弹幕阵型。

## 困难与反思

- **事件系统耦合**：当前使用 `Action<T>` 观察者模式，脚本间直接持有引用，耦合度偏高。后续应引入 Event Bus 解耦模块依赖。
- **对象池优化**：`Stack` 不支持随机访问，销毁对象后可能残留空引用；应改用 `HashSet` 管理，并以预制体 `InstanceID` 替代名称作为索引，避免运行时改名导致索引失效。
- **弹幕生成器架构**：现有实现将生成器作为敌人子物体，扩展性差；理想做法是将其设计为场景独立对象，由统一管理器统一调度。