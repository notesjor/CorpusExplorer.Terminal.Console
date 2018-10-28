# CorpusExplorer.Terminal.Console (früher: CorpusExplorer-Port-R)
Der CorpusExplorer steht neben der offiziellen GUI (http://www.corpusexplorer.de) auch als Konsolenanwendung zur Verfügung. Damit ist es möglich, aus anderen Programmen oder anderen Programmiersprachen auf Analysen/Daten des CorpusExplorers zuzugreifen. Ursprünglich wurde die Konsolen-Lösung unter dem Namen CorpusExplorer-Port-R entwickelt und sollte die Nutzung des CorpusExplorers innerhalb der Programmiersprache R ermöglichen.

## Installation Windows
1. Installieren Sie den CorpusExplorer (http://www.bitcutstudios.com/products/corpusexplorer/standard/publish.htm)
2. Fertig! (OPTIONAL) Installieren Sie eine aktuelle R-Version (http://ftp5.gwdg.de/pub/misc/cran/) WENN Sie den CorpusExplorer unter R nutzen möchten. Andernfalls können Sie auch auf die Windows-Konsole zurück greifen und die Programmausgabe mittels ">" in eine Datei (CSV) umleiten.

## Installation Linux/MacOS
1. Installieren Sie mono (http://www.mono-project.com/download/) - Mindestversion 4.x
2. Laden und entpacken Sie die folgende Datei: http://www.bitcutstudios.com/products/corpusexplorer/App.zip
3. Stellen Sie allen Aufrufen ein "mono" voran - z. B. "mono cec.exe import#ImporterCec6#demo.cec6 frequency". Mono führt die cec.exe aus (die sich im entpackten Ordner - siehe 2. - befindet).
__Einschränkung Linux/MacOS__: Gegenwärtig ist es noch nicht möglich, den Befehl "annotate" auszuführen. Alle anderen Befehle funktionieren einwandfrei (getestet auf Debian 8 - Mono 5.0.1).

## Grundlegendes
Der cec.exe greift auf das CorpusExplorer-Ökosystem zurück. D.h. auch alle installierten Erweiterungen für den CorpusExplorer sind nutzbar. Rufen Sie cec.exe über die Konsole ohne Parameter auf, dann erhalten Sie alle verfügbaren Scraper, Importer, Tagger und Exporter. Erweiterungen für den CorpusExplorer finden Sie hier: http://notes.jan-oliver-ruediger.de/software/corpusexplorer-overview/corpusexplorer-v2-0/erweiterungen/

Die Grundsyntax für den Konsolenaufruf lautet:
```SHELL
cec.exe [INPUT] [ACTION]
```
- Bsp. für R: 
```R
tbl <- read.table(pipe("cec.exe import#ImporterCec6#demo.cec6 frequency"), sep = "\t", header = TRUE, dec = ",", encoding = "UTF-8", quote = "")
```
- Bsp. für Konsole: 
```SHELL
cec.exe import#ImporterCec6#demo.cec6 frequency > frequency.tsv
```
- Bsp. für Konsole (andere Ausgabeformate): 
```SHELL
cec.exe F:TSV import#ImporterCec6#demo.cec6 frequency > frequency.tsv
cec.exe F:CSV import#ImporterCec6#demo.cec6 frequency > frequency.csv
cec.exe F:JSON import#ImporterCec6#demo.cec6 frequency > frequency.json
cec.exe F:XML import#ImporterCec6#demo.cec6 frequency > frequency.xml
cec.exe F:HTML import#ImporterCec6#demo.cec6 frequency > frequency.html
cec.exe F:SQL import#ImporterCec6#demo.cec6 frequency > frequency.sql
```
- Bsp. für REST-Aufruf (Maturity Level 2) - Erlaubt es ein Korpus als REST-Webservice zu hosten
cec.exe [F:FORMAT] PORT:[PORT] [INPUT]
```SHELL
cec.exe F:JSON PORT:3535 import#ImporterCec6#demo.cec6
```

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
cec.exe import#ImporterCec6#demo.cec6 [ACTION]
cec.exe import#DtaImporter#C:/DTAbf/doc01.tcf.xml [ACTION]
cec.exe import#WeblichtImporter#C:/Weblicht/page01.xml|C:/webL.xml [ACTION]
```
Hinweis: Einige Betriebssysteme (und Betriebssystemversionen) haben Problemen, wenn sich im Pfad Leerzeichen bedfinden. Vermeiden Sie daher soweit möglich Leerzeichen in Pfad und Dateinamen.

#### [INPUT] - annotate
Syntax für "annotate" (annotiert immer den gesamten Ordner):
```SHELL
cec.exe annotate#[SCRAPER]#[TAGGER]#[LANGUAGE]#[DIRECTORY] [ACTION]
```
Tipp: Starten Sie cec.exe ohne Parameter, um die verfügbaren [SCRAPER], [TAGGER] und [LANGUAGE] abzufragen.
Es wird empfohlen, "annotate" immer in Kombination mit den [ACTION]s "convert" oder "query" zu nutzen. Dadurch wird das fertig annotierte Korpus gespeichert und kann jederzeit mit "import" geladen werden, ohne das Korpus erneut zu annotieren. Bsp.:
```SHELL
cec.exe annotate#TwitterScraper#ClassicTreeTagger#Deutsch#"C:/korpus" convert ExporterCec6#"C:/korpus.cec6"
```

### [ACTION]
Für ACTION können folgenden Befehle genutzt werden (Argumente in [] sind verpflichtend, Argumente in {} sind optional.):
- basic-information - Gibt grundlegende Informationen aus Token/Sätze/Dokumente
- cluster [QUERY] [ACTION] {ARGUMENTS} - Erlaubt es einen [ACTION] über ein Cluster auszuführen. Das Cluster wird durch [QUERY] erzeugt.
- cluster-list [QUERY] - Führt die gleiche Analyse durch wie "cluster", doch werden die Cluster als Liste ausgegeben und nicht zur weiteren Auswertung herangezogen.
- convert [OUTPUT] - Konvertiert Korpusdaten in ein anderes Format - siehe [OUTPUT]
- cooccurrence [LAYER] {minSIGNI} {minFREQ} - Berechnet zu allen Worten in [LAYER] alle Kookkurrenzen. Erlaubt es optional ein Minimum für die Signifikanz [minSIGNI] und für die Frequenz [minFREQ] anzugeben. Standardwerte: [minSIGNI] = 0.9 / [minFREQ] = 1
- cooccurrence-select [LAYER] [WORDS] - Ermittel die Kookkurrenzen zu einem bestimmten Suchwort/Phrase.
- cross-frequency {LAYER} - Berechnet zu allen Worten in [LAYER] die Kreuzfrequenz (Standardlayer = Wort).
- frequency1 {LAYER} - Berechnet die Frequenzen für [LAYER] (Standardlayer = Wort).
- frequency1-select [LAYER1] [WORDS] - Berechnet die Frequenzen für [LAYER]. Dabei werden nur die gegebenen [WORDS] gezählt (Leerzeichen getrennt). Anstelle von [WORDS] kann auch FILE:[FILE], also eine Datei mit einer Wortliste (pro Zeile ein Wort), angegeben werden - ODER - SDM:[FILE], also eine Datei mit einem SDM-Datei (Sentiment-Detection-Model).
- frequency2 {LAYER1} {LAYER2} - Berechnet die Frequenzen über zwei Layer [LAYER1] [LAYER2] (Standardlayer = POS / Wort)
- frequency3 {LAYER1} {LAYER2} {LAYER3} - Berechnet die Frequenzen über drei Layer [LAYER1] [LAYER2] [LAYER3]  (Standardlayer = POS / Lemma / Wort)
- get-document [GUID] {LAYER} - Gibt alle Layer-Daten für das gewünschte Dokumente (GUID) zurück (Standardlayer = Wort).
- get-document-displaynames - Gibt alle Dokumente als GUID/Dokumentname zurück.
- get-document-metadata [GUID] - Gibt alle Metadaten eines Dokuments (GUID) zurück.
- get-types [LAYER] - Auflistung aller Types (ohne Frequenz) im [LAYER]
- how-many-documents - Anzahl der Dokumente
- how-many-sentences - Anzahl der Sätze
- how-many-tokens - Anzahl der Token
- how-many-types [LAYER] - Anzahl der Types in [LAYER]
- kwic-any [LAYER] [WORDS] - KWIC-Analyse der [WORDS] (Leerzeichen getrennt) in [LAYER]
- kwic-document [LAYER] [WORDS] - KWIC-Analyse der [WORDS] (Leerzeichen getrennt) in [LAYER] - Alle [WORDS] müssen in einem Dokument vorkommen
- kwic-first-any [LAYER] [WORD] - KWIC-Analyse der [WORDS] (Leerzeichen getrennt) in [LAYER] - Das [WORD] muss in einem Dokument vorkommen plus ein beliebiges [WORDS]
- kwic-phrase [LAYER] [WORDS] - KWIC-Analyse der [WORDS] (Leerzeichen getrennt) in [LAYER] - Alle [WORDS] müssen in exakt der gegebenen Reihenfolge vorkommen
- kwic-sentence [LAYER] [WORDS] - KWIC-Analyse der [WORDS] (Leerzeichen getrennt) in [LAYER] - Alle [WORDS] müssen in einem Satz vorkommen
- kwic-sig [LAYER] [WORDS] - KWIC-Analyse die wie kwic-phrase funktioniert aber zusätzliche Signifikanzdaten bietet.
- kwit [LAYER] [WORDS] - [WORDS] = Spezielle KWIC-Analyse, die die Daten als GraphViz-DiGraph aufbereitet. Siehe: kwic-phrase
- layer-names - Auflistung aller verfügbaren Layer
- meta - Auflistung aller Metadaten + Token/Type/Dokument-Frequenz
- meta-by-document - Auflistung aller Dokumente mit zugehörigen Metadaten
- meta-categories - Auflistung aller verfügbaren Meta-Kategorien
- mtld [LAYER] [META] - Berechnet MTLD für den [LAYER] automatische Clusterung basierend auf [META]
- ner [NERFILE] - Führt eine Named-Entity-Recognition mithilfe das zuvor ermittelten Wörterbuchs durch.
- n-gram [N] {LAYER} {minFREQ} - Berechnet [N]-Gramme für [LAYER] (Standardlayer = Wort). Optional: {minFREQ} = Mindestfrequenz / Standardwert: 1
- n-gram-selected [N] [LAYER] [minFREQ] [WORDS] - Gibt alle [N]-Gramme für [LAYER] mit einer Mindestfrequenz [minFREQ] aus, die [WORDS] enthalten.
- position-frequency [LAYER] [WORD] - Gibt die Links-/Rechtsfrequenz zu [WORD] in [LAYER] aus.
- query [QUERY] - Führt eine Abfrage auf der aktuell geladenen Korpusmenge durch. Siehe [OUTPUT]
- query-list [QUERY] [NAME] - Funktioniert wie query, nur dass das Ergebnis eine Liste mit den Ergebnissen ist und nicht ein Subkorpus.
- reading-ease [LAYER] - Berechnet verschiedene Lesbarkeitsindices für [LAYER]
- style-burrowsd [META1] [META2] - Stilanalyse mittels Burrows-Delta. Vergleicht zwei Metaangaben miteinander
- style-ngram [LAYER] [META] [N] [minFREQ] - Stilanalyse mittels N-Grammen. Siehe: n-gram
- vocabulary-complexity [LAYER] - Berechnet verschiedene Vokabularkomplexitäten für [LAYER]
- vocd [LAYER] [META] - Berechnet VOCD für [LAYER] automatische Clusterung basierend auf [META]

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

Typ, Operator und Abfrageziel werden ohne trennende Leerzeichen geschrieben. Dieser Erste Teil ist durch :> vom Wert-Teil getrennt.

Beispiele:
```SHELL
cec.exe import#ImporterCec6#C:/input.cec6 query !T~Wort:>aber;kein ExporterCec6#C:/output.cec6
```
Was bedeutet diese Abfrage? negiere (__!__) die Text-Abfrage (__T__) welche einen beliebigen Wert (__~__) aus der Liste (alles nach __:>__ - Trennung mittels ;) sucht. Gefunden werden also alle Dokumente, die weder __aber__ noch __kein__ enthalten.

Um die folgenden Beispiele möglichst kurz zu halten, wurde die sonst übliche Dokumentation des Konsolenaufrufs eingekürzt. Wiedergeben wird hier nur die Abfragesyntax.

```SHELL
(!M-Verlag:>Spiegel | !T~Wort:>Diktator)
```
Gruppen-Abfrage: Gefunden werden alle Dokumente die entweder nicht (__!__) in der Meta-Angabe (__M__) __Spiegel__ enthalten (__-__) ODER nicht (__!__) das __Wort__ __Diktator__ enthalten (__~__).

```SHELL
M!Verlag:>
```
Leere-Metadaten: Gibt alle Dokumente zurück, für die keine Meta-Daten zu __Verlag__ hinterlegt sind.

```SHELL
T§Wort:>den;Tag;nicht;vor;dem;Abend;loben
```
Findet alle Dokumente, die im Volltext (__T__) (__Wort__-Layer) die Phrase (__§__) "den Tag nicht vor dem Abend loben" enthalten.

#### [ACTION] convert
Erlaubt es, ein bestehendes Korpus in ein anderes Korpusformat zu konvertieren.
```SHELL
cec.exe import#ImporterCec6#"C:/korpus.cec6" convert ExporterCec6#corpusOut.cec6
```
