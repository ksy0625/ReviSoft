using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.Xml;
using System.Collections;
using UserControls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Data;

namespace RevisoftApplication
{

  public partial class wWorkAreaTree_PianificazioniVerifiche : System.Windows.Window
  {
    public int id;
    private DataTable dati = null;
    private DataTable datiTestata = null;

    public bool m_isModified = false;
    public string IDP = "";
    public string SelectedTreeSource = "";
    public string SelectedDataSource = "";
    public string SelectedSessioneSource = "";

    private string _cliente = "";
    private App.TipoAttivita _TipoAttivita = App.TipoAttivita.Sconosciuto;

    public string TitoloSessione = "";
    public string ImportFileName = "";

    public string IDTree = "-1";
    public string IDCliente = "-1";
    public string IDSessione = "-1";

    public string DataInizio = "";
    public string DataFine = "";

    public bool ReadOnly = true;

    public XmlDataProviderManager _x;


    Hashtable htComboID = new Hashtable();

    public string Cliente
    {
      get
      {
        return _cliente;
      }
      set
      {
        _cliente = value;
        GeneraTitolo();
      }
    }

    public App.TipoAttivita TipoAttivita
    {
      get
      {
        return _TipoAttivita;
      }
      set
      {
        _TipoAttivita = value;
      }
    }

    private void GeneraTitolo()
    {
      txtTitoloPeriodo.Text = "Perioido dal " + DataInizio + " al " + DataFine;
    }

    public wWorkAreaTree_PianificazioniVerifiche()
    {
      if (alreadydone) { }
      InitializeComponent();
      lab1.Foreground = App._arrBrushes[0];
    }

    #region TreeDataSource

    private void SaveTreeSource()
    {
      cBusinessObjects.SaveData(id, datiTestata, typeof(PianificazioneVerificheTestata));
      cBusinessObjects.SaveData(id, dati, typeof(PianificazioneVerifiche));
    }

    ArrayList sortingal = new ArrayList();

    public void sorting()
    {
      sortingal.Clear();

      foreach (DictionaryEntry item in htSessioni)
      {
        sortingal.Add(Convert.ToDateTime(item.Value));
      }

      sortingal.Sort();
    }

    Hashtable htSessioni = new Hashtable();

    public void LoadTreeSource()
    {
      id = 100013;

      htSessioni.Clear();
      dati = cBusinessObjects.GetData(id, typeof(PianificazioneVerifiche));
      datiTestata = cBusinessObjects.GetData(id, typeof(PianificazioneVerificheTestata));

      foreach (DataRow itemV in datiTestata.Rows)
      {
        if (!htSessioni.Contains(itemV["ID"].ToString()))
        {
          htSessioni.Add(itemV["ID"].ToString(), itemV["Data"].ToString());
        }
      }

      sorting();

      grdHeaderContainer.Children.Clear();
      grdHeaderContainer.ColumnDefinitions.Clear();
      grdHeaderContainer.RowDefinitions.Clear();

      ColumnDefinition gridCol1H = new ColumnDefinition();
      gridCol1H.Width = new GridLength(350, GridUnitType.Pixel);
      grdHeaderContainer.ColumnDefinitions.Add(gridCol1H);

      ColumnDefinition gridCol2H = new ColumnDefinition();
      gridCol2H.Width = new GridLength(70.0, GridUnitType.Pixel);
      grdHeaderContainer.ColumnDefinitions.Add(gridCol2H);

      foreach (DictionaryEntry item in htSessioni)
      {
        ColumnDefinition gridColN = new ColumnDefinition();
        gridColN.Width = new GridLength(70.0, GridUnitType.Pixel);
        grdHeaderContainer.ColumnDefinitions.Add(gridColN);
      }

      ColumnDefinition gridColAllH = new ColumnDefinition();
      gridColAllH.Width = new GridLength(70.0, GridUnitType.Pixel);
      grdHeaderContainer.ColumnDefinitions.Add(gridColAllH);

      RowDefinition gridRow1 = new RowDefinition();
      gridRow1.Height = new GridLength(20);
      grdHeaderContainer.RowDefinitions.Add(gridRow1);

      RowDefinition gridRow2 = new RowDefinition();
      gridRow2.Height = new GridLength(20);
      grdHeaderContainer.RowDefinitions.Add(gridRow2);

      Border brd = new Border();
      brd.BorderBrush = Brushes.Black;
      brd.BorderThickness = new Thickness(1, 1, 1, 1);
      TextBlock txtb = new TextBlock();
      txtb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
      txtb.Text = "NA";
      txtb.Height = 20;
      txtb.Margin = new Thickness(0, 0, 0, 0);
      brd.Child = txtb;
      Grid.SetRow(brd, 0);
      Grid.SetRowSpan(brd, 2);
      Grid.SetColumn(brd, 1);
      grdHeaderContainer.Children.Add(brd);

      int indexcolumn = 2;

      foreach (DateTime date in sortingal)
      {
        foreach (DictionaryEntry item in htSessioni)
        {
          if (date.ToShortDateString() != item.Value.ToString())
          {
            continue;
          }

          brd = new Border();
          brd.BorderBrush = Brushes.Black;
          brd.BorderThickness = new Thickness(0, 1, 1, 0);
          txtb = new TextBlock();
          txtb.Text = (indexcolumn - 1).ToString() + "° Sessione";
          txtb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          txtb.Height = 20;
          txtb.Margin = new Thickness(0, 0, 0, 0);
          brd.Child = txtb;
          Grid.SetRow(brd, 0);
          Grid.SetColumn(brd, indexcolumn);
          grdHeaderContainer.Children.Add(brd);

          brd = new Border();
          brd.BorderBrush = Brushes.Black;
          brd.BorderThickness = new Thickness(0, 0, 1, 1);
          txtb = new TextBlock();
          txtb.Text = item.Value.ToString();
          txtb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          txtb.Height = 20;
          txtb.Margin = new Thickness(0, 0, 0, 0);
          brd.Child = txtb;
          Grid.SetRow(brd, 1);
          Grid.SetColumn(brd, indexcolumn);
          grdHeaderContainer.Children.Add(brd);

          indexcolumn++;
        }
      }

      Border brdall = new Border();
      brdall.BorderBrush = Brushes.Black;
      brdall.BorderThickness = new Thickness(0, 1, 1, 0);
      TextBlock txtball = new TextBlock();
      txtball.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
      txtball.Text = "Check";
      txtball.Height = 20;
      txtball.Margin = new Thickness(0, 0, 0, 0);
      brdall.Child = txtball;
      Grid.SetRow(brdall, 0);
      Grid.SetColumn(brdall, indexcolumn);
      grdHeaderContainer.Children.Add(brdall);

      brdall = new Border();
      brdall.BorderBrush = Brushes.Black;
      brdall.BorderThickness = new Thickness(0, 0, 1, 1);
      txtball = new TextBlock();
      txtball.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
      txtball.Text = "orizz.";
      txtball.Height = 20;
      txtball.Margin = new Thickness(0, 0, 0, 0);
      brdall.Child = txtball;
      Grid.SetRow(brdall, 1);
      Grid.SetColumn(brdall, indexcolumn);
      grdHeaderContainer.Children.Add(brdall);


      grdMainContainer.Children.Clear();
      grdMainContainer.ColumnDefinitions.Clear();
      grdMainContainer.RowDefinitions.Clear();

      ColumnDefinition gridCol1 = new ColumnDefinition();
      gridCol1.Width = new GridLength(350, GridUnitType.Pixel);
      grdMainContainer.ColumnDefinitions.Add(gridCol1);

      ColumnDefinition gridCol2 = new ColumnDefinition();
      gridCol2.Width = new GridLength(70.0, GridUnitType.Pixel);
      gridCol2.MinWidth = 70.0;
      grdMainContainer.ColumnDefinitions.Add(gridCol2);

      foreach (DictionaryEntry item in htSessioni)
      {
        ColumnDefinition gridColN = new ColumnDefinition();
        gridColN.Width = new GridLength(70.0, GridUnitType.Pixel);
        grdMainContainer.ColumnDefinitions.Add(gridColN);
      }

      ColumnDefinition gridColALL2 = new ColumnDefinition();
      gridColALL2.Width = new GridLength(70.0, GridUnitType.Pixel);
      gridColALL2.MinWidth = 70.0;
      grdMainContainer.ColumnDefinitions.Add(gridColALL2);


      DataTable datip = cBusinessObjects.GetData(id, typeof(PianificazioneVerifiche));
      datip = cBusinessObjects.GetDataFiltered(datip, "0", "PianificazioneID");

      foreach (DataRow item in datip.Rows)
      {
        RowDefinition gridRow3 = new RowDefinition();
        gridRow3.Height = new GridLength(20);
        grdMainContainer.RowDefinitions.Add(gridRow3);
      }


      int indexrow = 0;

      foreach (DataRow item in datip.Rows)
      {
        indexcolumn = 0;

        bool istitolo = false;
        if (item["Father"].ToString() != "")
        {
          istitolo = true;
        }

        txtb = new TextBlock();
        txtb.Text = item["Codice"].ToString() + " " + item["Titolo"].ToString();
        if (istitolo)
        {
          txtb.FontWeight = FontWeights.Bold;
        }
        txtb.Height = 20;
        txtb.Margin = new Thickness(item["Codice"].ToString().Split('.').Count() * 10, 0, 0, 0);
        Grid.SetRow(txtb, indexrow);
        Grid.SetColumn(txtb, indexcolumn);
        grdMainContainer.Children.Add(txtb);


        indexcolumn++;

        brd = new Border();
        brd.BorderBrush = Brushes.Black;
        brd.BorderThickness = new Thickness(1, 0, 1, 0);
        bool nachecked = false;
        if (!istitolo)
        {
          CheckBox chk = new CheckBox();
          chk.Name = "chkNA_" + item["ID"].ToString();
          chk.Tag = item["Codice"].ToString();
          this.RegisterName(chk.Name, chk);

          if (item["Checked"].ToString() != "" && item["Checked"].ToString() == "True")
          {
            nachecked = true;
            chk.IsChecked = true;
          }
          else
          {
            nachecked = false;
            chk.IsChecked = false;
          }

          chk.Checked += chk_Checked;
          chk.Unchecked += chk_Unchecked;

          chk.Margin = new Thickness(0, 0, 0, 0);
          chk.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          brd.Child = chk;
        }
        Grid.SetRow(brd, indexrow);
        Grid.SetColumn(brd, indexcolumn);
        grdMainContainer.Children.Add(brd);
        indexcolumn++;

        bool allischecked = true;
        foreach (DateTime date in sortingal)
        {
          foreach (DictionaryEntry itemS in htSessioni)
          {
            if (date.ToShortDateString() != itemS.Value.ToString())
            {
              continue;
            }


            if (!istitolo)
            {
              brd = new Border();
              brd.BorderBrush = Brushes.Black;
              brd.BorderThickness = new Thickness(0, 0, 1, 0);
              CheckBox chkS = new CheckBox();
              chkS.Name = "chk_" + itemS.Key.ToString() + "_" + item["ID"].ToString();
              this.RegisterName(chkS.Name, chkS);
              chkS.Margin = new Thickness(0, 0, 0, 0);
              chkS.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
              DataRow nodehere = null;


              foreach (DataRow itemhere in dati.Rows)
              {
                if (itemhere["PianificazioneID"].ToString() == itemS.Key.ToString() && itemhere["ID"].ToString() == item["ID"].ToString())
                {
                  nodehere = itemhere;
                }
              }

              if (nodehere != null && nodehere["Checked"].ToString() != "" && nodehere["Checked"].ToString() == "True")
              {
                chkS.IsChecked = true;
              }
              else
              {
                chkS.IsChecked = false;
                allischecked = false;
              }

              chkS.Checked += chkS_Checked;
              chkS.Unchecked += chkS_Unchecked;

              if (nachecked == true)
              {
                chkS.Visibility = System.Windows.Visibility.Collapsed;
              }
              brd.Child = chkS;
              Grid.SetRow(brd, indexrow);
              Grid.SetColumn(brd, indexcolumn);
              grdMainContainer.Children.Add(brd);

            }
            else
            {
              brd = new Border();
              brd.BorderBrush = Brushes.Black;
              brd.BorderThickness = new Thickness(0, 0, 1, 0);
              if (item["Codice"].ToString().Split('.').Count() > 1)
              {
                CheckBox chkGrp = new CheckBox();
                chkGrp.Name = "chkGRP_" + itemS.Key.ToString() + "_" + item["ID"].ToString();
                this.RegisterName(chkGrp.Name, chkGrp);
                chkGrp.Margin = new Thickness(4, 0, 4, 0);
                chkGrp.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                chkGrp.Checked += chkGRP_Checked;
                chkGrp.Unchecked += chkGRP_Unchecked;
                chkGrp.Tag = item["Codice"].ToString();
                brd.Child = chkGrp;
              }
              Grid.SetRow(brd, indexrow);
              Grid.SetColumn(brd, indexcolumn);
              grdMainContainer.Children.Add(brd);
            }



            indexcolumn++;
          }
        }

        indexcolumn++;

        brd = new Border();
        brd.BorderBrush = Brushes.Black;
        brd.BorderThickness = new Thickness(1, 0, 1, 0);

        if (!istitolo)
        {
          CheckBox chk = new CheckBox();
          chk.Name = "chkALL_" + item["ID"].ToString();
          this.RegisterName(chk.Name, chk);

          chk.IsChecked = allischecked;

          chk.Checked += chkALL_Checked;
          chk.Unchecked += chkALL_Unchecked;

          chk.Margin = new Thickness(0, 0, 0, 0);
          chk.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
          brd.Child = chk;
        }
        Grid.SetRow(brd, indexrow);
        Grid.SetColumn(brd, indexcolumn);
        grdMainContainer.Children.Add(brd);
        indexrow++;
      }


      LoadDataSource();
    }


    void chkS_Unchecked(object sender, RoutedEventArgs e)
    {
      string name = ((CheckBox)(sender)).Name;
      string[] splittedName = name.Split('_');

      foreach (DataRow itemhere in dati.Rows)
      {
        if (itemhere["ID"].ToString() == splittedName[2] && itemhere["PianificazioneID"].ToString() == splittedName[1])
        {

          itemhere["Checked"] = "False";

        }
      }

      m_isModified = true;
    }

    void chkS_Checked(object sender, RoutedEventArgs e)
    {
      string name = ((CheckBox)(sender)).Name;
      string[] splittedName = name.Split('_');

      ((CheckBox)(this.FindName("chkNA_" + splittedName[2]))).IsChecked = false;
      /*
      foreach (DataRow itemhere in dati.Rows)
      {
          if (itemhere["ID"].ToString() == splittedName[1])
          {
              if (itemhere["Checked"].ToString() != "False")
              {
                  MessageBox.Show("Sono già presenti delle scelte.", "Attenzione");
                  e.Handled = true;
                  ((CheckBox)(sender)).IsChecked = false;
                  return;
              }
          }
      }
      */
      foreach (DataRow itemhere in dati.Rows)
      {
        if (itemhere["ID"].ToString() == splittedName[2] && itemhere["PianificazioneID"].ToString() == splittedName[1])
        {
          itemhere["Checked"] = "True";
          //    ((CheckBox)(this.FindName("chk_" + itemhere["PianificazioneID"].ToString() + "_" + splittedName[2]))).IsChecked = false;
          //    ((CheckBox)(this.FindName("chk_" + itemhere["PianificazioneID"].ToString() + "_" + splittedName[2]))).Visibility = System.Windows.Visibility.Hidden;
        }
      }
      foreach (DataRow itemhere in datiTestata.Rows)
      {
        if (itemhere["ID"].ToString() == splittedName[1])
          itemhere["PianificazioneChecked"] = "True";
      }



      m_isModified = true;
    }

    bool alreadydone = false;
    void chkALL_Checked(object sender, RoutedEventArgs e)
    {
      string name = ((CheckBox)(sender)).Name;
      string[] splittedName = name.Split('_');




      foreach (DataRow itemhere in dati.Rows)
      {

        if (itemhere["ID"].ToString() == splittedName[1])
        {
          foreach (DictionaryEntry itemS in htSessioni)
          {
            ((CheckBox)(this.FindName("chk_" + itemS.Key.ToString() + "_" + itemhere["ID"].ToString()))).IsChecked = true;
          }
          if (itemhere["PianificazioneID"].ToString() == "0")
            itemhere["Checked"] = "False";
        }
      }

      m_isModified = true;

    }



    void chkALL_Unchecked(object sender, RoutedEventArgs e)
    {
      string name = ((CheckBox)(sender)).Name;
      string[] splittedName = name.Split('_');
      foreach (DataRow itemhere in dati.Rows)
      {

        if (itemhere["ID"].ToString() == splittedName[1])
        {

          foreach (DictionaryEntry itemS in htSessioni)
          {
            ((CheckBox)(this.FindName("chk_" + itemS.Key.ToString() + "_" + itemhere["ID"].ToString()))).IsChecked = false;
          }
          if (itemhere["PianificazioneID"].ToString() == "0")
            itemhere["Checked"] = "False";
        }
      }

      m_isModified = true;

    }

    void chk_Unchecked(object sender, RoutedEventArgs e)
    {
      string name = ((CheckBox)(sender)).Name;
      string[] splittedName = name.Split('_');
      foreach (DataRow itemhere in dati.Rows)
      {

        if (itemhere["ID"].ToString() == splittedName[1])
        {

          foreach (DictionaryEntry itemS in htSessioni)
          {
            ((CheckBox)(this.FindName("chk_" + itemS.Key.ToString() + "_" + itemhere["ID"].ToString()))).Visibility = System.Windows.Visibility.Visible;
          }
          if (itemhere["PianificazioneID"].ToString() == "0")
            itemhere["Checked"] = "False";
        }
      }

      m_isModified = true;

    }
    void chkGRP_Unchecked(object sender, RoutedEventArgs e)
    {
      string name = ((CheckBox)(sender)).Name;
      string[] splittedName = name.Split('_');
      int numdot = ((CheckBox)(sender)).Tag.ToString().Split('.').Count();
      bool prosegui = false;
      CheckBox tempchb = null;
      foreach (DataRow itemhere in dati.Rows)
      {

        if (prosegui)
        {
          try
          {
            tempchb = ((CheckBox)(this.FindName("chkNA_" + itemhere["ID"].ToString())));
            if (numdot >= tempchb.Tag.ToString().Split('.').Count())
            {
              prosegui = false;
            }
            else
            {
              tempchb = (CheckBox)(this.FindName("chk_" + splittedName[1] + "_" + itemhere["ID"].ToString()));
              tempchb.IsChecked = false;
            }

          }
          catch (Exception aa)
          {

            tempchb = ((CheckBox)(this.FindName("chkGRP_" + splittedName[1] + "_" + itemhere["ID"].ToString())));
            if (numdot >= tempchb.Tag.ToString().Split('.').Count())
            {
              prosegui = false;
            }
            else
            {
              tempchb.IsChecked = false;
            }

          }


        }
        if (itemhere["ID"].ToString() == splittedName[2])
        {
          prosegui = true;
        }
      }

      m_isModified = true;
    }

    void chkGRP_Checked(object sender, RoutedEventArgs e)
    {
      string name = ((CheckBox)(sender)).Name;
      string[] splittedName = name.Split('_');
      int numdot = ((CheckBox)(sender)).Tag.ToString().Split('.').Count();
      bool prosegui = false;
      CheckBox tempchb = null;
      foreach (DataRow itemhere in dati.Rows)
      {

        if (prosegui)
        {
          try
          {
            tempchb = ((CheckBox)(this.FindName("chkNA_" + itemhere["ID"].ToString())));
            if (numdot >= tempchb.Tag.ToString().Split('.').Count())
            {
              prosegui = false;
            }
            else
            {
              tempchb = (CheckBox)(this.FindName("chk_" + splittedName[1] + "_" + itemhere["ID"].ToString()));
              tempchb.IsChecked = true;
            }

          }
          catch (Exception aa)
          {

            tempchb = ((CheckBox)(this.FindName("chkGRP_" + splittedName[1] + "_" + itemhere["ID"].ToString())));
            if (numdot >= tempchb.Tag.ToString().Split('.').Count())
            {
              prosegui = false;
            }
            else
            {
              tempchb.IsChecked = true;
            }
          }


        }
        if (itemhere["ID"].ToString() == splittedName[2])
        {
          prosegui = true;
        }
      }

      m_isModified = true;
    }

    void chk_Checked(object sender, RoutedEventArgs e)
    {
      string name = ((CheckBox)(sender)).Name;
      string[] splittedName = name.Split('_');




      foreach (DataRow itemhere in dati.Rows)
      {

        if (itemhere["ID"].ToString() == splittedName[1])
        {
          foreach (DictionaryEntry itemS in htSessioni)
          {
            ((CheckBox)(this.FindName("chk_" + itemS.Key.ToString() + "_" + itemhere["ID"].ToString()))).Visibility = System.Windows.Visibility.Hidden;
          }
          if (itemhere["PianificazioneID"].ToString() == "0")
            itemhere["Checked"] = "True";
        }
      }

      m_isModified = true;

    }

    #endregion

    #region DataDataSource

    private void LoadDataSource()
    {
      ;
    }

    #endregion

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      SaveTreeSource();
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }

  }
}