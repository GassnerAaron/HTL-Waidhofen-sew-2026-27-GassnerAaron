# VerlinkteFigurListe – SEW 4AHIT

Die Lösung zur Erweiterungsaufgabe enthält alle zehn Methoden und das Testprogramm in `Program.cs`. Das Projekt ist mit `dotnet new console` erstellt und verwendet .NET 8, wie die erste Übung im Repository.

## Starten und prüfen

Im Stammverzeichnis des Repositorys:

```sh
dotnet build SEW_02_VerlinkteFigurListe/SEW_02_VerlinkteFigurListe.csproj --configuration Release --warnaserror
dotnet run --project SEW_02_VerlinkteFigurListe/SEW_02_VerlinkteFigurListe.csproj --configuration Release --no-build
```

Alternativ im Projektordner einfach `dotnet build` und `dotnet run` ausführen. Benötigt werden ein .NET-8-SDK oder neuer und die .NET-8-Runtime.

Geprüft mit SDK 9.0.301: **0 Warnungen, 0 Fehler; 40 von 40 Prüfungen erfolgreich.** Die tatsächliche Konsolenausgabe steht in `Testausgabe.txt`. Wenn ein Test fehlschlägt, gibt das Programm `FEHLER` aus und beendet sich mit Exitcode 1.

Der gelieferte Startcode (`IFigur`, `Figur`, `Kugel`, `Wuerfel`) ist unverändert enthalten. `Nullable` ist im Projekt deaktiviert, weil der Startcode `Figur Next` ohne `?` verwendet, obwohl `null` das Listenende kennzeichnet. Die Nullfälle werden im Code ausdrücklich geprüft. Es werden keine Arrays, Collections oder generischen Klassen verwendet. `StringWriter` und `StringReader` dienen im Testprogramm ausschließlich dazu, die Konsolenausgabe zu vergleichen.

## Die Liste verstehen

Eine Figur ist gleichzeitig ein Knoten. Ihr `Next` zeigt auf die nächste Figur. `first` zeigt auf die erste Figur, `null` bedeutet „kein Nachfolger“. `Count` zählt die Figuren.

```text
first
  |
  v
 [A] -> [B] -> [C] -> null       Count = 3
```

Eine Referenzvariable speichert einen Verweis auf ein vorhandenes Objekt. `Figur a = current` erzeugt keine neue Figur. Auch beim Tauschen und Umdrehen bleiben die Objekte und ihre Beschreibungen, Radien und Seitenlängen erhalten. Nur die Pfeile ändern sich.

Beim Einfügen wird eine neue oder bereits entfernte einzelne Figur übergeben. Eine Figur darf nicht gleichzeitig zu mehreren Listen gehören. Zum Übertragen einer ganzen Liste verwendet man `Anhaengen`.

## Alle zehn Methoden einfach erklärt

### 0. GetAt(index)

Startet bei `first` und geht `index` Pfeile weiter. Die erste Position hat Index 0. Ein ungültiger Index liefert `null`. Die Methode ist privat und hilft `InsertAt` und `Remove`.

```text
Index:   0      1      2
        [A] -> [B] -> [C] -> null
GetAt(2) liefert die vorhandene Figur [C].
```

`Count` bleibt gleich. Laufzeit: O(n), weil im ungünstigsten Fall die Liste durchlaufen wird.

### 1. Add(fig)

Sucht die letzte Figur und setzt deren `Next` auf die eingefügte Figur. Deren `Next` wird `null`, weil sie jetzt die letzte ist. Bei einer leeren Liste wird sie direkt zu `first`.

```text
Vorher: [A] -> [B] -> null
Add(X): [A] -> [B] -> [X] -> null
```

`Count` steigt um 1. Bei `null` passiert nichts. Laufzeit: O(n).

### 2. AusgabeAllerFiguren()

Geht von `first` bis `null` und gibt jede Figur mit `Console.WriteLine` aus. Dabei wird `ToString()` aufgerufen: Beschreibung, Oberfläche und Volumen erscheinen in einer Zeile. Eine leere Liste erzeugt keine Ausgabe.

```text
[K1] -> [W1] -> null
Ausgabe: zuerst K1, dann W1; die Pfeile bleiben gleich.
```

`Count` bleibt gleich. Laufzeit: O(n).

### 3. Push(fig)

Fügt vorne ein. Zuerst `fig.Next = first`, dann `first = fig`. So bleibt der bisherige Listenanfang erreichbar. Das entspricht einem Stapel: Die zuletzt vorne eingefügte Figur wird beim nächsten `Pop` zuerst entfernt (LIFO).

```text
Vorher: [A] -> [B] -> null
Push(X): [X] -> [A] -> [B] -> null
```

`Count` steigt um 1. Bei `null` passiert nichts. Laufzeit: O(1).

### 4. Pop()

Merkt sich die erste Figur, setzt `first` auf ihren Nachfolger und setzt den `Next`-Pfeil der entfernten Figur auf `null`. Anschließend gibt die Methode genau diese Figur zurück. Bei einer leeren Liste liefert sie `null`.

```text
Vorher:     [A] -> [B] -> [C] -> null
Liste danach:     [B] -> [C] -> null
Rückgabe:   [A] -> null
```

`Count` sinkt um 1, wenn eine Figur entfernt wurde. Laufzeit: O(1).

### 5. InsertAt(index, fig)

Bei `index <= 0` wird `Push` benutzt, bei `index >= Count` wird `Add` benutzt. Dazwischen wird mit `GetAt(index - 1)` der Vorgänger gesucht. Erst zeigt die neue Figur auf den bisherigen Nachfolger, dann zeigt der Vorgänger auf die neue Figur.

```text
Vorher:        [A] -> [B] -> [C] -> null
InsertAt(1,X): [A] -> [X] -> [B] -> [C] -> null
```

`Count` steigt genau einmal um 1. Nach `Push` oder `Add` folgt deshalb sofort `return`. Bei `null` passiert nichts. Laufzeit: O(n).

### 6. Remove(index)

Entfernt die Figur am Index. Bei Index 0 übernimmt `Pop` die Arbeit. Sonst wird der Vorgänger gesucht und sein `Next` auf den Nachfolger der zu entfernenden Figur gesetzt. Die entfernte Figur wird anschließend durch `Next = null` abgetrennt. Ein ungültiger Index verändert nichts.

```text
Vorher:    [A] -> [B] -> [C] -> null
Remove(1): [A] -------> [C] -> null
Entfernt:         [B] -> null
```

`Count` sinkt nur bei einem gültigen Index um 1. Laufzeit: O(n).

### 7. Umdrehen()

Kehrt in einem Durchlauf alle Pfeile um. Drei Referenzen reichen: `prev` ist der bereits umgedrehte Anfang, `current` die gerade bearbeitete Figur, `next` der vorher gesicherte Nachfolger.

```text
Vorher: [A] -> [B] -> [C] -> null
Danach: [C] -> [B] -> [A] -> null
```

Schritt für Schritt:

1. Anfang: `prev = null`, `current = A`.
2. Bei A: `next = B` merken; `A.Next = null`; `prev = A`, `current = B`.
3. Bei B: `next = C` merken; `B.Next = A`; `prev = B`, `current = C`.
4. Bei C: `next = null` merken; `C.Next = B`; `prev = C`, `current = null`.
5. Die Schleife endet. `first = prev`, also `first = C`.

```text
Nach Schritt 2: [A] -> null          Rest: [B] -> [C] -> null
Nach Schritt 3: [B] -> [A] -> null   Rest: [C] -> null
Nach Schritt 4: [C] -> [B] -> [A] -> null
```

Für die mündliche Überprüfung: **„Ich merke mir zuerst den nächsten Knoten, drehe dann den Pfeil um und gehe anschließend zum gemerkten Knoten weiter.“** Ohne das vorherige Merken von `next` würde der Weg zur restlichen Liste verloren gehen. `Count` bleibt gleich. Eine leere Liste und eine Liste mit einer Figur funktionieren ebenfalls. Laufzeit O(n), zusätzlicher Speicher O(1).

### 8. VertauscheBenachbarte()

Bearbeitet immer ein vollständiges Paar. `a` ist die erste Figur des Paares, `b` die zweite und `rest` die erste Figur danach. `prevPaarEnde` merkt sich das Ende des vorherigen, bereits getauschten Paares.

```text
Vorher: [A] -> [B] -> [C] -> [D] -> [E] -> null
Danach: [B] -> [A] -> [D] -> [C] -> [E] -> null
```

Schritt für Schritt:

1. Erstes Paar: `a = A`, `b = B`, `rest = C`.
2. `b.Next = a` setzt den Pfeil B -> A.
3. `a.Next = rest` setzt den Pfeil A -> C. Zusammen entsteht B -> A -> C.
4. Beim ersten Paar wird `first = b`, also B. Danach: `prevPaarEnde = A`, `current = C`.
5. Zweites Paar: `a = C`, `b = D`, `rest = E`. Die neuen Pfeile sind D -> C und C -> E.
6. Jetzt muss das vorige Paar angeschlossen werden: `prevPaarEnde.Next = b`, also A -> D.
7. Danach `prevPaarEnde = C`, `current = E`. E hat keinen Nachfolger; die Schleife endet und E bleibt an seinem Platz.

```text
Nach dem ersten Paar: [B] -> [A] -> [C] -> [D] -> [E] -> null
Nach dem zweiten Paar: [B] -> [A] -> [D] -> [C] -> [E] -> null
```

Die Bedingung `current != null && current.Next != null` lässt nur vollständige Paare zu. Durch `&&` wird `current.Next` erst geprüft, wenn `current` vorhanden ist.

Für die mündliche Überprüfung: **„Ich tausche die beiden Pfeile im Paar und verbinde dann den Listenanfang oder das vorherige Paar mit dem neuen Paaranfang.“** `Count` bleibt gleich. Laufzeit O(n), zusätzlicher Speicher O(1).

### 9. Anhaengen(andere)

Verbindet die letzte eigene Figur mit der ersten Figur der anderen Liste. Bei einer leeren eigenen Liste wird deren `first` direkt übernommen. Anschließend werden die Counts addiert und die andere Liste bekommt `first = null` und `Count = 0`.

```text
Eigene Liste: [A] -> [B] -> null      Count = 2
Andere Liste: [X] -> [Y] -> null      Count = 2

Danach: [A] -> [B] -> [X] -> [Y] -> null    Count = 4
Andere Liste: first = null                 Count = 0
```

Es werden keine Figuren kopiert. Nur die Verwaltung der anderen Liste wird geleert; ihre Figuren hängen jetzt in der eigenen Liste. Bei `null`, einer leeren anderen Liste oder beim Anhängen der Liste an sich selbst passiert nichts. Der Selbstvergleich verhindert einen Kreis in der Kette. Laufzeit: O(n) für die eigene Liste; bei leerer eigener Liste O(1).

## Erwarteter Hauptablauf

| Schritt | Reihenfolge | Count |
| --- | --- | --- |
| Add x5 | K1 W1 K2 W2 K3 | 5 |
| Push(W0-Push) | W0-Push K1 W1 K2 W2 K3 | 6 |
| Pop() liefert W0-Push | K1 W1 K2 W2 K3 | 5 |
| InsertAt(2, K-Insert) | K1 W1 K-Insert K2 W2 K3 | 6 |
| Remove(0) | W1 K-Insert K2 W2 K3 | 5 |
| VertauscheBenachbarte() | K-Insert W1 W2 K2 K3 | 5 |
| Umdrehen() | K3 K2 W2 W1 K-Insert | 5 |
| Anhaengen(zweiteListe) | K3 K2 W2 W1 K-Insert Z1 Z2 | 7 |

Danach ist `zweiteListe.Count == 0`. Das Programm zeigt auch nach den einzelnen Add-Aufrufen die Liste und den Count.

## Welche Randfälle getestet werden

- Leere Liste: `Pop`, `Remove`, `Umdrehen`, Paartausch und Ausgabe.
- Genau eine Figur: `Umdrehen`, Paartausch, `Pop` und `Remove(0)`.
- Ungültige Löschindizes und Einfügen vor dem Anfang bzw. hinter dem Ende.
- Löschen in der Mitte und am Ende; Abtrennen der entfernten Figur.
- Anhängen einer leeren Liste, von `null` und an eine leere eigene Liste.
- Gerade und ungerade Anzahl beim Paartausch; Erhalt der Figur-Referenzen.
- Zweimal Umdrehen und zweimal Paartausch ergeben wieder die Ausgangsreihenfolge.
- Selbst-Anhängen, Null-Figuren und Einfügen an den exakten Indexgrenzen.
- Oberfläche und Volumen von Kugel und Würfel.

Die Figuren selbst berechnen ihre Werte: Kugeloberfläche `4 * PI * r²`, Kugelvolumen `4 * PI * r³ / 3`, Würfeloberfläche `6 * a²`, Würfelvolumen `a³`. Die Liste kümmert sich um die Verkettung; die Ausgabe nutzt die Berechnungen der jeweiligen Figur.
