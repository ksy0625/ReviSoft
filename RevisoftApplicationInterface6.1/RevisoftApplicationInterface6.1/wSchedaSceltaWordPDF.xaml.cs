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
using UserControls;
using System.Media;

namespace RevisoftApplication
{
  public partial class wSchedaSceltaWordPDF : Window
  {
    public int selectedprint = -1;

    public wSchedaSceltaWordPDF()
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      //System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Arrow;
      SystemSounds.Beep.Play();
    }

    private void buttonComandoWord_Click(object sender, RoutedEventArgs e)
    {
      selectedprint = 0;
      //System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
      this.Close();
    }

    private void buttonComandoPDF_Click(object sender, RoutedEventArgs e)
    {
      selectedprint = 1;
      //System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
      this.Close();
    }

    //private void BenvenutoWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    //{
    //    e.Cancel = true;
    //}

    protected override void OnClosing(CancelEventArgs e)
    {
      if (System.Windows.Input.Mouse.OverrideCursor == System.Windows.Input.Cursors.Arrow)
        e.Cancel = true;
    }
  }
}
