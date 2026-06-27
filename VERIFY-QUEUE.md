# VERIFY-QUEUE — 待上机人工验证清单

边建边记。每条 = 要做的操作（含游戏内控制台命令）+ 预期结果。机器只能编译+加载冒烟，**实战触发/数值/UI 必须上机验**。验过的打勾并写结果（或贴异常）。

## 用法
- 开发者控制台：游戏内呼出（默认反引号 `` ` `` 或波浪键；若无，查 DevConsole 绑定）。
- 加牌：`card <Entry>`，移除：`remove_card <Entry>`，升级：`upgrade <Entry>`。
- **我们的卡 Entry 带全前缀 `DUNDUNDUDU-`**（如 `card DUNDUNDUDU-STEADY_PUNCH`）。打 `card ` 后按 **Tab** 可补全/列出。
- 诊断日志在 `%APPDATA%\SlayTheSpire2\logs\godot.log`，我们的行带 `[闷气]` / `[DundunDudu]` 前缀，可回贴给我读。

---

## A. 进入 run / 崩溃回归（P1a 崩溃修复的欠债）
- [ ] **A1 墩墩进 run**：选墩墩 → Neow 选「给遗物」祝福（含七咒/LargeCapsule 那类）→ 不崩、正常进战斗。起手 = 5 猛击 + 4 防御 + 2 测试卡。
- [ ] **A2 嘟嘟进 run**：选嘟嘟同样选给遗物 → 不崩、进战斗。起手 = 4 轻击 + 4 轻避 + 1 灵巧。
- [ ] **A3 标签隐患抽查**：静态审计已确认全反编译里「按 CardTag 在角色池 First/Single 取牌」**只有 LargeCapsule**（Strike+Defend 已补标签）。上机随机吃几个会动起始牌的远古/遗物，确认不再 `Sequence contains no matching element`。

## B. 闷气 P1a — 分层改伤
- [ ] **B1**：战斗内 `card DUNDUNDUDU-STEADY_PUNCH`（加 1 张稳扎一拳，7 伤）。打 1 张测试卡（+3 闷气）→ 稳扎一拳应显示 **10**（×1.5）；再打 1 张（共 6）→ 显示 **14**（×2）。读 log `[闷气] 改伤 stacks=.. ×..`。
- [ ] **B2 只放大自己**：确认敌人攻击、嘟嘟（若在场）攻击的伤害**不**被墩墩闷气放大（钩子门控 `dealer==Owner`）。

## C. 闷气 P1b — 罚则
- [ ] **C1 免疫负面(6–9)**：闷气推到 6–9，让敌人对你施虚弱/易伤 → 应被挡下（debuff 不上身）。log `[闷气] 免疫负面 stacks=.. 挡下 WeakPower`。
- [ ] **C2 不免疫边界**：闷气 3–5 或 = 10 时，敌人 debuff **正常上身**（免疫只在 6–9）。
- [ ] **C3 临界穿透(9)**：闷气=9，**回合开始**受 1 点穿透（无视格挡，扣血）。log `[闷气] 临界自伤 stacks=9 ... 回合始`。
- [ ] **C4 自闭穿透(10)**：闷气=10，**回合结束**受 3 点穿透。log `[闷气] 自闭自伤 stacks=10 ... 回合末`。
- [ ] **C5 层数封顶(10)**：连打 4 张测试卡（请求 +12）→ 闷气停在 **10**，不超。log `[闷气] gain ...` 末次显示 `+0`（或 `[闷气] 封顶 ..→10`）。
- [ ] （待定，未做）**10 封技能**：本阶段未接 ShouldPlay；10 层暂不锁技能牌。要做再说。

## D. 卡池 / 控制台 / 卡面
- [ ] **D1**：`card DUNDUNDUDU-DOUBLE_KICK` / `GRAND_FEAST` / `WHIRLWIND` / `FEAST` 等，看中文卡名/数值/效果是否对。
- [ ] **D2 奖励池**：正常通关战斗，卡牌奖励里应能出现墩墩 12 张 / 嘟嘟 17 张 cheap（Common/Uncommon/Rare）。Basic（猛击/防御）与测试卡不进奖励池（正常）。

## E. 清理项（与崩溃分开）
- [ ] **E1 反斜杠路径**：进战斗看能量数字图标（text_energy）正常显示；log 不再有 `Asset not cached: res://DundunDudu\images\...`（反斜杠）。
- [ ] **E2 选角 bg**：选角界面不再报 `char_select_bg_dundundudu-*.tscn` 加载 [ERROR]（现继承占位 ironclad/silent bg）。

---

### 墩墩可加卡 Entry 速查（`card DUNDUNDUDU-<SLUG>`）
猛击 DUNDUN_STRIKE · 防御 DUNDUN_DEFEND · 测试卡 SULK_TEST_CARD
抓握点 GRIP · 滋补汤 NOURISHING_SOUP · 稳扎一拳 STEADY_PUNCH · 缠斗 GRAPPLE · 储备 RESERVE · 养生 WELLNESS · 防守垫步 DEFENSIVE_STEP · 对冲 HEDGE · 风控 RISK_CONTROL · 连续防守 SUSTAINED_DEFENSE · 二段蹬腿 DOUBLE_KICK · 满汉全席 GRAND_FEAST
