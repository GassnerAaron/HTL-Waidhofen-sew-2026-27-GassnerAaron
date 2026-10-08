# Arbeitsblatt Doppelt verkettete Liste Lösungen

## 1 Grundbegriffe

- Nachfolger: `Next`; Vorgänger: `Previous`.
- Erste Figur: `first`; letzte Figur: `last`.
- Leere Liste: `first == null`, `last == null` und `Count == 0`.
- Erste Figur: `Previous == null`; letzte Figur: `Next == null`.
- Spiegel-Regel: Wenn `a.Next == b`, dann gilt `b.Previous == a`.
- Eine Figur X: `first` und `last` zeigen beide auf X.

## 2 Aufwand

| Operation | Einfach verkettet nur first | Doppelt verkettet first und last |
| --- | --- | --- |
| Add(fig) | O(n) | O(1) |
| Push(fig) | O(1) | O(1) |
| Anhaengen(andere) | O(n) | O(1) |
| Remove(fig), Figur bereits bekannt | O(n) | O(1) |
| GetAt(index) | O(n) | O(n) |
| Liste rückwärts ausgeben | Nicht direkt durchführbar | O(n) |

Bei einfach verketteter Liste muss für Remove(fig) im allgemeinen Fall erst der Vorgänger gesucht werden. Bei der doppelt verketteten Liste steht er in `fig.Previous`.

Zur letzten Zeile: Gemeint ist das direkte Durchlaufen über Rückwärtspfeile. Eine einfach verkettete Liste könnte beispielsweise durch wiederholtes Suchen vom Anfang auch rückwärts ausgegeben werden, dann aber mit O(n²). Sie hat keinen direkten Previous-Weg.

## 3 Zeichnen Push X

Jedes Kästchen enthält `Previous | Daten | Next`.

```text
Vorher:
first = A                                         last = C
[null | A | B] <-> [A | B | C] <-> [B | C | null]

Nachher:
first = X                                                           last = C
[null | X | A] <-> [X | A | B] <-> [A | B | C] <-> [B | C | null]
Count = 4
```

Für die vorgegebene nicht leere Liste:

```csharp
fig.Previous = null;  // 1: X hat keinen Vorgänger.
fig.Next = first;     // 2: X.Next zeigt auf A.
first.Previous = fig; // 3: A.Previous zeigt auf X.
first = fig;          // 4: Anfang wird X.
Count++;             // Anzahl aktualisieren.
```

Im allgemeinen Code muss geprüft werden, ob `first` vorhanden ist. Bei leerer Liste wird stattdessen `last = fig` gesetzt.

## 4 Liste verfolgen

Ausgangsliste: A B C D, first = A, last = D, Count = 4. Die Operationen bauen aufeinander auf.

| Operation | Liste vorwärts | first | last | Count |
| --- | --- | --- | --- | --- |
| Push(X) | X A B C D | X | D | 5 |
| Add(Y) | X A B C D Y | X | Y | 6 |
| Pop() | A B C D Y | A | Y | 5 |
| InsertAt(2, Z) | A B Z C D Y | A | Y | 6 |
| Remove(1) | A Z C D Y | A | Y | 5 |
| Umdrehen() | Y D C Z A | Y | A | 5 |
| VertauscheBenachbarte() | D Y Z C A | D | A | 5 |

`Pop()` liefert X zurück. Rückwärts nach dem letzten Schritt: **A C Z Y D**.

## 5 Add ergänzen

Die Lücken von oben nach unten: `last;`, `last.Next`, `first`, `fig;`.

```csharp
public void Add(Figur fig)
{
    if (fig == null) return;
    fig.Next = null;
    fig.Previous = last;
    if (last != null)
        last.Next = fig;
    else
        first = fig;
    last = fig;
    Count++;
}
```

## 6 Remove einer bekannten Figur ergänzen

Die Lücken von oben nach unten: `fig.Next;`, `fig.Next;`, `fig.Previous;`, `fig.Previous;`, `Count--;`.

Voraussetzung: Die bekannte Figur gehört zu dieser Liste.

```csharp
public void Remove(Figur fig)
{
    if (fig == null) return;
    if (fig.Previous != null)
        fig.Previous.Next = fig.Next;
    else
        first = fig.Next;

    if (fig.Next != null)
        fig.Next.Previous = fig.Previous;
    else
        last = fig.Previous;

    fig.Next = null;
    fig.Previous = null;
    Count--;
}
```

Im Projekt ist diese Hilfsmethode privat. Öffentlich wird `Remove(int index)` aufgerufen, das die Figur vorher aus der eigenen Liste holt.

## 7 Fehler in Push finden

**a)** Rückwärts kommt **C B A** heraus. X fehlt, weil A.Previous weiterhin `null` ist. Vorwärts kommt X A B C heraus.

**b)** Die zwei fehlenden Zeilen für die vorgegebene nicht leere Liste:

```csharp
fig.Previous = null;
first.Previous = fig;
```

**c)** Beide können vor `first = fig;` stehen. Besonders `first.Previous = fig;` muss davor stehen, damit `first` noch auf das alte A zeigt. Danach würde man X.Previous auf X selbst setzen. `fig.Previous = null;` setzt direkt eine Eigenschaft von fig und wäre für sich genommen auch später möglich.

Vollständig einschließlich leerer Liste:

```csharp
public void Push(Figur fig)
{
    if (fig == null) return;
    fig.Previous = null;
    fig.Next = first;
    if (first != null)
        first.Previous = fig;
    else
        last = fig;
    first = fig;
    Count++;
}
```

## 8 Verständnisfragen

**a)** `tmp` merkt den alten Nachfolger. Nach dem Überschreiben von `current.Next` würde dieser Pfeil sonst in die andere Richtung zeigen. Mit `current = tmp` geht es trotzdem zur nächsten noch nicht bearbeiteten Figur.

**b)** `last` und `andere.first` sind bekannt. Deshalb können die beiden Listen mit wenigen Zuweisungen verbunden werden, unabhängig von ihrer Länge. Bei der einfach verketteten Liste mit nur `first` muss das eigene Ende erst gesucht werden. Mit zusätzlichem `last` wäre auch einfach verkettetes Anhängen O(1).

**c)** Jede Figur braucht zusätzlichen Speicher für `Previous`. Außerdem müssen bei Änderungen mehr Pfeile gepflegt werden.

## Selbstkontrolle

Die Kästchen im Original solltest du erst ankreuzen, wenn du die Aufgabe selbst ohne Vorlage lösen kannst. Zum Üben: Lösungen abdecken, die Liste verfolgen und Push einmal aus dem Kopf schreiben.
