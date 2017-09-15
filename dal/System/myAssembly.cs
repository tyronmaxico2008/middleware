using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Reflection;
namespace System
{

    public class myAssembly
    {
        private System.Collections.Generic.Dictionary<string, Assembly> lst = new Dictionary<string, Assembly>();

        
        public string appServerRootPath { get; set; }
        public string appName { get; set; }

        public myAssembly(string sAppServerRootPath)
        {
            appServerRootPath = sAppServerRootPath;
        }

        public myAssembly(string sAppServerRootPath,string sAppName)
        {
            appServerRootPath = sAppServerRootPath;
            appName = sAppName;
        }


        public myAssembly() { 

        }

        private string getPackagePath(string packageFile)
        {
            string sPath="";
            if (!packageFile.isEmpty())
            {
                sPath = getPackagePath2(packageFile);
                if (System.IO.File.Exists(sPath)) return sPath;
            }

            sPath = appServerRootPath + "\\packages\\" + packageFile;

            return sPath;
        }

        private string getPackagePath2(string packageFile)
        {
            string sPath = appServerRootPath + "\\apps\\" + appName + "\\packages\\" + packageFile;

            return sPath;
        }


        public void add(string sPackageName)
        {
            string sPath = getPackagePath(sPackageName);
            string sFileName = System.IO.Path.GetFileNameWithoutExtension(sPath);
            var asm = System.Reflection.Assembly.LoadFile(sPath);
            lst.Add(sFileName, asm);
        }

        public object createInstance(string sAssemblyName, string sClassPath)
        {

            if (lst.ContainsKey(sAssemblyName))
                return lst[sAssemblyName].CreateInstance(sClassPath);
            else
                return AppDomain.CurrentDomain.CreateInstance(sAssemblyName, sClassPath).Unwrap();


        }


        public void loadDll(XmlDocument xmlDoc)
        {
            XmlNodeList package_nodes = xmlDoc.SelectNodes("//request/packages/package");
            if (package_nodes.Count == 0)
                package_nodes = xmlDoc.SelectNodes("//appConfig/packages/package");

            
            foreach (XmlNode package_node in package_nodes)
            {
                string sPackageName = package_node.getXmlAttributeValue("assemblyName");

                
                add(sPackageName);
            }
        }
            
    }
}
