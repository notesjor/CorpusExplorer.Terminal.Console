# CorpusExplorer-Port-R
Erlaubt es, aus R oder über die Konsole auf den CorpusExplorer v2.0 (http://www.corpusexplorer.de) zuzugreifen.

## Voraussetzungen
1. Installieren Sie den CorpusExplorer (http://www.bitcutstudios.com/products/corpusexplorer/standard/publish.htm)
2. Laden Sie die aktuelle Version des R-Ports herunter und entpacken Sie die Dateien (http://bitcut.de/products/CorpusExplorer/addons/R-Language/CE-R-Port.zip)
3. (OPTIONAL) Installieren Sie eine aktuelle R-Version (http://ftp5.gwdg.de/pub/misc/cran/) WENN Sie den CorpusExplorer unter R nutzen möchten. Andernfalls können Sie auch auf die Windows-Konsole zurück greifen und die Programmausgabe mittels ">" in eine Datei (CSV) umleiten.

## Grundlegendes
Der ceRport.exe greift auf das CorpusExplorer-Ökosystem zurück. D.h. auch alle installierten Erweiterungen für den CorpusExplorer sind nutzbar. Rufen Sie ceRport.exe über die Konsole ohne Parameter auf, dann erhalten Sie alle verfügbaren Scraper, Importer, Tagger und Exporter. Erweiterungen für den CorpusExplorer finden Sie hier: http://notes.jan-oliver-ruediger.de/software/corpusexplorer-overview/corpusexplorer-v2-0/erweiterungen/

Die Grundsyntax für den Konsolenaufruf lautet:
```SHELL
ceRport.exe [INPUT] [TASK]
```
- Bsp. für R: 
```R
tbl <- read.table(pipe("ceRport.exe import#CorpusExplorerV5Importer#demo.cec5 frequency"), sep = "\t", header = TRUE, dec = ",", encoding = "UTF-8", quote = "")
```
- Bsp. für Konsole zu CSV: 
```SHELL
ceRport.exe import#CorpusExplorerV5Importer#demo.cec5 frequency > frequency.csv
```

### [INPUT]
Es können zwei unterschiedliche INPUT-Quellen genutzt werden. "import" für bereits annotiertes Korpusmaterial oder "annotate", um einen Annotationsprozess anzustoßen. 

#### [INPUT] - import
Syntax für "import" (trennen Sie mehrere [FILES] mittels #):
```SHELL
ceRport.exe import#[IMPORTER]#[FILES] [TASK]
```
Tipp: Starten Sie ceRport.exe ohne Parameter, um die verfügbaren [IMPORTER] abzufragen.
Bsp. für DTAbf und Weblicht:
```SHELL
ceRport.exe import#DtaImporter#"C:/DTAbf/doc01.tcf.xml" [TASK]
ceRport.exe import#WeblichtImporter#"C:/Weblicht/page01.xml"#"C:/webL.xml" [TASK]
```

#### [INPUT] - annotate
Syntax für "annotate" (annotiert immer den gesamten Ordner):
```SHELL
ceRport.exe annotate#[SCRAPER]#[TAGGER]#[LANGUAGE]#[DIRECTORY] [TASK]
```
Tipp: Starten Sie ceRport.exe ohne Parameter, um die verfügbaren [SCRAPER], [TAGGER] und [LANGUAGE] abzufragen.
Es wird empfohlen, "annotate" immer in Kombination mit den [TASK]s "convert" oder "query" zu nutzen. Dadurch wird das fertig annotierte Korpus gespeichert und kann jederzeit mit "import" geladen werden, ohne das Korpus erneut zu annotieren. Bsp. Erzeuge Korpus
```SHELL
ceRport.exe annotate#TwitterScraper#ClassicTreeTagger#Deutsch#"C:/korpus" convert ExporterCec5#"C:/korpus.cec5"
```
Lade das erzeugte Korpus "C:/korpus.cec5" für weitere Berechnungen:
```SHELL
ceRport.exe import#CorpusExplorerV5Importer#"C:/korpus.cec5" frequency > frequency.csv
ceRport.exe import#CorpusExplorerV5Importer#"C:/korpus.cec5" cooccurrence > cooccurrence.csv
```

### [TASK]
Für TASK können folgenden Befehle genutzt werden:
- frequency1 = Wort/Frequenz
- frequency2 = POS/Wort/Frequenz
- frequency3 = POS/Lemma/Wort/Frequenz
- cooccurrence = Kookkurrenzanalyse
- meta = Dokument-Metadaten
- crossfrequency = Kreuzfrequenz
- convert = Konvertiert unterschiedliche Korpusformate
- query = Erlaubt es ein Korpus zu filtern und das Resultat zu speichern (siehe [QUERY])
- ngram = Erzeugt eine N-Gramm-Liste
- vocabularycomplexity = Berechnet die Vokabularkomplexität mit unterschiedlichen Verfahren
- readingease = Berechnet verschiedene Lesbarkeits-Indices
- layernames = Gibt die verfügbaren Layernamen zurück (wird z. B. für query benötigt)
- metacategories = Gibt die verfügbaren Dokumentmetadaten-Kategorien zurück (wird z. B. für query benötigt)
- many-token = Summe der Token
- many-document = Summe der Dokumente
- many-sentence = Summe der Sätze
Bsp.:
```R
tbl <- read.table(pipe("ceRport.exe import#CorpusExplorerV5Importer#"C:/korpus.cec5" cooccurrence"), sep = "\t", header = TRUE, dec = ",", encoding = "UTF-8", quote = "")
```

#### [TASK] query
Query-Abfragen schreiben die Ergebnisse in ein neues Korpus [EXPORT]. Sie können entweder Dokument-Metadaten (meta) oder Volltext-Abfragen (text) zum Filtern verwenden. 

##### [TASK] query meta
Der Wert [CATEGORY] gibt die Metadaten-Ketegorie an - z. B. Datum, Autor, Verlag, etc. Bitte stellen Sie sicher, dass diese Kategorie im Korpus enthalten ist (nutzen Sie dazu den [TASK] "metacategories").
- query meta regex [CATEGORY] [RegEx] [EXPORT] = [CATEGORY] Abfrage via RegEx
- query meta !contains [CATEGORY] [VALUE] [EXPORT] = [CATEGORY] enthält [VALUE] nicht
- query meta contains [CATEGORY] [VALUE] [EXPORT] = [CATEGORY] enthält [VALUE]
Bsp.:
```SHELL
ceRport.exe import#CorpusExplorerV5Importer#"C:/korpus.cec5" query meta Verlag contains SPIEGEL ExporterCec6#corpusOut.cec6
```
Erklärung: Schreibt alle Dokumente aus corpusIn.cec5, die eine Meta-Angabe "Verlag" enthalten, deren Wert "SPIEGEL" ist, in die Datei corpusOut.cec6 (inkl. Formatkonvertierung)

##### [TASK] query text
Die Angabe [LAYER] bezieht sich auf die verfügbaren Layer, die im CorpusExplorer nach einem Import zur Verfügung stehen würden. Übliche Layer sind: Wort, Lemma, POS, Phrase, Satz. Layer sind abhängig vom Import-Prozess. Daher überprüfen Sie bitte vorab, ob der Layer im Ausgangskorpus verfügbar ist (nutzen Sie dazu den [TASK] "layernames"). Der Parameter [SEPARATEDVALUES] darf KEINE Leerzeichen enthalten. Trennen Sie Worte mit: # - Bsp.: Tag#Nacht#Korpus#Berge#Baum
- query text any [LAYER] [SEPARATEDVALUES] [EXPORT] = Dokument enthält [SEPARATEDVALUES]
- query text !any [LAYER] [SEPARATEDVALUES] [EXPORT] = Dokument enthält [SEPARATEDVALUES] nicht
- query text indocument [LAYER] [SEPARATEDVALUES] [EXPORT] = Alle [SEPARATEDVALUES] kommen in einem Dokument gemeinsam vor
- query text insentence [LAYER] [SEPARATEDVALUES] [EXPORT] = Alle [SEPARATEDVALUES] kommen in einem Satz gemeinsam vor (beliebige Reihenfolge)
- query text phrase [LAYER] [SEPARATEDVALUES] [OUTPUTFILE] = Alle [SEPARATEDVALUES] kommen exakt so hintereinander vor (Phrase)
- query text regex [LAYER] [RegEx] [OUTPUTFILE] = RegEx fragt Werte ab, die im Dokument vorkommen müssen
Bsp.: 
```SHELL
ceRport.exe import#CorpusExplorerV5Importer#"C:/korpus.cec5" query text any Wort Korpuslinguistik#Linguistik#Korpus ExporterCec6#corpusOut.cec6
```
Erklärung: Schreibt alle Dokumente aus corpusIn.cec5, die die Worte (Wort-Layer) Korpuslinguistik, Linguistik oder Korpus enthalten, in die Datei corpusOut.cec6  (inkl. Formatkonvertierung)

#### [TASK] ngram
Erfordert, dass Sie ein N festlegen. Bsp. N = 5:
```R
tbl <- read.table(pipe("ceRport.exe import#CorpusExplorerV5Importer#"C:/korpus.cec5" ngram 5"), sep = "\t", header = TRUE, dec = ",", encoding = "UTF-8", quote = "")
```

#### [TASK] convert
Erlaubt es, ein bestehndes Korpus in einem der verfügbaren Export-Formate zu speichern.
```SHELL
ceRport.exe import#CorpusExplorerV5Importer#"C:/korpus.cec5" convert ExporterCec6#corpusOut.cec6
```
