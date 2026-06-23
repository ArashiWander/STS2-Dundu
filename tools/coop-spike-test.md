# 联机跨玩家 spike — 人肉实证脚本 (Phase 2 关口)

目的：证明 **Route A**（出牌 → buff 队友）在真实两人联机里 **正确且无 desync**。
这是 Phase 2 的过关条件。代码侧（API 落实 + 最小卡编译 + 单机加载）已自动完成；
**这一步必须两台实例 + 真人联机**，自动化做不了，需要 Yemin（墩墩）+ CC（嘟嘟）来跑。

被验证的卡：`TogetherGuardSpikeCard`（在一起·守护）—— `TargetType.AnyAlly`，cost 0，
打出后给**选中的队友** 8 点格挡。源码：`DundunDudu/DundunDuduCode/Spike/TogetherGuardSpikeCard.cs`。
这是临时验证卡，Phase 5 做真正的 双人连携 内容时会删/替换。

---

## 0. 前置（两台都要，且必须完全一致）

- 两台都装了 **相同版本** 的 `DundunDudu` mod + **相同版本** 的 BaseLib（联机序列化安全的硬要求）。
  - 我方这台：`…\Slay the Spire 2\mods\DundunDudu\`（已部署最新 dll）。
  - 另一台（CC）：把同一个 `DundunDudu` 文件夹（dll+json）拷到她的 `…\mods\DundunDudu\`，
    并确保她订阅了 BaseLib 且版本一致（当前 3.3.2）。
- 游戏版本两台一致（当前 v0.107.1）。
- 开 mod 启动两台（mod 自动加载，无需特殊参数；详见 tools/smoke.ps1 的发现）。
- 开发者控制台可用（反引号 ` 键打开）。若没开，用 RitsuLib 的 mod 设置或控制台设置打开。

> 启用 gameplay mod 会禁用成就——正常，不是 bug。

---

## 1. 建联机房

1. 一人（建议 Yemin 当 **host**）创建联机房，另一人（CC）加入。
2. 角色随便选（spike 卡走无色池 / 控制台加牌，与角色无关；真正的墩墩/嘟嘟在 Phase 3+）。
3. 开局，推进到 **第一场战斗**，双方都在 `COMBAT_PLAYER_TURN`。

## 2. 拿到 spike 卡（控制台）

在 host(墩墩) 这边打开控制台，输入：

```
card DUNDUNDUDU-TOGETHER_GUARD_SPIKE_CARD
```

> 卡的 id 以控制台自动补全为准：输入 `card ` 后按 Tab，或找 `TOGETHER` / `在一起`。
> 若上面的 id 不对，用自动补全里的那个（前缀大概率是 `DUNDUNDUDU-…`）。

卡会进入 host 的手牌。

## 3. 跨玩家施放（核心步骤）

1. **墩墩（host）出这张「在一起·守护」**，目标选 **嘟嘟（队友）的角色**（AnyAlly 会让你选玩家目标）。
1a. **诊断 — 本地先于对端**：先只看 **墩墩(host) 自己屏幕** 上嘟嘟的格挡：
   - host 自己屏幕嘟嘟就 **没 +8** → 是**卡逻辑 bug**（OnPlay 没对 target 生效），与联机无关，先修卡逻辑（可用 §7 的 Self 版单机验证）。
   - host 屏幕 +8 **但嘟嘟屏幕没 +8** → 是**同步 bug**（跨玩家没复制）→ 记下，这是 Route-A 的核心失败模式。
   两者要分清，别混报。
2. 然后双方同时观察 **嘟嘟的格挡值**：
   - 嘟嘟这边屏幕：格挡 **+8**。
   - 墩墩这边屏幕：看到的嘟嘟格挡也应 **+8**（两屏一致）。
3. 反向再来一次：**嘟嘟出一张**（她也 `card …` 加一张），目标选 **墩墩**，确认墩墩 +8、两屏一致。
4. 各自 `end_turn`，**连打 3~4 个回合**（其间正常出牌/被打），观察战斗是否一直正常、不卡死。

## 4. 判过标准（全部满足才算 PASS）

- [ ] 队友格挡确实 **+8**，且 **两台屏幕数值一致**（不是只有出牌方看到）。
- [ ] 双向都成立（墩墩→嘟嘟、嘟嘟→墩墩）。
- [ ] 连续 3~4 回合 **无 desync**：不卡死、不弹「连接/同步」错误、战斗能正常打完。
- [ ] 两台的 `godot.log` 里 **没有** desync / 异常标记（见 §5）。

任一不满足 = **STOP 并报告**（这会改写 Route A 设计，可能要回到 Route B 自定义网络消息 / v2）。

## 5. 要抓的日志（两台都抓，发我）

日志路径（每台各自）：
```
%APPDATA%\SlayTheSpire2\logs\godot.log
```
（即 `C:\Users\<你>\AppData\Roaming\SlayTheSpire2\logs\godot.log`，每次启动会归档上一份为带时间戳的 .log）

测完，在两台分别跑（PowerShell）把关键行抓出来发我：

```powershell
$log = "$env:APPDATA\SlayTheSpire2\logs\godot.log"
Select-String -Path $log -Pattern 'divergence|checksum|desync|StateDivergence|\[ERROR\]|exception|TogetherGuard|DUNDUNDUDU|Block' -CaseSensitive:$false |
  Select-Object -Last 60 | ForEach-Object { $_.Line }
```

desync 的硬信号（出现即 FAIL）：`StateDivergenceMessage` / `checksum` 不一致 / `divergence`。
另外发我：嘟嘟格挡 +8 时 **两台各一张截图**（证明两屏一致）。

## 6. 结果回我

- 「PASS」+ 两台日志 grep + 截图；或
- 「FAIL，<现象>」+ 两台日志（我据 §desync 失败模式定位，决定是否转 Route B）。

我据此把 SPEC §6 的「已验证机制 / 实测到的 desync 模式」补实，然后才进 Phase 3（角色骨架）。
