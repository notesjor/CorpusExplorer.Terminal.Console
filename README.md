# Note: An English version can be found below.

# CorpusExplorer.Terminal.Console (früher: CorpusExplorer-Port-R)
Der CorpusExplorer steht neben der offiziellen GUI (http://www.corpusexplorer.de) auch als Konsolenanwendung zur Verfügung. Damit ist es möglich, aus anderen Programmen oder anderen Programmiersprachen auf Analysen/Daten des CorpusExplorers zuzugreifen. Ursprünglich wurde die Konsolen-Lösung unter dem Namen CorpusExplorer-Port-R entwickelt und sollte die Nutzung des CorpusExplorers innerhalb der Programmiersprache R ermöglichen.

## Installation Windows
1. Installieren Sie den CorpusExplorer (http://www.bitcutstudios.com/products/corpusexplorer/standard/publish.htm)
2. Fertig! (OPTIONAL) Installieren Sie eine aktuelle R-Version (http://ftp5.gwdg.de/pub/misc/cran/) WENN Sie den CorpusExplorer unter R nutzen möchten. Andernfalls können Sie auch auf die Windows-Konsole zurückgreifen und die Programmausgabe mittels ">" in eine Datei (CSV) umleiten.

## Installation Linux/MacOS
1. Installieren Sie mono (http://www.mono-project.com/download/) - Mindestversion 4.x
2. Laden und entpacken Sie die folgende Datei: http://www.bitcutstudios.com/products/corpusexplorer/App.zip
3. Stellen Sie allen Aufrufen ein "mono" voran - z. B. "mono cec.exe import#Cec6#demo.cec6 frequency". Mono führt die cec.exe aus (die sich im entpackten Ordner - siehe 2. - befindet).
__Einschränkung Linux/MacOS__: Gegenwärtig ist es noch nicht möglich, den Befehl "annotate" auszuführen. Alle anderen Befehle funktionieren einwandfrei (getestet auf Debian 8 - Mono 5.0.1).

## Grundlegendes
Der cec.exe greift auf das CorpusExplorer-Ökosystem zurück. D.h. auch alle installierten Erweiterungen für den CorpusExplorer sind nutzbar. Rufen Sie cec.exe über die Konsole ohne Parameter auf, dann erhalten Sie alle verfügbaren Scraper, Importer, Tagger, Actions und Exporter. Erweiterungen für den CorpusExplorer finden Sie hier: http://notes.jan-oliver-ruediger.de/software/corpusexplorer-overview/corpusexplorer-v2-0/erweiterungen/

Die Grundsyntax für verschiedene Konsolenaufruf lauten (diese werden im Folgenden noch weiter ausgeführt):

Syntax-Grundidee:
```SHELL
cec.exe [INPUT] [ACTION] [OUTPUT]
```
Syntax für Annotation und Konvertierung (convert ist eine [ACTION]):
```SHELL
cec.exe [INPUT] convert [OUTPUT]
```
Syntax für Filter:
```SHELL
cec.exe [INPUT] [QUERY] [OUTPUT]
```
Syntax für Analysen (Ausgabe wird in STDOUT geschrieben):
```SHELL
cec.exe {F:FORMAT} [INPUT] [ACTION]
```
Syntax für Skripting:
```SHELL
cec.exe FILE:[PATH]
```
Erweiterte Fehlerausgabe im Skript-Modus:
```SHELL
cec.exe DEBUG:[PATH]
```
Um eine interaktive SHELL zu starten
```SHELL
cec.exe SHELL
```
Um einen REST-WebService zu starten
```SHELL
cec.exe {F:FORMAT} PORT:2312 {IP:127.0.0.1} {TIMEOUT:120} {INPUT}
```

## Nutzung z. B. in Programmiersprachen
### Bsp. für R: 
```R
tbl <- read.table(pipe("cec.exe import#Cec6#demo.cec6 frequency3"), sep = "\t", header = TRUE, dec = ".", encoding = "UTF-8", quote = "")
```
### Bsp. für Python: 
```Python
import subprocess;
from subprocess import PIPE;
from io import StringIO;

cec = subprocess.run("cec.exe import#Cec6#demo.cec6 frequency3", stdout=PIPE).stdout.decode(encoding='UTF-8');
cec = StringIO(cec);

# Einlesen mittels csv-Reader
import csv;
tsv = csv.reader(cec, delimiter='\t', quotechar='"');
for row in tsv:
	print(";".join(row);
	
# Einlesen als pandas / dataframe
import pandas as pd
df = pd.read_csv(cec, sep="\t")
```

## Nutzung in Jupyter Notebooks
Was in Python funktioniert, funktioniert natürlich auch in Jupyter Notebooks. D. h. Es lassen sich interaktive Korpusanalysen mit der CEC erstellen.
![Screenshot CEC und Jupyter Notebooks](https://github.com/notesjor/CorpusExplorer.Terminal.Console/blob/master/screen.png?raw=true)

## Andere Ausgabeformate
Die Ausgabe erfolgt im Fall einer normalen Analyse (ausgenommen 'convert' und 'query') direkt auf dem STDOUT-Stream.
Sie können dieses Stream mittels '>' umleiten. Beispiel:

```SHELL
cec.exe F:TSV import#Cec6#demo.cec6 frequency3 > frequency.tsv
cec.exe F:CSV import#Cec6#demo.cec6 frequency3 > frequency.csv
cec.exe F:JSON import#Cec6#demo.cec6 frequency3 > frequency.json
cec.exe F:XML import#Cec6#demo.cec6 frequency3 > frequency.xml
cec.exe F:HTML import#Cec6#demo.cec6 frequency3 > frequency.html
cec.exe F:SQL import#Cec6#demo.cec6 frequency3 > frequency.sql
```

Falls Sie keine TID-Spalte benötigen, können Sie anstelle von F: die Option FNT: verwenden. Bsp.:
```SHELL
cec.exe FNT:TSV import#Cec6#demo.cec6 frequency3 > frequency.tsv
```

Verfügbare Ausgabeformate in der Standardinstallation:
```SHELL
F:CSV - comma separated values
F:HTML - HTML5-document
F:TRTD - HTML-Table Skeleton
F:JSON - JavaScript Object Notation (JSON)
F:JSONR - JavaScript Object Notation (rounded values)
F:SQLDATA - SQL (data only)
F:SQLSCHEMA - SQL (schema only)
F:SQL - SQL (schema + data)
F:TSV - tab separated values
F:TSVR - tab separated values (rounded values)
F:XML - XML-document
```

Vorteile von STDTOUT: 
- Dieser Stream ist in allen Betriebssystemen verfügbar (Windows, Linux, MacOS). 
- Ergebnisse können (1) direkte in der Konsole (2) in einer Datei (3) an ein aderes Programm weitergeleitet werden.
- Sie können den Stream nicht nur in eine Datei umleiten, sondern auch direkt an andere Programme übergeben.

Nachteile von STDOUT:
- Dieser Stream ist langsam. Wenn Sie die Ausgabe nur in eine Datei benötigen, dann nutzen Sie die optimierte Version (F: oder FNT: und Dateiname):
```SHELL
cec.exe FNT:TSV#frequency.tsv import#Cec6#demo.cec6 frequency3
```

## Korpus als WebService
Korpora können auch als WebService (REST-basiert) bereitgestellt werden.
cec.exe [F:FORMAT] PORT:[PORT] [INPUT]
```SHELL
cec.exe F:JSON PORT:3535 import#Cec6#demo.cec6
```
Lassen Sie das Konsolenfenster geöffnet und starten Sie ihren WebBrowser. Sie können eine Dokumentation unter http://127.0.0.1:3535 einsehen.
Mehrere Korpora können gleichzeitig als WebService betrieben werden, dazu muss aber jedem Korpus ein separater Port zugewiesen werden.
Eine SSL/TLS-Transportverschlüsselung ist NICHT möglich - wenn Sie dies benötigen, dann erstellen Sie einen Reverse-Proxy z. B. mit Apache
(https://gridscale.io/community/tutorials/apache-server-reverse-proxy-ubuntu/).

### [INPUT]
Es können zwei unterschiedliche INPUT-Quellen genutzt werden. "import" für bereits annotiertes Korpusmaterial oder "annotate", um einen Annotationsprozess anzustoßen. 

#### [INPUT] - import
Syntax für "import" (trennen Sie mehrere [FILES] mittels |):
```SHELL
cec.exe import#[IMPORTER]#[FILES] [ACTION]
```
Tipp: Starten Sie cec.exe ohne Parameter, um die verfügbaren [IMPORTER] abzufragen.
Bsp. für CEC6, DTAbf.TCF und Weblicht:
```SHELL
cec.exe import#Cec6#demo.cec6 [ACTION]
cec.exe import#Dta#C:/DTAbf/doc01.tcf.xml [ACTION]
cec.exe import#Weblicht#C:/Weblicht/page01.xml|C:/webL.xml [ACTION]
```
Hinweis: Einige Betriebssysteme (und Betriebssystemversionen) haben Problemen, wenn sich im Pfad Leerzeichen bedfinden. Vermeiden Sie daher soweit möglich Leerzeichen in Pfad und Dateinamen.

Folgende Import-Formate stehen in der Standardinstallation zur Verfügung:
```SHELL
[INPUT] = import#Cec6#[FILES]
[INPUT] = import#Bnc#[FILES]
[INPUT] = import#Catma#[FILES]
[INPUT] = import#ClanChildes#[FILES]
[INPUT] = import#Conll#[FILES]
[INPUT] = import#CoraXml08#[FILES]
[INPUT] = import#CoraXml10#[FILES]
[INPUT] = import#EchtzeitEngine#[FILES]
[INPUT] = import#Cec5#[FILES]
[INPUT] = import#Cec6Stream#[FILES]
[INPUT] = import#CorpusWorkBench#[FILES]
[INPUT] = import#Dewac#[FILES]
[INPUT] = import#Dta#[FILES]
[INPUT] = import#Dta2017#[FILES]
[INPUT] = import#FnhdC#[FILES]
[INPUT] = import#FolkerFln#[FILES]
[INPUT] = import#Korap#[FILES]
[INPUT] = import#SimpleJsonStandoff#[FILES]
[INPUT] = import#CorpusExplorerV1toV5#[FILES]
[INPUT] = import#Redewiedergabe#[FILES]
[INPUT] = import#SketchEngine#[FILES]
[INPUT] = import#Speedy#[FILES]
[INPUT] = import#Tiger#[FILES]
[INPUT] = import#Tlv#[FILES]
[INPUT] = import#TreeTagger#[FILES]
[INPUT] = import#Txm#[FILES]
[INPUT] = import#Weblicht#[FILES]
[INPUT] = import#OpusXces#[FILES]
```

#### [INPUT] - annotate
Syntax für "annotate" (annotiert immer den gesamten Ordner):
```SHELL
cec.exe annotate#[SCRAPER]#[TAGGER]#[LANGUAGE]#[DIRECTORY] [ACTION]
```
Tipp: Starten Sie cec.exe ohne Parameter, um die verfügbaren [SCRAPER], [TAGGER] und [LANGUAGE] abzufragen.
Es wird empfohlen, "annotate" immer in Kombination mit den [ACTION]s "convert" oder "query" zu nutzen. Dadurch wird das fertig annotierte Korpus gespeichert und kann jederzeit mit "import" geladen werden, ohne das Korpus erneut zu annotieren. Bsp.:
```SHELL
cec.exe annotate#Twitter#ClassicTreeTagger#Deutsch#"C:/korpus" convert ExporterCec6#"C:/korpus.cec6"
```

Folgende Annotate-Formate stehen in der Standardinstallation zur Verfügung:
```SHELL
[INPUT] = annotate#Alto12#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#AnnotationPro#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Apaek#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Bawe#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Blogger#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#BundestagDrucksachen#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#BundestagPlenarprotokolle#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#BundestagDtdPlenarprotokolle#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Catma#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#ClarinContentSearch#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#ListOfScrapDocument#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Cosmas#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Csv#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#LeipzigerWortschatz#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#DigitalPlato#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Dpxc#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#DortmunderChatKorpus#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#SlashA#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#DtaBasisformat#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Dta#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Dta2017#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#DwdsTei#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#EasyHashtagSeparation#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#RawMsgMsg#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Epub#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Europarl#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#EuroParlUds#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#ExmaraldaExb#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#FolkerFln#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Folker#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Gutenberg#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Ids#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Korap#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#SimpleJsonStandoff#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Kidko#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#KleineanfrageDe#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#NexisCom#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Cec6#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#SimpleDocxDocument#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#SimpleHtmlDocument#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#TextSharpPdf#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#SimplePdfDocument#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#SimpleRtfDocument#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#PureXmlText#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Perseus#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Txt#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Pmg#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#PostgreSqlDump#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#PurlOrg#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#RssFeedItem#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Shakespeare#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Speedy#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Talkbank#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#P5Cal2#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#TextGrid#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Tiger#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Tsv#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Tumblr#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Twapper#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Twitter#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#TwitterStatus#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Txm#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#UniversalExcel#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Weblicht#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Wet#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Wordpress#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#YouTube#[TAGGER]#[LANGUAGE]#[DIRECTORY]
```
Notiz: [DIRECTORY] = Ein beliebiges Verzeichnis (alle Dateien werden verarbeitet)

Folgende Tagger und Sprachpakete stehen in der Standardinstallation zur Verfügung:
```SHELL
[TAGGER] = ClassicTreeTagger ([LANGUAGE] = Deutsch, Englisch, Französisch, Italienisch, Niederländisch, Spanisch)
[TAGGER] = SimpleTreeTagger ([LANGUAGE] = Deutsch, Englisch, Französisch, Italienisch, Niederländisch, Spanisch)
[TAGGER] = TnTTagger ([LANGUAGE] = Deutsch, Englisch)
[TAGGER] = RawTextTagger ([LANGUAGE] = Universal)
[TAGGER] = OwnTreeTagger ([LANGUAGE] = Durch Skript definiert.)
[TAGGER] = UdPipeExeTagger ([LANGUAGE] = )
```
Bsp.: cec.exe annotate#Dpxc#SimpleTreeTagger#Deutsch#C:\dpxc\ convert Cec6#C:\mycorpus.cec6

#### [OUTPUT] - Export ([ACTION] = convert oder query)
Wenn Sie die [ACTION]s convert oder query nutzen, erfolgt die Ausgabe in einem Korpusformat (und nicht als Tabelle). 
```SHELL
cec.exe import#[IMPORTER]#[FILES] convert [OUTPUT]
cec.exe import#[IMPORTER]#[FILES] query [QUERY] [OUTPUT]
```

Folgende Ausgabe-Formate stehen in der Standardinstallation zur Verfügung:
```SHELL
[OUTPUT] = Cec6#[FILE]
[OUTPUT] = Query#[FILE]
[OUTPUT] = AnnoationPro#[FILE]
[OUTPUT] = Catma#[FILE]
[OUTPUT] = Conll#[FILE]
[OUTPUT] = Cec5#[FILE]
[OUTPUT] = CorpusWorkBench#[FILE]
[OUTPUT] = Csv#[FILE]
[OUTPUT] = CsvMetadataOnly#[FILE]
[OUTPUT] = Dta#[FILE]
[OUTPUT] = Dta2017#[FILE]
[OUTPUT] = DwdsTei#[FILE]
[OUTPUT] = HtmlPure#[FILE]
[OUTPUT] = Json#[FILE]
[OUTPUT] = Plaintext#[FILE]
[OUTPUT] = PlaintextPureInOneFile#[FILE]
[OUTPUT] = PlaintextPure#[FILE]
[OUTPUT] = SketchEngine#[FILE]
[OUTPUT] = SlashA#[FILE]
[OUTPUT] = Speedy#[FILE]
[OUTPUT] = Tlv#[FILE]
[OUTPUT] = TreeTagger#[FILE]
[OUTPUT] = Txm#[FILE]
[OUTPUT] = Weblicht#[FILE]
[OUTPUT] = OpusXces#[FILE]
[OUTPUT] = Xml#[FILE]
```
Notiz: [FILE] = Eine beliebige Datei, in der die Ausgabe gespeichert werden soll
Bsp.: 'convert': cec.exe import#Cec5#C:\mycorpus.cec5 convert Cec6#C:\mycorpus.cec6
Bsp.: 'query': cec.exe import#Cec5#C:\mycorpus.cec5 query !M:Author::Jan Cec6#C:\mycorpus.cec6

### [ACTION]
Für ACTION können folgenden Befehle genutzt werden (Argumente in [] sind verpflichtend, Argumente in {} sind optional.):
```SHELL
[ACTION] = basic-information - Basis-Informationen wie z. B. Token, Sätze, Dokumente
[ACTION] = cluster [QUERY] [TASK] {ARGUMENTS} - Führt einen [TASK] für jedes Cluster aus (erzeugt durch [QUERY])
[ACTION] = cluster-list [QUERY] - Funktioniert wie 'cluster', mit dem Unterschied, dass hier eine Liste von Dokument GUIDs zurückgegeben wird.
[ACTION] = convert - siehe Hilfe-Abschnitt [OUTPUT] für weitere Informationen
[ACTION] = cooccurrence [LAYER] {minSIGNI} {minFREQ} -Signifikante Kookkurrenzen für alle [LAYER] Werte
[ACTION] = cooccurrence-corresponding [LAYER1] [LAYER2] [ANY] [WORDS] - Signifikante Kookkurrenzen in [LAYER1] korrespondierend gefiltert durch [LAYER2] [WORDS] - ([ANY] = any matches [bool]).
[ACTION] = cooccurrence-cross [LAYER] [WÖRTER] - Kreuz-Kookkurrenzen für die [WÖRTER] im [LAYER].
[ACTION] = cooccurrence-cross-full [LAYER] [WORDS] - Berechnet alle Kookkurrenzen zu [WORDS] in [LAYER] (vervollständigte Liste).
[ACTION] = cooccurrence-profile [LAYER] [WORD] - Kookkurrenz-Profil fürr [WORD] im [LAYER].
[ACTION] = cooccurrence-select [LAYER] [WORDS] - Signifikante Kookkurrenzen aller Werte in [LAYER]
[ACTION] = corresponding [LAYER1] [LAYER2] - findet alle korrespondierenden Werte zweischen den LAYERn 1 & 2 (z. B. 1: Lemma / 2: Wort)
[ACTION] = corresponding-metaphone [LAYER1] [LAYER2] - find all corresponding types betweet LAYER1 & LAYER2 (e. g. 1: Lemma / 2: Wort) based on metaphone
[ACTION] = cross-frequency {LAYER} - Kreuz-Frequenz in [LAYER]
[ACTION] = cross-frequency-corresponding [LAYER1] [LAYER2] [ANY] [WORDS] - Berechnet die Kreuz-Frequenz im [LAYER] die mit [LAYER2] [WORDS] korrespondieren.
[ACTION] = disambiguiert [WORD] im [LAYER]
[ACTION] = dispersion [LAYER] [META] - Berechnet die Dispersion von Werten in [LAYER] basierend auf [META]
[ACTION] = dispersion-corresponding [LAYER1] [META] [LAYER2] [ANY] [WORDS] - Berechnet die Dispersion aller Werte aus [LAYER1] in [META] korrespondierend gefiltert mit [LAYER2] [WORDS].
[ACTION] = editdist [LAYER] - Berechnet die Edit-Distanz aller Dokumente im [LAYER] untereinander.
[ACTION] = frequency1 {LAYER} - Frequenzliste für [LAYER] (Voreinstellung: Wort)
[ACTION] = frequency1-raw {LAYER} - Frequenz (ohne relative Frequenz)
[ACTION] = frequency1-select [LAYER] [WORDS/FILE/SDM] - Berechnet die Frequenz aller [WORDS in [LAYER] = Leerzeichen getrennt [FILE] = Pro Zeile ein Token [SDM] = SDM-Datei
[ACTION] = frequency2 {LAYER1} {LAYER2} - Frequenzliste über zwei Layer (Voreinstellung: Lemma, Wort)
[ACTION] = frequency2-raw {LAYER1} {LAYER2} - Frequenz über zwei Layer (ohne relative Frequenz)
[ACTION] = frequency3 {LAYER1} {LAYER2} {LAYER3} - Frequenzliste über 3 Layer (Voreinstellung: POS, Lemma, Wort)
[ACTION] = frequency3-raw {LAYER1} {LAYER2} {LAYER3} - Frequenz über drei Layer (ohne relative Frequenz)
[ACTION] = get-document [GUID] {LAYER} - Gibt das Dokument [GUID] von [LAYER] zurück.
[ACTION] = get-document-displaynames - Gibt alle Dokumente mit GUID und Anzeigename zurück.
[ACTION] = get-document-metadata [GUID] - Gibt alle Metadaten zum Dokument [GUID] zurück.
[ACTION] = get-types [LAYER] - list all [LAYER]-values (types)
[ACTION] = hash [LAYER] [ALGO] - Hashwert für alle Dokumente in [LAYER]. [ALGO] = MD5, SHA1, SHA256, SHA512
[ACTION] = hash-roll [LAYER] - calculates a rolling hashsum for all documents in [LAYER].
[ACTION] = how-many-documents - Summe aller Dokumente
[ACTION] = how-many-sentences - Summe aller Sätze
[ACTION] = how-many-tokens - Summe aller Token
[ACTION] = how-many-types [LAYER] - Summe aller [LAYER]-Werte (Types)
[ACTION] = idf [META] {LAYER} - Inverse Dokumenten-Frequenz für [META] im {LAYER} (Voreinstellung: WORT)
[ACTION] = keyword [LAYER] [TSV_RefFile] [COL_Token] [COL_RelFreq] - berechnet Keyworte im [LAYER]-mit Hilfe einer Referenzlist [TSV_RefFile].
[ACTION] = keyword [LAYER1] [TSV_RefFile] [COL_Token] [COL_RelFreq] [LAYER2] [WORDS2] - Berechnet Keywords (siehe [ACTION = keyword]) und wendet einen korrespondierenden Filter an.
[ACTION] = kwic-any [LAYER] [WORDS] - KWIC für jedes Vorkommen - [WORDS] = Leerzeichen getrennt
[ACTION] = kwic-document [LAYER] [WORDS] - [WORDS] = Leerzeichen getrennt - Ein Dokumente muss alle [WORDS] enthalten.
[ACTION] = kwic-first-any [LAYER] [WORD] [WORDS] - KWIC erstes [WORD] dann ein beliebiges [WORDS] = Leerzeichen getrennt
[ACTION] = ner [NERFILE] - Führt eine NER-Analyse durch und gibt auch alle Fundstellen aus.
[ACTION] = kwic-phrase [LAYER] [WORDS] - [WORDS] = Leerzeichen getrennt - Alle [WORDS] in der angegebenen Reihenfolge.
[ACTION] = kwic-sentence [LAYER] [WORDS] - [WORDS] = Leerzeichen getrennt - Ein Satz muss alle [WORDS] enthalten.
[ACTION] = kwic-sig [LAYER] [WORDS] - KWIC mit Siginifkanz-Metrik - [WORDS] = Leerzeichen getrennt - [y/n] y= Ja / n = nein - zum Aktivieren des HTML-Highlights
[ACTION] = kwit [LAYER1] [LAYER2] [minFREQ] [WORDS] - Erzeugt einen KWIT-Baum. Sucht alle [WORDS] in [LAYER1] ([minFREQ] = Minimum-Frequenz) - Ausgabe in [LAYER2] - [WORDS] = Token separiert durch Leerzeichen (die Abfrage erfolgt als Phrase).
[ACTION] = kwit-n [LAYER1] [LAYER2] [minFREQ] [PRE] [POST] [WORDS] - Funktioniert wie [ACTION = kwit], jedoch kann der Suchbereich mittels [PRE] und [POST] eingeschränkt werden.
[ACTION] = layer-names - Auflistung aller [LAYER]-Namen
[ACTION] = lda [CONFIG] {TOPIC-EXPORT} - [CONFIG] must be a JSON-Config file. If the file don't exists a new file will be created. Use {TOPIC-EXPORT} to export a additional topic-list.
[ACTION] = meta - Listet alle Meta-Kategorien, Meta-Werte sowie deren Token/Type/Dokumenten-Anzahl auf.
[ACTION] = meta-by-document - Listet alle Dokumente und deren Metadaten
[ACTION] = meta-categories - Liste aller Meta-Kategorien
[ACTION] = metaphone [LAYER] - convert all types (in layer) into metaphone representations.
[ACTION] = meta-select [category_1..n] - Listet alle Meta-Kategorien, Bezeichnungen und zählt für diese Tokens/Types/Dokumente
[ACTION] = meta-select+domain [category_1..n] - Listet alle Metadaten (Kategorien, Bezeichnungen sowie Frequenzen für Token/Type/Dokumente) - Reduziert URLs (falls vorhanden) auf die Domain.
[ACTION] = mtld [LAYER] [META] - Berechnet MTLD für [LAYER] cluster durch [META]
[ACTION] = ner [NERFILE] - Führt eine NER-Analyse durch
[ACTION] = ngram [N] {LAYER} {minFREQ} - [N]Gramme basierend auf [LAYER] (Voreinstellung: Wort)
[ACTION] = ngram [N] [LAYER1] [minFREQ] [LAYER2] [ANY] [WORDS2] - [N]-gram basierend auf [LAYER1] - korrespondierend gefiltert durch [LAYER2] [WORD2].
[ACTION] = ngram-select [N] [LAYER] [minFREQ] [WORDS] - Alle [N]Grammes im [LAYER] die [WORDS] beinhalten.
[ACTION] = position-frequency [LAYER1] [WORD] -links/recht-Frequenz um ein gegebenes [WORD]
[ACTION] = position-frequency [LAYER1] [WORD1] [LAYER2] [WORDS2] - Links-/Rechts-Positionsfrequenz von Kollokaten zu [WORD1] in [LAYER1] + korrespondierend gefiltert mit [LAYER2] [WORDS2].
[ACTION] = query [QUERY] - siehe Hilfe-Abschnitt [OUTPUT] für weitere Informationen
[ACTION] = query-count-documents [QUERY/FILE] - Zählt die Dokumente auf die [QUERY] zutrifft.
[ACTION] = query-count-sentences [QUERY/FILE] - Zählt, wieviele Sätze zum [QUERY] passen.
[ACTION] = query-count-tokens [QUERY/FILE] - Zählt, wie häufig [QUERY] vorkommt.
[ACTION] = query-list [QUERY] [NAME] - Funktioniert wir 'query' nur mit dem Unterschied, dass eine Liste aller [GUID]s zurückgegeben wird und kein Teilkorpus.
[ACTION] = reading-ease {LAYER} - reading ease of {LAYER} (default: Wort)
[ACTION] = similarity [META] {LAYER} - Ähnlichkeit von [META] basierend auf {LAYER} (Voreinstellung: WORT)
[ACTION] = style-burrowsd [META1] [META2] - vergleicht [META1] mit [META2] basiert auf "Burrows Delta"
[ACTION] = style-ngram [LAYER] [META] [N] [minFREQ] -Stil-Analyse basierend auf NGrammen
[ACTION] = tf [META] {LAYER} - Term-Frequenz für [META] in {LAYER} (Voreinstellung: WORT)
[ACTION] = tf-idf [META] {LAYER} - TF-IDF für [META] in {LAYER} (Voreinstellung: WORT)
[ACTION] = token-list [LAYER] - Liste aller Token im [LAYER]
[ACTION] = token-list-select [LAYER] [REGEX] - Listet alle Token in [LAYER] die zum [REGEX]-Ausdruck passen.
[ACTION] = vocabulary-complexity {LAYER} -Vokabular Komplexität in {LAYER}
[ACTION] = vocd [LAYER] [META] - berechnet VOCD auf [LAYER] Cluster mittels [META]
```
#### [ACTION] query
Seit August 2017 nutzen alle CorpusExplorer-Terminals die selbe Abfragesyntax. D. h. Sie können auch aus der Windows-GUI Abfragen exportieren und in der Konsole nutzen.
Beispiel der Abfragesyntax:

```SHELL
cec.exe [INPUT] query [QUERY] [EXPORT]
```

Aus einer oder mehren Quellen (gleichen Typs) werden mittels [QUERY] alle passenden Dokumente ausgewählt und in [EXPORT] geschrieben.
Sie können immer nur ein einzelne Abfrage(gruppe) pro Kommandozeilenaufruf übergeben.
Um mehrere hintereinander geschaltete Abfragen zu starten, rufen Sie entweder mehrfach hintereinander die Kommandozeile auf
oder nutzen Sie anstelle von [QUERY] folgende Angabe: FILE:[FILENAME]
Damit wird es möglich, Abfrage aus einer zuvor gespeicherten *.ceusd zu laden.
Bsp.:

```SHELL
cec.exe [INPUT] query FILE:C:/query.ceusd [EXPORT]
```

Pro Zeile ist nur eine Abfrage(-gruppe) zulässig.

##### [ACTION] query - Abfragesyntax

Primäre Typen und deren Operatoren (geordnet nach Priorität):
- ( = Beginnt eine Abfrage mit ( so muss das Ende der Zeile mit ) abgeschlossen werden. Dieser Typ definiert eine Abfragegruppe. Damit lassen sich mehrere Abfragen mittels OR verschachteln. Getrennt werden die Abfragen mit |.
- ! = Negiert die folgende Abfrage
- M = Abfrage von Metadaten
	- ? = Übergbener Wert wird als Regex-Ausdruck interpretiert.
	- ! = Meta-Angabe muss leer sein.
	- . = Meta-Angabe muss diesen Wert enthalten.
	- : = Meta-Angabe muss diesen Wert enthalten (case-sensitive).
	- \- = Meta-Angabe muss diesem Wert entsprechen.
	- = = Meta-Angabe muss diesem Wert entsprechen (case-sensitive).
- T = Abfragen von Text/Layer-Werten
	- ~ = Mindestens ein Wert aus der Liste muss im Dokument vorkommen.
	- \- = Alle Werte aus der Liste müssen im Dokument vorkommen.
	- = = Alle Werte aus der Liste müssen in einem Satz vorkommen.
	- § = Alle Werte aus der Liste müssen exakt in der Listenreihenfolge vorkommen.
- X = Erweiterte Optionen
	- R = Zufälle Auswahl (Bsp.: XR::100 - Wählt 100 Dokumente zufällig aus).
	- S = Autosplit auf Basis einer Metaanagabe - Syntax:
		- Direkt nach XS muss der Namen der Meta-Angabe erfolgen. Bsp.: XSAutor
		- Gefolgt vom Trennoperator ::
		- Nach dem Trennoperator muss angegben werden, wie die Meta-Angabe zu interpretieren ist. Zulässig sind folgende Werte: TEXT, INT, FLOAT oder DATE. Für diese Werte gelten folgende Regeln:
			- TEXT = Jeder unterschiedliche Wert wird als separat betrachtet.
			- INT und FLOAT = Geben Sie an, wieviele Cluster Sie benötigen. z. B. 10 -> INT;10
			- DATE = Für DATE stehen mehrere Optionen zur Verfügung:
				- DATE;C;10 = Datumscluster (siehe oben - INT und FLOAT).
				- DATE;CEN = Cluster nach Jahrhunderten (23.12.1985 > 19)
				- DATE;DEC = Cluster nach Jahrzehnten (23.12.1985 > 198)
				- DATE;Y = Cluster nach Jahren
				- DATE;YM = Cluster nach Jahr/Monat
				- DATE;YMD = Cluster nach Jahr/Monat/TAG
				- DATE;YMDH = Cluster nach Jahr/Monat/TAG/Stunde
				- DATE;YMDHM = Cluster nach Jahr/Monat/TAG/Stunde/Minute
				- DATE;ALL = Cluster nach exakter Zeitangabe (Millisekunde)
		- Beispiele:
			- XSAutor::TEXT - Cluster für unterschiedliche Autorennamen
			- XSJahr::INT;10 - Zehn Cluster mittels Jahreszahl-Angabe
			- XSDatum::DATE;C;10 - Zehn Cluster mittels Datums-Angabe
			- XSDatum::DATE;YM - Cluster für Monate mittels Datums-Angabe

Typ, Operator und Abfrageziel werden ohne trennende Leerzeichen geschrieben. Dieser Erste Teil ist durch :: vom Wert-Teil getrennt.

Beispiele:
```SHELL
cec.exe import#Cec6#C:/input.cec6 query !T~Wort::aber;kein ExporterCec6#C:/output.cec6
```
Was bedeutet diese Abfrage? negiere (__!__) die Text-Abfrage (__T__) welche einen beliebigen Wert (__~__) aus der Liste (alles nach __::__ - Trennung mittels ;) sucht. Gefunden werden also alle Dokumente, die weder __aber__ noch __kein__ enthalten.

Um die folgenden Beispiele möglichst kurz zu halten, wurde die sonst übliche Dokumentation des Konsolenaufrufs eingekürzt. Wiedergeben wird hier nur die Abfragesyntax.

```SHELL
(!M-Verlag::Spiegel | !T~Wort::Diktator)
```
Gruppen-Abfrage: Gefunden werden alle Dokumente die entweder nicht (__!__) in der Meta-Angabe (__M__) __Spiegel__ enthalten (__-__) ODER nicht (__!__) das __Wort__ __Diktator__ enthalten (__~__).

```SHELL
M!Verlag::
```
Leere-Metadaten: Gibt alle Dokumente zurück, für die keine Meta-Daten zu __Verlag__ hinterlegt sind.

```SHELL
T§Wort::den;Tag;nicht;vor;dem;Abend;loben
```
Findet alle Dokumente, die im Volltext (__T__) (__Wort__-Layer) die Phrase (__§__) "den Tag nicht vor dem Abend loben" enthalten.

#### [ACTION] convert
Erlaubt es, ein bestehendes Korpus in ein anderes Korpusformat zu konvertieren.
```SHELL
cec.exe import#Cec6#"C:/korpus.cec6" convert ExporterCec6#corpusOut.cec6
```

# ENGLISH VERSION <<< <<< <<< <<< <<< 

Syntax for annotation/conversion:
```SHELL
cec.exe [INPUT] convert [OUTPUT]
```
Syntax for filtering:
```SHELL
cec.exe [INPUT] [QUERY] [OUTPUT]
```
Syntax for analytics (writes output to stdout):
```SHELL
cec.exe {F:FORMAT} [INPUT] [ACTION]
```
Syntax for analytics (writes output to file - like C:\out.xyx):
```SHELL
cec.exe [F:FORMAT]#"C:\out.xyx" [INPUT] [ACTION]
```
Syntax for scripting:
```SHELL
cec.exe FILE:[PATH]
```
More detailed scripting errors:
```SHELL
cec.exe DEBUG:[PATH]
```
To start interactive shell mode
```SHELL
cec.exe SHELL
```
To start a REST-WebService
```SHELL
cec.exe {F:FORMAT} PORT:2312 {IP:127.0.0.1} {TIMEOUT:120} {INPUT}
```

### [INPUT]
Import corpus material - direct[INPUT]:
```SHELL
[INPUT] = import#Cec6#[FILES]
[INPUT] = import#Bnc#[FILES]
[INPUT] = import#Catma#[FILES]
[INPUT] = import#ClanChildes#[FILES]
[INPUT] = import#Conll#[FILES]
[INPUT] = import#CoraXml08#[FILES]
[INPUT] = import#CoraXml10#[FILES]
[INPUT] = import#EchtzeitEngine#[FILES]
[INPUT] = import#Cec5#[FILES]
[INPUT] = import#Cec6Stream#[FILES]
[INPUT] = import#CorpusWorkBench#[FILES]
[INPUT] = import#Dewac#[FILES]
[INPUT] = import#Dta#[FILES]
[INPUT] = import#Dta2017#[FILES]
[INPUT] = import#FnhdC#[FILES]
[INPUT] = import#FolkerFln#[FILES]
[INPUT] = import#Korap#[FILES]
[INPUT] = import#SimpleJsonStandoff#[FILES]
[INPUT] = import#CorpusExplorerV1toV5#[FILES]
[INPUT] = import#Redewiedergabe#[FILES]
[INPUT] = import#SketchEngine#[FILES]
[INPUT] = import#Speedy#[FILES]
[INPUT] = import#Tiger#[FILES]
[INPUT] = import#Tlv#[FILES]
[INPUT] = import#TreeTagger#[FILES]
[INPUT] = import#Txm#[FILES]
[INPUT] = import#Weblicht#[FILES]
[INPUT] = import#OpusXces#[FILES]
```
Note: [FILES] = separate files with & - merges all files before processing
Example: cec.exe import#Cec5#C:\mycorpus1.cec5&C:\mycorpus2.cec5 convert Cec6#C:\mycorpus.cec6


### Annotate raw text - [INPUT]:
```SHELL
[INPUT] = annotate#Alto12#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#AnnotationPro#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Apaek#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Bawe#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Blogger#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#BundestagDrucksachen#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#BundestagPlenarprotokolle#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#BundestagDtdPlenarprotokolle#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Catma#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#ClarinContentSearch#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#ListOfScrapDocument#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Cosmas#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Csv#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#LeipzigerWortschatz#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#DigitalPlato#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Dpxc#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#DortmunderChatKorpus#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#SlashA#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#DtaBasisformat#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Dta#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Dta2017#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#DwdsTei#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#EasyHashtagSeparation#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#RawMsgMsg#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Epub#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Europarl#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#EuroParlUds#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#ExmaraldaExb#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#FolkerFln#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Folker#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Gutenberg#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Ids#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Korap#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#SimpleJsonStandoff#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Kidko#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#KleineanfrageDe#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#NexisCom#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Cec6#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#SimpleDocxDocument#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#SimpleHtmlDocument#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#TextSharpPdf#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#SimplePdfDocument#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#SimpleRtfDocument#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#PureXmlText#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Perseus#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Txt#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Pmg#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#PostgreSqlDump#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#PurlOrg#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#RssFeedItem#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Shakespeare#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Speedy#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Talkbank#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#P5Cal2#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#TextGrid#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Tiger#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Tsv#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Tumblr#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Twapper#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Twitter#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#TwitterStatus#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Txm#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#UniversalExcel#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Weblicht#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Wet#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#Wordpress#[TAGGER]#[LANGUAGE]#[DIRECTORY]
[INPUT] = annotate#YouTube#[TAGGER]#[LANGUAGE]#[DIRECTORY]
```
Note: [DIRECTORY] = any directory you like - all files will be processed
#### [TAGGER] & [LANGUAGE]:
```SHELL
[TAGGER] = ClassicTreeTagger ([LANGUAGE] = Deutsch, Englisch, Französisch, Italienisch, Niederländisch, Spanisch)
[TAGGER] = SimpleTreeTagger ([LANGUAGE] = Deutsch, Englisch, Französisch, Italienisch, Niederländisch, Spanisch)
[TAGGER] = TnTTagger ([LANGUAGE] = Deutsch, Englisch)
[TAGGER] = RawTextTagger ([LANGUAGE] = Universal)
[TAGGER] = OwnTreeTagger ([LANGUAGE] = Durch Skript definiert.)
[TAGGER] = UdPipeExeTagger ([LANGUAGE] = )
```
Example: cec.exe annotate#Dpxc#SimpleTreeTagger#Deutsch#C:\dpxc\ convert Cec6#C:\mycorpus.cec6


### [OUTPUT]

[OUTPUT-EXPORTER] - for query or convert:
```SHELL
[OUTPUT] = Cec6#[FILE]
[OUTPUT] = Query#[FILE]
[OUTPUT] = AnnoationPro#[FILE]
[OUTPUT] = Catma#[FILE]
[OUTPUT] = Conll#[FILE]
[OUTPUT] = Cec5#[FILE]
[OUTPUT] = CorpusWorkBench#[FILE]
[OUTPUT] = Csv#[FILE]
[OUTPUT] = CsvMetadataOnly#[FILE]
[OUTPUT] = Dta#[FILE]
[OUTPUT] = Dta2017#[FILE]
[OUTPUT] = DwdsTei#[FILE]
[OUTPUT] = HtmlPure#[FILE]
[OUTPUT] = Json#[FILE]
[OUTPUT] = Plaintext#[FILE]
[OUTPUT] = PlaintextPureInOneFile#[FILE]
[OUTPUT] = PlaintextPure#[FILE]
[OUTPUT] = SketchEngine#[FILE]
[OUTPUT] = SlashA#[FILE]
[OUTPUT] = Speedy#[FILE]
[OUTPUT] = Tlv#[FILE]
[OUTPUT] = TreeTagger#[FILE]
[OUTPUT] = Txm#[FILE]
[OUTPUT] = Weblicht#[FILE]
[OUTPUT] = OpusXces#[FILE]
[OUTPUT] = Xml#[FILE]
```
Note: [FILE] = any file you like to store the output
Example 'convert': cec.exe import#Cec5#C:\mycorpus.cec5 convert Cec6#C:\mycorpus.cec6
Example 'query': cec.exe import#Cec5#C:\mycorpus.cec5 query !M:Author::Jan Cec6#C:\mycorpus.cec6

### [QUERY]:
A preceding ! inverts the entire query
- First character:
	- M = Metadata
	- T = (Full)Text
	- X = Extended Features
	- followed by configuration (see below), the :: separator and the values
- Second character [OPERATOR] 
	- (if you choose M):
		-  ? = regEx
		-  : = contains (case sensitive)
		-  . = contains (not case sensitive)
		-  = = match exact (case sensitive)
		-  - = match exact (not case sensitive) 
		-  ! = is empty
		-  ( = starts with (case sensitive)
		-  ) = ends with (case sensitive)
		If you have chosen M - enter the name of the meta category (see [ACTION] = meta-categories). 
		Example (query only): !M:Author::Jan - Finds all documents where "Jan" isn't an author 
		Example (in action): cec.exe import#Cec6#C:\mycorpus.cec6 query !M:Author::Jan Cec6#C:\mycorpus.cec6
	- (if you choose T):
		- ~ = any match
		- - = all in one document
		- = = all in one sentence
		- § = exact phrase
		- ? = regEx value 
		- F = regEx fulltext-search (very slow) 
		- 1 = first plus any other match
		If you have chosen T - enter the layer name (see [ACTION] = layer-names) 
		Example (query only): T§Wort::OpenSource;Software - Finds all documents with the exact phrase "OpensSource Software" 
		Example (in action): cec.exe import#Cec6#C:\mycorpus.cec6 query T§Wort::OpenSource;Software Cec6#C:\mycorpus.cec6 
		Note 1: If you use several words in a T-query, then separate them with ';'
		Note 2: You can also use a query file (*.ceusd) - use the FILE: prefix | Example: cec.exe import#Cec6#C:\mycorpus.cec6 query FILE:C:\query.ceusd Cec6#C:\mycorpus.cec6
	- (if you choose X):
		-  R = random selection
		If you use XR for random selection you need to specify the document count
Example: cec.exe import#Cec6#C:\mycorpus.cec6 query XR::100 frequency1 Wort | Note 4: XR will generate two outputs - the regular and the inverted output.
		-  S = auto split by meta-data (use cluster for auto split)
		If you have chosen XS - enter the name of the meta category (see [ACTION] = meta-categories)
	- Enter the separator :: followed by the query
		- If you use XS you must specify the meta data type - TEXT, INT, FLOAT or DATE
		Note 5: XS will generate multiple outputs - based on clusters.
		TEXT generates for every entry a separate snapshot
		Example: cec.exe import#Cec6#C:\mycorpus.cec6 cluster XSAuthor::TEXT frequency1 Wort
		INT / FLOAT you need to set up a [CLUSTERSIZE]
		Example: cec.exe import#Cec6#C:\mycorpus.cec6 cluster XSYear::INT;10 Cec6#C:\mycorpus.cec6
		DATE;C;[CLUSTERSIZE] - generates [CLUSTERSIZE] clusters.
		Example: cec.exe import#Cec6#C:\mycorpus.cec6 cluster XSDate::DATE;C;10 Cec6#C:\mycorpus.cec6
		- DATE;CEN = Century-Cluster / DATE;DEC = Decate-Cluster / DATE;Y = Year-Cluster
		- DATE;YW = Week-Cluster / DATE;YM = Year/Month-Cluster / DATE;YMD = Year/Month/Day-Cluster
		- DATE;YMDH = Year/Month/Day/Hour-Cluster / DATE;YMDHM = Year/Month/Day/Hour/Minute-Cluster / ALL = Every-Time-Cluster
		- Example: cec.exe import#Cec6#C:\mycorpus.cec6 cluster XSDate::DATE;YMD Cec6#C:\mycorpus.cec6
		- WINDOW = Add WINDOW + SIZE as an prefix for each cluster argument to enable the rolling window feature
		- Example: cec.exe import#Cec6#C:\mycorpus.cec6 cluster XSDate::WINDOW7;DATE;YMD Cec6#C:\mycorpus.cec6


### [ACTION] 

Most actions accept arguments. [ARG] is a required argument. {ARG} is an optional argument.
```SHELL
[ACTION] = basic-information - basic information tokens/sentences/documents
[ACTION] = cluster [QUERY] [TASK] {ARGUMENTS} - executes a [TASK] for every cluster (generated by [QUERY])
[ACTION] = cluster-list [QUERY] - works like cluster but returns clusters with document GUIDs.
[ACTION] = convert - see help section [OUTPUT] for more information
[ACTION] = cooccurrence [LAYER] {minSIGNI} {minFREQ} - significant cooccurrences for all [LAYER] values
[ACTION] = cooccurrence-corresponding [LAYER1] [LAYER2] [ANY] [WORDS] - significant cooccurrences for all [LAYER1] values with correspondig [LAYER2] [WORDS] - ([ANY] = any matches [bool]).
[ACTION] = cooccurrence-cross [LAYER] [WORDS] - significant cooccurrence cross for [WORDS] on [LAYER].
[ACTION] = cooccurrence-cross-full [LAYER] [WORDS] - significant cooccurrence cross for [WORDS] on [LAYER] (includes all cooccurrences).
[ACTION] = cooccurrence-profile [LAYER] [WORD] - significant cooccurrence profile for [WORD] on [LAYER].
[ACTION] = cooccurrence-select [LAYER] [WORDS] - significant cooccurrences for all [LAYER] values.
[ACTION] = corresponding [LAYER1] [LAYER2] - find all corresponding values betweet LAYER1 & LAYER2 (e. g. 1: Lemma / 2: Wort)
[ACTION] = corresponding-metaphone [LAYER1] [LAYER2] - find all corresponding types betweet LAYER1 & LAYER2 (e. g. 1: Lemma / 2: Wort) based on metaphone
[ACTION] = cross-frequency {LAYER} - calculates the cross-frequency based on [LAYER]
[ACTION] = cross-frequency-corresponding [LAYER1] [LAYER2] [ANY] [WORDS] - calculates the cross-frequency based on [LAYER] and apply corresponding [LAYER2] [WORDS] filter.
[ACTION] = disambiguation [LAYER] [WORD] - allows to disambiguate a [WORD] on [LAYER].
[ACTION] = dispersion [LAYER] [META] - calculates dispersions values of all [LAYER] values based on [META]
[ACTION] = dispersion-corresponding [LAYER1] [META] [LAYER2] [ANY] [WORDS] - calculates dispersions values of all [LAYER1] values based on [META] and annply correspondign filter.
[ACTION] = editdist [LAYER] - caculates the edit distance for all (to all) documents in [LAYER]
[ACTION] = frequency1 {LAYER} - count token frequency on {LAYER} (default: Wort)
[ACTION] = frequency1-raw {LAYER} - count token frequency on [LAYER] (no rel. frequency)
[ACTION] = frequency1-select [LAYER] [WORDS/FILE/SDM] - count token frequency on 1 [LAYER] - [WORDS] = space separated tokens [FILE] = one line one token [SDM] = SDM-File
[ACTION] = frequency2 {LAYER1} {LAYER2} - count token frequency on 2 layers (default: Lemma, Wort)
[ACTION] = frequency2-raw {LAYER1} {LAYER2} - count token frequency on 2 layers (no rel. frequency)
[ACTION] = frequency3 {LAYER1} {LAYER2} {LAYER3} - count token frequency on 3 layers (default: POS, Lemma, Wort)
[ACTION] = frequency3-raw {LAYER1} {LAYER2} {LAYER3} - count token frequency on 3 layers (no rel. frequency)
[ACTION] = get-document [GUID] {LAYER} - get all layer-information for specific [GUID] document. Use {LAYER} to filter output.
[ACTION] = get-document-displaynames - get all document GUID / display-names.
[ACTION] = get-document-metadata [GUID] - get all metadata for specific [GUID] document.
[ACTION] = get-types [LAYER] - list all [LAYER]-values (types)
[ACTION] = hash [LAYER] [ALGO] - calculates a hashsum for all documents in [LAYER]. [ALGO] = MD5, SHA1, SHA256, SHA512
[ACTION] = hash-roll [LAYER] - calculates a rolling hashsum for all documents in [LAYER].
[ACTION] = how-many-documents - sum of all documents
[ACTION] = how-many-sentences - sum of all sentences
[ACTION] = how-many-tokens - sum of all tokens
[ACTION] = how-many-types [LAYER] - sum of all [LAYER]-values (types)
[ACTION] = idf [META] {LAYER} - inverse document frequency for [META] on {LAYER} (default: WORT)
[ACTION] = keyword [LAYER] [TSV_RefFile] [COL_Token] [COL_RelFreq] - calculates the keynes of any [LAYER]-value by using a reference list [TSV_RefFile].
[ACTION] = keyword-corresponding [LAYER1] [TSV_RefFile] [COL_Token] [COL_RelFreq] [LAYER2] [WORDS2] - calculates keyword (see [ACTION = keyword]) and applies the corresponding filter.
[ACTION] = kwic-any [LAYER] [WORDS] - KWIC any occurrence - [WORDS] = space separated tokens
[ACTION] = kwic-document [LAYER] [WORDS] - [WORDS] = space separated tokens - a document must contains all token
[ACTION] = kwic-first-any [LAYER] [WORD] [WORDS] - KWIC any occurrence - [WORDS] = space separated tokens (KWIC must contains first token + any other)
[ACTION] = ner [NERFILE] - performs a named entity recorgnition + kwic-resuls
[ACTION] = kwic-phrase [LAYER] [WORDS] - [WORDS] = space separated tokens - all token in one sentence + given order
[ACTION] = kwic-sentence [LAYER] [WORDS] - [WORDS] = space separated tokens - a sentence must contains all token
[ACTION] = kwic-sig [LAYER] [y/n] [WORDS] - KWIC with significance metrics - [WORDS] = space separated tokens - Enable HTML-Highlight [y/n]
[ACTION] = kwit [LAYER1] [LAYER2] [minFREQ] [WORDS] - Builds a KWIT-Tree. Search all [WORDS] in [LAYER1] (with minimum frequency [minFREQ]) - Output in [LAYER2] - [WORDS] = space separated tokens - all token in one sentence + given order
[ACTION] = kwit-n [LAYER1] [LAYER2] [minFREQ] [PRE] [POST] [WORDS] - Like kwit (but you can specificate the range [PRE] and [POST] the match - e.g. [PRE] = 3)
[ACTION] = layer-names - all available names for [LAYER]
[ACTION] = lda [CONFIG] {TOPIC-EXPORT} - [CONFIG] must be a JSON-Config file. If the file don't exists a new file will be created. Use {TOPIC-EXPORT} to export a additional topic-list.
[ACTION] = meta - lists all meta-categories, labels and token/type/document-count
[ACTION] = meta-by-document - list all documents with meta-data
[ACTION] = meta-categories - all available names for meta categories
[ACTION] = metaphone [LAYER] - convert all types (in layer) into metaphone representations.
[ACTION] = meta-select [category_1..n] - lists all meta-categories, labels and token/type/document-count for [category_1..n]
[ACTION] = meta-select+domain [category_1..n] - lists all meta-categories, labels and token/type/document-count for [category_1..n] (reduces all URLs > domain name only)
[ACTION] = mtld [LAYER] [META] - calculates MTLD for [LAYER] clustered by [META]
[ACTION] = ner [NERFILE] - performs a named entity recorgnition
[ACTION] = ngram [N] {LAYER} {minFREQ} - [N] sized N-gram based on {LAYER} (default: Wort)
[ACTION] = ngram-corresponding [N] [LAYER1] [minFREQ] [LAYER2] [ANY?] [WORDS2] - [N] sized N-gram based on [LAYER1] - apply [LAYER2] corresponding [WORD2] filter.
[ACTION] = ngram-select [N] [LAYER] [minFREQ] [WORDS/FILE] - all [N]-grams on [LAYER] containing [WORDS] or FILE:[FILE].
[ACTION] = position-frequency [LAYER1] [WORD] - left/right position of words around [WORD]
[ACTION] = position-frequency [LAYER1] [WORD1] [LAYER2] [WORDS2] - left/right position of words around [WORD1] in [LAYER1] + corresponding [LAYER2] [WORDS2] filter.
[ACTION] = query [QUERY] - see help section [OUTPUT] for more information
[ACTION] = query-count-documents [QUERY/FILE] - counts how many documents match the [QUERY]
[ACTION] = query-count-sentences [QUERY/FILE] - counts how many sentences match the [QUERY]
[ACTION] = query-count-tokens [QUERY/FILE] - counts how many token-spans match the [QUERY]
[ACTION] = query-list [QUERY] [NAME] - works like query, but returns a [NAME]ed list of document GUIDs.
[ACTION] = reading-ease {LAYER} - reading ease of {LAYER} (default: Wort)
[ACTION] = similarity [META] {LAYER} - [META] similarity based on {LAYER} (default: WORT)
[ACTION] = style-burrowsd [META1] [META2] - compares [META1] with [META2] based on "Burrows Delta"
[ACTION] = style-ngram [LAYER] [META] [N] [minFREQ] - style analytics based on ngram
[ACTION] = tf [META] {LAYER} - term frequency for [META] on {LAYER} (default: WORT)
[ACTION] = tf-idf [META] {LAYER} - term frequency * inverse term frequency for [META] on {LAYER} (default: WORT)
[ACTION] = token-list [LAYER] - list of all tokens in [LAYER]
[ACTION] = token-list-select [LAYER] [REGEX] - list of all tokens in [LAYER] who are matching the [REGEX]-expression
[ACTION] = vocabulary-complexity {LAYER} - vocabulary complexity in {LAYER}
[ACTION] = vocd [LAYER] [META] - calculates VOCD for [LAYER] clustered by [META]
```
Example: cec.exe import#Cec6#C:\mycorpus.cec6 frequency3 POS Lemma Wort


### [SCRIPTING] 

All actionss above can be stored in a file to build up a automatic process.
In this case it's recommended to redirect the [ACTION]-output to a file and not to stdout
Example: import#Cec6#C:\mycorpus.cec6 frequency3 POS Lemma Wort > output.csv


### [F:FORMAT]

If you use [ACTION] or the scripting-mode [FILE: / DEBUG:], you can change the output format.
You need to set one of the following tags as first parameter:
```SHELL
F:CSV - comma separated values
F:HTML - HTML5-document
F:TRTD - HTML-Table Skeleton
F:JSON - JavaScript Object Notation (JSON)
F:JSONR - JavaScript Object Notation (rounded values)
F:SQLDATA - SQL (data only)
F:SQLSCHEMA - SQL (schema only)
F:SQL - SQL (schema + data)
F:TSV - tab separated values
F:TSVR - tab separated values (rounded values)
F:XML - XML-document
```

Example: cec.exe F:JSON import#Cec6#C:\mycorpus.cec6 frequency3 POS Lemma Wort
Use FNT: to hide the TID (be carefull with this, if you are using CLUSTER)
Normal STDOUT redirections is very slow (like: cec.exe F:CSV import#Cec6#C:\corpus.cec6 frequency1 > C:\out.csv)
Use the optimized direct way, by adding the output-path to the [F:FORMAT]-option
Example: cec.exe F:CSV#C:\out.tsv import#Cec6#C:\mycorpus.cec6 frequency1
