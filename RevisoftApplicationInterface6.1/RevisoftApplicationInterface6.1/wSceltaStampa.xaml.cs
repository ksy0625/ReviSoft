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
  public partial class wSceltaStampa : Window
  {
    public ArrayList StampePossibili = new ArrayList();
    public bool reallychosen = false;

    public wSceltaStampa()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
    }

    public void Load()
    {
      collectorRadiobutton.Children.Clear();

      bool alreadychecked = false;

      foreach (string item in StampePossibili)
      {
        RadioButton rdb = new RadioButton();

        if (!alreadychecked)
        {
          rdb.IsChecked = true;
          alreadychecked = true;
        }

        rdb.Content = item;
        rdb.GroupName = "Stampe";
        rdb.Margin = new Thickness(10, 10, 10, 0);

        collectorRadiobutton.Children.Add(rdb);
      }

      if (StampePossibili.Count == 1)
      {
        reallychosen = true;
      }
    }

    private void buttonApplica_Click(object sender, RoutedEventArgs e)
    {
      reallychosen = true;
      base.Close();
    }
  }
}
