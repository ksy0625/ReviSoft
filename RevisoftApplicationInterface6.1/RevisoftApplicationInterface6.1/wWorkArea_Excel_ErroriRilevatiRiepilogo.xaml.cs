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
using System.Threading;
using RevisoftApplication;

namespace UserControls
{
	public partial class uc_Excel_ErroriRilevatiRiepilogo : UserControl
    {
        private XmlDataProviderManager _x = null;

		public uc_Excel_ErroriRilevatiRiepilogo()
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("it-IT"); 
            InitializeComponent();
        }

        public bool ReadOnly
        {
            set
            {
                dtgErroriRilevati.IsReadOnly = value;
            }
        }


        public void LoadDataSource(ref XmlDataProviderManager x, string ID)
        {
            _x = x;

            Binding b = new Binding();
            b.Source = _x.xdp;
            b.XPath = "/Dati/Dato[@ID]/Valore[@tipo='ErroriRilevati']";
            dtgErroriRilevati.SetBinding(ItemsControl.ItemsSourceProperty, b);
        }
        
        private void AggiungiNodo(string Alias, string ID)
        {
            if (_x.Document.SelectSingleNode("//Dati/Dato[@ID]/Valore[@tipo='ErroriRilevati'][@name='" + Alias + "']") == null || Alias == "")
            {
                string xml = "<Valore tipo=\"ErroriRilevati\" " + ((Alias == "") ? " new=\"true\" " : " ") + " name=\"" + Alias + "\" importo=\"0\"/>";

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);

                XmlNode tmpNode = doc.SelectSingleNode("/Valore");

                XmlNode importedNode = _x.xdp.Document.ImportNode(tmpNode, true);

				//Verifico se c'è un nodo selezionato
				XmlNode node = null;

				if (dtgErroriRilevati.SelectedCells.Count >= 1)
				{
					node = (XmlNode)(dtgErroriRilevati.SelectedCells[0].Item);

					_x.Document.SelectSingleNode("//Dati/Dato[@ID" + ID + "]").InsertAfter(importedNode, node);
				}
				else
				{
					_x.Document.SelectSingleNode("//Dati/Dato[@ID" + ID + "]").AppendChild(importedNode);
				}

                //_x.Save();

                //dtgCapitaleSociale.Items.Refresh();
            }
        }

        private void DeleteTotal()
        {
            if (_x.Document.SelectSingleNode("//Dati/Dato[@ID]/Valore[@tipo='ErroriRilevati'][@name='Totale']") != null)
            {
               // _x.Document.SelectSingleNode("//Dati/Dato[@ID]").RemoveChild(_x.Document.SelectSingleNode("//Dati/Dato[@ID]/Valore[@tipo='ErroriRilevati'][@name='Totale']"));
            }
        }

        private void GenerateTotal()
        {
            DeleteTotal();

            double importo = 0.0;

            foreach (XmlNode item in _x.Document.SelectNodes("/Dati/Dato[@ID]/Valore[@tipo='ErroriRilevati']"))
            {
                importo += Convert.ToDouble(item.Attributes["importo"].Value);
            }

            string xmlcs = "<Valore tipo=\"ErroriRilevati\" name=\"Totale\" importo=\"" + importo.ToString() + "\" bold=\"true\"/>";

            XmlDocument doccs = new XmlDocument();
            doccs.LoadXml(xmlcs);

            XmlNode tmpNodecs = doccs.SelectSingleNode("/Valore");

            XmlNode importedNodecs = _x.xdp.Document.ImportNode(tmpNodecs, true);

            _x.Document.SelectSingleNode("//Dati/Dato[@ID]").AppendChild(importedNodecs);
        }

        private void DataGrid_SourceUpdated(object sender, DataTransferEventArgs e)
        {
            DataGrid grd = (DataGrid)sender;
            grd.CommitEdit(DataGridEditingUnit.Cell, true);
        }

        public T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                string controlName = child.GetValue(Control.NameProperty) as string;

                if (controlName == name)
                {
                    return child as T;
                }

                else
                {
                    T result = FindVisualChildByName<T>(child, name);

                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }


        private void DataGrid_GotFocus(object sender, RoutedEventArgs e)
        { 
            if (e.OriginalSource.GetType() == typeof(DataGridCell))
            {
                //ComboBox comboBox = FindVisualChildByName<ComboBox>(((DataGridCell)(e.OriginalSource)), "cmb");

                //if (comboBox != null)
                //{
                //    comboBox.Focus();
                //}
                //else
                {
                    DataGrid grd = (DataGrid)sender;
                    grd.BeginEdit(e);
                }
            }
        }

        private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            ;
        }

        private void dtgErroriRilevati_Loaded(object sender, RoutedEventArgs e)
        {
            GenerateTotal();
        }

        private void dtgErroriRilevati_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            GenerateTotal();

            //_x.Save();
        }

        private void AddRowErroriRilevati(object sender, MouseButtonEventArgs e)
        {
            GenerateTotal();
        }

        private void DeleteRowErroriRilevati(object sender, MouseButtonEventArgs e)
        {
			if (MessageBox.Show("Si è sicuri di procedere con l'eliminazione?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
			{
				XmlNode node = null;

				if (dtgErroriRilevati.SelectedCells.Count >= 1)
				{
					node = (XmlNode)(dtgErroriRilevati.SelectedCells[0].Item);
				}
				else
				{
					MessageBox.Show("Selezionare una riga");
					return;
				}

				try
				{
					string ID = node.Attributes["new"].Value;

					node.ParentNode.RemoveChild(node);

					GenerateTotal();

					//_x.Save();

					return;
				}
				catch (Exception ex)
				{
                    cBusinessObjects.logger.Error(ex, "wWorkArea_Excel_ErroriRilevatiRiepilogo.DeleteRowErroriRilevati exception");
                    string log = ex.Message;

					MessageBox.Show("Solo le righe inserite dall'utente possono essere cancellate");
				}
			}
        }
    }
}
