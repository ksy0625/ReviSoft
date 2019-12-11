using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Windows;
using System.Collections;



namespace RevisoftApplication
{
    class cXBRL
    {
		public Hashtable htXBRL = new Hashtable();
		public Hashtable htSegno = new Hashtable();

		public cXBRL(string file)
        {
			XmlDataProviderManager _y = new XmlDataProviderManager(file, true);

			foreach (XmlNode item in _y.Document.SelectNodes("/xbrl//Nodo"))
			{
				htXBRL.Add(item.Attributes["nome"].Value, item.Attributes["ID"].Value);
				if (item.Attributes["segno"] != null)
				{
					htSegno.Add(item.Attributes["nome"].Value, item.Attributes["segno"].Value);
				}
			}		
        }
    }
}
