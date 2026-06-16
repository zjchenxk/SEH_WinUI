# SEH - So Easy 口琴

## 项目概述

这是一个基于 WinUI 3 的桌面应用程序，旨在为口琴爱好者提供乐谱管理和笔记记录功能。

**技术栈**: WinUI 3 + .NET 8.0 + CommunityToolkit.Mvvm + SQLite

## 项目结构

```
SEH/
├── Models/               # 数据模型层 (SQLite ORM)
│   ├── File.cs           # 笔记文件模型 (Windows.Storage)
│   ├── AllFile.cs       # 笔记集合管理
│   ├── Category.cs      # 乐谱类别
│   ├── Score.cs         # 乐谱主表
│   ├── Line.cs          # 乐谱行
│   ├── Measure.cs       # 小节
│   ├── Note.cs          # 音符
│   ├── Beam.cs          # 连尾组合
│   └── BeamNote.cs      # 连尾组合音符关联表
│
├── ViewModels/          # 视图模型层 (MVVM)
│   └── MainViewModel.cs # 示例ViewModel（当前未使用）
│
├── Views/               # 视图层 (XAML)
│   ├── AllNotesPage.xaml/cs   # 笔记列表页
│   └── NotePage.xaml/cs       # 笔记编辑页
│
├── DataServices/        # 数据服务层 (待实现)
│
├── App.xaml/cs         # 应用入口，配置 Serilog
└── MainWindow.xaml/cs  # 主窗口，包含导航Frame
```

## 开发规范

### 命名约定

- **文件夹**: PascalCase (如 `Views`, `ViewModels`)
- **文件**: PascalCase (如 `NotePage.xaml`)
- **类名**: PascalCase (如 `MainViewModel`)
- **方法**: PascalCase (如 `LoadFiles`)
- **属性**: PascalCase (如 `FileName`)
- **私有字段**: _camelCase (如 `_count`)

### MVVM 模式

使用 CommunityToolkit.Mvvm：

```csharp
public partial class MainViewModel : ObservableObject
{
    // 自动生成属性和通知
    [ObservableProperty]
    private int _count = 0;

    // 自动生成 Command
    [RelayCommand]
    private void Increment()
    {
        Count++;
    }
}
```

### XAML 绑定

优先使用 `x:Bind` 编译时绑定（性能更好）：
```xml
<TextBlock Text="{x:Bind ViewModel.Title}" />
```

需要双向绑定时使用 Mode：
```xml
<TextBox Text="{x:Bind model.Text, Mode=TwoWay}" />
```

### SQLite 模型

使用 sqlite-net-pcl 的特性标注：
```csharp
[Table("Note")]
public class Note
{
    [PrimaryKey, Column("id")]
    public string Id { get; set; }

    [Column("pitch")]
    public string Pitch { get; set; }
}
```

## 功能模块

### 笔记模块（已实现）

- 使用 `Windows.Storage.ApplicationData.Current.LocalFolder` 存储
- 文件命名: `notes{timestamp}.txt`
- 页面: `AllNotesPage` (列表) → `NotePage` (编辑)

### 乐谱模块（待实现）

数据模型已完整定义，包含：
- **Score**: 乐谱信息（标题、作曲、调号、拍号、速度）
- **Line** → **Measure** → **Note**: 三层嵌套结构
- **Beam**: 连尾组合（用于八分/十六分音符连写）

## 当前开发状态

- ✅ 笔记列表和编辑功能
- ✅ 页面导航（Frame + 自定义 TitleBar）
- ✅ Serilog 日志配置
- ⏳ 乐谱功能（模型已完成，UI和数据服务待开发）
- ⏳ 数据库集成（SQLite已引入，Repository待实现）

## UI 样式

应用使用放大的字体设置（可访问性），定义在 `App.xaml` 资源中：
- `ContentControlFontSize`: 22
- `BodyFontSize`: 22
- `TitleFontSize`: 42
- 使用 Mica 背景效果

## 依赖包

| 包 | 版本 | 用途 |
|---|------|------|
| Microsoft.WindowsAppSDK | 2.2.0 | WinUI 3 运行时 |
| CommunityToolkit.Mvvm | 8.4.2 | MVVM 框架 |
| sqlite-net-pcl | 1.9.172 | SQLite ORM |
| Serilog | 4.3.1 | 日志记录 |

## 构建运行

```bash
# 恢复 NuGet 包
dotnet restore

# 构建
dotnet build

# 运行
dotnet run
```
