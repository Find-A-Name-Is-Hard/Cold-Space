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

- **MVC 架构**：玩家控制器采用 MVC 模式解耦逻辑、数据与表现层，降低模块间依赖，提升可维护性
- **对象池**：通用对象池，以 `Dictionary<string, Stack<GameObject>>` 管理多类对象；支持同步/协程两种预加载策略，避免批量实例化造成的帧卡顿
- **全局单例管理**：关卡管理器、输入管理器采用单例模式，确保全局状态唯一且易于访问
- **数据驱动弹幕系统**：基于 Scriptable Object 配置弹幕攻击参数（发射速度、波次数量、角度间隔等），支持旋转发射、自机狙、随机扩散等多种弹幕类型，多发射器可组合叠加形成复杂弹幕

## 反思与改进方向

- **引入事件总线**：当前用 `Action<T>` 实现模块间通信，导致脚本间直接持有引用、耦合度高。应引入事件总线作为中介，彻底解耦各模块
- **对象池数据结构优化**：Stack 不支持随机访问，子弹被提前销毁时无法从池中移除，需额外判空。改用 List 或 HashSet 可支持按需查找和移除，更适合游戏对象的生命周期管理
- **对象池索引优化**：当前以 GameObject 名称为索引，名称被修改后会导致找不到对应池。改用预制体实例 ID 作为索引更稳健