# Doppelt verkettete FigurListe einfach erklärt

Das Projekt enthält alle verlangten Methoden und den Beispielablauf. In `Program.cs` gibt es keine Arrays, fertigen Collections, Generics oder zusätzlichen Knotenklassen. Die Figur selbst ist der Knoten. Jede Methode verändert nur die Verbindungen zwischen vorhandenen Figuren. Neue Figuren werden im Testprogramm erzeugt und der Liste übergeben.

## Starten

Öffne den Projektordner in Visual Studio, Rider oder VS Code. Im Terminal dieses Ordners:

```powershell
dotnet run
```

Benötigt wird ein .NET SDK ab Version 8. Das Projekt erlaubt auch eine neuere installierte Runtime. Hier wurde es erfolgreich kompiliert und ausgeführt. Die Verbindungen, Objektidentität, Indizes und Randfälle wurden zusätzlich mit einem unabhängigen Prüfprogramm kontrolliert.

Für dein Git-Repository kannst du diesen ganzen Ordner neben `SEW_01_FigurListe` und `SEW_02_VerlinkteFigurListe` einfügen. `SEW_03` ist die fortlaufende Nummer. Die beiden älteren Projekte bleiben eigenständig. Die Ordner `bin` und `obj` gehören nicht in die Abgabe.

## 1 Die Idee in einer Minute

Eine Liste besteht aus Figuren, die auf ihre Nachbarn zeigen:

```text
           first                 last
             |                     |
             v                     v
null <---- [ A ] <----> [ B ] <----> [ C ] ----> null
```

`Next` zeigt nach rechts, `Previous` nach links. `first` merkt sich den Anfang, `last` das Ende, `Count` die Anzahl.

Für A und B gilt:

```csharp
A.Next = B;
B.Previous = A;
```

Die Referenzen sind wie Pfeile zu Objekten. `Figur vor = nach.Previous;` erzeugt keine neue Figur. Die Variable `vor` zeigt auf eine Figur, die bereits existiert.

**Merksatz: Eine Verbindung braucht zwei Pfeile.**

Die erste Figur hat `Previous == null`. Die letzte Figur hat `Next == null`. Bei einer leeren Liste sind `first` und `last` beide `null`, `Count` ist 0. Bei einer einzigen Figur zeigen `first` und `last` auf dasselbe Objekt.

## 2 Die C# Wörter verstehen

| Code | Bedeutung |
| --- | --- |
| `Figur current = first;` | Merke die erste Figur in einer Arbeitsvariable. |
| `current = current.Next;` | Gehe eine Figur nach rechts. |
| `current = current.Previous;` | Gehe eine Figur nach links. |
| `fig.Next = first;` | Ändere den Pfeil in fig: Er zeigt nun auf die bisher erste Figur. |
| `first = fig;` | Ändere den gemerkten Listenanfang. |
| `null` | Hier gibt es kein Objekt. |
| `if (first != null)` | Nur ausführen, wenn eine erste Figur vorhanden ist. |
| `while (current != null)` | Wiederholen, solange noch eine Figur da ist. |
| `return;` | Die Methode sofort beenden. |
| `return ergebnis;` | Die Methode beenden und die gemerkte Figur zurückgeben. |
| `Count++;` / `Count--;` | Die Anzahl um 1 erhöhen / verringern. |

Wichtig: `=` weist etwas zu. `==` prüft, ob zwei Dinge gleich sind.

`fig.Previous.Next = fig.Next;` liest du so: **Nimm den Vorgänger von fig und ändere seinen Next-Pfeil auf den Nachfolger von fig.**

## 3 Die Methoden verstehen

### GetAt holt eine Figur

Die Indizes beginnen bei 0:

```text
Index:  0   1   2   3   4
Figur:  A   B   C   D   E
```

`GetAt(3)` liefert D. Dafür startet die Methode bei E (`last`) und geht einmal über `Previous` zurück. Bei einem Index in der vorderen Hälfte startet sie bei `first`. Ungültige Indizes liefern `null`.

### Add hängt hinten an

```text
A <-> B       wird       A <-> B <-> X
```

Die neue Figur zeigt zurück auf B. B zeigt vorwärts auf die neue Figur. Dann wird X das neue Ende:

```csharp
fig.Previous = last;
last.Next = fig;
last = fig;
```

Zusätzlich braucht die neue letzte Figur `fig.Next = null`, und `Count` steigt. Bei leerer Liste fehlt die alte letzte Figur: Statt `last.Next = fig` setzt man `first = fig`.

### Ausgabe geht die Pfeile entlang

Vorwärts beginnt man bei `first` und geht immer zu `current.Next`. Rückwärts beginnt man bei `last` und geht immer zu `current.Previous`. Beide Schleifen enden bei `null`. Dabei wird nichts an der Liste verändert.

### Push fügt vorne ein

```text
A <-> B       wird       X <-> A <-> B
```

```csharp
fig.Previous = null;  // X hat keinen Vorgänger.
fig.Next = first;     // X zeigt auf das alte A.
first.Previous = fig; // A zeigt zurück auf X.
first = fig;          // X ist jetzt der Anfang.
Count++;
```

Dieser Ausschnitt zeigt die nicht leere Liste. Im vollständigen Code wird bei leerer Liste stattdessen `last = fig` gesetzt.

**Die Reihenfolge ist entscheidend:** Erst `fig.Next = first`, dann `first = fig`. Andersherum zeigt X auf sich selbst und du verlierst vom Listenanfang aus den Zugang zur alten Liste.

### Pop entfernt vorne

```text
A <-> B <-> C       wird       B <-> C
Rückgabe: A mit Next == null und Previous == null
```

1. Bei leerer Liste `null` zurückgeben.
2. Die alte erste Figur als `ergebnis` merken.
3. `first = first.Next` macht B zum Anfang.
4. B hat nun keinen Vorgänger: `first.Previous = null`.
5. Ist kein B da, wird auch `last = null`.
6. Die Pfeile in A auf `null` setzen, `Count--`, A zurückgeben.

`Push` und `Pop` funktionieren zusammen wie ein Stapel: Was du zuletzt vorne einfügst, kommt zuerst wieder heraus. Das heißt LIFO.

### InsertAt fügt in der Mitte ein

```text
A <-> B       wird       A <-> X <-> B
```

`vor` ist A, `nach` ist B. Vier Pfeile:

```csharp
fig.Previous = vor;  // X -> A nach links
fig.Next = nach;     // X -> B nach rechts
vor.Next = fig;      // A -> X nach rechts
nach.Previous = fig; // B -> X nach links
```

Die Nachbarn vorher merken: `nach = GetAt(index)`, `vor = nach.Previous`. Dann `Count++`. Bei `index <= 0` übernimmt `Push`, bei `index >= Count` übernimmt `Add`. Danach folgt `return`, damit `Count` nicht zweimal erhöht wird.

### Remove überspringt eine Figur

```text
A <-> X <-> B       wird       A <-> B
```

```csharp
fig.Previous.Next = fig.Next;     // A zeigt auf B.
fig.Next.Previous = fig.Previous; // B zeigt auf A.
```

Fehlt der Vorgänger, wird `first` auf den Nachfolger gesetzt. Fehlt der Nachfolger, wird `last` auf den Vorgänger gesetzt. Danach beide Pfeile der entfernten Figur auf `null` setzen und `Count--`.

`Remove(index)` sucht zuerst die Figur mit `GetAt`. Die private Hilfsmethode `Remove(Figur fig)` bekommt deshalb immer eine Figur aus dieser Liste und braucht keine weitere Suche.

### Umdrehen tauscht links und rechts

```text
A <-> B <-> C       wird       C <-> B <-> A
```

Bei jeder Figur werden die beiden Pfeile getauscht:

```csharp
Figur tmp = current.Next;
current.Next = current.Previous;
current.Previous = tmp;
current = tmp;
```

**Warum tmp?** Du überschreibst `Next`. Deshalb musst du vorher merken, wo es in der alten Reihenfolge weitergeht. Bei A ist das B. Ohne `tmp` würdest du nach dem Überschreiben von A.Next bei `null` landen.

Ganz am Schluss `first` und `last` tauschen. Sonst würde die Vorwärtsausgabe noch bei der alten ersten Figur starten. `Count` bleibt gleich.

### VertauscheBenachbarte tauscht Paare

```text
A B C D E       wird       B A D C E
```

Merke zuerst `vor`, `a`, `b`, `rest`:

```text
vor <-> a <-> b <-> rest
```

Verbinde danach in beiden Richtungen:

```text
vor <-> b <-> a <-> rest
```

Fehlt `vor`, wird `first = b`. Fehlt `rest`, wird `last = a`. Dann geht es beim alten `rest` mit dem nächsten Paar weiter. Eine einzelne letzte Figur bleibt stehen. `Count` bleibt gleich.

### Anhaengen verbindet zwei ganze Listen

```text
A <-> B    und    X <-> Y       wird       A <-> B <-> X <-> Y
```

Verbinde die beiden Endpunkte:

```csharp
last.Next = andere.first;
andere.first.Previous = last;
```

Dann `last` von der anderen Liste übernehmen und beide Anzahlen addieren. Zum Schluss wird die Verwaltung der anderen Liste geleert: `first = null`, `last = null`, `Count = 0`. Die Figuren sind jetzt in der eigenen Liste.

Bei leerer eigener Liste wird direkt `andere.first` übernommen. Ist die andere Liste leer, `null` oder dieselbe Liste, passiert nichts. Das Anhängen an sich selbst würde sonst einen Kreis erzeugen.

## 4 Aufwand ganz einfach

`n` ist die Anzahl der Figuren. O(1) bedeutet: Die Zahl der Schritte hängt nicht von der Listenlänge ab. O(n) bedeutet: Im ungünstigsten Fall muss man proportional zur Listenlänge viele Figuren besuchen.

| Methode | Aufwand | Grund |
| --- | --- | --- |
| Add, Push, Pop | O(1) | Anfang und Ende sind schon bekannt. |
| Anhaengen | O(1) | Die beiden Endpunkte sind schon bekannt. |
| GetAt | O(n) | Die gesuchte Figur muss erreicht werden. |
| InsertAt in der Mitte, Remove per Index | O(n) | Erst suchen, dann Pfeile ändern. |
| Entfernen einer bekannten Figur | O(1) | Die Figur kennt beide Nachbarn. |
| Ausgabe, Umdrehen, Paare tauschen | O(n) | Die Liste durchlaufen. |

Bei der einfach verketteten Liste aus der alten Übung gibt es nur `first` und `Next`. Add und Anhaengen sind dort O(n), weil das Ende gesucht werden muss. Mit einer gespeicherten `last`-Referenz könnten auch diese Operationen bei einer einfach verketteten Liste O(1) sein.

## 5 Dein Lernplan für zwei Stunden

| Zeit | Aufgabe |
| --- | --- |
| Erste 15 Minuten | Zeichne A, B, C. Beschrifte Next, Previous, first, last und Count. Lies die C# Wörter oben. |
| Nächste 30 Minuten | Zeichne und schreibe Add, Push und Pop. Jeweils normale Liste, leere Liste und eine Figur durchspielen. |
| Nächste 25 Minuten | InsertAt und Remove mit vier bzw. zwei Nachbarpfeilen üben. Danach die Tabelle im Arbeitsblatt selbst ausfüllen. |
| Nächste 20 Minuten | Umdrehen erklären und einmal auf A B C von Hand durchgehen. Paartausch und Anhaengen zeichnen. |
| Nächste 20 Minuten | Die vier Aufgaben aus dem MAK-PDF ohne Lösungen versuchen. Danach mit `MAK_Antworten.md` vergleichen. |
| Letzte 10 Minuten | Ohne Vorlage erklären: Was ist null? Warum tmp? Wann ändern sich first, last und Count? |

**Wenn die Zeit knapp wird: Push, Pop, InsertAt und Umdrehen zuerst üben.** Genau diese vier Themen stehen im beigefügten MAK-PDF. Das PDF behandelt die einfach verkettete Liste; eure neue Übung behandelt die doppelt verkettete Liste. Welche Variante in deiner kommenden MAK gefragt wird, lässt sich daraus nicht sicher sagen.

## 6 Regeln zum Kontrollieren

- Passt zu jedem Next-Pfeil ein Previous-Pfeil in Gegenrichtung?
- Sind `first.Previous` und `last.Next` beide `null`, wenn die Liste nicht leer ist?
- Nach Einfügen: Count + 1. Nach Entfernen: Count - 1. Nach Tauschen oder Umdrehen: Count unverändert.
- Rückwärts muss exakt die umgekehrte Reihenfolge von vorwärts ergeben.
- Bevor du einen Pfeil überschreibst, merke den Nachbarn, den du noch brauchst.

Beim Einfügen muss die übergebene Figur neu oder bereits vollständig abgetrennt sein. Dieselbe Figur darf nicht gleichzeitig in mehreren Listen stehen. Eine ganze bestehende Liste wird mit `Anhaengen` übertragen.

Die Kugel- und Würfelklassen berechnen nur Oberfläche und Volumen. Für die Verkettung sind die fünf Dinge `Next`, `Previous`, `first`, `last` und `Count` entscheidend.
