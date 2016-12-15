using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using ReSharperTutorials.TutStep;

namespace ReSharperTutorials.Utils
{
    public static class TutorialXmlReader
    {

        public static int ReadCurrentStep(string path)
        {            
            using (var reader = XmlReader.Create(new StreamReader(path)))
            {
                while (reader.ReadToFollowing("currentStep"))
                {
                    return Convert.ToInt32(reader.ReadElementContentAsString());                    
                }
            }

            throw new Exception("Missing tutorial content.Please reinstall the plugin!");
        }

        public static void WriteCurrentStep(string path, string value)
        {            
            var doc = new XmlDocument();
            doc.Load(path);
            var node = doc.SelectSingleNode("/tutorial/" + "currentStep");
            if (node != null)
            {
                node.InnerText = value;
            }
            else
            {
                XmlNode root = doc.DocumentElement;
                var elem = doc.CreateElement("currentStep");
                elem.InnerText = value;
                root?.AppendChild(elem);
            }
            doc.Save(path);
            doc = null;          
        }

        public static string ReadIntro(string path)
        {
            using (var reader = XmlReader.Create(new StreamReader(path)))
            {
                while (reader.ReadToFollowing("intro"))
                {
                    return reader.ReadInnerXml();
                }
            }            

            return "Missing tutorial content. Please reinstall the plugin!";            
        }

        public static string ReadTitle(string path)
        {
            using (var reader = XmlReader.Create(new StreamReader(path)))
            {
                
                reader.ReadToFollowing("title");
                reader.Read();
                return reader.Value;
            }
        }

        public static Dictionary<int, TutorialStep> ReadTutorialSteps(string path)
        {
            var result = new Dictionary<int, TutorialStep>();            
            NavNode navNode = null;
            Check check = null;
            string text = null;

            using (var reader = XmlReader.Create(new StreamReader(path)))
                {
                while (reader.ReadToFollowing("step"))
                {
                    var li = Convert.ToInt32(reader.GetAttribute("li"));                                                            
                    var nextStep = reader.GetAttribute("nextStep");
                    var strikeOnDone = Convert.ToBoolean(reader.GetAttribute("strikeOnDone"));
                    var subTreeReader = reader.ReadSubtree();

                    while (subTreeReader.Read())
                    {
                        switch (subTreeReader.Name)
                        {
                            case "navigate":
                                var projectName = reader.GetAttribute("project");
                                var file = reader.GetAttribute("file");
                                var typeName = reader.GetAttribute("type");
                                var methodName = reader.GetAttribute("method");
                                var methodNameOccurrence = Convert.ToInt32(reader.GetAttribute("methodOccurrence"));
                                var textToFind = reader.GetAttribute("textToFind");
                                var textToFindOccurrence = Convert.ToInt32(reader.GetAttribute("textToFindOccurrence"));
                                var runMethod = reader.GetAttribute("runMethod");
                                navNode = new NavNode(projectName, file, typeName, methodName, methodNameOccurrence, textToFind, 
                                    textToFindOccurrence, runMethod);
                                break;

                            case "check":
                                var action = reader.GetAttribute("action");
                                var method = reader.GetAttribute("method");
                                string[] actions = null;
                                
                                if (action != null || method != null)
                                {
                                    if (action != null)                                   
                                        actions = Regex.Split(action, ";");                                        
                                    
                                    check = new Check(actions, method);                                                                                                            
                                }
                                break;

                            case "text":
                                text = reader.ReadInnerXml();
                                text = Regex.Replace(text, @"\s+", " ");
                                break;                                
                        }
                    }
                                        
                    var step = new TutorialStep(li, navNode, check, text, nextStep, strikeOnDone);                    
                    result.Add(li, step);
                    navNode = null;
                    check = null;
                }
            }
            return result;
        }        
    }        
}
