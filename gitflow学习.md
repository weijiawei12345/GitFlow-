分支名称	分支说明
Production	生产分支，即 Master分支。只能从其他分支合并，不能直接修改
Release	发布分支，基于 Develop 分支创建，待发布完成后合并到 Develop 和 Production 分支去
Develop	主开发分支，包含所有要发布到下一个 Release 的代码，该分支主要合并其他分支内容
Feature	新功能分支，基于 Develop 分支创建，开发新功能，待开发完毕 合并至 Develop 分支
Hotfix	修复分支，基于 Production 分支创建，待修复完成后合并到 Develop 和 Production 分支去，同时在 Master 上打一个tag

主分支：master分支、develop分支；
辅助分支：feature分支、release分支、hotfix分支

主要分支（Master）
master分支只存放历史发布(release)版本的源代码。即用于存放对外发布的版本，任何时候在这个分支获取到的都是稳定的已发布的版本。各个版本通过tag来标记。上图里的v0.1和v0.2就是tag。
任何人不允许在主要分支上进行代码的直接提交，只接受其他分支的合入。原则上主要分支上的代码必须是合并自经过多轮测试及已经发布一段时间且线上稳定的预发分支。

开发分支（Develop）
开发分支接受其他辅助分支的合入，最常见的就是功能分支，开发一个新功能时拉取新的功能分支，开发完成后再并入开发分支。需要注意的是，合入开发的分支必须保证功能完整，不影响开发分支的正常运行。 
develop分支则用来整合各个feature分支。开发中的版本的源代码存放在这里。即用于日常开发，存放最新的开发版。

功能分支（Feature）
功能分支只能拉取自开发分支，用于开发即将发布版本或未来版本的新功能或者探索新功能。该分支通常存在于开发人员的本地代码库而不要求提交到远程代码库上。
开发完成后要么合并回开发分支，要么因为新功能的尝试不如人意而直接丢弃。
每一个特性(feature)都必须在自己的分支里开发，feature分支派生自develop分支。
feature分支只存在于开发者本地，不能被提交到远程库。当feature开发完毕后，要合并回develop分支。feature分支永远不会和master分支打交道。

预发分支（Release）
该分支专为测试—发布新的版本而开辟，允许做小量级的Bug修复和准备发布版本的元数据信息（版本号、编译时间等）。
预发分支需要提交到服务器上，交由测试工程师进行测试，并由开发工程师修复Bug。同时根据该分支的特性我们可以部署自动化测试以及生产环境代码的自动化更新和部署。
预发分支只能拉取自开发分支，合并回开发分支和主要分支。
release分支不是一个放正式发布产品的分支，你可以将它理解为“待发布”分支。
我们用这个分支干所有和发布有关的事情，比如：
把这个分支打包给测试人员测试
在这个分支里修复bug
编写发布文档
所以，在这个分支里面绝对不会添加新的特性。
当和发布相关的工作都完成后，release分支合并回develop和master分支。

热修复分支（Hotfix）
一个项目发布后或多或少肯定会有一些bug存在，而bug的修复工作并不适合在develop上做，这是因为
develop分支上包含还未验证过的feature
用户未必需要develop上的feature
develop还不能马上发布，而客户急需这个bug的修复。
这时就需要新建hotfix分支，hotfix分支派生自master分支，仅仅用于修复bug，当bug修复完毕后，马上回归到master分支，然后发布一个新版本。
同时hotfix也要合并回develop分支，这样develop分支就能享受到bug修复的好处了。
当生产环境的代码（主要分支上代码）遇到严重到必须立即修复的缺陷时，就需要从主要分支上指定的tag版本（比如1.2）拉取热修复分支进行代码的紧急修复，并附上版本号（比如1.2.1）。这样做的好处是不会打断正在进行的开发分支的开发工作，能够让团队中负责功能开发的人与负责代码修复的人并行、独立的开展工作。

太棒了！你成功了！🎉 从最后几行输出可以看到，你的 `gitflow学习.md` 文件已经顺利推送到 GitHub 仓库了。

根据你的要求，我把这次完整操作中所有用到的 Git 命令提取出来，并整理成一份清晰的学习流程，特别标注了中间遇到的分支名称不匹配问题及解决方法，方便你复习和制作学习笔记。

---

### 📚 Git 命令学习流程（完整版）

#### 阶段一：初始化本地仓库
```bash
# 1. 在项目文件夹中打开 Git Bash
# 2. 初始化空的 Git 仓库（创建 .git 文件夹）
git init
```
> 执行后，终端提示符会显示 `(master)` 或 `(main)`，表示当前分支名。

#### 阶段二：链接远程仓库
```bash
# 3. 添加远程仓库地址（origin 是默认远程仓库名）
git remote add origin https://github.com/weijiawei12345/GitFlow-.git
```

#### 阶段三：添加并提交文件到本地仓库
```bash
# 4. 将指定文件添加到暂存区
git add "gitflow学习.md"

# 5. 将暂存区内容提交到本地仓库（-m 后面是提交说明）
git commit -m "初始化：添加 gitflow 学习笔记"
```

#### 阶段四：推送到远程仓库（关键！）
```bash
# 6. 第一次推送：将本地 master 分支推送到远程，并建立跟踪关系
git push -u origin master
```

---

### ⚠️ 重点：分支名称不匹配问题解析

| 你的操作 | 报错信息 | 原因分析 | 正确做法 |
|---|---|---|---|
| `git push -u origin main` | `error: src refspec main does not match any` | 本地分支是 `master`，但命令中写了 `main`，两者不匹配 | 先查看本地分支名：`git branch`，然后用实际分支名替换命令中的分支名 |
| `git push -u origin master` | 成功！ | 本地分支名 `master` 与命令中的分支名一致 | ✅ |

**核心原则：** 推送命令中的分支名 **必须** 与本地当前分支名完全一致。

---

### 🧠 两个实用命令（以备后用）

```bash
# 查看本地所有分支（当前分支前会有 * 号）
git branch

# 重命名当前分支（例如将 master 改为 main）
git branch -m master main   # 然后推送时用 git push -u origin main
```

---

### 📝 你的操作日志对照表

| 你的实际命令 | 执行结果 | 关键知识点 |
|---|---|---|
| `git init` | 初始化成功 | 创建 `.git` 文件夹，开始版本控制 |
| `git remote add origin ...` | 成功 | 设置远程仓库地址 |
| `git add "gitflow学习.md"` | 成功 | 将文件加入暂存区 |
| `git commit -m "..."` | 成功（提交 ID: `db53ba3`） | 保存快照到本地仓库 |
| `git push -u origin main` | ❌ 失败 | 分支名不匹配（本地是 master） |
| `git push -u origin master` | ✅ 成功 | 分支名匹配，推送成功 |

---

### 🎯 总结流程图

```mermaid
flowchart LR
    A[git init] --> B[git remote add origin ...]
    B --> C[git add 文件名]
    C --> D[git commit -m "说明"]
    D --> E{检查当前分支名<br>git branch}
    E -->|分支是 master| F[git push -u origin master]
    E -->|分支是 main| G[git push -u origin main]
    F --> H[✅ 推送成功]
    G --> H
```

现在你的仓库已经建立好了，以后每次更新笔记，只需要重复 `git add` → `git commit` → `git push` 这三个步骤（不需要再 `init` 和 `remote add` 了）。祝你 GitFlow 学习顺利！😊

## 一、仓库初始化与远程连接
git init                                                  # 初始化本地仓库
git remote add origin https://github.com/weijiawei12345/GitFlow-.git   # 连接远程仓库

## 二、首次提交与推送（master 分支）
git add "gitflow学习.md"                                  # 添加文件到暂存区
git commit -m "初始化：添加 gitflow 学习笔记"              # 提交到本地仓库
git push -u origin master                                 # 首次推送 master 分支（-u 建立跟踪）

## 三、创建并推送 develop 分支
git branch develop                                        # 创建 develop 分支（基于当前 HEAD）
git push -u origin develop                                # 推送 develop 分支到远程

## 四、获取远程最新信息
git fetch origin                                          # 拉取远程所有分支的最新状态

## 五、切换分支
git checkout develop                                      # 切换到 develop 分支
git checkout master                                       # 切换到 master 分支
git checkout -b some-feature develop                      # 创建并切换到新分支（基于 develop）

## 六、查看状态与历史
git status                                                # 查看当前工作区状态
git branch                                                # 查看本地所有分支
git branch -a                                             # 查看所有分支（含远程）
git log --oneline --graph --all                           # 查看分支图谱（简洁版）
git ls -la                                                # 查看目录文件（Shell 命令）

## 七、功能分支完整流程
git add .                                                 # 添加所有修改到暂存区
git commit -m "feat: 更新学习gitflow笔记并添加按钮流程分析"  # 提交
git push -u origin some-feature                           # 首次推送功能分支
git pull origin develop                                   # 拉取 develop 最新代码（保持同步）
git checkout develop                                      # 切回 develop
git merge some-feature                                    # 合并功能分支（Fast-forward）
git push                                                  # 推送 develop 到远程
git branch -d some-feature                                # 删除本地功能分支

## 八、发布分支完整流程
git checkout -b release-0.1 develop                       # 创建发布分支
git push -u origin release-0.1                            # 推送发布分支
git checkout master                                       # 切换到 master
git merge release-0.1                                     # 合并发布分支到 master
git push                                                  # 推送 master
git checkout develop                                      # 切回 develop
git merge release-0.1                                     # 合并发布分支到 develop（同步修复）
git push                                                  # 推送 develop
git branch -d release-0.1                                 # 删除本地发布分支

## 九、打版本标签
git tag -a 0.1 -m "Initial public release" master         # 在 master 上打带注释标签
git push --tags                                           # 推送所有标签到远程
git push origin v0.1.1                                    # 推送指定标签

## 十、热修复分支完整流程
git checkout -b issue-#001 master                         # 创建热修复分支（基于 master）
git add hotfix.md                                         # 添加修复文件
git commit -m "fix: 添加 hotfix 记录文档"                  # 提交修复
git push -u origin issue-#001                             # 推送热修复分支
git checkout master                                       # 切回 master
git pull origin master                                    # 拉取最新 master（保持同步）
git merge issue-#001                                      # 合并热修复分支到 master
git tag -a v0.1.1 -m "热修复 issue-#001"                   # 打补丁版本标签
git push origin master                                    # 推送 master
git push origin v0.1.1                                    # 推送标签
git checkout develop                                      # 切回 develop
git merge issue-#001                                      # 合并热修复到 develop（同步）
git push origin develop                                   # 推送 develop
git branch -d issue-#001                                  # 删除本地热修复分支