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
  public partial class wSceltaTipologiaEsecuzione : Window
  {
    public wSceltaTipologiaEsecuzione()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
    }

    private void buttonApplica_Click(object sender, RoutedEventArgs e)
    {
      if (rdbAlone.IsChecked == false && rdbTeam.IsChecked == false)
      {
        MessageBox.Show("E' necessario elezionare una tipologia", "Mancata selezione", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      //if (rdbAlone.IsChecked == true)
      //   App.AppTipo = App.TipoEsecuzione.StandAlone;

      //if (rdbTeam.IsChecked == true)
      //   App.AppTipo = App.TipoEsecuzione.Team;

      base.Close();
    }

    private void rdbAlone_Checked(object sender, RoutedEventArgs e)
    {
      rdbTeam.IsChecked = false;
    }

    private void rdbTeam_Checked(object sender, RoutedEventArgs e)
    {
      rdbAlone.IsChecked = false;
    }
  }
}
