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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.Security.Cryptography;
using System.IO;
using System.Xml;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Collections;
using RevisoftApplication;
using System.Data;

namespace UserControls
{
  public partial class ucProspettoIVA : UserControl
  {
    public int id;
    private DataTable dati = null;

    private bool _ReadOnly = false;
 

    public string PrevSession = "";
    public bool normal = true;

    public bool ReadOnly
    {
      set
      {
        _ReadOnly = value;
      }
    }

    public ucProspettoIVA()
    {
      InitializeComponent();
      CultureInfo culture = CultureInfo.CreateSpecificCulture("it-IT");
    }

    private string getvalorefromdt(string nomecampo)
    {
      string ret = "";
      foreach (DataRow dtrow in dati.Rows)
      {
        if (nomecampo == dtrow["nomecampo"].ToString())
          //ret = dtrow["nomecampo"].ToString();
          ret = dtrow["valore"].ToString();
      }
      return ret;
    }



    private void LoadDataSourceInternal()
    {
    
     
        for (int i = 0; i < 13; i++)
            {
              if (normal == false)
              {
                TextBox _txt = (TextBox)this.FindName("txt" + i.ToString());
                TextBox _txtn = (TextBox)this.FindName("txtn" + i.ToString());

                _txt.Text = getvalorefromdt(_txt.Name);
                _txt.BorderThickness = new Thickness(1);
                _txt.Background = Brushes.White;
                _txtn.Text = getvalorefromdt(_txtn.Name);
                _txtn.BorderThickness = new Thickness(1);
                _txtn.Background = Brushes.White;

                TextBox _txttmp = (TextBox)this.FindName("txt" + i.ToString().PadLeft(2, '0') + "02");
                _txttmp.IsReadOnly = false;
                _txttmp.IsTabStop = true;
                _txttmp = (TextBox)this.FindName("txt" + i.ToString().PadLeft(2, '0') + "03");
                _txttmp.IsReadOnly = false;
                _txttmp.IsTabStop = true;
                _txttmp = (TextBox)this.FindName("txt" + i.ToString().PadLeft(2, '0') + "07");
                _txttmp.IsReadOnly = false;
                _txttmp.IsTabStop = true;

                btnPrev.Visibility = Visibility.Collapsed;
              }

              for (int j = 0; j <= 10; j++)
              {
                if (j == 5)
                {
                  continue;
                }

                string namehere = "txt" + i.ToString().PadLeft(2, '0') + j.ToString().PadLeft(2, '0');
                foreach (DataRow dtrow in dati.Rows)
                    {
                        if (dtrow["nomecampo"].ToString() == namehere)
                        {
                          TextBox txt = (TextBox)this.FindName(namehere);
                          txt.Text = dtrow["valore"].ToString();
                        }
                    }
                }
            }
     
    }

    public void LoadDataSource(string ID, string IDCliente, string IDSessione)
    {

      id = int.Parse(ID.ToString());
      cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
      cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());
      if (PrevSession != "")
      {
        btnPrev.Visibility = Visibility.Visible;
      }
      else
      {
        btnPrev.Visibility = Visibility.Collapsed;
      }

   

     
      dati = cBusinessObjects.GetData(id, typeof(ProspettoIVA));

      LoadDataSourceInternal();
    }
    public int Save()
    {
     
      for (int i = 0; i < 13; i++)
      {
        for (int j = 0; j <= 10; j++)
        {
          if (j == 5)
          {
            continue;
          }

          string namehere = "txt" + i.ToString().PadLeft(2, '0') + j.ToString().PadLeft(2, '0');


          TextBox txt = (TextBox)this.FindName(namehere);

          bool trovato =false;
          foreach (DataRow dtrow in dati.Rows)
          {
            if (namehere == dtrow["nomecampo"].ToString())
            {
              trovato = true;
              dtrow["valore"] = txt.Text;
              break;
            }
            
          }
          if(!trovato)
          {
            dati.Rows.Add(id,cBusinessObjects.idcliente,cBusinessObjects.idsessione, namehere, txt.Text);
          }
      
        }
      }

      return cBusinessObjects.SaveData(id, dati, typeof(ProspettoIVA));

    }

    private void txt_KeyUp(object sender, RoutedEventArgs e)
    {
      if (normal == false)
      {
        return;
      }

      for (int i = 0; i < 13; i++)
      {
        for (int j = 0; j <= 10; j++)
        {
          if (j == 5)
          {
            continue;
          }

          if (j == 0 || j == 1)
          {
            TextBox txt0 = (TextBox)this.FindName("txt" + i.ToString().PadLeft(2, '0') + "00");
            TextBox txt1 = (TextBox)this.FindName("txt" + i.ToString().PadLeft(2, '0') + "01");
            TextBox txt2 = (TextBox)this.FindName("txt" + i.ToString().PadLeft(2, '0') + "02");

            double valuetxt0 = 0.0;
            double.TryParse(txt0.Text, out valuetxt0);

            double valuetxt1 = 0.0;
            double.TryParse(txt1.Text, out valuetxt1);

            txt0.Text = cBusinessObjects.ConvertNumber(txt0.Text);
            txt1.Text = cBusinessObjects.ConvertNumber(txt1.Text);
            txt2.Text = cBusinessObjects.ConvertNumber((valuetxt0 - valuetxt1).ToString());

            // if ( i != 0 )
            {
              TextBox txt3 = (TextBox)this.FindName("txt" + i.ToString().PadLeft(2, '0') + "03");
              TextBox txt7 = (TextBox)this.FindName("txt" + ((i != 0) ? (i - 1) : i).ToString().PadLeft(2, '0') + "07");

              double valuetxt7 = 0.0;
              double.TryParse(txt7.Text, out valuetxt7);

              if (i != 0)
              {
                txt3.Text = cBusinessObjects.ConvertNumber((valuetxt0 - valuetxt1 + valuetxt7).ToString());
              }
              else
              {
                txt3.Text = cBusinessObjects.ConvertNumber((valuetxt0 - valuetxt1).ToString());
              }
            }
          }

          if (j == 3 || j == 4 || j == 6)
          {
            TextBox txt3 = (TextBox)this.FindName("txt" + i.ToString().PadLeft(2, '0') + "03");
            TextBox txt4 = (TextBox)this.FindName("txt" + i.ToString().PadLeft(2, '0') + "04");
            TextBox txt6 = (TextBox)this.FindName("txt" + i.ToString().PadLeft(2, '0') + "06");
            TextBox txt7 = (TextBox)this.FindName("txt" + i.ToString().PadLeft(2, '0') + "07");

            double valuetxt3 = 0.0;
            double.TryParse(txt3.Text, out valuetxt3);

            double valuetxt4 = 0.0;
            double.TryParse(txt4.Text, out valuetxt4);

            double valuetxt6 = 0.0;
            double.TryParse(txt6.Text, out valuetxt6);

            txt3.Text = cBusinessObjects.ConvertNumber(txt3.Text);
            txt4.Text = cBusinessObjects.ConvertNumber(txt4.Text);
            txt6.Text = cBusinessObjects.ConvertNumber(txt6.Text);
            txt7.Text = cBusinessObjects.ConvertNumber((valuetxt3 + valuetxt4 - valuetxt6).ToString());
          }
        }
      }
    }

   

    private void obj_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      if (_ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }
    }

    private void obj_PreviewKeyDown(object sender, KeyEventArgs e)
    {
      if (_ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }

      if (e.Key == Key.Enter)
      {
        e.Handled = true;
        ((TextBox)sender).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
      }
    }

    private void Txt_GotFocus(object sender, RoutedEventArgs e)
    {
      ((TextBox)sender).Focus();
      ((TextBox)sender).SelectAll();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      if (_ReadOnly)
      {
        MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
        return;
      }


      dati = cBusinessObjects.GetData(int.Parse(PrevSession), typeof(ProspettoIVA));

      LoadDataSourceInternal();
    }

    private void txt_LostFocus(object sender, RoutedEventArgs e)
    {
      TextBox txt = (TextBox)sender;

      bool trovato = false;
      foreach (DataRow dtrow in dati.Rows)
      {
        if (txt.Name == dtrow["nomecampo"].ToString())
        {
          trovato = true;
          dtrow["valore"] = txt.Text;
          break;
        }

      }
      if (!trovato)
      {
        dati.Rows.Add(id,cBusinessObjects.idcliente,cBusinessObjects.idsessione, txt.Name, txt.Text);
      }
    
    }
  }
}
