using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using System.Data;


namespace RevisoftApplication
{
    class GenericTable
    {
        public DataTable dati;
        public string filtercolumn = "";
        public string filtervalue = "";
        public bool xml=true;

        private XmlDataProviderManager _x;
        private Grid gridcontainer = null;
        private bool ReadOnly = false;

        public event EventHandler TotalToBeCalculated = delegate { };
        public event EventHandler TotalHasBeenCalculated = delegate { }; 
        public int row = 0;

        private int numcolumn = 0;
        private string[] columnsAlias = { };
        private string[] columnsValues = { };
        private double[] columnsWidth = { };
        private double totalwidth = 0.0;
        private double[] columnsMinWidth = { };
        private string[] columnsTypes = { };
        private string[] columnsAlignment = { };
        private bool[] columnsReadonly = { };        
        private bool[] conditionalReadonly = { };
        private string conditionalAttribute = "";
        private bool[] columnsHasTotal = { };
        private double[] columnsTotals = { };
        private string aliasTotale = "";
        private int columnAliasTotale = 0;
        private string xpath = "";
        private string xpathparentnode = "";
        private string templateNewNode = "";

        public GenericTable( Grid _gridcontainer, bool _ReadOnly)
        {
            
            ReadOnly = _ReadOnly;
            gridcontainer = _gridcontainer;
            gridcontainer.HorizontalAlignment = HorizontalAlignment.Stretch;
        }
        
        public string Xpath
        {
            get
            {
                return xpath;
            }
            set
            {
                xpath = value;
            }
        }

        public string Xpathparentnode
        {
            get
            {
                return xpathparentnode;
            }
            set
            {
                xpathparentnode = value;
            }
        }        

        public string TemplateNewNode
        {
            get
            {
                return templateNewNode;
            }
            set
            {
                templateNewNode = value;
            }
        }
        

        public string[] ColumnsAlias
        {
            get
            {
                return columnsAlias;
            }
            set
            {
                numcolumn = value.Length;
                columnsAlias = value;
            }
        }

        public string[] ColumnsValues
        {
            get
            {
                return columnsValues;
            }
            set
            {
                columnsValues = value;
            }
        }
                
        public double[] ColumnsWidth
        {
            get
            {
                return columnsWidth;
            }
            set
            {
                totalwidth = 0.0;

                for (int i = 0; i < value.Length; i++)
                {
                    totalwidth += value[i];
                }
                 
                columnsWidth = value;
            }
        }

        public double[] ColumnsMinWidth
        {
            get
            {
                return columnsMinWidth;
            }
            set
            {
                columnsMinWidth = value;
            }
        }
                
        public bool[] ColumnsReadOnly
        {
            get
            {
                return columnsReadonly;
            }
            set
            {
                columnsReadonly = value;
            }
        }

        public bool[] ConditionalReadonly
        {
            get
            {
                return conditionalReadonly;
            }
            set
            {
                conditionalReadonly = value;
            }
        }
                
        public string ConditionalAttribute
        {
            get
            {
                return conditionalAttribute;
            }
            set
            {
                conditionalAttribute = value;
            }
        }

        public string AliasTotale
        {
            get
            {
                return aliasTotale;
            }
            set
            {
                aliasTotale = value;
            }
        }

        public int ColumnAliasTotale
        {
            get
            {
                return columnAliasTotale;
            }
            set
            {
                columnAliasTotale = value;
            }
        }

        public string[] ColumnsTypes
        {
            get
            {
                return columnsTypes;
            }
            set
            {
                columnsTypes = value;
            }
        }

        public string[] ColumnsAlignment
        {
            get
            {
                return columnsAlignment;
            }
            set
            {
                columnsAlignment = value;
            }
        }

        public bool[] ColumnsHasTotal
        {
            get
            {
                return columnsHasTotal;
            }
            set
            {
                columnsTotals = new double[value.Length];
                columnsHasTotal = value;
            }
        }        

        public void GenerateTable()
        {
            row = 0;

            gridcontainer.SizeChanged += Gridcontainer_SizeChanged;

            gridcontainer.ColumnDefinitions.Clear();
            gridcontainer.RowDefinitions.Clear();
            gridcontainer.Children.Clear();
            XmlNodeList nl=null;
            if(this.xml)
               nl = _x.Document.SelectNodes(xpath);
            
            /*DEFINIZIONE COLONNE*/
            ColumnDefinition cd;

            for (int i = 0; i < numcolumn; i++)
            {
                cd = new ColumnDefinition();
                cd.Width = new GridLength(columnsWidth[i], GridUnitType.Star);
                cd.MinWidth = columnsMinWidth[i];
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
            if(xml)
            { 
                /*DATI*/
                foreach (XmlNode node in nl)
                {
                    if(columnsHasTotal.Contains(true) && node.Attributes[columnsValues[columnAliasTotale]].Value == aliasTotale)
                    {
                        continue;
                    }

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
                        txt.Name = "txt_" + i.ToString() + "_" + row.ToString();

                        if (columnsReadonly.Length > i && columnsReadonly[i] == true)
                        {
                            txt.IsReadOnly = true;
                            txt.IsTabStop = false;
                        }
                        else
                        {
                            if (conditionalReadonly[i] == true)
                            {
                                if (conditionalAttribute == "" || node.Attributes[conditionalAttribute] == null)
                                {
                                    txt.IsReadOnly = true;
                                    txt.IsTabStop = false;
                                }
                            }
                        }
                    
                        if(node.Attributes["txtfinder"] == null)
                        {
                            XmlAttribute attr = node.OwnerDocument.CreateAttribute("txtfinder");
                            node.Attributes.Append(attr);
                        }

                        node.Attributes["txtfinder"].Value = row.ToString();

                        txt.Text = ((node.Attributes[columnsValues[i]] == null)? "" : node.Attributes[columnsValues[i]].Value);
                        switch (columnsAlignment[i])
                        {
                            case "right":
                                txt.TextAlignment = TextAlignment.Right;
                                break;
                            case "left":
                            default:
                                txt.TextAlignment = TextAlignment.Left;
                                break;
                        }

                        txt.Tag = xpath;
                        txt.TextWrapping = TextWrapping.Wrap;
                        txt.GotFocus += Txt_GotFocus;
                        txt.LostFocus += Txt_LostFocus;
                        txt.PreviewKeyDown += Txt_PreviewKeyDown;
                        txt.PreviewMouseDown += Txt_PreviewMouseDown;
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
            else
            {
                /*DATI*/
                foreach (DataRow dtrow in this.dati.Rows)
                {            
                    if(filtercolumn!="")
                    {
                        if (dtrow[filtercolumn].ToString() != filtervalue)
                            continue;
                    }
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
                        txt.Name = "txt_" + i.ToString() + "_" + row.ToString();

                        if (columnsReadonly.Length > i && columnsReadonly[i] == true)
                        {
                          

                                    txt.IsReadOnly = true;
                                    txt.IsTabStop = false;


                        }
                        else
                        {
                            if (conditionalReadonly[i] == true)
                            {
                                if (conditionalAttribute == "" )
                                {
                                    txt.IsReadOnly = true;
                                    txt.IsTabStop = false;
                                }
                            }
                        }


                        dtrow["txtfinder"] = row.ToString();

                        txt.Text = ((dtrow[columnsValues[i]].ToString() == null) ? "" : dtrow[columnsValues[i]].ToString());
                        switch (columnsAlignment[i])
                        {
                            case "right":
                                txt.TextAlignment = TextAlignment.Right;
                                break;
                            case "left":
                            default:
                                txt.TextAlignment = TextAlignment.Left;
                                break;
                        }

                        txt.Tag = xpath;
                        txt.TextWrapping = TextWrapping.Wrap;
                        txt.GotFocus += Txt_GotFocus;
                        txt.LostFocus += Txt_LostFocus;
                        txt.PreviewKeyDown += Txt_PreviewKeyDown;
                        txt.PreviewMouseDown += Txt_PreviewMouseDown;
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
            /*FOOTER*/
            if (columnsHasTotal.Contains(true))
            {
                for (int i = 0; i < numcolumn; i++)
                {
                    rd = new RowDefinition();
                    gridcontainer.RowDefinitions.Add(rd);

                    brd = new Border();
                    brd.BorderThickness = new Thickness(1.0);
                    brd.BorderBrush = Brushes.LightGray;
                    brd.Background = Brushes.LightGray;
                    brd.Padding = new Thickness(2.0);

                    if (columnsHasTotal[i] == true)
                    {
                        txt = new TextBox();
                        txt.Name = "total_" + i.ToString();
                        txt.IsReadOnly = true;
                        txt.IsTabStop = false;
                        txt.Background = Brushes.LightGray;
                        txt.BorderThickness = new Thickness(0.0);
                        txt.FontWeight = FontWeights.Bold;
                        txt.TextAlignment = TextAlignment.Right;
                        brd.BorderThickness = new Thickness(0.0);
                        brd.Child = txt;
                    }
                    else
                    {
                        lbl = new TextBlock();
                        if (i == columnAliasTotale)
                        {
                            lbl.Text = aliasTotale;
                        }

                        lbl.TextAlignment = TextAlignment.Left;
                        lbl.FontWeight = FontWeights.Bold;
                        brd.Child = lbl;
                    }                   

                    gridcontainer.Children.Add(brd);
                    Grid.SetRow(brd, row);
                    Grid.SetColumn(brd, i);
                }

                if (this.xml)
                    GenerateTotalXML();
                else
                    GenerateTotal();
            }
        }

        public void AddRow()
        {
            AddRow(true);
        }
        public void AddRowWithTab(string tabcolumnname,string tabname)
        {
            if ( ReadOnly)
            {
                MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
                return;
            }

            //string name = "";
            int j = 1;
            foreach (Border item in gridcontainer.Children)
            {
                if (item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).Name.Split('_').Count() > 2)
                    if (item.BorderBrush == App._arrBrushes[0])
                    {
                        break;
                    }
                    else
                    {
                        j++;
                    }
            }
            DataRow dr = dati.NewRow();

            dr["isnew"] = "1";
            dr[tabcolumnname] = tabname;
            dati.Rows.InsertAt(dr, j);


            GenerateTable();
        }



        public void AddRow(bool tobechecked)
        {
            if (this.xml)
            {
                if (tobechecked && ReadOnly)
                {
                    MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
                    return;
                }

                string name = "";

                foreach (Border item in gridcontainer.Children)
                {
                    if (item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).Name.Split('_').Count() > 2)
                    {
                        name = ((TextBox)(item.Child)).Name;
                    }

                    if (item.BorderBrush == App._arrBrushes[0])
                    {
                        break;
                    }
                }

                XmlNodeList nl = _x.Document.SelectNodes(xpath);

                XmlNode node = null;

                foreach (XmlNode nodehere in nl)
                {
                    if (nodehere.Attributes["txtfinder"] != null && name.Split('_').Count() > 2 && nodehere.Attributes["txtfinder"].Value == name.Split('_')[2])
                    {
                        node = nodehere;
                        break;
                    }
                }

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(templateNewNode);

                XmlNode tmpNode = doc.SelectSingleNode("/Valore");

                XmlNode importedNode = _x.Document.ImportNode(tmpNode, true);

                foreach (string item in columnsValues)
                {
                    if (importedNode.Attributes[item] == null)
                    {
                        XmlAttribute attr = importedNode.OwnerDocument.CreateAttribute(item);
                        importedNode.Attributes.Append(attr);
                        importedNode.Attributes[item].Value = "";
                    }
                }

                if (name != "")
                {
                    _x.Document.SelectSingleNode(Xpathparentnode).InsertAfter(importedNode, node);
                }
                else
                {
                    _x.Document.SelectSingleNode(Xpathparentnode).AppendChild(importedNode);
                }

                //_x.Save();

                GenerateTable();
            }
            else
            {
             if (tobechecked && ReadOnly)
                {
                    MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
                    return;
                }

                //string name = "";
                int j = 1;
                foreach (Border item in gridcontainer.Children)
                {
                    if (item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).Name.Split('_').Count() > 2)
                        if (item.BorderBrush == App._arrBrushes[0])
                        {
                            break;
                        }
                        else
                        {
                            j++;
                        }
                }
                DataRow dr = dati.NewRow();
             
                dr["isnew"] = "1";
                dati.Rows.InsertAt(dr, j);
               

                GenerateTable();
            }
        }

        public void DeleteRow()
        {
           if(this.xml)
            {

                if (ReadOnly)
                {
                    MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
                    return;
                }

                if (MessageBox.Show("Si è sicuri di procedere con l'eliminazione?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    string name = "";

                    foreach (Border item in gridcontainer.Children)
                    {
                        if (item.BorderBrush == App._arrBrushes[0])
                        {
                            name = ((TextBox)(item.Child)).Name;
                            break;
                        }
                    }

                    XmlNodeList nl = _x.Document.SelectNodes(xpath);

                    XmlNode node = null;

                    foreach (XmlNode nodehere in nl)
                    {
                        if (nodehere.Attributes["txtfinder"] != null && name.Split('_').Count() > 2 && nodehere.Attributes["txtfinder"].Value == name.Split('_')[2])
                        {
                            node = nodehere;
                            break;
                        }
                    }

                    if (node == null)
                    {
                        MessageBox.Show("Selezionare una riga");
                        return;
                    }

                    if (conditionalAttribute != "" && node.Attributes[conditionalAttribute] == null)
                    {
                        MessageBox.Show("Solo le righe inserite dall'utente possono essere cancellate");
                        return;
                    }
                    else
                    {
                        node.ParentNode.RemoveChild(node);

                        //_x.Save();

                        GenerateTable();
                    }
                }
            }
           else
            {
                if (ReadOnly)
                {
                    MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
                    return;
                }

                if (MessageBox.Show("Si è sicuri di procedere con l'eliminazione?", "Attenzione", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    string name = "";

                    foreach (Border item in gridcontainer.Children)
                    {
                        if (item.BorderBrush == App._arrBrushes[0])
                        {
                            name = ((TextBox)(item.Child)).Name;
                            break;
                        }
                    }
                    bool trovato = false;
                    foreach (DataRow dtrow in this.dati.Rows)
                    {
                        if (filtercolumn != "")
                        {
                            if (dtrow[filtercolumn].ToString() != filtervalue)
                                continue;
                        }
                        if (dtrow["txtfinder"] != null && name.Split('_').Count() > 2 && dtrow["txtfinder"].ToString() == name.Split('_')[2])
                        {
                           
                             if (conditionalAttribute != "" && dtrow["isnew"].ToString() != "1")
                             {
                              MessageBox.Show("Solo le righe inserite dall'utente possono essere cancellate");
                              return;
                            }
                            dtrow.Delete();
                            trovato = true;
                            break;
                        }
                    }
                    this.dati.AcceptChanges();
                    if (!trovato)
                    {
                        MessageBox.Show("Selezionare una riga");
                        return;
                    }

                    GenerateTable();
                }
           }
        }


        private void GenerateTotal()
        {
            for (int i = 0; i < columnsTotals.Length; i++)
            {
                columnsTotals[i] = 0.0;
            }

            foreach (Border item in gridcontainer.Children)
            {
                if (item.Child != null && item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).Name.Split('_').Count() > 1)
                {
                    int idcolumn = Convert.ToInt32(((TextBox)(item.Child)).Name.Split('_')[1]);

                    if (columnsHasTotal[idcolumn])
                    {
                        double valuehere = 0.0;
                        DataRow datarow = null;
                        foreach (DataRow dtrow in this.dati.Rows)
                        {
                            if (filtercolumn != "")
                            {
                                if (dtrow[filtercolumn].ToString() != filtervalue)
                                    continue;
                            }
                            if (dtrow["txtfinder"] != null && ((TextBox)(item.Child)).Name.Split('_').Count() > 2 && dtrow["txtfinder"].ToString() == ((TextBox)(item.Child)).Name.Split('_')[2])
                           {
                                datarow = dtrow;
                                break;
                            }
                        }


                        if (datarow != null)
                        {
                            double.TryParse(datarow[columnsValues[idcolumn]].ToString(), out valuehere);
                            columnsTotals[idcolumn] += valuehere;
                        }
                    }
                }
            }

            foreach (Border item in gridcontainer.Children)
            {
                if (item.Child != null && item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).Name.Split('_').Count() > 1 && ((TextBox)(item.Child)).Name.Contains("total_"))
                {
                    int idcolumn = Convert.ToInt32(((TextBox)(item.Child)).Name.Split('_')[1]);

                    if (columnsHasTotal[idcolumn])
                    {
                        string valuehere = "";

                        switch (columnsTypes[idcolumn])
                        {
                            case "money":
                                valuehere = ConvertMoney((columnsTotals[idcolumn]).ToString());
                                break;
                            case "percent":
                                valuehere = ConvertPercent((columnsTotals[idcolumn]).ToString());
                                break;
                            case "int":
                                valuehere = ConvertInt((columnsTotals[idcolumn]).ToString());
                                break;
                            case "string":
                            default:
                                break;
                        }

                        ((TextBox)(item.Child)).Text = valuehere;

                    }
                }
            }

            TotalHasBeenCalculated("", new RoutedEventArgs());
        }

        private void GenerateTotalXML()
        {
            for (int i = 0; i < columnsTotals.Length; i++)
            {
                columnsTotals[i] = 0.0;
            }

            foreach (Border item in gridcontainer.Children)
            {
                if (item.Child != null && item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).Name.Split('_').Count() > 1)
                {
                    int idcolumn = Convert.ToInt32(((TextBox)(item.Child)).Name.Split('_')[1]);

                    if(columnsHasTotal[idcolumn])
                    {
                        double valuehere = 0.0;

                      

                        foreach (DataRow dtrow in this.dati.Rows)
                        {
                            if (dtrow["txtfinder"] != null && ((TextBox)(item.Child)).Name.Split('_').Count() > 2 && dtrow["txtfinder"].ToString() == ((TextBox)(item.Child)).Name.Split('_')[2])
                            {
                                double.TryParse(dtrow[columnsValues[idcolumn]].ToString(), out valuehere);
                                columnsTotals[idcolumn] += valuehere;
                                break;
                            }
                        }

                    
                    }
                }
            }

            XmlNode nodetotal = _x.Document.SelectSingleNode(xpath + "[@" + columnsValues[columnAliasTotale] + "='" + aliasTotale + "']");

            if (nodetotal == null)
            {
                XmlNode node = null;

                foreach (Border item in gridcontainer.Children)
                {
                    if (item.Child != null && item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).Name.Split('_').Count() > 2)
                    {
                        XmlNodeList nl = _x.Document.SelectNodes(xpath);
                        
                        foreach (XmlNode nodehere in nl)
                        {
                            node = nodehere;
                        }
                    }
                }

                if (node == null)
                {
                    return;
                }

                XmlNode tmpNode = node.CloneNode(true);

                tmpNode.Attributes[columnsValues[columnAliasTotale]].Value = aliasTotale;

                if (tmpNode.Attributes["ID"] != null)
                {
                    tmpNode.Attributes.RemoveNamedItem("ID");
                }

                XmlAttribute bold = tmpNode.OwnerDocument.CreateAttribute("bold");
                bold.Value = "true";
                tmpNode.Attributes.Append(bold);

                XmlNode importedNode = _x.Document.ImportNode(tmpNode, true);

                node.ParentNode.InsertAfter(importedNode, node);
                
                nodetotal = _x.Document.SelectSingleNode(xpath + "[@" + columnsValues[columnAliasTotale] + "='" + aliasTotale + "']");
            }

            foreach (Border item in gridcontainer.Children)
            {
                if (item.Child != null && item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).Name.Split('_').Count() > 1 && ((TextBox)(item.Child)).Name.Contains("total_"))
                {
                    int idcolumn = Convert.ToInt32(((TextBox)(item.Child)).Name.Split('_')[1]);

                    if (columnsHasTotal[idcolumn])
                    {
                        string valuehere = "";

                        switch (columnsTypes[idcolumn])
                        {
                            case "money":
                                valuehere = ConvertMoney((columnsTotals[idcolumn]).ToString());
                                break;
                            case "percent":
                                valuehere = ConvertPercent((columnsTotals[idcolumn]).ToString());
                                break;
                            case "int":
                                valuehere =  ConvertInt((columnsTotals[idcolumn]).ToString());
                                break;
                            case "string":
                            default:
                                break;
                        }

                        ((TextBox)(item.Child)).Text = valuehere;
                        nodetotal.Attributes[columnsValues[idcolumn]].Value = valuehere;
                    }
                }
            }
            
            TotalHasBeenCalculated("", new RoutedEventArgs());
        }

        public string GenerateSpecificTotal(string idcolumn)
        {
            double columnTotals = 0.0;
            if (this.xml)
            {
                foreach (Border item in gridcontainer.Children)
                {
                    if (item.Child != null && item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).Name.Split('_').Count() > 1)
                    {
                        if (idcolumn == ((TextBox)(item.Child)).Name.Split('_')[1])
                        {
                            double valuehere = 0.0;

                            XmlNodeList nl = _x.Document.SelectNodes(xpath);

                            XmlNode node = null;

                            foreach (XmlNode nodehere in nl)
                            {
                                if (nodehere.Attributes["txtfinder"] != null && ((TextBox)(item.Child)).Name.Split('_').Count() > 2 && nodehere.Attributes["txtfinder"].Value == ((TextBox)(item.Child)).Name.Split('_')[2])
                                {
                                    node = nodehere;
                                    break;
                                }
                            }

                            if (node != null)
                            {
                                double.TryParse(node.Attributes[columnsValues[Convert.ToInt32(idcolumn)]].Value.ToString(), out valuehere);
                                columnTotals += valuehere;
                            }
                        }
                    }
                }

                if (columnsTypes.Count() > Convert.ToInt32(idcolumn))
                {
                    switch (columnsTypes[Convert.ToInt32(idcolumn)])
                    {
                        case "money":
                            return ConvertMoney(columnTotals.ToString());
                        case "percent":
                            return ConvertPercent(columnTotals.ToString());
                        case "int":
                            return ConvertInt(columnTotals.ToString());
                        case "string":
                        default:
                            break;
                    }
                }

                return "";
            }
            else
            {
                foreach (Border item in gridcontainer.Children)
                {
                    if (item.Child != null && item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).Name.Split('_').Count() > 1)
                    {
                        if (idcolumn == ((TextBox)(item.Child)).Name.Split('_')[1])
                        {
                            double valuehere = 0.0;

                            DataRow datar = null;
                            foreach (DataRow dtrow in this.dati.Rows)
                            {
                                if (filtercolumn != "")
                                {
                                    if (dtrow[filtercolumn].ToString() != filtervalue)
                                        continue;
                                }
                                if (dtrow["txtfinder"] != null && ((TextBox)(item.Child)).Name.Split('_').Count() > 2 && dtrow["txtfinder"].ToString() == ((TextBox)(item.Child)).Name.Split('_')[2])
                                {
                                    datar = dtrow;
                                    break;
                                }
                            }

                            if (datar != null)
                            {
                                double.TryParse(datar[columnsValues[Convert.ToInt32(idcolumn)]].ToString(), out valuehere);
                                columnTotals += valuehere;
                            }
                        }
                    }
                }

                if (columnsTypes.Count() > Convert.ToInt32(idcolumn))
                {
                    switch (columnsTypes[Convert.ToInt32(idcolumn)])
                    {
                        case "money":
                            return ConvertMoney(columnTotals.ToString());
                        case "percent":
                            return ConvertPercent(columnTotals.ToString());
                        case "int":
                            return ConvertInt(columnTotals.ToString());
                        case "string":
                        default:
                            break;
                    }
                }

                return "";
            }
        }

        public string GetValue(string idcolumn, string idrow)
        {
            foreach (Border item in gridcontainer.Children)
            {
                if (item.Child != null && item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).Name.Split('_').Count() > 2)
                {
                    if (idcolumn == ((TextBox)(item.Child)).Name.Split('_')[1] && idrow == ((TextBox)(item.Child)).Name.Split('_')[2])
                    {
                        return ((TextBox)(item.Child)).Text;
                    }
                }
            }

            return "";
        }

        public void SetValue(string idcolumn, string idrow, string value)
        {
            foreach (Border item in gridcontainer.Children)
            {
                if (item.Child != null && item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).Name.Split('_').Count() > 2)
                {
                    if (idcolumn == ((TextBox)(item.Child)).Name.Split('_')[1] && idrow == ((TextBox)(item.Child)).Name.Split('_')[2])
                    {
                        ((TextBox)(item.Child)).Text = value;
       
                        foreach (DataRow dtrow in this.dati.Rows)
                        {
                            if (filtercolumn != "")
                            {
                                if (dtrow[filtercolumn].ToString() != filtervalue)
                                    continue;
                            }
                            if (dtrow["txtfinder"] != null && dtrow["txtfinder"].ToString() == idrow)
                            {
                        //     if(dati.Columns[columnsValues[Convert.ToInt32(idcolumn)]].DataType==typeof(System.Double))
                             try
                              {
                                dtrow[columnsValues[Convert.ToInt32(idcolumn)]] = value;
                              }
                              catch (Exception)
                              {
                              
                              }
                            }
                        }

                    }
                }
            }
        }
        
        public string GetTotalValue(string idcolumn)
        {
            foreach (Border item in gridcontainer.Children)
            {
                if (item.Child != null && item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).Name.Split('_').Count() == 2)
                {
                    if ("total" == ((TextBox)(item.Child)).Name.Split('_')[0] && idcolumn == ((TextBox)(item.Child)).Name.Split('_')[1])
                    {
                        return ((TextBox)(item.Child)).Text;
                    }
                }
            }

            return "";
        }

        public void SetTotalValue(string idcolumn, string value)
        {
            foreach (Border item in gridcontainer.Children)
            {
                if (item.Child != null && item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).Name.Split('_').Count() == 2)
                {
                    if ("total" == ((TextBox)(item.Child)).Name.Split('_')[0] && idcolumn == ((TextBox)(item.Child)).Name.Split('_')[1])
                    {
                        ((TextBox)(item.Child)).Text = value;
                        return;
                    }
                }
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
                        if (((TextBox)(item.Child)).Name.Split('_').Count() > 1 && i == Convert.ToInt32(((TextBox)(item.Child)).Name.Split('_')[1]))
                        {
                            ((TextBox)(item.Child)).Width = ((e.NewSize.Width - 10.0) / totalwidth * columnsWidth[i]) - 4.0;
                        }
                    }
                }
            }
        }

        private void Txt_GotFocus(object sender, RoutedEventArgs e)
        {
            foreach (Border item in gridcontainer.Children)
            {
                if (item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).Name.Split('_').Count() > 2 && ((TextBox)(item.Child)).Name.Split('_')[2] == ((TextBox)sender).Name.Split('_')[2])
                {
                    item.BorderBrush = App._arrBrushes[0];
                }
                else
                {
                    item.BorderBrush = Brushes.LightGray;
                }
            }

            ((TextBox)sender).Focus();
            ((TextBox)sender).SelectAll();// (((TextBox)sender).Text.Length, 0);
        }

        private void Txt_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (ReadOnly)
            {
                MessageBox.Show(App.MessaggioSolaScrittura, "Attenzione");
                return;
            }
        }

        private void Txt_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (ReadOnly)
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
            if(((TextBox)sender).Name.Split('_').Count() < 2)
            {
                return;
            }

            string value = ((TextBox)sender).Text;

            int idcolumn = Convert.ToInt32(((TextBox)sender).Name.Split('_')[1]);

            switch (columnsTypes[idcolumn])
            {
                case "money":
                    value = ConvertMoney(value);
                    break;
                case "percent":
                    value = ConvertPercent(value);
                    break;
                case "int":
                    value = ConvertInt(value);
                    break;
                case "string":
                default:
                    break;
            }

            if (this.xml)
            {
                //PRISC NUOVA MODIFICA PER XPATH
                XmlNodeList nl = _x.Document.SelectNodes(((TextBox)sender).Tag.ToString());

                XmlNode node = null;

                foreach (XmlNode nodehere in nl)
                {
                    if (nodehere.Attributes["txtfinder"] != null && ((TextBox)sender).Name.Split('_').Count() > 2 && nodehere.Attributes["txtfinder"].Value == ((TextBox)sender).Name.Split('_')[2])
                    {
                        node = nodehere;
                        break;
                    }
                }

                TotalToBeCalculated(((TextBox)sender).Name, e);

                if (node != null)
                {
                    if (node.Attributes[columnsValues[idcolumn]] == null)
                    {
                        XmlAttribute attr = node.OwnerDocument.CreateAttribute(columnsValues[idcolumn]);
                        node.Attributes.Append(attr);
                    }
                    node.Attributes[columnsValues[idcolumn]].Value = value;

                    ((TextBox)sender).Text = value;

                    if (columnsHasTotal[idcolumn])
                    {
                        if (this.xml)
                            GenerateTotalXML();
                        else
                            GenerateTotal();
                    }
                }

                TotalToBeCalculated(((TextBox)sender).Name, e);
                //_x.Save();

            }
            else
            {
                DataRow dtcurrentrow = null;
                foreach (DataRow dtrow in this.dati.Rows)
                {
                    if (filtercolumn != "")
                    {
                        if (dtrow[filtercolumn].ToString() != filtervalue)
                            continue;
                    }
                    if (dtrow["txtfinder"] != null && ((TextBox)sender).Name.Split('_').Count() > 2 && dtrow["txtfinder"].ToString() == ((TextBox)sender).Name.Split('_')[2])
                    {
                        dtcurrentrow = dtrow;
                        break;
                    }
                }

                TotalToBeCalculated(((TextBox)sender).Name, e);

                if (dtcurrentrow != null)
                {
                    try
                    {
                      dtcurrentrow[columnsValues[idcolumn]] = value;
                    }
                    catch (Exception)
                    {

                    }
         

                    ((TextBox)sender).Text = value;

                    if (columnsHasTotal[idcolumn])
                    {
                        if (this.xml)
                            GenerateTotalXML();
                        else
                            GenerateTotal();
                    }
                }

                TotalToBeCalculated(((TextBox)sender).Name, e);
             
            }

        }

        public void SetFocus()
        {
            foreach (Border item in gridcontainer.Children)
            {
                if (item.Child != null && item.Child.GetType().Name == "TextBox" && ((TextBox)(item.Child)).Name.Split('_').Count() > 1)
                {
                    ((TextBox)(item.Child)).Focus();
                    ((TextBox)(item.Child)).SelectAll();
                    return;
                }
            }
        }

        private string ConvertMoney(string valore)
        {
            double dblValore = 0.0;

            double.TryParse(valore, out dblValore);

            if (dblValore == 0.0)
            {
                return "";
            }
            else
            {
                return String.Format("{0:#,#.00}", dblValore);
            }
        }

        private string ConvertPercent(string valore)
        {
            double dblValore = 0.0;

            double.TryParse(valore, out dblValore);

            if (dblValore == 0.0)
            {
                return "";
            }
            else
            {
                return String.Format("{0:#,#.00}", dblValore * 100);
            }
        }

        private string ConvertInt(string valore)
        {
            int dblValore = 0;

            int.TryParse(valore, out dblValore);

            if (dblValore == 0)
            {
                return "";
            }
            else
            {
                return String.Format("{0:#,#}", dblValore);
            }
        }
    }
}
