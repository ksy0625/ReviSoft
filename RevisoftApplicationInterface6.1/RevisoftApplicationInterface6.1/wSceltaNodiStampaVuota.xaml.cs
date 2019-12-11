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
  public partial class wSceltaNodiStampaVuota : Window
  {
    public List<string> _lista = null;
    public List<string> listahere = null;

    public bool isok = true;

    public wSceltaNodiStampaVuota(List<string> lista)
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];

      _lista = lista;

      listahere = new List<string>();

      foreach (string item in _lista)
      {
        listahere.Add(item.Split('|')[0]);
      }

      Load();
    }

    private void Load()
    {
      stpList.Children.Clear();

      foreach (string item in _lista)
      {
        StackPanel stp = new StackPanel();
        stp.Orientation = Orientation.Horizontal;

        CheckBox chk = new CheckBox();

        if (listahere.Contains(item.Split('|')[0]))
        {
          chk.IsChecked = false;
        }
        else
        {
          chk.IsChecked = true;
        }
        chk.Tag = item.Split('|')[0];
        chk.Checked += Chk_Checked;
        chk.Unchecked += Chk_Unchecked;

        stp.Children.Add(chk);

        TextBlock txt = new TextBlock();
        txt.Text = item.Split('|')[0] + " - " + item.Split('|')[1];
        txt.Padding = new Thickness(item.Split('|')[0].Split('.').Count() * 10, 0, 0, 0);

        stp.Children.Add(txt);

        stpList.Children.Add(stp);
      }
    }

    private void Chk_Unchecked(object sender, RoutedEventArgs e)
    {
      if (!listahere.Contains(((CheckBox)sender).Tag.ToString()))
      {
        listahere.Add(((CheckBox)sender).Tag.ToString());
      }
    }

    private void Chk_Checked(object sender, RoutedEventArgs e)
    {
      if (listahere.Contains(((CheckBox)sender).Tag.ToString()))
      {
        listahere.Remove(((CheckBox)sender).Tag.ToString());
      }
    }

    private void buttonApplica_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }

    private void btnDeseleziona_Click(object sender, RoutedEventArgs e)
    {
      listahere = new List<string>();
      foreach (string item in _lista)
      {
        listahere.Add(item.Split('|')[0]);
      }
      Load();
    }

    private void btnSeleziona_Click(object sender, RoutedEventArgs e)
    {
      listahere = new List<string>();
      Load();
    }

    private void btnAnnulla_Click(object sender, RoutedEventArgs e)
    {

      isok = false;
      this.Close();
    }
  }
}
