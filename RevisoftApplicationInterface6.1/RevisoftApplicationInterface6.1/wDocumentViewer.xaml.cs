﻿using System;
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
using System.ComponentModel;
using System.Collections;

namespace RevisoftApplication
{
    public partial class DocumentViewer : Window
    {
        public DocumentViewer(string file)
        {
            InitializeComponent();
            
            webbrowser1.Navigate("file:///" + file);
        }
    }
}
