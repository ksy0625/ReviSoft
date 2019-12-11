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
  public partial class wSceltaTipologiaCaricamentoDati : Window
  {
    public string typechosen = "";

    public wSceltaTipologiaCaricamentoDati(string IDTree)
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];

      if (IDTree == "2")
      {
        rdbBVEA.Content = "Bilancio di verifica Periodo Attuale";
        rdbBVEP.Content = "Bilancio di verifica Periodo Precedente";
      }
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

      if (rdbXBRL.IsChecked == true)
      {
        typechosen = "XBRL";
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

    private void RdbBVEA_Checked(object sender, RoutedEventArgs e)
    {

    }
  }
}
