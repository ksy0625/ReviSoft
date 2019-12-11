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

namespace RevisoftApplication
{
    public partial class wWorkAreaTesto : Window
    {
        //private XmlDataProviderManager _x;
        
        public wWorkAreaTesto()
        {   
            InitializeComponent();
        }

        public void LoadDataSource( string ID,string IDCliente,string IDSessione)
        {
           

            Testo.Load( ID,IDCliente,IDSessione);
        }

        public bool ReadOnly 
        {
            set
            {
                Testo.ReadOnly = value;
            }
        }
    }
}
