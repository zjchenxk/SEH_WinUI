# SEH - So Easy 口琴

<div align="center">

![Windows](https://img.shields.io/badge/Windows-10%2B-blue?logo=windows)
![.NET](https://img.shields.io/badge/.NET-8.0-purple?logo=dotnet)
![WinUI](https://img.shields.io/badge/WINUI-3.0-%238656b6?logo=windows)

**一个为口琴爱好者打造的现代化 Windows 桌面应用**

[功能特性](#功能特性) • [快速开始](#快速开始) • [项目结构](#项目结构) • [开发计划](#开发计划)

</div>

---

## ✨ 功能特性

### 已实现
- 📝 **快速笔记** - 创建、编辑、删除本地笔记
- 🎨 **现代化 UI** - WinUI 3 + Mica 材质效果
- 📱 **响应式导航** - 自定义标题栏与页面导航
- 🔍 **大号字体** - 优化的可访问性体验

### 计划中
- 🎼 **乐谱管理** - 创建、编辑口琴乐谱
- 🎵 **音符编辑** - 支持音高、时值、演奏技法
- 📂 **分类整理** - 按类别管理乐谱
- 🔍 **搜索筛选** - 快速查找乐谱和笔记

---

## 🚀 快速开始

### 环境要求

- Windows 10 版本 1809+ 或 Windows 11
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 17.8+ （推荐）或最新版 VS Code

### 安装运行

```bash
# 克隆仓库
git clone https://github.com/yourusername/SEH.git
cd SEH

# 恢复依赖
dotnet restore

# 运行应用
dotnet run
```

---

## 📁 项目结构

```
SEH/
├── Models/              # 数据模型
│   ├── 笔记相关 (File, AllFile)
│   └── 乐谱相关 (Score, Line, Measure, Note, Beam, Category)
│
├── ViewModels/          # MVVM 视图模型
│   └── MainViewModel.cs
│
├── Views/               # 页面视图
│   ├── AllNotesPage     # 笔记列表
│   └── NotePage         # 笔记编辑
│
├── DataServices/        # 数据服务层
└── App.xaml            # 应用入口
```

---

## 🛠️ 技术栈

| 技术 | 版本 | 说明 |
|------|------|------|
| **WinUI 3** | Windows App SDK 2.2.0 | 现代 Windows 原生 UI 框架 |
| **.NET** | 8.0 | 最新 LTS 版本 |
| **CommunityToolkit.Mvvm** | 8.4.2 | 轻量级 MVVM 框架 |
| **SQLite** | sqlite-net-pcl 1.9.172 | 本地数据存储 |

---

## 🗺️ 开发计划

- [x] 项目初始化与 MVVM 框架搭建
- [x] 笔记功能实现
- [x] 页面导航系统
- [ ] 乐谱数据服务层
- [ ] 乐谱列表页面
- [ ] 乐谱编辑器
- [ ] 音符渲染引擎
- [ ] 分类管理
- [ ] 导出/导入功能

---

## 📄 许可证

[MIT License](LICENSE)

---

<div align="center">

**Built with ❤️ for harmonica players**

</div>
