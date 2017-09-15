using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PropertiesGenerator
{
    /// <summary>
    /// Data class to describe a ViewModel-Property
    /// </summary>
    public class PropertyDesription : ViewModelBase
    {
        public string PropertyType
        {
            get
            {
                return myPropertyType;
            }
            set
            {
                myPropertyType = value;
                RaisePropertyChanged( nameof( PropertyType ) );
            }
        }
        string myPropertyType;

        public string Name
        {
            get
            {
                return myName;
            }
            set
            {
                myName = value;
                RaisePropertyChanged();
            }
        }
        string myName;

        public string Comment
        {
            get
            {
                return myComment;
            }
            set
            {
                myComment = value;
                RaisePropertyChanged();
            }
        }
        string myComment;

        public PropertyDesription( string thePropertyType, string theName, string theComment )
        {
            PropertyType = thePropertyType;
            Name = theName;
            Comment = theComment;
        }
    }

    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand CommandAddRow { get; private set; }
        public ICommand CommandGenerateCode { get; private set; }
        public ICommand CommandCopyToClipboard { get; private set; }

        private static string _ = new String( ' ', 4 );


        public ObservableCollection<string> SupportedTypes
        {
            get { return mySupportedTypes; }
            set
            {
                mySupportedTypes = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<string> mySupportedTypes;

        public ObservableCollection<PropertyDesription> Properties
        {
            get
            {
                return myProps;
            }
            set
            {
                myProps = value;
                RaisePropertyChanged( nameof( Properties ) );
            }
        }
        private ObservableCollection<PropertyDesription> myProps = new ObservableCollection<PropertyDesription>();



        public string SelectedTemplate
        {
            get
            {
                return Templates[SelectedTemplateKey];
            }
        }



        public string SelectedTemplateKey
        {
            get
            {
                return mySelectedTemplateKey;
            }
            set
            {
                mySelectedTemplateKey = value;
                RaisePropertyChanged();
                RaisePropertyChanged( nameof( SelectedTemplate ) );
            }
        }
        private string mySelectedTemplateKey;



        public Dictionary<string, string> Templates
        {
            get
            {
                return myTemplates;
            }
            set
            {
                myTemplates = value;
                RaisePropertyChanged();
            }
        }
        private Dictionary<string, string> myTemplates;

        /// <summary>
        ///Prefix for private field
        /// </summary>
        public string PrivatePrefix
        {
            get
            {
                return myPrivatePrefix;
            }
            set
            {
                myPrivatePrefix = value;
                updateTemplates();
                RaisePropertyChanged();
            }
        }
        private string myPrivatePrefix;

        /// <summary>
        /// Generated source code
        /// </summary>
        public string ResultSourceCode
        {
            get
            {
                return myCode;
            }
            set
            {
                myCode = value;
                RaisePropertyChanged();
            }
        }
        private string myCode;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindowViewModel()
        {
            Templates = new Dictionary<string, string>();

            SupportedTypes = new ObservableCollection<string>()
            {
                "string",
                "bool",
                "int", "long",
                "double", "float", "decimal",
                "object",
                "char",
                "uint", "ulong", "short", "ushort", "byte", "sbyte",
            };

            PrivatePrefix = "my";  // inkl. updateTemplate();

            //create some rows
            for ( int i = 0; i < 10; i++ )
            {
                AddRow();
            }

            CommandAddRow = new RelayCommand( p1 => AddRow() );

            CommandGenerateCode = new RelayCommand( p1 =>
             {
                 ResultSourceCode = "";
                 foreach ( var aProp in Properties )
                 {
                     if ( !String.IsNullOrEmpty( aProp.Name ) )
                     {
                         ResultSourceCode += SelectedTemplate.Replace( "#Name#", aProp.Name ).
                                          Replace( "#Comment#", aProp.Comment ).
                                          Replace( "#Type#", aProp.PropertyType );
                         ResultSourceCode += Environment.NewLine;
                     }
                 }
             } );

            CommandCopyToClipboard = new RelayCommand( p1 => Clipboard.SetText( ResultSourceCode ) );

            ResultSourceCode = Environment.NewLine + "   Fill out the table and then press 'Generate Code'";
        }

        private string myViewModelBaseCode =
                _ + "public abstract class ViewModelBase : INotifyPropertyChanged" + Environment.NewLine +
                _ + "{" + Environment.NewLine +
                _ + _ + "public event PropertyChangedEventHandler PropertyChanged;" + Environment.NewLine +
                Environment.NewLine +
                _ + _ + "public void RaisePropertyChanged( [CallerMemberName] string thePropertyName = \"\" )" + Environment.NewLine +
                _ + _ + "{" + Environment.NewLine +
                _ + _ + _ + "PropertyChanged?.Invoke( this, new PropertyChangedEventArgs( thePropertyName ) );" + Environment.NewLine +
                _ + _ + "}" + Environment.NewLine +
                _ + "}"+ Environment.NewLine;

        private readonly string myClassBeginCode =
                _ + "public class CLASS_NAME : ViewModelBase" + Environment.NewLine +
                _ + "{"+ Environment.NewLine;

        private readonly string myClassEndCode =
                _ + "}" + Environment.NewLine;

        private void updateTemplates()
        {
            Templates["default"] = getBaseTemplate( 0, "nameof(#Name#)" );

            Templates["compact"] =
                _ + "public #Type# #Name# { get { return " + PrivatePrefix + "#Name#; } set { " + PrivatePrefix + "#Name# = value; RaisePropertyChanged(); } } private #Type# " + PrivatePrefix + "#Name#;";

            Templates["CallerMemberName"] =
                myViewModelBaseCode + Environment.NewLine +
                myClassBeginCode +
                getBaseTemplate( 1, "" ) +
                myClassEndCode;


            if ( String.IsNullOrEmpty( SelectedTemplateKey ) )
            {
                SelectedTemplateKey = "default";
            }

            RaisePropertyChanged( nameof( SelectedTemplate ) );
        }


        private string getBaseTemplate( int additionalIndents, string raisePropertyChangedParam )
        {
            string _ = new String( ' ', ( 1 + additionalIndents ) * 4 );

            return _ + "/// <summary>" + Environment.NewLine +
                   _ + "/// #Comment#" + Environment.NewLine +
                   _ + "/// </summary>" + Environment.NewLine +
                   _ + "public #Type# #Name#" + Environment.NewLine +
                   _ + "{" + Environment.NewLine +
                   _ + _ + "get" + Environment.NewLine +
                   _ + _ + "{" + Environment.NewLine +
                   _ + _ + _ + "return " + PrivatePrefix + "#Name#;" + Environment.NewLine +
                   _ + _ + "}" + Environment.NewLine +
                   _ + _ + "set" + Environment.NewLine +
                   _ + _ + "{" + Environment.NewLine +
                   _ + _ + _ + "if ( #Name# != value )" + Environment.NewLine +
                   _ + _ + _ + "{" + Environment.NewLine +
                   _ + _ + _ + _ + PrivatePrefix + "#Name# = value;" + Environment.NewLine +
                   _ + _ + _ + _ + "RaisePropertyChanged(nameof(#Name#));" + Environment.NewLine +
                   _ + _ + _ + "}" + Environment.NewLine +
                   _ + _ + "}" + Environment.NewLine +
                   _ + "}" + Environment.NewLine +
                   _ + "private #Type# " + PrivatePrefix + "#Name#;" + Environment.NewLine;
        }

        internal void AddRow()
        {
            Properties.Add( new PropertyDesription( "string", "", "" ) );
        }

    }
}
