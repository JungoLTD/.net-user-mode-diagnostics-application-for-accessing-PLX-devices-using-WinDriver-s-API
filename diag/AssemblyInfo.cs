// Jungo Connectivity Confidential. Copyright (c) 2023 Jungo Connectivity Ltd.  https://www.jungo.com

// Note: This code sample is provided AS-IS and as a guiding sample only.

using System.Reflection;
using Jungo.wdapi_dotnet;

//
// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyTitle("WinDriver PLX Diagnostic Utility for C#.NET " +
    windrvr_decl.WD_VER_STRING)]
[assembly: AssemblyDescription("WinDriver PLX Diagnostic Utility for C#.NET " +
    windrvr_decl.WD_VER_STRING)]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Jungo Connectivity")]
[assembly: AssemblyProduct("plx_dotnet_diag")]
[assembly: AssemblyCopyright(windrvr_decl.JUNGO_COPYRIGHT)]
[assembly: AssemblyTrademark("Trademark is a legal trademark of Jungo Connectivity")]
[assembly: AssemblyCulture("")]

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers
// by using the '*' as shown below:

[assembly: AssemblyVersion(windrvr_decl.WD_VER_STRING)]

//
// In order to sign your assembly you must specify a key to use. Refer to the
// Microsoft .NET Framework documentation for more information on assembly signing.
//
// Use the attributes below to control which key is used for signing.
//
// Notes:
//   (*) If no key is specified, the assembly is not signed.
//   (*) KeyName refers to a key that has been installed in the Crypto Service
//       Provider (CSP) on your machine. KeyFile refers to a file which contains
//       a key.
//   (*) If the KeyFile and the KeyName values are both specified, the
//       following processing occurs:
//       (1) If the KeyName can be found in the CSP, that key is used.
//       (2) If the KeyName does not exist and the KeyFile does exist, the key
//           in the KeyFile is installed into the CSP and used.
//   (*) In order to create a KeyFile, you can use the sn.exe (Strong Name) utility.
//       When specifying the KeyFile, the location of the KeyFile should be
//       relative to the project output directory which is
//       %Project Directory%\obj\<configuration>. For example, if your KeyFile is
//       located in the project directory, you would specify the AssemblyKeyFile
//       attribute as [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//   (*) Delay Signing is an advanced option - see the Microsoft .NET Framework
//       documentation for more information on this.
//
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]
