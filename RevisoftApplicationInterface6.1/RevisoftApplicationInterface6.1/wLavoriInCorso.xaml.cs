using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Windows.Interop;


namespace RevisoftApplication
{

  public partial class wLavoriInCorso : Window
  {
    public bool SupportsCancellation;



    public wLavoriInCorso(string messaggio)
    {
      InitializeComponent();
      winBorder.BorderBrush = App._arrBrushes[0];
      this.UpdateLayout();
      textBlockInfo.Text = messaggio;
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      e.Cancel = true;
      this.Hide();
    }

    public new void Close()
    {
      this.Closing -= Window_Closing;
      base.Close();
    }
  }
}
