using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.DocumentModel;
using JetBrains.ProjectModel;
using JetBrains.ReSharper.Resources.Shell;
using JetBrains.TextControl;

namespace ReSharperTutorials
{
    [ZoneMarker]
    public class ZoneMarker :
        IRequire<IEnvironmentZone>, 
        IRequire<IProjectModelZone>, 
        IRequire<IDocumentModelZone>,  
        IRequire<ITextControlsZone>, 
        IRequire<PsiFeaturesImplZone>
    {
    }

}
