using System;
using System.Windows.Controls;

namespace WpfChess.WpfPresentation.View
{
    /// <summary>
    /// Interaction logic for FieldView.xaml
    /// </summary>
    public partial class FieldView : UserControl
    {
        public FieldView()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            for (int i = 0; i < 64; i++)
            {
                var cell = new CellView();
                cell.SetBinding(DataContextProperty, "[" + i + "]");

                this.chessBoard.Children.Add(cell);
            }
        }
    }
}
