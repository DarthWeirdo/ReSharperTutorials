using System;
using System.Diagnostics;

namespace ReSharper20163
{
    public class LanguageInjections
    {
        private const string Style = @".title
            {
                color: red;
                font-size: 12pt;
                font-weight: bold;                            
            }";

        // language=
        private string _script = "alert('Hello World!');";

        //language=css
        private string toolBarColor = "green";    
    }
}