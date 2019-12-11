using System.Windows;
using System.Windows.Controls;
using System.Collections;

namespace RevisoftApplication
{
  public partial class wSceltaIncarico : Window
  {
    ArrayList alIncarichi = new ArrayList();
    public string IncaricoSelected = "-1";

    public wSceltaIncarico(string IDCliente)
    {
      InitializeComponent();
      labelTitolo.Foreground = App._arrBrushes[0];
      //interfaccia 
      ConfiguraMaschera(IDCliente);
    }

    public void ConfiguraMaschera(string IDCliente)
    {
      stpSessioni1.Children.Clear();

      MasterFile mf = MasterFile.Create();
      alIncarichi = mf.GetIncarichi(IDCliente);

      foreach (Hashtable hthere in alIncarichi)
      {
        RadioButton chkSessione = new RadioButton();
        chkSessione.Tag = hthere["ID"].ToString();
        chkSessione.Content = hthere["DataNomina"].ToString();
        stpSessioni1.Children.Add(chkSessione);
      }
    }

    private void buttonStampa_Click(object sender, RoutedEventArgs e)
    {
      foreach (RadioButton rdb in stpSessioni1.Children)
      {
        if (rdb.IsChecked == true)
        {
          IncaricoSelected = rdb.Tag.ToString();
          base.Close();
        }
      }

      base.Close();
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      IncaricoSelected = "-1";
      base.Close();
    }
  }
}
