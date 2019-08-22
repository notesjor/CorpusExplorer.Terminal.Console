﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CorpusExplorer.Terminal.Console.Properties {
    using System;
    
    
    /// <summary>
    ///   Eine stark typisierte Ressourcenklasse zum Suchen von lokalisierten Zeichenfolgen usw.
    /// </summary>
    // Diese Klasse wurde von der StronglyTypedResourceBuilder automatisch generiert
    // -Klasse über ein Tool wie ResGen oder Visual Studio automatisch generiert.
    // Um einen Member hinzuzufügen oder zu entfernen, bearbeiten Sie die .ResX-Datei und führen dann ResGen
    // mit der /str-Option erneut aus, oder Sie erstellen Ihr VS-Projekt neu.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Gibt die zwischengespeicherte ResourceManager-Instanz zurück, die von dieser Klasse verwendet wird.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CorpusExplorer.Terminal.Console.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Überschreibt die CurrentUICulture-Eigenschaft des aktuellen Threads für alle
        ///   Ressourcenzuordnungen, die diese stark typisierte Ressourcenklasse verwenden.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die help - to display command help
        ///FILE:[FILE] - to execute script
        ///SAVE:[FILE] - to save command history as script
        ///quit - to exit shell mode ähnelt.
        /// </summary>
        public static string BaseHelp {
            get {
                return ResourceManager.GetString("BaseHelp", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die done ähnelt.
        /// </summary>
        public static string Done {
            get {
                return ResourceManager.GetString("Done", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die error! ähnelt.
        /// </summary>
        public static string Error {
            get {
                return ResourceManager.GetString("Error", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die execute script: {0} ähnelt.
        /// </summary>
        public static string ExecuteScript {
            get {
                return ResourceManager.GetString("ExecuteScript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Example: cec.exe import#ImporterCec6#C:\mycorpus.cec6 frequency3 POS Lemma Wort ähnelt.
        /// </summary>
        public static string HelpActionExample {
            get {
                return ResourceManager.GetString("HelpActionExample", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die 
        ///
        ///&lt;: --- [ACTION] --- :&gt;
        ///
        ///Most actions accept arguments. [ARG] is a required argument. {ARG} is an optional argument. ähnelt.
        /// </summary>
        public static string HelpActionHeader {
            get {
                return ResourceManager.GetString("HelpActionHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die [ACTION] = {0} ähnelt.
        /// </summary>
        public static string HelpActionPattern {
            get {
                return ResourceManager.GetString("HelpActionPattern", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Example: cec.exe annotate#DpxcScraper#SimpleTreeTagger#Deutsch#C:\dpxc\ convert ExporterCec6#C:\mycorpus.cec6 ähnelt.
        /// </summary>
        public static string HelpAnnotateExample {
            get {
                return ResourceManager.GetString("HelpAnnotateExample", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Annotate raw text - [INPUT]: ähnelt.
        /// </summary>
        public static string HelpAnnotateHeader {
            get {
                return ResourceManager.GetString("HelpAnnotateHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Note: [DIRECTORY] = any directory you like - all files will be processed ähnelt.
        /// </summary>
        public static string HelpAnnotateNote {
            get {
                return ResourceManager.GetString("HelpAnnotateNote", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die [INPUT] = annotate#{0}#[TAGGER]#[LANGUAGE]#[DIRECTORY] ähnelt.
        /// </summary>
        public static string HelpAnnotatePattern {
            get {
                return ResourceManager.GetString("HelpAnnotatePattern", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die [TAGGER] &amp; [LANGUAGE]: ähnelt.
        /// </summary>
        public static string HelpAnnotateTaggerHeader {
            get {
                return ResourceManager.GetString("HelpAnnotateTaggerHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die ([LANGUAGE] = {0}) ähnelt.
        /// </summary>
        public static string HelpAnnotateTaggerLanguagePattern {
            get {
                return ResourceManager.GetString("HelpAnnotateTaggerLanguagePattern", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die [TAGGER] = {0}  ähnelt.
        /// </summary>
        public static string HelpAnnotateTaggerPattern {
            get {
                return ResourceManager.GetString("HelpAnnotateTaggerPattern", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die 
        ///
        ///&lt;: --- [F:FORMAT] --- :&gt;
        ///
        ///If you use [ACTION] or the scripting-mode [FILE: / DEBUG:], you can change the output format.
        ///You need to set one of the following tags as first parameter:
        ///F:TSV - (standard output format) tab separated values
        ///F:CSV - &apos;;&apos; separated values
        ///F:JSON - JSON-array
        ///F:XML - XML-Document
        ///F:HTML - HTML5-Document
        ///F:SQL - SQL-statement
        ///F:SQLSCHEMA - SQL-statement (schema only)
        ///F:SQLDATA - SQL-statement (insert data only - no schema)
        ///Example: cec.exe F:JSON import#ImporterCec6#C [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        /// </summary>
        public static string HelpFormat {
            get {
                return ResourceManager.GetString("HelpFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Note: [FILES] = separate files with &amp; - merges all files before processing
        ///Example: cec.exe import#ImporterCec5#C:\mycorpus1.cec5&amp;C:\mycorpus2.cec5 convert ExporterCec6#C:\mycorpus.cec6
        ///
        /// ähnelt.
        /// </summary>
        public static string HelpImportExample {
            get {
                return ResourceManager.GetString("HelpImportExample", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die &lt;: --- [INPUT]-- - :&gt;
        ///
        ///Import corpus material - direct[INPUT]: ähnelt.
        /// </summary>
        public static string HelpImportHeader {
            get {
                return ResourceManager.GetString("HelpImportHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die [INPUT] = import#{0}#[FILES] ähnelt.
        /// </summary>
        public static string HelpImportPattern {
            get {
                return ResourceManager.GetString("HelpImportPattern", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Syntax for annotation/conversion:
        ///cec.exe [INPUT] convert [OUTPUT]
        ///Syntax for filtering:
        ///cec.exe [INPUT] [QUERY] [OUTPUT]
        ///Syntax for analytics (writes output to stdout):
        ///cec.exe {F:FORMAT} [INPUT] [ACTION]
        ///Syntax for scripting:
        ///cec.exe FILE:[PATH]
        ///More detailed scripting errors:
        ///cec.exe DEBUG:[PATH]
        ///To start interactive shell mode
        ///cec.exe SHELL
        ///To start a web-service
        ///cec.exe {F:FORMAT} PORT:2312 {IP:127.0.0.1} {TIMEOUT:30000} {INPUT}
        /// ähnelt.
        /// </summary>
        public static string HelpModes {
            get {
                return ResourceManager.GetString("HelpModes", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Note: [FILE] = any file you like to store the output
        ///Example &apos;convert&apos;: cec.exe import#ImporterCec5#C:\mycorpus.cec5 convert ExporterCec6#C:\mycorpus.cec6
        ///Example &apos;query&apos;: cec.exe import#ImporterCec5#C:\mycorpus.cec5 query !M:Author::Jan ExporterCec6#C:\mycorpus.cec6 ähnelt.
        /// </summary>
        public static string HelpOutputExample {
            get {
                return ResourceManager.GetString("HelpOutputExample", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die 
        ///
        ///&lt;: --- [OUTPUT] --- :&gt;
        ///
        ///[OUTPUT-EXPORTER] - for query or convert: ähnelt.
        /// </summary>
        public static string HelpOutputHeader {
            get {
                return ResourceManager.GetString("HelpOutputHeader", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die [OUTPUT] = {0}#[FILE] ähnelt.
        /// </summary>
        public static string HelpOutputPattern {
            get {
                return ResourceManager.GetString("HelpOutputPattern", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die 
        ///[QUERY]:
        ///A preceding ! inverts the entire query
        ///First character:
        ///M = Metadata -OR- T = (Full)Text -OR- X = Extended Features
        ///followed by configuration (see below), the :: separator and the values
        ///
        ///Second character [OPERATOR] (if you choose M):
        ///  ? = regEx | : = contains (case sensitive) | . = contains (not case sensitive)
        ///  = = match exact (case sensitive) | - = match exact (not case sensitive) | ! = is empty
        ///  ( = starts with (case sensitive) | ) = ends with (case sensitive)
        ///If you have chosen  [Rest der Zeichenfolge wurde abgeschnitten]&quot;; ähnelt.
        /// </summary>
        public static string HelpQueryClusterSyntax {
            get {
                return ResourceManager.GetString("HelpQueryClusterSyntax", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die 
        ///
        ///&lt;: --- [SCRIPTING] --- :&gt;
        ///
        ///All actionss above can be stored in a file to build up a automatic process.
        ///In this case it&apos;s recommended to redirect the [ACTION]-output to a file and not to stdout
        ///Example: import#ImporterCec6#C:\mycorpus.cec6 frequency3 POS Lemma Wort &gt; output.csv ähnelt.
        /// </summary>
        public static string HelpScripting {
            get {
                return ResourceManager.GetString("HelpScripting", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die ok! ähnelt.
        /// </summary>
        public static string Ok {
            get {
                return ResourceManager.GetString("Ok", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die ...ok! ähnelt.
        /// </summary>
        public static string PointPointPointOk {
            get {
                return ResourceManager.GetString("PointPointPointOk", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die ready! ähnelt.
        /// </summary>
        public static string Ready {
            get {
                return ResourceManager.GetString("Ready", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die running ähnelt.
        /// </summary>
        public static string Running {
            get {
                return ResourceManager.GetString("Running", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die action unavailable ähnelt.
        /// </summary>
        public static string WebErrorActionUnavailable {
            get {
                return ResourceManager.GetString("WebErrorActionUnavailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die corpus unavailable ähnelt.
        /// </summary>
        public static string WebErrorCorpusUnavailable {
            get {
                return ResourceManager.GetString("WebErrorCorpusUnavailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die invalid post-data ähnelt.
        /// </summary>
        public static string WebErrorInvalidPostData {
            get {
                return ResourceManager.GetString("WebErrorInvalidPostData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die this service is only up to 100 documents/pages ähnelt.
        /// </summary>
        public static string WebErrorPostMax100Pages {
            get {
                return ResourceManager.GetString("WebErrorPostMax100Pages", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die tagging process failed ähnelt.
        /// </summary>
        public static string WebErrorTaggingProcessError {
            get {
                return ResourceManager.GetString("WebErrorTaggingProcessError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die wrong language selected - use: {0} ähnelt.
        /// </summary>
        public static string WebErrorWrongLanguage {
            get {
                return ResourceManager.GetString("WebErrorWrongLanguage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die The name of the action ähnelt.
        /// </summary>
        public static string WebHelpActionsActionName {
            get {
                return ResourceManager.GetString("WebHelpActionsActionName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Short description - action and parameter ähnelt.
        /// </summary>
        public static string WebHelpActionsDescription {
            get {
                return ResourceManager.GetString("WebHelpActionsDescription", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Adds/analyzes a new corpus ähnelt.
        /// </summary>
        public static string WebHelpAddCorpus {
            get {
                return ResourceManager.GetString("WebHelpAddCorpus", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die text = document-text / meta = key/value dictionary - example: {&quot;text&quot;:&quot;annotate this text&quot;,&quot;meta&quot;:{&quot;Author&quot;:&quot;Jan&quot;,&quot;Integer&quot;:5,&quot;Date&quot;:&quot;2019-01-08T21:32:01.0194747+01:00&quot;}} ähnelt.
        /// </summary>
        public static string WebHelpAddCorpusParameterDocuments {
            get {
                return ResourceManager.GetString("WebHelpAddCorpusParameterDocuments", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die the language of all documents ähnelt.
        /// </summary>
        public static string WebHelpAddCorpusParameterLanguage {
            get {
                return ResourceManager.GetString("WebHelpAddCorpusParameterLanguage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Shows all available Actions for {0}execute/ ähnelt.
        /// </summary>
        public static string WebHelpExecute {
            get {
                return ResourceManager.GetString("WebHelpExecute", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die name of the action to execute ähnelt.
        /// </summary>
        public static string WebHelpExecuteParameterAction {
            get {
                return ResourceManager.GetString("WebHelpExecuteParameterAction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die example: [&apos;POS&apos;, &apos;Lemma&apos;, &apos;Wort&apos;] ähnelt.
        /// </summary>
        public static string WebHelpExecuteParameterArguments {
            get {
                return ResourceManager.GetString("WebHelpExecuteParameterArguments", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die the id of the corpus you added via {0}add/ ähnelt.
        /// </summary>
        public static string WebHelpExecuteParameterCorpusId {
            get {
                return ResourceManager.GetString("WebHelpExecuteParameterCorpusId", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die example: [&apos;guid1&apos;, &apos;guid2&apos;, &apos;guid3&apos;] ähnelt.
        /// </summary>
        public static string WebHelpExecuteParameterGuids {
            get {
                return ResourceManager.GetString("WebHelpExecuteParameterGuids", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die execution result ähnelt.
        /// </summary>
        public static string WebHelpExecuteResult {
            get {
                return ResourceManager.GetString("WebHelpExecuteResult", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die Lists of all available Actions for {0}execute/ ähnelt.
        /// </summary>
        public static string WebHelpListActions {
            get {
                return ResourceManager.GetString("WebHelpListActions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die lists all available languages for {0}add/ ähnelt.
        /// </summary>
        public static string WebHelpListAvailableLanguages {
            get {
                return ResourceManager.GetString("WebHelpListAvailableLanguages", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die all available languages ähnelt.
        /// </summary>
        public static string WebHelpParameterLanguages {
            get {
                return ResourceManager.GetString("WebHelpParameterLanguages", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die INIT WebService (mode: file)
        ///LOAD: {0}... ähnelt.
        /// </summary>
        public static string WebInit {
            get {
                return ResourceManager.GetString("WebInit", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die ..:: CURRENT ACTIONS ::.. ähnelt.
        /// </summary>
        public static string XmlScriptCurrentActions {
            get {
                return ResourceManager.GetString("XmlScriptCurrentActions", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die E001: XML Parser Error ähnelt.
        /// </summary>
        public static string XmlScriptParserError001 {
            get {
                return ResourceManager.GetString("XmlScriptParserError001", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Sucht eine lokalisierte Zeichenfolge, die --- SCRIPT SUCCESSFULLY EXECUTED --- ähnelt.
        /// </summary>
        public static string XmlScriptSuccess {
            get {
                return ResourceManager.GetString("XmlScriptSuccess", resourceCulture);
            }
        }
    }
}