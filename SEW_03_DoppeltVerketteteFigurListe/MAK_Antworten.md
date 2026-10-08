# MAK Übungsblatt Einfach verkettete Liste Lösungen

Dieses beigefügte PDF behandelt ausdrücklich die einfach verkettete Liste mit `Next`, `first` und `Count`. Hier stehen die vier Lösungen. Für die doppelt verkettete Variante kommen `Previous` und `last` dazu.

## 1 Fehlerhaftes Push analysieren

Der fehlerhafte Code:

```csharp
first = fig;
fig.Next = first;
Count++;
```

Angenommen fig ist X: Nach der ersten Zeile zeigt `first` bereits auf X. Deshalb bedeutet die zweite Zeile jetzt `X.Next = X`. X zeigt auf sich selbst. A B C sind vom Listenanfang aus nicht mehr erreichbar. Eine Ausgabe über Next läuft endlos.

```text
first --> X --+
          ^   |
          +---+

A --> B --> C --> null   (vom neuen first aus nicht erreichbar)
Count = 4, obwohl die Kette fehlerhaft ist.
```

Richtige Reihenfolge:

```csharp
fig.Next = first; // Alten Anfang A zuerst verbinden.
first = fig;     // Danach X zum neuen Anfang machen.
Count++;
```

Merksatz: **Erst den alten Weg sichern, dann den Anfang überschreiben.**

## 2 InsertAt 1 X zeichnen

Index 0 ist A, Index 1 ist B. X soll zwischen A und B stehen.

```text
Vorher:  [A] --> [B] --> [C] --> null
Nachher: [A] --> [X] --> [B] --> [C] --> null
```

Mit `vor = A` und `fig = X`:

```csharp
fig.Next = vor.Next; // 1: X zeigt auf B. Alten Nachfolger zuerst sichern.
vor.Next = fig;      // 2: A zeigt auf X.
```

Die neu gesetzten Pfeile sind zuerst X -> B und dann A -> X. Zusätzlich muss die komplette Methode `Count++` ausführen.

Bei der doppelt verketteten Liste werden auch `X.Previous = A` und `B.Previous = X` gesetzt. Der vollständige Ausschnitt dafür steht im README.

## 3 Umdrehen ergänzen

Die drei Lücken: `current.Next;`, `prev;`, `prev;`.

```csharp
public void Umdrehen()
{
    Figur prev = null;
    Figur current = first;
    while (current != null)
    {
        Figur next = current.Next;
        current.Next = prev;
        prev = current;
        current = next;
    }
    first = prev;
}
```

Auf A B C:

| Aktuelle Figur | next merken | Neuer Next-Pfeil | Danach prev | Danach current |
| --- | --- | --- | --- | --- |
| A | B | A.Next = null | A | B |
| B | C | B.Next = A | B | C |
| C | null | C.Next = B | C | null |

Dann `first = C`. Ergebnis C -> B -> A -> null. `Count` bleibt 3.

`next` ist nötig, weil `current.Next` überschrieben wird. Die doppelt verkettete Version tauscht stattdessen bei jeder Figur Next und Previous und danach first und last.

## 4 Fehlerhaftes Pop korrigieren

Die drei fehlenden Dinge:

1. **Leere Liste prüfen:** Sonst stürzt `first.Next` ab, weil first `null` ist.
2. **Ergebnis abtrennen:** `ergebnis.Next = null;`, damit die entfernte Figur nicht weiter auf die restliche Liste zeigt.
3. **Anzahl aktualisieren:** `Count--;`.

```csharp
public Figur Pop()
{
    if (first == null) return null;

    Figur ergebnis = first;
    first = first.Next;
    ergebnis.Next = null;
    Count--;
    return ergebnis;
}
```

Bei A -> B -> C bleibt B -> C in der Liste. Zurückgegeben wird A, vollständig abgetrennt. Bei einer einzigen Figur wird first `null` und Count 0.

Bei der doppelt verketteten Liste zusätzlich: Die neue erste Figur bekommt `Previous = null`. Wird die Liste leer, muss auch `last = null` sein. Das zurückgegebene Objekt bekommt beide Pfeile auf `null`.
