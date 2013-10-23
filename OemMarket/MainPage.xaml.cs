using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace OemMarket
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            lstItems.ItemsSource = new List<string>() {LocalizedResources.Default, 
					"Acer",  
					"Fujitsu",  
					"HTC",   
					"LG",
				    "Nokia",
					"Samsung",	
					"ZTE" 
            };
            InteropSvc.InteropLib.Initialize();
            if (!InteropSvc.InteropLib.Instance.HasRootAccess())
            {
                MessageBox.Show(LocalizedResources.NoRootAccess, LocalizedResources.AppTitle, MessageBoxButton.OK);
                throw new Exception("Quit");
            }
            InteropSvc.InteropLib.Instance.RegistrySetDWORD7(InteropSvc.InteropLib.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Zune\\Events", "ZNetSyncState", 0);
            try
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(500);
                InteropSvc.InteropLib.Instance.RegistryGetString7(InteropSvc.InteropLib.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Zune\\Settings", "LKGOemStoreConfigOverride", sb, 500);
                string s = sb.ToString();
                if (String.IsNullOrEmpty(s))
                {
                    lstItems.SelectedIndex = 0;
                }
                else
                {
                    if (s.StartsWith("\\Windows\\Custom_LKG_OEMStoreConfig_") && s.EndsWith(".xml"))
                    {
                        s = s.Replace("\\Windows\\Custom_LKG_OEMStoreConfig_", "");
                        s = s.Replace(".xml", "");
                    }
                    foreach (var item in lstItems.Items)
                    {
                        if (s == (item as string))
                            lstItems.SelectedItem = item;
                    }
                }
                
            }
            catch (Exception ex)
            {
                lstItems.SelectedIndex = 0;
            }
        }

        private void lstItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstItems.SelectedIndex != -1)
            {
                if (lstItems.SelectedIndex == 0)
                {
                    InteropSvc.InteropLib.Instance.DeleteRegVal(InteropSvc.InteropLib.HKEY_LOCAL_MACHINE, "Software\\Microsoft\\Zune\\Settings", "LKGOemStoreConfigOverride");
                }
                else
                {
                    string s = lstItems.SelectedItems[0] as string;
                    InteropSvc.InteropLib.Instance.CopyFile7("\\Applications\\Install\\239CFF8C-AA2A-49E0-87F8-F0DFB9A6F11A\\Install\\Stores\\Custom_LKG_OEMStoreConfig_" + s + ".xml", 
                        "\\Windows\\Custom_LKG_OEMStoreConfig_" + s + ".xml",
                        false);
                    InteropSvc.InteropLib.Instance.RegistrySetString7(InteropSvc.InteropLib.HKEY_LOCAL_MACHINE,
                        "Software\\Microsoft\\Zune\\Settings",
                        "LKGOemStoreConfigOverride",
                        "\\Windows\\Custom_LKG_OEMStoreConfig_" + s + ".xml");
                    InteropSvc.InteropLib.Instance.ReloadMarketplaceConfigs();
                }

            }
        }
    }
}