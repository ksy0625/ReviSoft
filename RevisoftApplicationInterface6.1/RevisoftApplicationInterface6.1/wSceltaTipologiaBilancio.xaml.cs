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
  public partial class wSceltaTipologiaBilancio : Window
  {
    public string typechosen = "";

    public wSceltaTipologiaBilancio()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
    }

    public void setFather(string father)
    {
      if (father == "227" || father == "134" || father == "2016134" || father == "166")
      {
        TitoloBilancio.Text = "Bilancio Ordinario";
        rdb2016_Micro.Visibility = Visibility.Collapsed;
      }
      else
      {
        TitoloBilancio.Text = "Bilancio Abbreviato";
        rdb2016_Micro.Visibility = Visibility.Visible;
      }

    }

    private void buttonApplica_Click(object sender, RoutedEventArgs e)
    {
      if (rdbAnte2016.IsChecked == true)
      {
        typechosen = "ante2016";
      }

      if (rdb2016.IsChecked == true)
      {
        typechosen = "2016";
      }

      if (rdb2016_Micro.IsChecked == true)
      {
        typechosen = "Micro";
      }

      if (typechosen == "")
      {
        MessageBox.Show("Selezionare una voce");
        return;
      }

      base.Close();
    }
  }
}
