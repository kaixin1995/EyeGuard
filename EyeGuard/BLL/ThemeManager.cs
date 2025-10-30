using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EyeGuard.BLL
{
    /// <summary>
    /// 主题管理器,只管理提示页主题,设置页和关于页使用默认样式
    /// </summary>
    public static class ThemeManager
    {
        /// <summary>
        /// 应用设置页主题(所有风格使用默认)
        /// </summary>
        public static void ApplySetUpTheme(Window window, Model.widget_style style)
        {
            // 所有风格都使用默认HandyControl主题
        }

        /// <summary>
        /// 应用关于页主题(所有风格使用默认)
        /// </summary>
        public static void ApplyAboutTheme(Window window, Model.widget_style style)
        {
            // 所有风格都使用默认HandyControl主题
        }

        /// <summary>
        /// 应用提示页主题
        /// </summary>
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

        // 经典风格 - 提示页
        private static void ApplyClassicTips(Border border, TextBlock textBlock)
        {
            border.Background = new SolidColorBrush(Color.FromArgb(200, 0, 0, 0));
            border.BorderBrush = new SolidColorBrush(Colors.White);
            border.BorderThickness = new Thickness(1);
            border.CornerRadius = new CornerRadius(0);
            textBlock.Foreground = new SolidColorBrush(Colors.White);
        }

        // 现代风格 - 提示页(渐变紫蓝)
        private static void ApplyModernTips(Border border, TextBlock textBlock)
        {
            var gradient = new LinearGradientBrush();
            gradient.StartPoint = new Point(0, 0);
            gradient.EndPoint = new Point(1, 0);
            gradient.GradientStops.Add(new GradientStop(Color.FromArgb(240, 106, 17, 203), 0));
            gradient.GradientStops.Add(new GradientStop(Color.FromArgb(240, 37, 117, 252), 1));
            border.Background = gradient;
            border.BorderBrush = new SolidColorBrush(Colors.White);
            border.BorderThickness = new Thickness(2);
            border.CornerRadius = new CornerRadius(20);
            textBlock.Foreground = new SolidColorBrush(Colors.White);
            textBlock.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color = Colors.Black,
                BlurRadius = 8,
                ShadowDepth = 2,
                Opacity = 0.5
            };
        }

        // 科技风格 - 提示页(浅灰+绿色)
        private static void ApplyTechTips(Border border, TextBlock textBlock)
        {
            border.Background = new SolidColorBrush(Color.FromArgb(240, 45, 45, 48));
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(0, 255, 127));
            border.BorderThickness = new Thickness(2);
            border.CornerRadius = new CornerRadius(8);
            textBlock.Foreground = new SolidColorBrush(Color.FromRgb(0, 255, 127));
            textBlock.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color = Color.FromRgb(0, 255, 127),
                BlurRadius = 10,
                ShadowDepth = 0,
                Opacity = 0.6
            };
        }

        // 简洁风格 - 提示页(纯白)
        private static void ApplyMinimalTips(Border border, TextBlock textBlock)
        {
            border.Background = new SolidColorBrush(Colors.White);
            border.BorderBrush = new SolidColorBrush(Color.FromRgb(220, 220, 220));
            border.BorderThickness = new Thickness(1);
            border.CornerRadius = new CornerRadius(15);
            textBlock.Foreground = new SolidColorBrush(Color.FromRgb(60, 60, 60));
            border.Effect = new System.Windows.Media.Effects.DropShadowEffect
            {
                Color = Colors.Gray,
                BlurRadius = 15,
                ShadowDepth = 3,
                Opacity = 0.3
            };
        }
    }
}
