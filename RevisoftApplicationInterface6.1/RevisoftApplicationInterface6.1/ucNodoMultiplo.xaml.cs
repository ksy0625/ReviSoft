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
using UserControls2;
using System.Data;

namespace UserControls
{
    public partial class ucNodoMultiplo : UserControl
    {
        public int id;
        private DataTable dati = null;
        private XmlDataProviderManager _x;
        private string _ID;

        public ucNodoMultiplo()
        {
            InitializeComponent();            
        }

        private bool _ReadOnly = true;

        public bool ReadOnly
        {
            set
            {
                _ReadOnly = value;
            }
        }

		public void Load(ref XmlDataProviderManager x, string ID, string tab, Hashtable Sessioni, int SessioneNow, string Tab, string IDTree,string IDCliente)
        {
            _x = x;
            _ID = ID;
            id = int.Parse(ID.ToString());
            cBusinessObjects.idcliente = int.Parse(IDCliente.ToString());
            cBusinessObjects.idsessione = int.Parse(Sessioni[SessioneNow].ToString());

            TabItem ti;
            dati = cBusinessObjects.GetData(id, typeof(NodoMultiplo));
            foreach (DataRow dtrow in dati.Rows)
            {
               

                ti = new TabItem();
                ti.Header = dtrow["Tab"].ToString();

                switch (dtrow["Tipologia"].ToString())
                {
                    case "Testo":
                        ucTesto Testo = new ucTesto();
                        Testo.ReadOnly = _ReadOnly;
                        Testo.Load(dtrow["ID"].ToString(), IDCliente, Sessioni[SessioneNow].ToString());

                        ti.Content = Testo;
                        break;

                    case "Tabella":
                        ucTabella Tabella = new ucTabella();
                        Tabella.ReadOnly = _ReadOnly;
                        Tabella.Load(dtrow["ID"].ToString(), "", IDTree, "",IDCliente, Sessioni[SessioneNow].ToString());

                        ti.Content = Tabella;
                        break;

                    case "Tabella Replicabile":
                        ucTabellaReplicata TabellaReplicata = new ucTabellaReplicata();
                        TabellaReplicata.ReadOnly = _ReadOnly;
						TabellaReplicata.Load(dtrow["ID"].ToString(), dtrow["Tab"].ToString(), IDTree, IDCliente,Sessioni[SessioneNow].ToString());

                        ti.Content = TabellaReplicata;
                        break;

                    case "Check List":
                        ucCheckList CheckList = new ucCheckList();
                        CheckList.ReadOnly = _ReadOnly;
                        CheckList.Load(dtrow["ID"].ToString(), IDCliente, Sessioni[SessioneNow].ToString());

                        ti.Content = CheckList;
                        break;

                    case "Check List +":
                        ucCheckListPlus CheckListPlus = new ucCheckListPlus();
                        CheckListPlus.ReadOnly = _ReadOnly;
                        CheckListPlus.Load(dtrow["ID"].ToString(), IDCliente, Sessioni[SessioneNow].ToString());

                        ti.Content = CheckListPlus;
                        break;

                    case "Nodo Multiplo":
                        ucNodoMultiplo NodoMultiplo = new ucNodoMultiplo();
                        NodoMultiplo.ReadOnly = _ReadOnly;
						NodoMultiplo.Load(ref _x, dtrow["ID"].ToString(), dtrow["Tab"].ToString(),  Sessioni, SessioneNow, Tab, IDTree,IDCliente);

                        ti.Content = NodoMultiplo;
                        break;

					case "Excel: Errori Rilevati":
						uc_Excel_ErroriRilevati uce_er = new uc_Excel_ErroriRilevati();

						try
						{
                            uce_er.LoadDataSource(dtrow["ID"].ToString(), IDCliente, Sessioni[SessioneNow].ToString());
                        }
						catch (Exception ex)
						{
							string log = ex.Message;
							
                        }

						ti.Content = uce_er;
						break;

					case "Excel: Bilancio":

						uc_Excel_Bilancio uce_b = new uc_Excel_Bilancio(1);

						try
						{
							XmlDataProviderManager _x_AP = null;

                            //if (Sessioni.Contains((SessioneNow + 1)))
                            //{
                            //	_x_AP = new XmlDataProviderManager(Sessioni[(SessioneNow + 1)].ToString());
                            //}

                            uce_b.LoadDataSource(ref _x, ID, _x_AP, App.AppDataFolder + "\\" + Tab, IDCliente, cBusinessObjects.idsessione.ToString());
						}
						catch (Exception ex)
						{
							string log = ex.Message;
							break;// uce_b.LoadDataSource(ref _x, "-1", null);
						}

						ti.Content = uce_b;
						break;

                    default:
                        break;
                } 
                
                tabControl.Items.Add(ti);
            }
        }

        public void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Resizer(Convert.ToInt32(e.NewSize.Width));
        }

        public void Resizer(int newsize)
        {
            foreach (TabItem item in tabControl.Items)
            {
                ((UserControl)(item.Content)).Width = newsize - 20;
            }
        }
    }
}
