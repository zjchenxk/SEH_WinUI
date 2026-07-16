using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;

namespace SEH.Controls
{
    public class MultiSelectComboBox : Control
    {
        private ListView? _listView;
        private TextBlock? _textBlock;
        private Popup? _popup;
        private Border? _rootBorder;
        private bool _isUpdatingSelection; //防止循环更新

        public MultiSelectComboBox()
        {
            DefaultStyleKey = typeof(MultiSelectComboBox);
        }

        #region 依赖属性

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IList), typeof(MultiSelectComboBox), new PropertyMetadata(null));

        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(IList), typeof(MultiSelectComboBox), new PropertyMetadata(null, OnSelectedItemsChanged));

        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(MultiSelectComboBox), new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty DelimiterProperty =
            DependencyProperty.Register("Delimiter", typeof(string), typeof(MultiSelectComboBox), new PropertyMetadata(", "));

        public static readonly DependencyProperty PlaceholderTextProperty =
            DependencyProperty.Register("PlaceholderText", typeof(string), typeof(MultiSelectComboBox), new PropertyMetadata("请选择..."));

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(object), typeof(MultiSelectComboBox), new PropertyMetadata(null));

        public IList ItemsSource
        {
            get => (IList)GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public IList SelectedItems
        {
            get => (IList)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }

        public string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }

        public string Delimiter
        {
            get => (string)GetValue(DelimiterProperty);
            set => SetValue(DelimiterProperty, value);
        }

        public string PlaceholderText
        {
            get => (string)GetValue(PlaceholderTextProperty);
            set => SetValue(PlaceholderTextProperty, value);
        }

        public object Header
        {
            get => (object)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        #endregion

        #region 重写方法

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            //获取模板部件
            _rootBorder = GetTemplateChild("RootBorder") as Border;
            _listView = GetTemplateChild("MultiSelectListView") as ListView;
            _popup = GetTemplateChild("Popup") as Popup;
            _textBlock = GetTemplateChild("TextBlock") as TextBlock;

            if (_rootBorder != null)
            {
                _rootBorder.PointerPressed += (s, e) => TogglePopup();
            }

            if (_listView != null)
            {
                _listView.SelectionChanged += OnListViewSelectionChanged;
            }

            //监听外部 SelectedItems 集合的变化
            if (SelectedItems is INotifyCollectionChanged observable)
            {
                observable.CollectionChanged -= SelectedItems_CollectionChanged; //防止重复订阅
                observable.CollectionChanged += SelectedItems_CollectionChanged;
            }

            SyncSelectionToListView();
            UpdateText();
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as MultiSelectComboBox;
            if (control == null) return;

            //移除旧集合的监听
            if (e.OldValue is INotifyCollectionChanged oldObservable)
            {
                oldObservable.CollectionChanged -= control.SelectedItems_CollectionChanged;
            }

            //添加新集合的监听
            if (e.NewValue is INotifyCollectionChanged newObservable)
            {
                newObservable.CollectionChanged += control.SelectedItems_CollectionChanged;
            }

            control.SyncSelectionToListView();
            control.UpdateText();
        }

        #endregion

        #region 私方法

        private void TogglePopup()
        {
            if (_popup != null)
            {
                _popup.IsOpen = !_popup.IsOpen;
            }
        }

        private void OnListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isUpdatingSelection) return;

            if (SelectedItems == null) return;

            _isUpdatingSelection = true;
            try
            {
                //同步 ListView 的选中项到 SelectedItems 集合
                foreach (var item in e.RemovedItems)
                {
                    if (SelectedItems.Contains(item))
                    {
                        SelectedItems.Remove(item);
                    }
                }

                foreach (var item in e.AddedItems)
                {
                    if (!SelectedItems.Contains(item))
                    {
                        SelectedItems.Add(item);
                    }
                }

                UpdateText();
            }
            finally
            {
                _isUpdatingSelection = false;
            }
        }

        private void SelectedItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (_isUpdatingSelection) return;

            SyncSelectionToListView();
            UpdateText();
        }

        private void SyncSelectionToListView()
        {
            if (_listView == null || SelectedItems == null) return;

            _isUpdatingSelection = true;
            try
            {
                _listView.SelectedItems.Clear();
                foreach (var item in SelectedItems)
                {
                    _listView.SelectedItems.Add(item);
                }
            }
            finally
            {
                _isUpdatingSelection = false;
            }
        }

        private void UpdateText()
        {
            if (_textBlock == null) return;

            if (SelectedItems == null || SelectedItems.Count == 0)
            {
                _textBlock.Text = PlaceholderText;
                _textBlock.Opacity = 0.5; //占位符样式
            }
            else
            {
                _textBlock.Opacity = 1.0;

                //使用反射或手动拼接显示文本
                var names = SelectedItems.Cast<object>().Select(item =>
                {
                    if (string.IsNullOrEmpty(DisplayMemberPath))
                        return item?.ToString() ?? "";

                    var prop = item.GetType().GetProperty(DisplayMemberPath);
                    return prop?.GetValue(item)?.ToString() ?? "";
                });

                _textBlock.Text = string.Join(Delimiter, names);
            }
        }

        #endregion
    }
}
