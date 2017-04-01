# CorpusExplorer-Port-R
Erlaubt es, dass R-Skripte auf den CorpusExplorer zugreifen...

## Voraussetzungen
1. Installieren Sie den CorpusExplorer (http://www.bitcutstudios.com/products/corpusexplorer/standard/publish.htm)
2. Installieren Sie eine aktuelle R-Version (http://ftp5.gwdg.de/pub/misc/cran/)
3. Laden Sie die aktuelle Version des R-Ports herunter und entpacken Sie die Dateien (http://bitcut.de/products/CorpusExplorer/addons/R-Language/CE-R-Port.zip)

## Grundlegendes
ceRport.exe [CORPUSFILE] [TASK]
- Bsp. für R: 
```R
tbl <- read.table(pipe("ceRport.exe demo.cec5 frequency"), sep = "\t", header = TRUE, dec = ",", encoding = "UTF-8", quote = "")
```
- Bsp. für Konsole zu CSV: 
```SHELL
ceRport.exe demo.cec5 frequency > frequency.csv
```

### [CORPUSFILE]
Für [CORPUSFILE] können folgende Dateitypen genutzt werden:
- CorpusExplorer *.cec5 & *.cec6
- DTA-Basisformat *.tcf.xml
- Weblicht *.xml
Die Korpusformate werden anhand der Dateiendung ermittelt. Stellen Sie daher sicher, dass die Dateien korrekt benannt sind.

### [TASK]
Für TASK können folgenden Befehle genutzt werden:
- frequency1 = Wort/Frequenz
- frequency2 = POS/Wort/Frequenz
- frequency3 = POS/Lemma/Wort/Frequenz
- cooccurrence = Kookkurrenzanalyse
- meta = Dokument-Metadaten
- crossfrequency = Kreuzfrequenz
- convert = Konvertiert unterschiedliche Korpusformate (siehe [CORPUSFILE])
- query = Erlaubt es ein Korpus zu filtern und das Resultat zu speichern (siehe [QUERY])
Bsp.:
```R
tbl <- read.table(pipe("ceRport.exe demo.cec5 cooccurrence"), sep = "\t", header = TRUE, dec = ",", encoding = "UTF-8", quote = "")
```

### [QUERY]
Query-Abfragen schreiben die Ergebnisse in ein neues Korpus [OUTPUTFILE]. Sie können entweder Dokument-Metadaten (meta) oder Volltext-Abfragen (text) zum Filtern verwenden. 

#### query meta
Der Wert [CATEGORY] gibt die Metadaten-Ketegorie an - z. B. Datum, Autor, Verlag, etc. Bitte stellen Sie sicher, dass diese Kategorie im Korpus enthalten ist (z. B. via CorpusExplorer > Analysen > Korpusverteilung).
- query meta regex [CATEGORY] [RegEx] [OUTPUTFILE] = [CATEGORY] Abfrage via RegEx
- query meta !contains [CATEGORY] [VALUE] [OUTPUTFILE] = [CATEGORY] enthält [VALUE] nicht
- query meta contains [CATEGORY] [VALUE] [OUTPUTFILE] = [CATEGORY] enthält [VALUE]
Bsp.:
```SHELL
ceRport.exe corpusIn.cec5 query meta Verlag contains SPIEGEL corpusOut.cec6
```
Erklärung: Schreibt alle Dokumente aus corpusIn.cec5, die eine Meta-Angabe "Verlag" enthalten, deren Wert "SPIEGEL" ist, in die Datei corpusOut.cec6 (inkl. Formatkonvertierung)

#### query text
Die Angabe [LAYER] bezieht sich auf die verfügbaren Layer, die im CorpusExplorer nach einem Import zur Verfügung stehen würden. Übliche Layer sind: Wort, Lemma, POS, Phrase, Satz. Layer sind abhängig vom Import-Prozess. Daher überprüfen Sie bitte vorab, ob der Layer im Ausgangskorpus verfügbar ist (z. B. via CorpusExplorer > Analysen > Volltext). Der Parameter [SEPARATEDVALUES] darf KEINE Leerzeichen enthalten. Trennen Sie Worte mit: | oder ; - Bsp.: Tag;Nacht;Korpus|Berge|Baum
- query text any [LAYER] [SEPARATEDVALUES] [OUTPUTFILE] = Dokument enthält [SEPARATEDVALUES]
- query text !any [LAYER] [SEPARATEDVALUES] [OUTPUTFILE] = Dokument enthält [SEPARATEDVALUES] nicht
- query text indocument [LAYER] [SEPARATEDVALUES] [OUTPUTFILE] = Alle [SEPARATEDVALUES] kommen in einem Dokument gemeinsam vor
- query text insentence [LAYER] [SEPARATEDVALUES] [OUTPUTFILE] = Alle [SEPARATEDVALUES] kommen in einem Satz gemeinsam vor (beliebige Reihenfolge)
- query text phrase [LAYER] [SEPARATEDVALUES] [OUTPUTFILE] = Alle [SEPARATEDVALUES] kommen exakt so hintereinander vor (Phrase)
- query text regex [LAYER] [RegEx] [OUTPUTFILE] = RegEx fragt Werte ab, die im Dokument vorkommen müssen
Bsp.: 
```SHELL
ceRport.exe corpusIn.cec5 query text any Wort Korpuslinguistik|Linguistik|Korpus corpusOut.cec6
```
Erklärung: Schreibt alle Dokumente aus corpusIn.cec5, die die Worte (Wort-Layer) Korpuslinguistik, Linguistik oder Korpus enthalten, in die Datei corpusOut.cec6  (inkl. Formatkonvertierung)
