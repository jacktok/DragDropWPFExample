using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Microsoft.Win32;
using Calendar = System.Globalization.Calendar;

namespace BoxManage
{
    /// <summary>
    /// Interaction logic for Window2.xaml
    /// </summary>
    public partial class Window2 : Window
    {
        public Window2()
        {
            InitializeComponent();
        }

        private Grid _selectedGrid;

        private Grid selectedGrid
        {
            get => _selectedGrid;
            set
            {
                RemoveSelectedStyle(_selectedGrid);
                _selectedGrid = value;
                SetSelectedStyle(_selectedGrid);
            }
        }
        private IInputElement objectElement;
        private double _x;

        private double x
        {
            get => _x;
            set
            {
                _x = value;
                T1.Text = _x
                    .ToString(CultureInfo.InvariantCulture);
            }
        }

        private double _y;

        private double y
        {
            get => _y;
            set
            {
                _y = value;
                T2.Text = _y.ToString(CultureInfo.InstalledUICulture);
            }
        }

        private bool? isDrag = null;


        private void OtherMouseMove(object sender, MouseEventArgs e)
        {
            T3.Text = Tag1.TransformToAncestor(Root).Transform(new Point()).ToString();

            if (e.LeftButton == MouseButtonState.Pressed && isDrag != null && selectedGrid != null)
            {
                var m = Mouse.GetPosition((IInputElement) sender);

                if (isDrag.Value)
                {
                    Cursor = Cursors.SizeAll;
                    Drag(m);
                }
                else
                {
                    Resize(m);
                }
            }
        }

        private void Drag(Point m)
        {
            var transformGroup = ((UIElement) selectedGrid).RenderTransform as TransformGroup;
            if (transformGroup == null)
                return;

            var translateTransforms = from transform in transformGroup.Children
                where transform.GetType().Name == "TranslateTransform"
                select transform;

            foreach (var transform in translateTransforms)
            {
                var tt = (TranslateTransform) transform;
                tt.X += m.X - x;
                tt.Y += m.Y - y;
            }


            var scaleTransform = from transform in transformGroup.Children
                where transform.GetType().Name == "ScaleTransform"
                select transform;

            foreach (var transform in scaleTransform)
            {
                var st = (ScaleTransform) transform;
                st.ScaleX = .8;
                st.ScaleY = .8;
            }

            Console.WriteLine($@"{m.X}:{x} | {m.Y}:{y}");

            // Update the beginning position of the mouse
            x = m.X;
            y = m.Y;
        }

        private int cx = 4;
        private int cy = 2;

        private void DropItem()
        {
            if (selectedGrid == null) return;
            /*var point = selectedGrid.TransformToAncestor(Root).Transform(new Point());*/
            var point = Mouse.GetPosition(Root);
            Console.WriteLine($"p: {point}");
            var w = Root.ActualWidth / cx;
            var h = Root.ActualHeight / cy;
            var col = (int) Math.Ceiling(point.X / w) - 1;
            col = col < 0 ? 0 : col;
            col = col > (cx - 1) ? cx - 1 : col;
            var row = (int) Math.Ceiling(point.Y / h) - 1;
            row = row < 0 ? 0 : row;
            row = row > (cy - 1) ? cy - 1 : row;

            selectedGrid.Width = w;
            selectedGrid.Height = h;
            Canvas.SetLeft(selectedGrid, col * w);
            Canvas.SetTop(selectedGrid, row * h);
            selectedGrid.RenderTransform = new TransformGroup()
            {
                Children = new TransformCollection(new List<Transform>()
                    {new ScaleTransform(), new TranslateTransform()})
            };
        }


        private void DropResize()
        {
            if (selectedGrid == null) return;
            var point = Mouse.GetPosition(Root);
            Console.WriteLine($"p: {point}");
            var w = Root.ActualWidth / cx;
            var h = Root.ActualHeight / cy;
            w = (int) Math.Ceiling(selectedGrid.ActualWidth / w) * w;
            //col = col < 0 ? 0 : col;
            //col = col > (cx - 1) ? cx - 1 : col;
            h = (int) Math.Ceiling(selectedGrid.ActualHeight / h) * h;
            //row = row < 0 ? 0 : row;
            //row = row > (cy - 1) ? cy - 1 : row;
            selectedGrid.Width = w;
            selectedGrid.Height = h;
        }

        private void Resize(Point m)
        {
            if (Cursor == Cursors.SizeNWSE && selectedGrid != null)
            {
                var difX = m.X - x;
                var preWidth = selectedGrid.ActualWidth + difX;
                if (preWidth > selectedGrid.MinWidth && preWidth < selectedGrid.MaxHeight)
                {
                    selectedGrid.Width = preWidth;
                }

                var difY = m.Y - y;
                var preHeight = selectedGrid.ActualHeight + difY;
                if (preHeight > selectedGrid.MinHeight && preHeight < selectedGrid.MaxHeight)
                {
                    selectedGrid.Height = preHeight;
                }

                Console.WriteLine($@"{m.X}:{x} | {m.Y}:{y}");

                x = m.X;
                y = m.Y;
            }
        }


        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            var m = Mouse.GetPosition(Root);
            x = m.X;
            y = m.Y;

            objectElement?.CaptureMouse();

            //if (!this.current.IsStretching)
            //    this.current.IsDragging = true;
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (isDrag == true)
            {
                DropItem();
            }
            else if (isDrag == false)
            {
                DropResize();
            }

            objectElement?.ReleaseMouseCapture();

            objectElement = null;
            this.Cursor = Cursors.Arrow;
            isDrag = null;
        }

        private void SizeNWSE_OnMouseEnter(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.SizeNWSE;
        }

        private void Size_OnMouseLeave(object sender, MouseEventArgs e)
        {
            Cursor = Cursors.Arrow;
        }

        private void Size_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedGrid = ((Grid) ((FrameworkElement) ((FrameworkElement) ((FrameworkElement) sender).Parent).Parent)
                .Parent);
            isDrag = false;
            objectElement = (IInputElement) sender;
            var m = Mouse.GetPosition(selectedGrid);
        }

        private int c = 0;

        private void Content_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedGrid = (Grid) ((FrameworkElement) sender).Parent;
            isDrag = true;
            objectElement = (IInputElement) sender;
            var m = Mouse.GetPosition(selectedGrid);
        }

        private void SetSelectedStyle(Panel selected)
        {
            if (selected?.Children == null) return;
            foreach (UIElement selectedChild in selected.Children)
            {
                if (selectedChild is Border border)
                {
                    border.Visibility = Visibility.Visible;
                }
            }
        }

        private void RemoveSelectedStyle(Panel selected)
        {
            if (selected?.Children == null) return;
            foreach (UIElement selectedChild in selected.Children)
            {
                if (selectedChild is Border border)
                {
                    border.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void Background_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedGrid = null;
        }
    }
}