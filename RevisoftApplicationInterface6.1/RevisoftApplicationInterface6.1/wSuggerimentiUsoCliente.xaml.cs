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
using System.Xml;
using System.IO;
using System.Collections;
using System.ComponentModel;

namespace RevisoftApplication
{
  public partial class wSuggerimentiUsoCliente : Window
  {
    public wSuggerimentiUsoCliente()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }
  }
}
