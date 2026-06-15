using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SEH.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        //[ObservableProperty] 特性会自动生成一个名为 Count 的公共属性和对应的属性变更通知代码，只需要维护私有字段就够了
        [ObservableProperty]
        private int _count = 0;  //私有字段命名为 _count，会自动生成 Count 属性

        //[RelayCommand] 特性会自动生成一个名为 IncrementCommand 的公共命令属性。这个方法会在按钮被点击时执行
        [RelayCommand]
        private void Increment()
        {
            Count++;  // 这里的 Count 是自动生成的属性，赋值时会自动通知界面更新
        }

        [RelayCommand]
        private void Decrement()
        {
            Count--;
        }

        [RelayCommand]
        private void Reset()
        {
            Count = 0;
        }
    }
}
