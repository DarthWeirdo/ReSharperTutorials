namespace ReSharperTutorials.TutStep
{
    public class NavNode
    {
        public readonly string ProjectName;
        public readonly string FileName;
        public readonly string TypeName;
        public readonly string MethodName;
        public readonly int MethodNameOccurrence;
        public readonly string TextToFind;
        public readonly int TextToFindOccurrence;
        public readonly string RunMethod;


        public NavNode(string projectName, string fileName, string typeName, string methodName, int methodNameOccurrence, string textToFind, int textToFindOccurrence, string runMethod)
        {
            ProjectName = projectName;
            FileName = fileName;
            TypeName = typeName;
            MethodName = methodName;
            MethodNameOccurrence = methodNameOccurrence;
            TextToFind = textToFind;
            TextToFindOccurrence = textToFindOccurrence;
            RunMethod = runMethod;
        }        

    }
}
