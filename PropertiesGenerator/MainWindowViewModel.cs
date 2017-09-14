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
                RaisePropertyChanged(nameof(PropertyType));
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

        public PropertyDesription( string thePropertyType, string theName, string theComment)
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
        private const string myIndent1 = "    ";
        private const string myIndent2 = "        ";
        private const string myIndent3 = "            ";
        private const string myIndent4 = "                ";

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
                RaisePropertyChanged(nameof(Properties));
            }
        }
        private ObservableCollection<PropertyDesription> myProps = new ObservableCollection<PropertyDesription>();

        /// <summary>
        /// Property Template 
        /// </summary>
        public string Template
        {
            get
            {
                return myTemplate;
            }
            set
            {
                myTemplate = value;
                RaisePropertyChanged(nameof(Template));
            }
        }
        private string myTemplate;


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
                updateTemplate();
                RaisePropertyChanged(nameof(PrivatePrefix));
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
                RaisePropertyChanged(nameof(ResultSourceCode));
            }
        }
        private string myCode;

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindowViewModel()
        {
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
            for (int i = 0; i < 10; i++)
            {
                AddRow();
            }

            CommandAddRow = new RelayCommand(p1 => AddRow());

            CommandGenerateCode = new RelayCommand(p1 =>
            {
                ResultSourceCode = "";
                foreach (var aProp in Properties)
                {
                    if (!String.IsNullOrEmpty(aProp.Name))
                    {
                        ResultSourceCode += Template.Replace("#Name#", aProp.Name).
                                         Replace("#Comment#", aProp.Comment).
                                         Replace("#Type#", aProp.PropertyType);
                        ResultSourceCode += Environment.NewLine;
                    }
                }
            });

            CommandCopyToClipboard = new RelayCommand(p1 => Clipboard.SetText(ResultSourceCode));

            ResultSourceCode = Environment.NewLine + "   Fill out the table and then press 'Generate Code'";
        }

        private void updateTemplate()
        {
            Template =
                myIndent1 + "/// <summary>" + Environment.NewLine +
                myIndent1 + "/// #Comment#" + Environment.NewLine +
                myIndent1 + "/// </summary>" + Environment.NewLine +
                myIndent1 + "public #Type# #Name#" + Environment.NewLine +
                myIndent2 + "get" + Environment.NewLine +
                myIndent2 + "{" + Environment.NewLine +
                myIndent3 + "return " + PrivatePrefix +"#Name#;" + Environment.NewLine +
                myIndent2 + "}" + Environment.NewLine +
                myIndent2 + "set" + Environment.NewLine +
                myIndent2 + "{" + Environment.NewLine +
                myIndent3 + "if ( #Name# != value )" + Environment.NewLine +
                myIndent3 + "{" + Environment.NewLine +
                myIndent4 + PrivatePrefix + "#Name# = value;" + Environment.NewLine +
                myIndent4 + "RaisePropertyChanged(nameof(#Name#));" + Environment.NewLine +
                myIndent3 + "}" + Environment.NewLine +
                myIndent2 + "}" + Environment.NewLine +
                myIndent1 + "private #Type " + PrivatePrefix + "#Name#;" + Environment.NewLine;
        }

        internal void AddRow()
        {
            Properties.Add(new PropertyDesription("string", "", ""));
        }
    }
}
