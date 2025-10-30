using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EyeGuard.BLL
{
    /// <summary>
    /// 主题管理器，负责动态切换和应用不同窗口的视觉主题。
    /// 这个类目前主要管理“提示页”的主题，而“设置页”和“关于页”默认使用统一的基础样式。
    /// </summary>
    public static class ThemeManager
    {
        /// <summary>
        /// 为“设置”窗口应用主题。
        /// 注意：当前实现中，所有风格均采用默认的HandyControl主题，具体风格的代码已被注释。
        /// </summary>
        /// <param name="window">需要应用主题的目标窗口。</param>
        /// <param name="style">指定的窗口风格枚举。</param>
        public static void ApplySetUpTheme(Window window, Model.widget_style style)
        {
            // 当前逻辑：所有风格都使用默认的HandyControl主题，以保持界面统一和简洁。

            #region 设置页主题（已注释但内部已彻底修复）
            /*
            // 如果未来需要为不同风格启用独立的设置页主题，可以取消以下代码的注释。
            switch (style)
            {
                case Model.widget_style.现代风格:
                    // 设置现代风格：紫蓝色渐变背景，从左上到右下由 #6A11CB 过渡到 #2575FC，搭配白色文字。
                    var modernGradient = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1)
                    };
                    modernGradient.GradientStops.Add(new GradientStop(Color.FromRgb(106, 17, 203), 0));
                    modernGradient.GradientStops.Add(new GradientStop(Color.FromRgb(37, 117, 252), 1));
                    
                    // 步骤 1: 只设置内容区域的背景，保护标题栏
                    if (window.Content is Panel contentPanel)
                    {
                        contentPanel.Background = modernGradient;
                    }

                    // 步骤 2: [BUG根源修复]
                    // 原代码: window.Dispatcher.BeginInvoke(new System.Action(() => SetModernColors(window))...);
                    // 问题: 以 window 为起点遍历，会污染HandyControl的标题栏模板，导致按钮消失、出现黑线。
                    // 彻底修复: 递归的起点必须是 window.Content，确保样式只应用于我们自己的内容。
                    window.Dispatcher.BeginInvoke(new System.Action(() => SetModernColors(window.Content as DependencyObject)), System.Windows.Threading.DispatcherPriority.Loaded);
                    break;
                    
                case Model.widget_style.科技风格:
                    var techBackground = new SolidColorBrush(Color.FromRgb(45, 45, 48));
                    
                    if (window.Content is Panel contentPanelTech)
                    {
                        contentPanelTech.Background = techBackground;
                    }
                    
                    // [BUG根源修复] 递归起点从 window 改为 window.Content
                    window.Dispatcher.BeginInvoke(new System.Action(() => SetTechColors(window.Content as DependencyObject)), System.Windows.Threading.DispatcherPriority.Loaded);
                    break;
                    
                case Model.widget_style.简洁风格:
                    var minimalBackground = new SolidColorBrush(Colors.White);
                    
                    if (window.Content is Panel contentPanelMinimal)
                    {
                        contentPanelMinimal.Background = minimalBackground;
                    }
                    
                    // [BUG根源修复] 递归起点从 window 改为 window.Content
                    window.Dispatcher.BeginInvoke(new System.Action(() => SetMinimalColors(window.Content as DependencyObject)), System.Windows.Threading.DispatcherPriority.Loaded);
                    break;
            }
            */
            #endregion
        }

        /// <summary>
        /// 为“关于”窗口应用主题。
        /// </summary>
        /// <param name="window">需要应用主题的目标窗口。</param>
        /// <param name="style">指定的窗口风格枚举。</param>
        public static void ApplyAboutTheme(Window window, Model.widget_style style)
        {
            // 根据传入的风格应用不同的主题
            switch (style)
            {
                case Model.widget_style.现代风格:
                    // 设置现代风格：紫蓝色渐变背景 (#6A11CB → #2575FC)，搭配白色文字。
                    var modernGradient = new LinearGradientBrush
                    {
                        StartPoint = new Point(0, 0),
                        EndPoint = new Point(1, 1)
                    };
                    modernGradient.GradientStops.Add(new GradientStop(Color.FromRgb(106, 17, 203), 0));
                    modernGradient.GradientStops.Add(new GradientStop(Color.FromRgb(37, 117, 252), 1));

                    // 步骤 1: 只设置内容区域的背景，保护标题栏
                    if (window.Content is Panel contentPanel)
                    {
                        contentPanel.Background = modernGradient;
                    }
                    window.Background = modernGradient;
                   
                    // 问题: 以 window 为起点遍历，会污染HandyControl的标题栏模板，导致按钮消失、出现黑线。
                    // 彻底修复: 递归的起点必须是 window.Content，确保样式只应用于我们自己的内容。
                    window.Dispatcher.BeginInvoke(new System.Action(() => SetModernColors(window.Content as DependencyObject)), System.Windows.Threading.DispatcherPriority.Loaded);
                    break;

                case Model.widget_style.科技风格:
                    // 设置科技风格：深灰色背景 (#2D2D30)，搭配亮绿色文字 (#00FF7F)。
                    var techBackground = new SolidColorBrush(Color.FromRgb(45, 45, 48));

                    if (window.Content is Panel contentPanelTech)
                    {
                        contentPanelTech.Background = techBackground;
                    }
                    window.Background = techBackground;
                    // 递归起点从 window 改为 window.Content
                    window.Dispatcher.BeginInvoke(new System.Action(() => SetTechColors(window.Content as DependencyObject)), System.Windows.Threading.DispatcherPriority.Loaded);
                    break;

                case Model.widget_style.简洁风格:
                    // 设置简洁风格：纯白色背景，搭配深灰色文字 (#3C3C3C)。
                    var minimalBackground = new SolidColorBrush(Colors.White);

                    if (window.Content is Panel contentPanelMinimal)
                    {
                        contentPanelMinimal.Background = minimalBackground;
                    }
                    window.Background = minimalBackground;
                    // 递归起点从 window 改为 window.Content
                    window.Dispatcher.BeginInvoke(new System.Action(() => SetMinimalColors(window.Content as DependencyObject)), System.Windows.Threading.DispatcherPriority.Loaded);
                    break;
            }
        }

        /// <summary>
        /// 根据指定的风格，为“提示页”的UI元素（边框和文本块）应用主题。
        /// </summary>
        /// <param name="border">提示页的背景边框控件。</param>
        /// <param name="textBlock">提示页的文本显示控件。</param>
        /// <param name="style">要应用的窗口风格枚举。</param>
        public static void ApplyTipsTheme(Border border, TextBlock textBlock, Model.widget_style style)
        {
            switch (style)
            {
                case Model.widget_style.经典风格:
                    ApplyClassicTips(border, textBlock);
                    break;
                case Model.widget_style.现代风格:
                    ApplyModernTips(border, textBlock);
                    break;
                case Model.widget_style.科技风格:
                    ApplyTechTips(border, textBlock);
                    break;
                case Model.widget_style.简洁风格:
                    ApplyMinimalTips(border, textBlock);
                    break;
            }
        }

        /// <summary>
        /// 应用经典风格到提示页。
        /// </summary>
        /// <param name="border">提示页的背景边框。</param>
        /// <param name="textBlock">提示页的文本块。</param>
        private static void ApplyClassicTips(Border border, TextBlock textBlock)
        {
            // 背景: 半透明黑色 (Alpha=200)，提供良好的内容可读性。
            border.Background = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0));
            // 边框: 1像素宽的白色实线边框。
            border.BorderBrush = new SolidColorBrush(Colors.White);
            border.BorderThickness = new Thickness(1);
            // 圆角: 0，呈现为直角矩形。
            border.CornerRadius = new CornerRadius(0);
            // 文字: 纯白色，与深色背景形成鲜明对比。
            textBlock.Foreground = new SolidColorBrush(Colors.White);
        }

        /// <summary>
        /// 应用现代风格到提示页。
        /// </summary>
        /// <param name="border">提示页的背景边框。</param>
        /// <param name="textBlock">提示页的文本块。</param>
        private static void ApplyModernTips(Border border, TextBlock textBlock)
        {
            // 背景: 紫蓝色水平渐变 (#6A11CB → #2575FC)，透明度为240，富有动感。
            var gradient = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0),
                EndPoint = new Point(1, 0)
            };
            gradient.GradientStops.Add(new GradientStop(Color.FromArgb(240, 106, 17, 203), 0));
            gradient.GradientStops.Add(new GradientStop(Color.FromArgb(240, 37, 117, 252), 1));
            border.Background = gradient;
            // 边框: 2像素宽的白色实线边框。
            border.BorderBrush = new SolidColorBrush(Colors.White);
            border.BorderThickness = new Thickness(2);
            // 圆角: 20px，使外观更柔和。
            border.CornerRadius = new CornerRadius(20);
            // 文字: 纯白色。
            textBlock.Foreground = new SolidColorBrush(Colors.White);
            // 文字效果: 添加黑色投影，增加立体感和可读性。
            textBlock.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color = Colors.Black,       // 阴影颜色
                BlurRadius = 8,             // 模糊半径
                ShadowDepth = 2,            // 阴影深度
                Opacity = 0.5               // 不透明度
            };
        }

        /// <summary>
        /// 应用科技风格到提示页。
        /// </summary>
        /// <param name="border">提示页的背景边框。</param>
        /// <param name="textBlock">提示页的文本块。</param>
        private static void ApplyTechTips(Border border, TextBlock textBlock)
        {
            // 背景: 半透明的深灰色 (#2D2D30, Alpha=240)。
            border.Background = new SolidColorBrush(Color.FromArgb(240, 45, 45, 48));
            // 边框: 2像素宽的亮绿色 (#00FF7F) 实线边框，突出科技感。
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 127));
            border.BorderThickness = new Thickness(2);
            // 圆角: 8px，带有现代感的轻微圆角。
            border.CornerRadius = new CornerRadius(8);
            // 文字: 亮绿色 (#00FF7F)。
            textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 127));
            // 文字效果: 添加同色的光晕效果，模拟霓虹灯或HUD显示效果。
            textBlock.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color = Color.FromRgb(0, 255, 127), // 光晕颜色
                BlurRadius = 10,                    // 模糊半径
                ShadowDepth = 0,                    // 深度为0，实现均匀光晕
                Opacity = 0.6                       // 不透明度
            };
        }

        /// <summary>
        /// 应用简洁风格到提示页。
        /// </summary>
        /// <param name="border">提示页的背景边框。</param>
        /// <param name="textBlock">提示页的文本块。</param>
        private static void ApplyMinimalTips(Border border, TextBlock textBlock)
        {
            // 背景: 纯白色，干净、明亮。
            border.Background = new SolidColorBrush(Colors.White);
            // 边框: 1像素宽的浅灰色 (#DCDCDC) 边框，提供细微的轮廓。
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(220, 220, 220));
            border.BorderThickness = new Thickness(1);
            // 圆角: 15px，较大的圆角半径，显得更加友好和现代。
            border.CornerRadius = new CornerRadius(15);
            // 文字: 深灰色 (#3C3C3C)，确保在白色背景上的高可读性。
            textBlock.Foreground = new SolidColorBrush(Color.FromRgb(60, 60, 60));
            // 边框效果: 添加柔和的灰色投影，使元素看起来悬浮在界面之上，增加层次感。
            border.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color = Colors.Gray,        // 阴影颜色
                BlurRadius = 15,            // 模糊半径
                ShadowDepth = 3,            // 阴影深度
                Opacity = 0.3               // 不透明度
            };
        }

        #region 主题辅助方法

        // 以下是一组辅助方法，用于递归遍历窗口的可视化树，并为不同类型的控件应用特定的颜色方案。
        // 这些方法目前未被调用，但为未来实现更精细化的“设置页”和“关于页”主题提供了基础框架。

        /// <summary>
        /// 为现代风格递归设置子控件的颜色。
        /// </summary>
        /// <param name="parent">要遍历的父级依赖对象。</param>
        private static void SetModernColors(DependencyObject parent)
        {
            if (parent == null) return;
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // 文本：设置为白色
                if (child is TextBlock tb)
                    tb.Foreground = new SolidColorBrush(Colors.White);
                // 按钮：设置白色前景，并定义鼠标悬停样式
                else if (child is Button btn)
                {
                    btn.Foreground = new SolidColorBrush(Colors.White);
                    var style = new Style(typeof(Button));
                    style.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.White)));
                    var trigger = new Trigger { Property = Button.IsMouseOverProperty, Value = true };
                    trigger.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Colors.White)));
                    style.Triggers.Add(trigger);
                    btn.Style = style;
                }
                // 其他控件：统一设置白色前景
                else if (child is Control control)
                    control.Foreground = new SolidColorBrush(Colors.White);
                // 边框：背景设为半透明白色，边框线设为半透明白色
                else if (child is Border border)
                {
                    if (border.Background != null && border.Background is SolidColorBrush)
                        border.Background = new SolidColorBrush(Color.FromArgb(40, 255, 255, 255));
                    if (border.BorderBrush != null)
                        border.BorderBrush = new SolidColorBrush(Color.FromArgb(100, 255, 255, 255));
                }
                // 递归处理子控件
                SetModernColors(child);
            }
        }

        /// <summary>
        /// 为科技风格递归设置子控件的颜色。
        /// </summary>
        /// <param name="parent">要遍历的父级依赖对象。</param>
        private static void SetTechColors(DependencyObject parent)
        {
            if (parent == null) return;
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // 文本：设置为亮绿色
                if (child is TextBlock tb)
                    tb.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 127));
                // 按钮：设置绿色前景和深灰色背景，并定义悬停样式
                else if (child is Button btn)
                {
                    btn.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 127));
                    btn.Background = new SolidColorBrush(Color.FromRgb(55, 55, 58));
                    var style = new Style(typeof(Button));
                    style.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Color.FromRgb(0, 255, 127))));
                    style.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Color.FromRgb(55, 55, 58))));
                    var trigger = new Trigger { Property = Button.IsMouseOverProperty, Value = true };
                    trigger.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Color.FromRgb(0, 255, 127))));
                    trigger.Setters.Add(new Setter(Control.BackgroundProperty, new SolidColorBrush(Color.FromRgb(75, 75, 78)))); // 悬停时背景变亮
                    style.Triggers.Add(trigger);
                    btn.Style = style;
                }
                // 开关按钮：前景和背景均设为绿色
                else if (child is System.Windows.Controls.Primitives.ToggleButton toggle)
                {
                    toggle.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 127));
                    toggle.Background = new SolidColorBrush(Color.FromRgb(0, 255, 127));
                }
                // 滑块：前景（即填充部分）设为绿色
                else if (child is Slider slider)
                    slider.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 127));
                // 下拉框：设置绿色文字、边框和深灰色背景
                else if (child is ComboBox combo)
                {
                    combo.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 127));
                    combo.Background = new SolidColorBrush(Color.FromRgb(55, 55, 58));
                    combo.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 127));
                }
                // 其他控件：统一设置绿色前景
                else if (child is Control control)
                    control.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 127));
                // 边框：背景设为深灰色，边框线设为绿色
                else if (child is Border border)
                {
                    if (border.Background != null && border.Background is SolidColorBrush)
                        border.Background = new SolidColorBrush(Color.FromRgb(55, 55, 58));
                    if (border.BorderBrush != null)
                        border.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 127));
                }
                // 递归处理子控件
                SetTechColors(child);
            }
        }

        /// <summary>
        /// 为简洁风格递归设置子控件的颜色。
        /// </summary>
        /// <param name="parent">要遍历的父级依赖对象。</param>
        private static void SetMinimalColors(DependencyObject parent)
        {
            if (parent == null) return;
            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // 文本：设置为深灰色
                if (child is TextBlock tb)
                    tb.Foreground = new SolidColorBrush(Color.FromRgb(60, 60, 60));
                // 按钮：设置深灰色前景，悬停时不变
                else if (child is Button btn)
                {
                    btn.Foreground = new SolidColorBrush(Color.FromRgb(60, 60, 60));
                    var style = new Style(typeof(Button));
                    style.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Color.FromRgb(60, 60, 60))));
                    var trigger = new Trigger { Property = Button.IsMouseOverProperty, Value = true };
                    trigger.Setters.Add(new Setter(Control.ForegroundProperty, new SolidColorBrush(Color.FromRgb(60, 60, 60))));
                    style.Triggers.Add(trigger);
                    btn.Style = style;
                }
                // 其他控件：统一设置深灰色前景
                else if (child is Control control)
                    control.Foreground = new SolidColorBrush(Color.FromRgb(60, 60, 60));
                // 边框：背景设为非常浅的灰色，边框线设为浅灰色
                else if (child is Border border)
                {
                    if (border.Background != null && border.Background is SolidColorBrush)
                        border.Background = new SolidColorBrush(Color.FromRgb(250, 250, 250)); // #FAFAFA
                    if (border.BorderBrush != null)
                        border.BorderBrush = new SolidColorBrush(Color.FromRgb(220, 220, 220)); // #DCDCDC
                }
                // 递归处理子控件
                SetMinimalColors(child);
            }
        }

        #endregion
    }
}