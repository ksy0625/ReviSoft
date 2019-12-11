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

namespace RevisoftApplication
{

  public partial class wFileImporta : Window
  {
    public wFileImporta()
    {
      InitializeComponent();
      label9.Foreground = App._arrBrushes[0];
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }

  }
}
