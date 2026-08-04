主分支是main和develop，辅助分支是feature，release，hotfix
个人总结：
开发一个新功能的时候，从develop分支新建新的新功能feature分支，开发完毕提交后，需要先对develop分支进行拉取最新状态，再把本地的feacture分支合并本地的develop分支，再推送到远程，并删除本地的feature分支。
发布一个版本的时候，从develop分支新建一个新的本地release分支，发布相关工作完成后，需要先对main分支，develop分支本地都拉取最新状态，再把本地的release分支分别合并到本地main，develop分支，新建对应的版本号tag，再推送到远程，并删除本地release分支。
修复bug的时候，从main分支新建一个新的本地hotfix分支，完成相关修复工作后，需要先对main分支，develop分支本地都拉取最新状态，再把本地的hotfix分支分别合并到本地main，develop分支，新建对应的版本号tag，再推送到远程，并删除本地hotfix分支。

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
git checkout -b some-feature develop                      # 创建并切换到新的功能开发feature分支（基于 develop）

## 六、查看状态与历史
git status                                                # 查看当前工作区状态
git branch                                                # 查看本地所有分支
git branch -a                                             # 查看所有分支（含远程）
git log --oneline --graph --all                           # 查看分支图谱（简洁版）
git ls -la                                                # 查看目录文件（Shell 命令）

## 七、功能分支完整流程
git add .                                                 # 添加所有修改到暂存区
git commit -m "feat: 更新学习gitflow笔记并添加按钮流程分析"  # 提交
git push -u origin some-feature                           # 首次推送功能分支 (辅助分支不是必须推送)
git pull origin develop                                   # 拉取 develop 最新代码（保持同步）
git checkout develop                                      # 切回 develop
git merge some-feature                                    # 合并功能分支（Fast-forward）
git push                                                  # 推送 develop 到远程
git branch -d some-feature                                # 删除本地功能分支

## 八、发布分支完整流程
git checkout -b release-0.1 develop                       # 创建发布分支（从develop分支创建发布分支）
git push -u origin release-0.1                            # 推送发布分支(辅助分支不是必须推送)
git pull origin master                                    # 拉取 master 最新代码（保持同步）
git checkout master                                       # 切换到 master

//命令本身不会产生合并节点，因为缺少 --no-ff 参数。使用SourceTree操作， 在后台帮你加了 --no-ff 执行。
//如果你希望命令行操作也产生同样的节点，就把所有 git merge 改成 git merge --no-ff -m "..."。

git merge release-0.1                                     # 合并发布分支到 master
git push                                                  # 推送 master
git pull origin develop                                   # 拉取 develop 最新代码（保持同步）
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
git push -u origin issue-#001                             # 推送热修复分支(辅助分支不是必须推送)
git checkout master                                       # 切回 master
git pull origin master                                    # 拉取最新 master（保持同步）
git merge issue-#001                                      # 合并热修复分支到 master
git tag -a v0.1.1 -m "热修复 issue-#001"                   # 打补丁版本标签
git push origin master                                    # 推送 master
git push origin v0.1.1                                    # 推送标签
git pull origin develop                                   # 拉取 develop 最新代码（保持同步）
git checkout develop                                      # 切回 develop
git merge issue-#001                                      # 合并热修复到 develop（同步）
git push origin develop                                   # 推送 develop
git branch -d issue-#001                                  # 删除本地热修复分支

使用SourceTree注意事项：
首先查看develop分支有没有其他人提交的记录，如果有需要pull下来的，先切回到develop分支先pull下来。然后切回到feature分支下。进行finish current操作。
12345