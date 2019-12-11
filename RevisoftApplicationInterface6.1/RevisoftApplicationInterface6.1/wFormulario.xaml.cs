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

namespace RevisoftApplication
{
  public partial class Formulario : Window
  {
    public string SelectedTreeSource = "";
    public string SelectedDataSource = "";
    public string SelectedSessioneSource = "";
    private bool firsttime = true;

    XmlDataProviderManager _x;
    XmlDataProvider TreeXmlProvider;

    Hashtable YearColor = new Hashtable();
    Hashtable htStati = new Hashtable();


    public Formulario()
    {
      InitializeComponent();
      txtTitoloRagioneSociale.Foreground = App._arrBrushes[0];

      TreeXmlProvider = this.FindResource("xdpTree") as XmlDataProvider;

      //Dati
      SelectedTreeSource = App.AppFormularioFile;
      SelectedDataSource = App.AppFormularioFileDati;
    }

    #region TreeDataSource

    public void LoadTreeSource()
    {


      RevisoftApplication.XmlManager x = new XmlManager();
      x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;
      TreeXmlProvider.Document = x.LoadEncodedFile(SelectedTreeSource);

      Utilities u = new Utilities();
      if (!u.CheckXmlDocument(TreeXmlProvider.Document, App.TipoFile.Formulario, "Tree"))
      {
        this.Close();
        return;
      }

      if (firsttime)
      {
        firsttime = false;

        foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("//Node"))
        {
          if (item.ParentNode.Name == "Tree")
          {
            item.Attributes["Expanded"].Value = "True";
          }
          else
          {
            item.Attributes["Expanded"].Value = "False";
          }

          item.Attributes["Selected"].Value = "False";
        }
      }

      TreeXmlProvider.Refresh();

      LoadDataSource();


    }

    #endregion

    #region DataDataSource

    private void LoadDataSource()
    {
      _x = new XmlDataProviderManager(SelectedDataSource);

      RevisoftApplication.XmlManager x = new XmlManager();
      x.TipoCodifica = RevisoftApplication.XmlManager.TipologiaCodifica.Normale;

      XmlDocument tmpDoc = x.LoadEncodedFile(SelectedDataSource);

      Utilities u = new Utilities();
      if (!u.CheckXmlDocument(tmpDoc, App.TipoFile.Formulario, "Data"))
      {
        this.Close();
        return;
      }

      foreach (XmlNode node in tmpDoc.SelectNodes("/Dati//Dato"))
      {
        XmlNode nodeTree = TreeXmlProvider.Document.SelectSingleNode("/Tree//Node[@ID=" + node.Attributes["ID"].Value + "]");

        if (nodeTree != null)
        {
          string estensione = "";
          string file = "";
          string image = ".\\Images\\icone\\Stato\\nothing.png";

          try
          {
            estensione = node.SelectSingleNode("Valore").Attributes["NomeFile"].Value.Split('.').Last();
            file = node.SelectSingleNode("Valore").Attributes["NomeFile"].Value.Replace("ruf\\", "");
            string pathfile = App.AppFormularioFolder + "\\" + file;
            if (!(new FileInfo(pathfile)).Exists)
            {
              estensione = "";
              file = "";
            }
          }
          catch (Exception ex)
          {
            cBusinessObjects.logger.Error(ex, "wFormulario.LoadDataSource exception");
            string log = ex.Message;
          }

          switch (estensione)
          {
            case "doc":
            case "docx":
              image = ".\\Images\\icone\\Documenti\\word.png";
              break;
            case "pdf":
              image = ".\\Images\\icone\\Documenti\\pdf.png";
              break;
            case "xls":
            case "xlsx":
              image = ".\\Images\\icone\\Documenti\\excel.png";
              break;
            case "zip":
              image = ".\\Images\\icone\\Documenti\\zip.png";
              break;
            default:
              image = ".\\Images\\icone\\Documenti\\nothing.png";
              break;
          }

          try
          {
            nodeTree.Attributes["TipoDocumento"].Value = image;
          }
          catch (Exception ex)
          {
            cBusinessObjects.logger.Error(ex, "wFormulario.CreateAttributeTipoDocumento exception");
            string log = ex.Message;
            XmlAttribute attr = nodeTree.OwnerDocument.CreateAttribute("TipoDocumento");
            attr.Value = image;
            nodeTree.Attributes.Append(attr);
          }

          try
          {
            nodeTree.Attributes["NomeFile"].Value = file;
          }
          catch (Exception ex)
          {
            cBusinessObjects.logger.Error(ex, "wFormulario.CreateAttributeNomeFile exception");
            string log = ex.Message;
            XmlAttribute attr = nodeTree.OwnerDocument.CreateAttribute("NomeFile");
            attr.Value = file;
            nodeTree.Attributes.Append(attr);
          }
        }
      }
    }

    #endregion

    private void Tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      ;
    }

    private void searchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
      string SearchFor = ((TextBox)sender).Text.ToUpper();
      int foundID = -1;
      bool found = false;

      if (TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
      {
        foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
        {
          if (item.Attributes["Selected"] != null)
          {
            if (item.Attributes["Selected"].Value == "True")
            {
              foundID = Convert.ToInt32(item.Attributes["ID"].Value);
            }

            item.Attributes["Selected"].Value = "False";
          }
        }

        foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
        {
          if (found == false /*&& foundID != Convert.ToInt32(item.Attributes["ID"].Value)*/ && (item.Attributes["Titolo"].Value.ToUpper().Contains(SearchFor) || item.Attributes["Codice"].Value.ToUpper().Contains(SearchFor)))
          {
            found = true;
            item.Attributes["Selected"].Value = "True";

            if (item.ParentNode != null)
            {
              XmlNode parent = item.ParentNode;

              while (parent != null && parent.GetType().Name == "XmlElement" && parent.Name == "Node")
              {
                parent.Attributes["Expanded"].Value = "True";
                parent = parent.ParentNode;
              }
            }
          }
        }
      }

      if (found == false)
      {
        MessageBox.Show("Nessuna voce presente per il testo ricercato");
      }
    }

    private void ItemsControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      ;
    }

    private void searchTextBox_KeyUp(object sender, KeyEventArgs e)
    {
      if (e.Key == Key.Enter || e.Key == Key.Tab)
      {
        string SearchFor = ((TextBox)sender).Text.ToUpper();
        int foundID = -1;
        bool found = false;

        if (TreeXmlProvider.Document != null && TreeXmlProvider.Document.SelectSingleNode("/Tree") != null)
        {
          foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
          {
            if (item.Attributes["Selected"] != null)
            {
              if (item.Attributes["Selected"].Value == "True")
              {
                foundID = Convert.ToInt32(item.Attributes["ID"].Value);
              }

              item.Attributes["Selected"].Value = "False";
            }
          }

          foreach (XmlNode item in TreeXmlProvider.Document.SelectNodes("/Tree//Node"))
          {
            if (found == false /*&& foundID != Convert.ToInt32(item.Attributes["ID"].Value)*/ && (item.Attributes["Titolo"].Value.ToUpper().Contains(SearchFor) || item.Attributes["Codice"].Value.ToUpper().Contains(SearchFor)))
            {
              found = true;
              item.Attributes["Selected"].Value = "True";

              if (item.ParentNode != null)
              {
                XmlNode parent = item.ParentNode;

                while (parent != null && parent.GetType().Name == "XmlElement" && parent.Name == "Node")
                {
                  parent.Attributes["Expanded"].Value = "True";
                  parent = parent.ParentNode;
                }
              }
            }
          }
        }

        if (found == false)
        {
          MessageBox.Show("Nessuna voce presente per il testo ricercato");
        }
      }
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      //SaveTreeSource();
    }

    private void TreeViewItem_Selected(object sender, RoutedEventArgs e)
    {
    }

    private void OpenDocumento()
    {
      XmlNode node;

      try
      {
        node = ((XmlNode)(tvMain.SelectedItem));
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wFormulario.OpenDocumento1 exception");
        MessageBox.Show("Selezionare un documento");
        string log = ex.Message;
        return;
      }

      try
      {
        string file = node.Attributes["NomeFile"].Value.Replace("ruf\\", "");

        if (file != "")
        {
          //string pathfile = App.AppFormularioFolder + "\\" + file;
          //FileInfo fi = new FileInfo(pathfile);
          //if (fi.Exists)
          //{
          //    System.Diagnostics.Process.Start(pathfile);
          //}

          try
          {
            string directory = App.AppFormularioFolder;

            string pathfile = directory + "\\" + file;
            FileInfo fi = new FileInfo(pathfile);
            if (fi.Exists)
            {
              string newfile = System.IO.Path.GetTempPath() + file;
              FileInfo newfi = new FileInfo(newfile);
              if (newfi.Exists)
              {
                newfi.Delete();
              }

              fi.CopyTo(newfile);

              System.Diagnostics.Process process = new System.Diagnostics.Process();
              process.Refresh();
              process.StartInfo.FileName = newfile;
              process.StartInfo.ErrorDialog = false;
              process.StartInfo.Verb = "open";
              process.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Maximized;
              process.Start();
            }
          }
          catch (Exception ex)
          {
            cBusinessObjects.logger.Error(ex, "wFormulario.OpenDocumentoAppFormularioFolder exception");
            string log = ex.Message;
          }
        }
      }
      catch (Exception ex)
      {
        cBusinessObjects.logger.Error(ex, "wFormulario.OpenDocumento2 exception");
        string log = ex.Message;
      }
    }

    private void OnItemMouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
      if (e.ClickCount != 2)
      {
        return;
      }

      OpenDocumento();

      e.Handled = true;
    }

    private void buttonChiudi_Click(object sender, RoutedEventArgs e)
    {
      base.Close();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {

    }

    private void buttonApri_Click(object sender, RoutedEventArgs e)
    {
      OpenDocumento();
    }
  }
}
