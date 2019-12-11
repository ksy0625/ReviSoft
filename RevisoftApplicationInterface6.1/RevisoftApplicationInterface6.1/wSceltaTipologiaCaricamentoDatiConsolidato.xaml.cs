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
  public partial class wSceltaTipologiaCaricamentoDatiConsolidato : Window
  {
    public string typechosen = "";

    public wSceltaTipologiaCaricamentoDatiConsolidato()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
    }

    private void buttonApplica_Click(object sender, RoutedEventArgs e)
    {
      if (rdbBVEA.IsChecked == true)
      {
        typechosen = "BVEA";
      }

      if (rdbBVEP.IsChecked == true)
      {
        typechosen = "BVEP";
      }

      if (rdbBV.IsChecked == true)
      {
        typechosen = "BV";
      }

      if (rdbBVCancella.IsChecked == true)
      {
        typechosen = "Cancella";
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
