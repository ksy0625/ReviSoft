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
using System.Xml;
using RevisoftApplication;
using System.Collections;
using System.Data;

namespace UserControls
{
    public partial class ucTabella : UserControl
    {
        public int id;
        public DataTable dati = null;
        private string fromtab="";
        private string _IDTree;

        Hashtable htData = new Hashtable();
        int numcolumns = 2;
        string[] columnsAlias = { "Descrizione", "Dati" };
        string[] columnsValues = { "name", "value" };
        double[] columnsWidth = { 1.0, 2.0 };
        bool[] conditionalReadonly = { true, false };
        double totalwidth = 3.0;

        //private XmlDataProviderManager _x;
		    private string _ID = "";
		    private string _tab = "";

		    private bool _ReadOnly = false;

		    public string XPath = "";
		    public ArrayList OldXPath = new ArrayList();

        //GenericTable gtF24 = null;

        public ucTabella()
        {
            InitializeComponent();   
        }
		
        public bool ReadOnly
        {
            set  {_ReadOnly = value; }
        }

        public void Load( string ID, string tab, string IDTree, string _fromtab, string IDCliente, string IDSessione)
        {
			   
            _IDTree = IDTree;
            id = int.Parse(ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(IDSessione.ToString());

            _ID = ID;
		   _tab = tab;
            dati = cBusinessObjects.GetData(id, typeof(Tabella));
            if (tab != "")
                dati = cBusinessObjects.GetDataFiltered(dati, tab, "tab");
            fromtab = tab;
          
            
            GenerateTable(tblMainContainer, numcolumns, columnsAlias, columnsValues, columnsWidth, conditionalReadonly);
            //gtF24 = new GenericTable(ref _x, ref tblMainContainer, _ReadOnly);

            //gtF24.ColumnsAlias = new string[] { "Descrizione", "Dati" };
            //gtF24.ColumnsValues = new string[] { "name", "value" };
            //gtF24.ColumnsWidth = new double[] { 1.0, 2.0 };
            //gtF24.ColumnsMinWidth = new double[] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
            //gtF24.ColumnsTypes = new string[] { "string", "string" };
            //gtF24.ColumnsAlignment = new string[] { "left", "left" };
            //gtF24.ConditionalReadonly = new bool[] { true, false };
            //gtF24.ConditionalAttribute = "new";
            //gtF24.ColumnsHasTotal = new bool[] { false, false };
            //gtF24.ColumnAliasTotale = 1;

            //gtF24.Xpath = XPath;

            //gtF24.GenerateTable();
        }

        public void DoGenerateTable()
        {
            GenerateTable(tblMainContainer, numcolumns, columnsAlias, columnsValues, columnsWidth, conditionalReadonly);
            
        }

        private void GenerateTable(Grid gridcontainer, int numcolumn, string[] columnsAlias, string[] columnsValues, double[] columnsWidth, bool[] conditionalReadonly)
        {
            gridcontainer.SizeChanged += Gridcontainer_SizeChanged;
            htData = new Hashtable();
                    
            gridcontainer.ColumnDefinitions.Clear();
            gridcontainer.RowDefinitions.Clear();
            gridcontainer.Children.Clear();
      
            int row = 0;

            /*DEFINIZIONE COLONNE*/
            ColumnDefinition cd;

            for (int i = 0; i < numcolumn; i++)
            {
                cd = new ColumnDefinition();
                cd.Width = new GridLength(columnsWidth[i], GridUnitType.Star);
                gridcontainer.ColumnDefinitions.Add(cd);
            }

            /*HEADERS*/
            RowDefinition rd;
            TextBox txt;
            TextBlock lbl;
            Border brd;

            for (int i = 0; i < numcolumn; i++)
            {
                rd = new RowDefinition();
                gridcontainer.RowDefinitions.Add(rd);

                brd = new Border();
                brd.BorderThickness = new Thickness(1.0);
                brd.BorderBrush = Brushes.LightGray;
                brd.Background = Brushes.LightGray;
                brd.Padding = new Thickness(2.0);

                lbl = new TextBlock();
                lbl.Text = columnsAlias[i];
                lbl.TextAlignment = TextAlignment.Center;
                lbl.TextWrapping = TextWrapping.Wrap;
                lbl.FontWeight = FontWeights.Bold;

                brd.Child = lbl;

                gridcontainer.Children.Add(brd);
                Grid.SetRow(brd, row);
                Grid.SetColumn(brd, i);
            }

            row++;

            /*DATI*/
            foreach (DataRow orow in dati.Rows)
             {
                if (orow["tab"].ToString()!=fromtab)
                    continue;
                rd = new RowDefinition();
                gridcontainer.RowDefinitions.Add(rd);

                for (int i = 0; i < numcolumn; i++)
                {
                    brd = new Border();
                    brd.BorderThickness = new Thickness(1.0);
                    brd.BorderBrush = Brushes.LightGray;

                    brd.Padding = new Thickness(0.0);
                    brd.Margin = new Thickness(0.0);

                    if (row % 2 == 0)
                    {
                        brd.Background = new SolidColorBrush(Color.FromArgb(255, 241, 241, 241));
                    }
                    else
                    {
                        brd.Background = Brushes.White;
                    }

                    txt = new TextBox();
                    txt.Name = "txt_" + columnsValues[i] + "_" + row.ToString();

                    if (conditionalReadonly[i] == true)
                    {
                        if (orow["isnew"].ToString() != "1")
                        {
                            txt.IsReadOnly = true;
                            txt.IsTabStop = false;
                        }
                    }
                    
                    htData.Add(txt.Name, orow["ID"].ToString());
                    txt.Tag = columnsValues[i];
                    txt.Text = orow[columnsValues[i]].ToString();
                    txt.TextWrapping = TextWrapping.Wrap;
                    txt.GotFocus += Txt_GotFocus;
                    txt.LostFocus += Txt_LostFocus;
                    txt.PreviewKeyDown += Txt_PreviewKeyDown;
                    txt.PreviewMouseDown += Txt_PreviewMouseDown;
                    txt.TextAlignment = TextAlignment.Left;
                    txt.TextWrapping = TextWrapping.Wrap;
                    txt.BorderThickness = new Thickness(0.0);
                    txt.Background = Brushes.Transparent;

                    brd.Child = txt;

                    gridcontainer.Children.Add(brd);
                    Grid.SetRow(brd, row);
                    Grid.SetColumn(brd, i);
                }

                row++;
            }
        }

        private void Gridcontainer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            foreach (Border item in ((Grid)(sender)).Children)
            {
                if (item.Child.GetType().Name == "TextBox")
                {
                    for (int i = 0; i < columnsValues.Length; i++)
                    {
                        if (columnsValues[i] == ((TextBox)(item.Child)).Name.Split('_')[1])
                        {
                            ((TextBox)(item.Child)).Width = ((e.NewSize.Width - 10.0) / totalwidth * columnsWidth[i]) - 4.0;
                        }
                    }
                }
            }
        }

        private void Txt_GotFocus(object sender, RoutedEventArgs e)
        {
            foreach (Border item in ((Grid)(((Border)(((TextBox)sender).Parent)).Parent)).Children)
            {
                if(item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).Name.Split('_')[2] == ((TextBox)sender).Name.Split('_')[2])
                {
                    item.BorderBrush = App._arrBrushes[0];
        }
                else
                {
                    item.BorderBrush = Brushes.LightGray;
                }
            }

            ((TextBox)sender).SelectAll();// (((TextBox)sender).Text.Length, 0);
        }

        private void Txt_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_ReadOnly)
            {
                MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
                return;
            }
        }

        private void Txt_PreviewKeyDown(object sender, KeyEventArgs e)
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

        private void Txt_LostFocus(object sender, RoutedEventArgs e)
        {
            if (_ReadOnly)
            {
                return;
            }
          
            foreach (DataRow orow in dati.Rows)
            {
                if (orow["tab"].ToString() != fromtab)
                    continue;
                if (int.Parse(htData[((TextBox)(sender)).Name].ToString()) == int.Parse(orow["ID"].ToString()))
                {
                    if (((TextBox)(sender)).Name.ToString().Contains("value"))
                       orow["value"] = ((TextBox)sender).Text;
                    else
                       orow["name"] = ((TextBox)sender).Text;
                    break;
                }
            }
            
        }

    public int Save()
        {
            int j=1;
            foreach (DataRow orow in dati.Rows)
            {
                orow["ID"] = j;
                j++;
            }

             int ret= cBusinessObjects.SaveData(id, dati, typeof(Tabella));

       //   GenerateTable(tblMainContainer, numcolumns, columnsAlias, columnsValues, columnsWidth, conditionalReadonly);

          return ret;
        }

        public DataTable Merge()
        {
            
            for (int i = dati.Rows.Count - 1; i >= 0; i--)
            {
                DataRow dtrow = dati.Rows[i];
                if (dtrow["tab"].ToString() != fromtab)
                    dtrow.Delete();
            }
            dati.AcceptChanges();
           
            return dati;
        }


        private void AddRow(object sender, RoutedEventArgs e)
        {
			      if (_ReadOnly)
			      {
				      MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				      return;
			      }
            int cid = -1;

            foreach (Border item in tblMainContainer.Children)
            {
                if (item.Child.GetType().Name == "TextBox")
                {
                    cid = int.Parse(htData[((TextBox)(item.Child)).Name].ToString());
                }

                if (item.BorderBrush == App._arrBrushes[0])
                {
                    break;
                }
            }

            int j = 1;
            if(cid!=-1)
            {
             
             foreach (DataRow orow in dati.Rows)
                  {
                    if (orow["tab"].ToString() != fromtab)
                        continue;
                    if (cid == int.Parse(orow["ID"].ToString()))
                        break;
                    j++;
                 }
            }
            int lastid = 0;
            foreach (DataRow orow in dati.Rows)
            {
                if (lastid < int.Parse(orow["ID"].ToString()))
                    lastid = int.Parse(orow["ID"].ToString());
             
            }
            lastid++;
            DataRow dr = dati.NewRow();
            dr["ID_SCHEDA"]= id;
            dr["ID_CLIENTE"] = cBusinessObjects.idcliente;
            dr["ID_SESSIONE"] = cBusinessObjects.idsessione;
            dr["ID"] = lastid;
            dr["tab"] = _tab;
            dr["isnew"]= "1";
            dati.Rows.InsertAt(dr,j);
            GenerateTable(tblMainContainer, numcolumns, columnsAlias, columnsValues, columnsWidth, conditionalReadonly);
 
         }

    private void DeleteRow(object sender, RoutedEventArgs e)
        {
			      if (_ReadOnly)
			      {
				      MessageBox.Show( App.MessaggioSolaScrittura, "Attenzione");
				      return;
			      }

			      if (MessageBox.Show("Si è sicuri di procedere con l'eliminazione?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			      {
            int cid = -1;

            foreach (Border item in tblMainContainer.Children)
              {
                  if (item.BorderBrush == App._arrBrushes[0])
                   {
                   cid = int.Parse(htData[((TextBox)(item.Child)).Name].ToString());
                    break;
                  }
              }

              if(cid == -1)
              {
                  MessageBox.Show("Selezionare una riga");
                  return;
              }
                
                foreach (DataRow orow in dati.Rows)
                    {
                    if (int.Parse(orow["ID"].ToString()) == cid)
                        {
                        if(orow["isnew"].ToString()=="1")
                        {
                            dati.Rows.Remove(orow);
                            GenerateTable(tblMainContainer, numcolumns, columnsAlias, columnsValues, columnsWidth, conditionalReadonly);
                            break;
                        }
                        else
                        {
                            MessageBox.Show("Solo le righe inserite dall'utente possono essere cancellate");
                            return;
                        }

                        }
                    }
            
            }
    }

		public void ChangeAlias(string newTab)
		{
			
          foreach (DataRow orow in dati.Rows)
          {
            orow["Tab"] = newTab;
          }

            //   int ret = cBusinessObjects.SaveData(id, dati, typeof(Tabella));

            //   Load( id.ToString(), newTab, _IDTree, "",cBusinessObjects.idcliente.ToString(),cBusinessObjects.idsessione.ToString());
          fromtab = newTab;
          GenerateTable(tblMainContainer, numcolumns, columnsAlias, columnsValues, columnsWidth, conditionalReadonly);

          _tab = newTab;

		}

		public void DeleteAll()
		{
		
      dati.Rows.Clear();
      int ret = cBusinessObjects.SaveData(id, dati, typeof(Tabella));

      GenerateTable(tblMainContainer,  numcolumns, columnsAlias, columnsValues, columnsWidth, conditionalReadonly);
     }

		private void UserControl_Unloaded(object sender, RoutedEventArgs e)
		{
            if (!_ReadOnly)
            {
              //int ret = cBusinessObjects.SaveData(id, dati, typeof(Tabella));

              GenerateTable(tblMainContainer,  numcolumns, columnsAlias, columnsValues, columnsWidth, conditionalReadonly);
            }
        }

        public void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Border item in tblMainContainer.Children)
            {
                if (item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).IsReadOnly == false)
                {
                    ((TextBox)(item.Child)).Focus();
                    ((TextBox)(item.Child)).SelectAll();
                    return;
                }
            }            
        }
    }
}
